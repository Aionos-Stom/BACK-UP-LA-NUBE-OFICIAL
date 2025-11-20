using BackUp.Aplication.Dtos.Usuario;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domainn.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BackUp.Aplication.Services.UsuarioService
{
    public sealed class UsuarioService : IUsuarioService
    {
        private readonly ILogger<UsuarioService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(
            ILogger<UsuarioService> logger,
            IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            return await _usuarioRepository.ObtenerPorIdAsync(id);
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            return await _usuarioRepository.ObtenerTodosAsync();
        }

        public async Task<OperationResult> AgregarAsync(SaveUsuarioDTO saveUsuario)
        {
            return await _usuarioRepository.AgregarAsync(saveUsuario);
        }

        public async Task<OperationResult> ActualizarAsync(UpdateUsuarioDTO updateUsuario)
        {
            return await _usuarioRepository.ActualizarAsync(updateUsuario);
        }

        public async Task<OperationResult> EliminarAsync(RemoveUsuarioDTO removeUsuario)
        {
            return await _usuarioRepository.EliminarAsync(removeUsuario);
        }

        // Método adicional para obtener usuario por email
        public async Task<OperationResult> ObtenerPorEmailAsync(string email)
        {
            return await _usuarioRepository.ObtenerPorEmailAsync(email);
        }
    }
}
