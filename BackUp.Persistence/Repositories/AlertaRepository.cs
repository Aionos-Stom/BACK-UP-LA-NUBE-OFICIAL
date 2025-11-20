using BackUp.Aplication.Interfaces.Base;
using BackUp.Application.Dtos.Alerta;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Persistence.Repositories
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly BackUpDbContext _context;

        public AlertaRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<ObtenerAlertaDTO> ObtenerPorIdAsync(int id)
        {
            var alerta = await _context.Alerta
                .Include(a => a.Usuario)
                .Include(a => a.JobBackup)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alerta == null) return null;

            return new ObtenerAlertaDTO
            {
                Id = alerta.Id,
                UsuarioId = alerta.UsuarioId,
                UsuarioNombre = alerta.Usuario.Nombre,
                JobId = alerta.JobId,
                JobNombre = alerta.JobBackup?.SourceData, // Usar SourceData como nombre del job
                Tipo = alerta.Tipo,
                Severidad = alerta.Severidad,
                Mensaje = alerta.Mensaje,
                IsAcknowledged = alerta.IsAcknowledged,
                CreatedAt = DateTime.UtcNow // Valor temporal, ajustar según tu entidad
            };
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerTodosAsync()
        {
            return await _context.Alerta
                .Include(a => a.Usuario)
                .Include(a => a.JobBackup)
                .OrderByDescending(a => a.Id) // Ordenar por ID temporalmente
                .Select(a => new ObtenerAlertaDTO
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    UsuarioNombre = a.Usuario.Nombre,
                    JobId = a.JobId,
                    JobNombre = a.JobBackup != null ? a.JobBackup.SourceData : null,
                    Tipo = a.Tipo,
                    Severidad = a.Severidad,
                    Mensaje = a.Mensaje,
                    IsAcknowledged = a.IsAcknowledged,
                    CreatedAt = DateTime.UtcNow // Valor temporal
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _context.Alerta
                .Include(a => a.Usuario)
                .Include(a => a.JobBackup)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.Id)
                .Select(a => new ObtenerAlertaDTO
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    UsuarioNombre = a.Usuario.Nombre,
                    JobId = a.JobId,
                    JobNombre = a.JobBackup != null ? a.JobBackup.SourceData : null,
                    Tipo = a.Tipo,
                    Severidad = a.Severidad,
                    Mensaje = a.Mensaje,
                    IsAcknowledged = a.IsAcknowledged,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerNoReconocidasAsync()
        {
            return await _context.Alerta
                .Include(a => a.Usuario)
                .Include(a => a.JobBackup)
                .Where(a => !a.IsAcknowledged)
                .OrderByDescending(a => a.Id)
                .Select(a => new ObtenerAlertaDTO
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    UsuarioNombre = a.Usuario.Nombre,
                    JobId = a.JobId,
                    JobNombre = a.JobBackup != null ? a.JobBackup.SourceData : null,
                    Tipo = a.Tipo,
                    Severidad = a.Severidad,
                    Mensaje = a.Mensaje,
                    IsAcknowledged = a.IsAcknowledged,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorJobAsync(int? jobId)
        {
            var query = _context.Alerta
                .Include(a => a.Usuario)
                .Include(a => a.JobBackup)
                .AsQueryable();

            if (jobId.HasValue)
            {
                query = query.Where(a => a.JobId == jobId.Value);
            }

            return await query
                .OrderByDescending(a => a.Id)
                .Select(a => new ObtenerAlertaDTO
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    UsuarioNombre = a.Usuario.Nombre,
                    JobId = a.JobId,
                    JobNombre = a.JobBackup != null ? a.JobBackup.SourceData : null,
                    Tipo = a.Tipo,
                    Severidad = a.Severidad,
                    Mensaje = a.Mensaje,
                    IsAcknowledged = a.IsAcknowledged,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorSeveridadAsync(string severidad)
        {
            return await _context.Alerta
                .Include(a => a.Usuario)
                .Include(a => a.JobBackup)
                .Where(a => a.Severidad == severidad)
                .OrderByDescending(a => a.Id)
                .Select(a => new ObtenerAlertaDTO
                {
                    Id = a.Id,
                    UsuarioId = a.UsuarioId,
                    UsuarioNombre = a.Usuario.Nombre,
                    JobId = a.JobId,
                    JobNombre = a.JobBackup != null ? a.JobBackup.SourceData : null,
                    Tipo = a.Tipo,
                    Severidad = a.Severidad,
                    Mensaje = a.Mensaje,
                    IsAcknowledged = a.IsAcknowledged,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<ObtenerAlertaDTO> CrearAsync(SaveAlertaDTO saveAlerta)
        {
            var alerta = new Alerta
            {
                UsuarioId = saveAlerta.UsuarioId,
                JobId = saveAlerta.JobId,
                Tipo = saveAlerta.Tipo,
                Severidad = saveAlerta.Severidad,
                Mensaje = saveAlerta.Mensaje,
                IsAcknowledged = saveAlerta.IsAcknowledged
            };

            _context.Alerta.Add(alerta);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(alerta.Id);
        }

        public async Task<ObtenerAlertaDTO> ActualizarAsync(UpdateAlertaDTO updateAlerta)
        {
            var alerta = await _context.Alerta.FindAsync(updateAlerta.Id);
            if (alerta == null) return null;

            if (updateAlerta.JobId.HasValue) alerta.JobId = updateAlerta.JobId.Value;
            if (!string.IsNullOrEmpty(updateAlerta.Tipo)) alerta.Tipo = updateAlerta.Tipo;
            if (!string.IsNullOrEmpty(updateAlerta.Severidad)) alerta.Severidad = updateAlerta.Severidad;
            if (!string.IsNullOrEmpty(updateAlerta.Mensaje)) alerta.Mensaje = updateAlerta.Mensaje;
            if (updateAlerta.IsAcknowledged.HasValue) alerta.IsAcknowledged = updateAlerta.IsAcknowledged.Value;

            _context.Alerta.Update(alerta);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(alerta.Id);
        }

        public async Task<bool> EliminarAsync(RemoveAlertaDTO removeAlerta)
        {
            var alerta = await _context.Alerta.FindAsync(removeAlerta.Id);
            if (alerta == null) return false;

            _context.Alerta.Remove(alerta);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Alerta.AnyAsync(a => a.Id == id);
        }

        public async Task<int> ContarAlertasNoReconocidasAsync()
        {
            return await _context.Alerta
                .Where(a => !a.IsAcknowledged)
                .CountAsync();
        }

    }
}
