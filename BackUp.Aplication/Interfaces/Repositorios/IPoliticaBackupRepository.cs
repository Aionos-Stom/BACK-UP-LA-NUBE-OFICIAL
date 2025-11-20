
using BackUp.Aplication.Dtos.politicaBackup;
using BackUp.Aplication.Dtos.PoliticaBackup;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Domain.Base;

using System.Linq.Expressions;

namespace BackUp.Aplication.Interfaces.Repositorios
{
    public interface IPoliticaBackupRepository
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SavePoliticaBackupDTO savePoliticaBackup);
        Task<OperationResult> ActualizarAsync(UpdatePoliticaBackupDTO updatePoliticaBackup);
        Task<OperationResult> EliminarAsync(RemovePoliticaBackupDTO removePoliticaBackup);
        Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId);
        Task<OperationResult> ObtenerActivasAsync();
        Task<OperationResult> ObtenerPorTipoAsync(string tipoBackup);
    }

}