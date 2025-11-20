using BackUp.Aplication.Dtos.politicaBackup;
using BackUp.Aplication.Dtos.PoliticaBackup;
using BackUp.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Interfaces.IService
{
    public interface IPoliticaBackupService
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> CrearAsync(SavePoliticaBackupDTO savePoliticaBackup);
        Task<OperationResult> ActualizarAsync(UpdatePoliticaBackupDTO updatePoliticaBackup);
        Task<OperationResult> EliminarAsync(RemovePoliticaBackupDTO removePoliticaBackup);
        Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId);
        Task<OperationResult> ObtenerActivasAsync();
        Task<OperationResult> ObtenerPorTipoAsync(string tipoBackup);
        Task<OperationResult> CambiarEstadoAsync(int id, bool activo);
    }
}
