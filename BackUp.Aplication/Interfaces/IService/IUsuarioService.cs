using BackUp.Aplication.Dtos.Usuario;
using BackUp.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BackUp.Aplication.Interfaces.IService
{
    public interface IUsuarioService
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveUsuarioDTO saveUsuario);
        Task<OperationResult> ActualizarAsync(UpdateUsuarioDTO updateUsuario);
        Task<OperationResult> EliminarAsync(RemoveUsuarioDTO removeUsuario);
        Task<OperationResult> ObtenerPorEmailAsync(string email); // AÑADIDO
    }
}
