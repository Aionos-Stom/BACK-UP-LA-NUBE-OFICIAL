using BackUp.Application.Dtos.Sesion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Interfaces.IService
{
    public interface ISesionService
    {
        Task<ObtenerSesionDTO> ObtenerPorIdAsync(int id);
        Task<ObtenerSesionDTO> ObtenerPorTokenAsync(string token);
        Task<IEnumerable<ObtenerSesionDTO>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<ObtenerSesionDTO>> ObtenerTodasAsync();
        Task<ObtenerSesionDTO> CrearAsync(SaveSesionDTO saveSesion);
        Task<ObtenerSesionDTO> ActualizarAsync(UpdateSesionDTO updateSesion);
        Task<bool> EliminarAsync(RemoveSesionDTO removeSesion);
        Task<bool> EliminarExpiradasAsync();
        Task<bool> ValidarTokenAsync(string token);
        Task<bool> ExtenderSesionAsync(int id, DateTime nuevaExpiracion);
    }
}
