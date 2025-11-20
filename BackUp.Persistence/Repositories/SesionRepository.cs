using BackUp.Application.Dtos.Sesion;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Domainn.Entities.Users;
using BackUp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Persistence.Repositories
{
    public class SesionRepository : ISesionRepository
    {
        private readonly BackUpDbContext _context;

        public SesionRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<ObtenerSesionDTO> ObtenerPorIdAsync(int id)
        {
            var sesion = await _context.Sesion
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sesion == null) return null;

            return new ObtenerSesionDTO
            {
                Id = sesion.Id,
                UsuarioId = sesion.UsuarioId,
                UsuarioNombre = sesion.Usuario.Nombre,
                UsuarioEmail = sesion.Usuario.Email,
                Token = sesion.Token,
                ExpiresAt = sesion.ExpiresAt,
                IpAddress = sesion.IpAddress,
                UserAgent = sesion.UserAgent,
                CreatedAt = DateTime.UtcNow // Valor temporal
            };
        }

        public async Task<ObtenerSesionDTO> ObtenerPorTokenAsync(string token)
        {
            var sesion = await _context.Sesion
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(s => s.Token == token);

            if (sesion == null) return null;

            return new ObtenerSesionDTO
            {
                Id = sesion.Id,
                UsuarioId = sesion.UsuarioId,
                UsuarioNombre = sesion.Usuario.Nombre,
                UsuarioEmail = sesion.Usuario.Email,
                Token = sesion.Token,
                ExpiresAt = sesion.ExpiresAt,
                IpAddress = sesion.IpAddress,
                UserAgent = sesion.UserAgent,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<ObtenerSesionDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _context.Sesion
                .Include(s => s.Usuario)
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.Id)
                .Select(s => new ObtenerSesionDTO
                {
                    Id = s.Id,
                    UsuarioId = s.UsuarioId,
                    UsuarioNombre = s.Usuario.Nombre,
                    UsuarioEmail = s.Usuario.Email,
                    Token = s.Token,
                    ExpiresAt = s.ExpiresAt,
                    IpAddress = s.IpAddress,
                    UserAgent = s.UserAgent,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ObtenerSesionDTO>> ObtenerTodasAsync()
        {
            return await _context.Sesion
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.Id)
                .Select(s => new ObtenerSesionDTO
                {
                    Id = s.Id,
                    UsuarioId = s.UsuarioId,
                    UsuarioNombre = s.Usuario.Nombre,
                    UsuarioEmail = s.Usuario.Email,
                    Token = s.Token,
                    ExpiresAt = s.ExpiresAt,
                    IpAddress = s.IpAddress,
                    UserAgent = s.UserAgent,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ObtenerSesionDTO>> ObtenerExpiradasAsync()
        {
            return await _context.Sesion
                .Include(s => s.Usuario)
                .Where(s => s.ExpiresAt < DateTime.UtcNow)
                .OrderByDescending(s => s.Id)
                .Select(s => new ObtenerSesionDTO
                {
                    Id = s.Id,
                    UsuarioId = s.UsuarioId,
                    UsuarioNombre = s.Usuario.Nombre,
                    UsuarioEmail = s.Usuario.Email,
                    Token = s.Token,
                    ExpiresAt = s.ExpiresAt,
                    IpAddress = s.IpAddress,
                    UserAgent = s.UserAgent,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }

        public async Task<ObtenerSesionDTO> CrearAsync(SaveSesionDTO saveSesion)
        {
            var sesion = new Sesion
            {
                UsuarioId = saveSesion.UsuarioId,
                Token = saveSesion.Token,
                ExpiresAt = saveSesion.ExpiresAt,
                IpAddress = saveSesion.IpAddress,
                UserAgent = saveSesion.UserAgent
            };

            _context.Sesion.Add(sesion);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(sesion.Id);
        }

        public async Task<ObtenerSesionDTO> ActualizarAsync(UpdateSesionDTO updateSesion)
        {
            var sesion = await _context.Sesion.FindAsync(updateSesion.Id);
            if (sesion == null) return null;

            if (updateSesion.ExpiresAt.HasValue) sesion.ExpiresAt = updateSesion.ExpiresAt.Value;
            if (!string.IsNullOrEmpty(updateSesion.IpAddress)) sesion.IpAddress = updateSesion.IpAddress;
            if (!string.IsNullOrEmpty(updateSesion.UserAgent)) sesion.UserAgent = updateSesion.UserAgent;

            _context.Sesion.Update(sesion);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(sesion.Id);
        }

        public async Task<bool> EliminarAsync(RemoveSesionDTO removeSesion)
        {
            var sesion = await _context.Sesion.FindAsync(removeSesion.Id);
            if (sesion == null) return false;

            _context.Sesion.Remove(sesion);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> EliminarExpiradasAsync()
        {
            var expiradas = _context.Sesion.Where(s => s.ExpiresAt < DateTime.UtcNow);
            int count = await expiradas.CountAsync();

            _context.Sesion.RemoveRange(expiradas);
            await _context.SaveChangesAsync();

            return count;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Sesion.AnyAsync(s => s.Id == id);
        }
    }
}
