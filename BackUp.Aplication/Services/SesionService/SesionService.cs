using BackUp.Application.Dtos.Sesion;
using BackUp.Application.Interfaces.IService;
using BackUp.Application.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Services.SesionService
{
    public sealed class SesionService : ISesionService
    {
        private readonly ISesionRepository _sesionRepository;

        public SesionService(ISesionRepository sesionRepository)
        {
            _sesionRepository = sesionRepository;
        }

        public async Task<ObtenerSesionDTO> ObtenerPorIdAsync(int id)
        {
            return await _sesionRepository.ObtenerPorIdAsync(id);
        }

        public async Task<ObtenerSesionDTO> ObtenerPorTokenAsync(string token)
        {
            return await _sesionRepository.ObtenerPorTokenAsync(token);
        }

        public async Task<IEnumerable<ObtenerSesionDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _sesionRepository.ObtenerPorUsuarioAsync(usuarioId);
        }

        public async Task<IEnumerable<ObtenerSesionDTO>> ObtenerTodasAsync()
        {
            return await _sesionRepository.ObtenerTodasAsync();
        }

        public async Task<IEnumerable<ObtenerSesionDTO>> ObtenerExpiradasAsync()
        {
            return await _sesionRepository.ObtenerExpiradasAsync();
        }

        public async Task<ObtenerSesionDTO> CrearAsync(SaveSesionDTO saveSesion)
        {
            return await _sesionRepository.CrearAsync(saveSesion);
        }

        public async Task<ObtenerSesionDTO> ActualizarAsync(UpdateSesionDTO updateSesion)
        {
            return await _sesionRepository.ActualizarAsync(updateSesion);
        }

        public async Task<bool> EliminarAsync(RemoveSesionDTO removeSesion)
        {
            return await _sesionRepository.EliminarAsync(removeSesion);
        }

        public async Task<bool> EliminarExpiradasAsync()
        {
            var resultado = await _sesionRepository.EliminarExpiradasAsync();
            return resultado > 0;
        }

        public async Task<bool> ValidarTokenAsync(string token)
        {
            var sesion = await _sesionRepository.ObtenerPorTokenAsync(token);
            return sesion != null && !sesion.IsExpired;
        }

        public async Task<bool> ExtenderSesionAsync(int id, DateTime nuevaExpiracion)
        {
            var updateSesion = new UpdateSesionDTO
            {
                Id = id,
                ExpiresAt = nuevaExpiracion
            };

            var resultado = await _sesionRepository.ActualizarAsync(updateSesion);
            return resultado != null;
        }
     }
}

