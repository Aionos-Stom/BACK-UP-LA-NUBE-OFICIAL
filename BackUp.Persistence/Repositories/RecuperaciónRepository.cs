using BackUp.Aplication.Dtos.Recuperacion;
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
    public class RecuperaciónRepository : IRecuperacionRepository
    {
        private readonly BackUpDbContext _context;

        public RecuperaciónRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<ObtenerRecuperacionDTO> ObtenerPorIdAsync(int id)
        {
            var recuperacion = await _context.Recuperacion
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recuperacion == null) return null;

            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Id == recuperacion.UsuarioId);
            var job = await _context.JobBackup.FirstOrDefaultAsync(j => j.Id == recuperacion.JobId);

            return new ObtenerRecuperacionDTO
            {
                Id = recuperacion.Id,
                UsuarioId = recuperacion.UsuarioId,
                JobId = recuperacion.JobId,
                TipoRecuperacion = recuperacion.TipoRecuperacion,
                PuntoTiempo = recuperacion.PuntoTiempo,
                InputPath = recuperacion.InputPath,
                Estado = recuperacion.Estado,
                IsSimulacion = recuperacion.IsSimulacion,
                FechaCreacion = DateTime.UtcNow,
                CompletedAt = recuperacion.CompletedAt
            };
        }

        public async Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerTodosAsync()
        {
            var recuperaciones = await _context.Recuperacion
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            var resultado = new List<ObtenerRecuperacionDTO>();

            foreach (var recuperacion in recuperaciones)
            {
                resultado.Add(new ObtenerRecuperacionDTO
                {
                    Id = recuperacion.Id,
                    UsuarioId = recuperacion.UsuarioId,
                    JobId = recuperacion.JobId,
                    TipoRecuperacion = recuperacion.TipoRecuperacion,
                    PuntoTiempo = recuperacion.PuntoTiempo,
                    InputPath = recuperacion.InputPath,
                    Estado = recuperacion.Estado,
                    IsSimulacion = recuperacion.IsSimulacion,
                    FechaCreacion = DateTime.UtcNow,
                    CompletedAt = recuperacion.CompletedAt
                });
            }

            return resultado;
        }

        public async Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            var recuperaciones = await _context.Recuperacion
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            var resultado = new List<ObtenerRecuperacionDTO>();

            foreach (var recuperacion in recuperaciones)
            {
                resultado.Add(new ObtenerRecuperacionDTO
                {
                    Id = recuperacion.Id,
                    UsuarioId = recuperacion.UsuarioId,
                    JobId = recuperacion.JobId,
                    TipoRecuperacion = recuperacion.TipoRecuperacion,
                    PuntoTiempo = recuperacion.PuntoTiempo,
                    InputPath = recuperacion.InputPath,
                    Estado = recuperacion.Estado,
                    IsSimulacion = recuperacion.IsSimulacion,
                    FechaCreacion = DateTime.UtcNow,
                    CompletedAt = recuperacion.CompletedAt
                });
            }

            return resultado;
        }

        public async Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerPorJobAsync(int jobId)
        {
            var recuperaciones = await _context.Recuperacion
                .Where(r => r.JobId == jobId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            var resultado = new List<ObtenerRecuperacionDTO>();

            foreach (var recuperacion in recuperaciones)
            {
                resultado.Add(new ObtenerRecuperacionDTO
                {
                    Id = recuperacion.Id,
                    UsuarioId = recuperacion.UsuarioId,
                    JobId = recuperacion.JobId,
                    TipoRecuperacion = recuperacion.TipoRecuperacion,
                    PuntoTiempo = recuperacion.PuntoTiempo,
                    InputPath = recuperacion.InputPath,
                    Estado = recuperacion.Estado,
                    IsSimulacion = recuperacion.IsSimulacion,
                    FechaCreacion = DateTime.UtcNow,
                    CompletedAt = recuperacion.CompletedAt
                });
            }

            return resultado;
        }

        public async Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerPorEstadoAsync(string estado)
        {
            var recuperaciones = await _context.Recuperacion
                .Where(r => r.Estado == estado)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            var resultado = new List<ObtenerRecuperacionDTO>();

            foreach (var recuperacion in recuperaciones)
            {
                resultado.Add(new ObtenerRecuperacionDTO
                {
                    Id = recuperacion.Id,
                    UsuarioId = recuperacion.UsuarioId,
                    JobId = recuperacion.JobId,
                    TipoRecuperacion = recuperacion.TipoRecuperacion,
                    PuntoTiempo = recuperacion.PuntoTiempo,
                    InputPath = recuperacion.InputPath,
                    Estado = recuperacion.Estado,
                    IsSimulacion = recuperacion.IsSimulacion,
                    FechaCreacion = DateTime.UtcNow,
                    CompletedAt = recuperacion.CompletedAt
                });
            }

            return resultado;
        }

        public async Task<ObtenerRecuperacionDTO> CrearAsync(SaveRecuperacionDTO saveRecuperacion)
        {
            var recuperacion = new Recuperacion
            {
                UsuarioId = saveRecuperacion.UsuarioId,
                JobId = saveRecuperacion.JobId,
                TipoRecuperacion = saveRecuperacion.TipoRecuperacion,
                PuntoTiempo = saveRecuperacion.PuntoTiempo,
                InputPath = saveRecuperacion.InputPath,
                Estado = "pendiente",
                IsSimulacion = saveRecuperacion.IsSimulacion
            };

            _context.Recuperacion.Add(recuperacion);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(recuperacion.Id);
        }

        public async Task<ObtenerRecuperacionDTO> ActualizarAsync(UpdateRecuperacionDTO updateRecuperacion)
        {
            var recuperacion = await _context.Recuperacion.FindAsync(updateRecuperacion.Id);
            if (recuperacion == null) return null;

            if (!string.IsNullOrEmpty(updateRecuperacion.Estado)) recuperacion.Estado = updateRecuperacion.Estado;
            if (updateRecuperacion.CompletedAt.HasValue) recuperacion.CompletedAt = updateRecuperacion.CompletedAt.Value;

            _context.Recuperacion.Update(recuperacion);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(recuperacion.Id);
        }

        public async Task<bool> EliminarAsync(RemoveRecuperacionDTO removeRecuperacion)
        {
            var recuperacion = await _context.Recuperacion.FindAsync(removeRecuperacion.Id);
            if (recuperacion == null) return false;

            _context.Recuperacion.Remove(recuperacion);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Recuperacion.AnyAsync(r => r.Id == id);
        }
    }
}
