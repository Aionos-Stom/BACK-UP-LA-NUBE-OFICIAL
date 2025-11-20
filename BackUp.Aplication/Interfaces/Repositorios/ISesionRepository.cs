using BackUp.Application.Dtos.Sesion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Interfaces.Repositorios
{
    public interface ISesionRepository
    {
        Task<ObtenerSesionDTO> ObtenerPorIdAsync(int id);
        Task<ObtenerSesionDTO> ObtenerPorTokenAsync(string token);
        Task<IEnumerable<ObtenerSesionDTO>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<ObtenerSesionDTO>> ObtenerTodasAsync();
        Task<IEnumerable<ObtenerSesionDTO>> ObtenerExpiradasAsync();
        Task<ObtenerSesionDTO> CrearAsync(SaveSesionDTO saveSesion);
        Task<ObtenerSesionDTO> ActualizarAsync(UpdateSesionDTO updateSesion);
        Task<bool> EliminarAsync(RemoveSesionDTO removeSesion);
        Task<int> EliminarExpiradasAsync();
        Task<bool> ExisteAsync(int id);
    }
}
