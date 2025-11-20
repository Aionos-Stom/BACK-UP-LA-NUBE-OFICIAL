using BackUp.Application.Dtos.Alerta;
using BackUp.Application.Interfaces.IService;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Domain.Entities.Bac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Services.AlertaService
{
    public sealed class AlertaService : IAlertaService
    {
        private readonly IAlertaRepository _alertaRepository;

        public AlertaService(IAlertaRepository alertaRepository)
        {
            _alertaRepository = alertaRepository;
        }

        public async Task<ObtenerAlertaDTO> ObtenerPorIdAsync(int id)
        {
            return await _alertaRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerTodosAsync()
        {
            return await _alertaRepository.ObtenerTodosAsync();
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _alertaRepository.ObtenerPorUsuarioAsync(usuarioId);
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerNoReconocidasAsync()
        {
            return await _alertaRepository.ObtenerNoReconocidasAsync();
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorJobAsync(int? jobId)
        {
            return await _alertaRepository.ObtenerPorJobAsync(jobId);
        }

        public async Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorSeveridadAsync(string severidad)
        {
            return await _alertaRepository.ObtenerPorSeveridadAsync(severidad);
        }

        public async Task<ObtenerAlertaDTO> CrearAsync(SaveAlertaDTO saveAlerta)
        {
            return await _alertaRepository.CrearAsync(saveAlerta);
        }

        public async Task<ObtenerAlertaDTO> ActualizarAsync(UpdateAlertaDTO updateAlerta)
        {
            return await _alertaRepository.ActualizarAsync(updateAlerta);
        }

        public async Task<bool> EliminarAsync(RemoveAlertaDTO removeAlerta)
        {
            return await _alertaRepository.EliminarAsync(removeAlerta);
        }

        public async Task<bool> MarcarComoReconocidaAsync(int id)
        {
            var updateAlerta = new UpdateAlertaDTO
            {
                Id = id,
                IsAcknowledged = true
            };

            var resultado = await _alertaRepository.ActualizarAsync(updateAlerta);
            return resultado != null;
        }

        public async Task<int> ContarAlertasNoReconocidasAsync()
        {
            return await _alertaRepository.ContarAlertasNoReconocidasAsync();
        }

    }
}
