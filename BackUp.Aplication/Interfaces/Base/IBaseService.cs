using BackUp.Domain.Base;


namespace BackUp.Aplication.Interfaces.Base
{
    public interface IBaseService<TSaveDto, TUpdateDto, TRemoveDto>
    {
        Task<OperationResult> AgregarAsync(TSaveDto dto);
        Task<OperationResult> EliminarAsync(TRemoveDto dto);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ActualizarAsync(TUpdateDto dto);
    }
}
