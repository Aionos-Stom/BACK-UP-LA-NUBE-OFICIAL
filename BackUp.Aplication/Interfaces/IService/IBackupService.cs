using BackUp.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackUp.Application.Dtos.Dashboard;

namespace BackUp.Application.Interfaces.IService
{
    public interface IBackupService
    {
       
        Task<OperationResult> RestaurarBackupAsync(int JobId, string destino);
        Task<OperationResult> VerificarIntegridadAsync(int JobId);
       
    }
}
