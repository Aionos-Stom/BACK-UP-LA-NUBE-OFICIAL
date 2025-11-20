using BackUp.Aplication.Dtos.VerificacionIntegridad;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
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
    public class VerificacionIntegridadRepository : IVerificacionIntegridadRepository
    {
        private readonly BackUpDbContext _context;

        public VerificacionIntegridadRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            try
            {
                var verificacion = await _context.VerificacionIntegridad
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (verificacion == null)
                    return OperationResult.Failure("Verificación de integridad no encontrada");

                var dto = new ObtenerVerificacionIntegridadDTO
                {
                    Id = verificacion.Id,
                    JobId = verificacion.job_id, // Corregido: job_id en lugar de JobId
                    ChecksumSha256 = verificacion.ChecksumSha256,
                    Resultado = verificacion.Resultado,
                    FechaVerificacion = verificacion.FechaVerificacion,
                    Detalles = verificacion.Detalles,
                    IntegrityScore = verificacion.IntegrityScore
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener verificación: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            try
            {
                var verificaciones = await _context.VerificacionIntegridad
                    .OrderByDescending(v => v.FechaVerificacion)
                    .ToListAsync();

                var dtos = verificaciones.Select(v => new ObtenerVerificacionIntegridadDTO
                {
                    Id = v.Id,
                    JobId = v.job_id, // Corregido: job_id en lugar de JobId
                    ChecksumSha256 = v.ChecksumSha256,
                    Resultado = v.Resultado,
                    FechaVerificacion = v.FechaVerificacion,
                    Detalles = v.Detalles,
                    IntegrityScore = v.IntegrityScore
                }).ToList();

                return OperationResult.Success(dtos);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener verificaciones: {ex.Message}");
            }
        }

        public async Task<OperationResult> AgregarAsync(SaveVerificacionIntegridadDTO saveVerificacionIntegridad)
        {
            try
            {
                var verificacion = new VerificacionIntegridad
                {
                    job_id = saveVerificacionIntegridad.JobId, // Corregido: job_id en lugar de JobId
                    ChecksumSha256 = saveVerificacionIntegridad.ChecksumSha256,
                    Resultado = saveVerificacionIntegridad.Resultado,
                    FechaVerificacion = DateTime.UtcNow,
                    Detalles = saveVerificacionIntegridad.Detalles,
                    IntegrityScore = saveVerificacionIntegridad.Resultado == true ? 100m : 0m
                };

                _context.VerificacionIntegridad.Add(verificacion);
                await _context.SaveChangesAsync();

                var dto = new ObtenerVerificacionIntegridadDTO
                {
                    Id = verificacion.Id,
                    JobId = verificacion.job_id, // Corregido: job_id en lugar de JobId
                    ChecksumSha256 = verificacion.ChecksumSha256,
                    Resultado = verificacion.Resultado,
                    FechaVerificacion = verificacion.FechaVerificacion,
                    Detalles = verificacion.Detalles,
                    IntegrityScore = verificacion.IntegrityScore
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al agregar verificación: {ex.Message}");
            }
        }

        public async Task<OperationResult> EliminarAsync(RemoveVerificacionIntegridadDTO removeVerificacionIntegridad)
        {
            try
            {
                var verificacion = await _context.VerificacionIntegridad
                    .FirstOrDefaultAsync(v => v.Id == removeVerificacionIntegridad.Id);

                if (verificacion == null)
                    return OperationResult.Failure("Verificación de integridad no encontrada");

                _context.VerificacionIntegridad.Remove(verificacion);
                await _context.SaveChangesAsync();

                return OperationResult.Success("Verificación eliminada correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al eliminar verificación: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorJobAsync(int jobId)
        {
            try
            {
                var verificaciones = await _context.VerificacionIntegridad
                    .Where(v => v.job_id == jobId) // Corregido: job_id en lugar de JobId
                    .OrderByDescending(v => v.FechaVerificacion)
                    .ToListAsync();

                var dtos = verificaciones.Select(v => new ObtenerVerificacionIntegridadDTO
                {
                    Id = v.Id,
                    JobId = v.job_id, // Corregido: job_id en lugar de JobId
                    ChecksumSha256 = v.ChecksumSha256,
                    Resultado = v.Resultado,
                    FechaVerificacion = v.FechaVerificacion,
                    Detalles = v.Detalles,
                    IntegrityScore = v.IntegrityScore
                }).ToList();

                return OperationResult.Success(dtos);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener verificaciones por job: {ex.Message}");
            }
        }

        public async Task<ObtenerVerificacionIntegridadDTO> ObtenerUltimaPorJobAsync(int jobId)
        {
            var verificacion = await _context.VerificacionIntegridad
                .Where(v => v.job_id == jobId) // Corregido: job_id en lugar de JobId
                .OrderByDescending(v => v.FechaVerificacion)
                .FirstOrDefaultAsync();

            if (verificacion == null) return null;

            return new ObtenerVerificacionIntegridadDTO
            {
                Id = verificacion.Id,
                JobId = verificacion.job_id, // Corregido: job_id en lugar de JobId
                ChecksumSha256 = verificacion.ChecksumSha256,
                Resultado = verificacion.Resultado,
                FechaVerificacion = verificacion.FechaVerificacion,
                Detalles = verificacion.Detalles,
                IntegrityScore = verificacion.IntegrityScore
            };
        }
    }
}
