using BackUp.Aplication.Dtos.Usuario;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly BackUpDbContext _context;

        public UsuarioRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            try
            {
                var usuario = await _context.Usuario
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

                if (usuario == null)
                    return OperationResult.Failure("Usuario no encontrado");

                var dto = new ObtenerUsuarioDTO
                {
                    Id = usuario.Id,
                    OrganizacionId = usuario.OrganizacionId,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Rol = usuario.Rol,
                    MfaHabilitado = usuario.MfaHabilitado,
                    PhoneNumber = usuario.PhoneNumber,
                    LastLogin = usuario.LastLogin,
                    IsActive = usuario.IsActive,
                    FechaCreacion = usuario.CreatedAt
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener usuario: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            try
            {
                var usuarios = await _context.Usuario
                    .AsNoTracking()
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.Nombre)
                    .ToListAsync();

                var dtos = usuarios.Select(u => new ObtenerUsuarioDTO
                {
                    Id = u.Id,
                    OrganizacionId = u.OrganizacionId,
                    Email = u.Email,
                    Nombre = u.Nombre,
                    Rol = u.Rol,
                    MfaHabilitado = u.MfaHabilitado,
                    PhoneNumber = u.PhoneNumber,
                    LastLogin = u.LastLogin,
                    IsActive = u.IsActive,
                    FechaCreacion = u.CreatedAt
                }).ToList();

                return OperationResult.Success(dtos);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener usuarios: {ex.Message}");
            }
        }

        public async Task<OperationResult> AgregarAsync(SaveUsuarioDTO saveUsuario)
        {
            try
            {
                // Verificar si el email ya existe
                var usuarioExistente = await _context.Usuario
                    .AnyAsync(u => u.Email == saveUsuario.Email);

                if (usuarioExistente)
                    return OperationResult.Failure("El email ya está registrado");

                var usuario = new Usuario
                {
                    OrganizacionId = saveUsuario.OrganizacionId,
                    Email = saveUsuario.Email,
                    PasswordHash = HashPassword(saveUsuario.Password),
                    Nombre = saveUsuario.Nombre,
                    Rol = saveUsuario.Rol,
                    PhoneNumber = saveUsuario.PhoneNumber,
                    IsActive = true,
                    MfaHabilitado = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Usuario.AddAsync(usuario);
                await _context.SaveChangesAsync();

                var dto = new ObtenerUsuarioDTO
                {
                    Id = usuario.Id,
                    OrganizacionId = usuario.OrganizacionId,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Rol = usuario.Rol,
                    MfaHabilitado = usuario.MfaHabilitado,
                    PhoneNumber = usuario.PhoneNumber,
                    LastLogin = usuario.LastLogin,
                    IsActive = usuario.IsActive,
                    FechaCreacion = usuario.CreatedAt
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al agregar usuario: {ex.Message}");
            }
        }

        public async Task<OperationResult> ActualizarAsync(UpdateUsuarioDTO updateUsuario)
        {
            try
            {
                var usuario = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.Id == updateUsuario.Id);

                if (usuario == null)
                    return OperationResult.Failure("Usuario no encontrado");

                // Verificar si el email ya existe (excluyendo el usuario actual)
                var usuarioConEmail = await _context.Usuario
                    .AnyAsync(u => u.Email == updateUsuario.Email && u.Id != updateUsuario.Id);

                if (usuarioConEmail)
                    return OperationResult.Failure("El email ya está en uso por otro usuario");

                usuario.Email = updateUsuario.Email;
                usuario.Nombre = updateUsuario.Nombre;
                usuario.Rol = updateUsuario.Rol;
                usuario.PhoneNumber = updateUsuario.PhoneNumber;
                usuario.IsActive = updateUsuario.IsActive;

                await _context.SaveChangesAsync();

                var dto = new ObtenerUsuarioDTO
                {
                    Id = usuario.Id,
                    OrganizacionId = usuario.OrganizacionId,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Rol = usuario.Rol,
                    MfaHabilitado = usuario.MfaHabilitado,
                    PhoneNumber = usuario.PhoneNumber,
                    LastLogin = usuario.LastLogin,
                    IsActive = usuario.IsActive,
                    FechaCreacion = usuario.CreatedAt
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al actualizar usuario: {ex.Message}");
            }
        }

        public async Task<OperationResult> EliminarAsync(RemoveUsuarioDTO removeUsuario)
        {
            try
            {
                var usuario = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.Id == removeUsuario.Id);

                if (usuario == null)
                    return OperationResult.Failure("Usuario no encontrado");

                // Soft delete
                usuario.IsActive = false;

                await _context.SaveChangesAsync();

                return OperationResult.Success("Usuario eliminado correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al eliminar usuario: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorEmailAsync(string email)
        {
            try
            {
                var usuario = await _context.Usuario
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

                if (usuario == null)
                    return OperationResult.Failure("Usuario no encontrado");

                var dto = new ObtenerUsuarioDTO
                {
                    Id = usuario.Id,
                    OrganizacionId = usuario.OrganizacionId,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Rol = usuario.Rol,
                    MfaHabilitado = usuario.MfaHabilitado,
                    PhoneNumber = usuario.PhoneNumber,
                    LastLogin = usuario.LastLogin,
                    IsActive = usuario.IsActive,
                    FechaCreacion = usuario.CreatedAt
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener usuario por email: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            // Implementar hashing seguro (BCrypt, Argon2, etc.)
            // Ejemplo básico - en producción usar una librería especializada
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
