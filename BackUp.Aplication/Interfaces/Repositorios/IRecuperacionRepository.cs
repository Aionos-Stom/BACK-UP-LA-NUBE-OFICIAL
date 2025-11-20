using BackUp.Aplication.Dtos.Recuperacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Interfaces.Repositorios
{
    public interface IRecuperacionRepository
    {
        Task<ObtenerRecuperacionDTO> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerTodosAsync();
        Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerPorJobAsync(int jobId);
        Task<IEnumerable<ObtenerRecuperacionDTO>> ObtenerPorEstadoAsync(string estado);
        Task<ObtenerRecuperacionDTO> CrearAsync(SaveRecuperacionDTO saveRecuperacion);
        Task<ObtenerRecuperacionDTO> ActualizarAsync(UpdateRecuperacionDTO updateRecuperacion);
        Task<bool> EliminarAsync(RemoveRecuperacionDTO removeRecuperacion);
        Task<bool> ExisteAsync(int id);
    }
}
