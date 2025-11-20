using BackUp.Application.Dtos.Dashboard;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BackUp.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly BackUpDbContext _context;

        public DashboardRepository(BackUpDbContext context)
        {
            _context = context;
        }
        public async Task<MetricasDTO> ObtenerMetricasAsync()
        {
            try
            {
                var hoy = DateTime.UtcNow.Date;
                var mesPasado = DateTime.UtcNow.AddMonths(-1);
                var dosMesesAtras = DateTime.UtcNow.AddMonths(-2);

                // Obtener jobs completados
                var jobsCompletados = await _context.JobBackup
                    .Where(j => j.FechaCompletado != null)
                    .ToListAsync();

                var totalAlmacenadoTB = jobsCompletados.Sum(j => j.TamanoBytes ?? 0) / 1099511627776.0;
                var backupsHoy = jobsCompletados.Count(j => j.FechaCompletado?.Date == hoy);

                var jobsExitosos = jobsCompletados.Count(j => j.Estado == "completado");
                var tasaExito = jobsCompletados.Count > 0 ? (jobsExitosos * 100.0 / jobsCompletados.Count) : 0;

                var ultimaActualizacion = jobsCompletados.Any()
                    ? jobsCompletados.Max(j => j.FechaCompletado) ?? DateTime.UtcNow
                    : DateTime.UtcNow;

                // Calcular incrementos
                var jobsMesActual = jobsCompletados
                    .Where(j => j.FechaCompletado >= mesPasado)
                    .ToList();

                var jobsMesAnterior = jobsCompletados
                    .Where(j => j.FechaCompletado >= dosMesesAtras && j.FechaCompletado < mesPasado)
                    .ToList();

                var incrementoAlmacenamiento = jobsMesActual.Sum(j => j.TamanoBytes ?? 0) / 1099511627776.0
                                             - jobsMesAnterior.Sum(j => j.TamanoBytes ?? 0) / 1099511627776.0;

                var incrementoBackups = jobsMesActual.Count - jobsMesAnterior.Count;

                var tasaExitoMesActual = jobsMesActual.Count > 0
                    ? (jobsMesActual.Count(j => j.Estado == "completado") * 100.0 / jobsMesActual.Count)
                    : 0;

                var tasaExitoMesAnterior = jobsMesAnterior.Count > 0
                    ? (jobsMesAnterior.Count(j => j.Estado == "completado") * 100.0 / jobsMesAnterior.Count)
                    : 0;

                var incrementoTasaExito = tasaExitoMesActual - tasaExitoMesAnterior;

                return new MetricasDTO
                {
                    TotalAlmacenadoTB = Math.Round(totalAlmacenadoTB, 2),
                    BackupsHoy = backupsHoy,
                    TasaExitoPorcentaje = Math.Round(tasaExito, 2),
                    UltimaActualizacion = ultimaActualizacion,
                    IncrementoAlmacenamiento = Math.Round(incrementoAlmacenamiento, 2),
                    IncrementoBackups = Math.Round((double)incrementoBackups, 2),
                    IncrementoTasaExito = Math.Round(incrementoTasaExito, 2)
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error en ObtenerMetricasAsync: {ex.Message}");
                return new MetricasDTO
                {
                    TotalAlmacenadoTB = 0,
                    BackupsHoy = 0,
                    TasaExitoPorcentaje = 0,
                    UltimaActualizacion = DateTime.UtcNow,
                    IncrementoAlmacenamiento = 0,
                    IncrementoBackups = 0,
                    IncrementoTasaExito = 0
                };
            }
        }

        public async Task<List<ProveedorStorageDTO>> ObtenerProveedoresAsync()
        {
            try
            {
                // Obtener cloud storages activos con sus jobs
                var cloudStorages = await _context.CloudStorage
                    .Where(cs => cs.IsActive)
                    .Include(cs => cs.JobsBackup) // Usar el nombre correcto de la propiedad
                    .ToListAsync();

                var proveedores = new List<ProveedorStorageDTO>();

                foreach (var grupo in cloudStorages.GroupBy(cs => cs.Proveedor))
                {
                    var proveedor = grupo.Key;
                    var usadoTB = grupo
                        .SelectMany(cs => cs.JobsBackup
                            .Where(j => j.Estado == "completado")
                            .Select(j => j.TamanoBytes ?? 0))
                        .Sum() / 1099511627776.0;

                    var tieneJobsEjecutando = grupo
                        .Any(cs => cs.JobsBackup.Any(j => j.Estado == "ejecutando"));

                    var estado = tieneJobsEjecutando ? "Activo - Ejecutando" : "Activo";

                    proveedores.Add(new ProveedorStorageDTO
                    {
                        Proveedor = proveedor,
                        UsadoTB = Math.Round(usadoTB, 2),
                        TotalTB = proveedor switch
                        {
                            "aws" => 1000.00,
                            "azure" => 500.00,
                            "gcp" => 750.00,
                            _ => 100.00
                        },
                        Estado = estado
                    });
                }

                return proveedores;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerProveedoresAsync: {ex.Message}");
                return new List<ProveedorStorageDTO>();
            }
        }

        public async Task<List<BackupRecienteDTO>> ObtenerBackupsRecientesAsync()
        {
            try
            {
                // Obtener backups recientes con las relaciones incluidas
                var backups = await _context.JobBackup
                    .Where(j => j.FechaEjecucion != null)
                    .Include(j => j.Politica)
                    .Include(j => j.CloudStorage)
                    .OrderByDescending(j => j.FechaEjecucion)
                    .Take(10)
                    .Select(j => new BackupRecienteDTO
                    {
                        Nombre = j.Politica != null ? j.Politica.Nombre : "Desconocido",
                        TamanioGB = Math.Round((j.TamanoBytes ?? 0) / 1073741824.0, 2),
                        Proveedor = j.CloudStorage != null ? j.CloudStorage.Proveedor : "Desconocido",
                        Estado = j.Estado,
                        HoraEjecucion = j.FechaEjecucion
                    })
                    .ToListAsync();

                return backups;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerBackupsRecientesAsync: {ex.Message}");
                return new List<BackupRecienteDTO>();
            }
        }

        public async Task<bool> CrearBackupAsync(CrearBackupDTO backupDto)
        {
            try
            {
                // Validar que la política existe
                var politica = await _context.PoliticaBackup
                    .FirstOrDefaultAsync(p => p.Id == backupDto.PoliticaId && p.IsActive);

                if (politica == null)
                    return false;

                // Validar que el cloud storage existe
                var cloudStorage = await _context.CloudStorage
                    .FirstOrDefaultAsync(cs => cs.Id == backupDto.CloudStorageId && cs.IsActive);

                if (cloudStorage == null)
                    return false;

                // Crear el job de backup
                var job = new JobBackup
                {
                    PoliticaId = backupDto.PoliticaId,
                    CloudStorageId = backupDto.CloudStorageId,
                    Estado = "programado",
                    FechaProgramada = backupDto.FechaProgramada,
                    SourceData = backupDto.RutaOrigen,
                    
                };

                _context.JobBackup.Add(job);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CrearBackupAsync: {ex.Message}");
                return false;
            }
        }
    }
}