using BackUp.Aplication.Dtos.Usuario;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Domain.Base;
using BackUp.Domainn.Entities.Users;

namespace BackUp.Aplication.Interfaces.Repositorios
{
    public interface IUsuarioRepository
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveUsuarioDTO saveUsuario);
        Task<OperationResult> ActualizarAsync(UpdateUsuarioDTO updateUsuario);
        Task<OperationResult> EliminarAsync(RemoveUsuarioDTO removeUsuario);
        Task<OperationResult> ObtenerPorEmailAsync(string email);
    }

}
