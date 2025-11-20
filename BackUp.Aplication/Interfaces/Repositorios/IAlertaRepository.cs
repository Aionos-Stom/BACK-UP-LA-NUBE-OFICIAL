using BackUp.Application.Dtos.Alerta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Interfaces.Repositorios
{
    public interface IAlertaRepository
    {
        Task<ObtenerAlertaDTO> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ObtenerAlertaDTO>> ObtenerTodosAsync();
        Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<ObtenerAlertaDTO>> ObtenerNoReconocidasAsync();
        Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorJobAsync(int? jobId); // AÑADIDO
        Task<IEnumerable<ObtenerAlertaDTO>> ObtenerPorSeveridadAsync(string severidad); // AÑADIDO
        Task<ObtenerAlertaDTO> CrearAsync(SaveAlertaDTO saveAlerta);
        Task<ObtenerAlertaDTO> ActualizarAsync(UpdateAlertaDTO updateAlerta);
        Task<bool> EliminarAsync(RemoveAlertaDTO removeAlerta);
        Task<bool> ExisteAsync(int id);
        Task<int> ContarAlertasNoReconocidasAsync();

    }
}
