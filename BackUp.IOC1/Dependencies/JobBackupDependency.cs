using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Aplication.Services.JobBackupService;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.IOC1.Dependencies
{
    public static class JobBackupDependency
    {
        public static void AddJobBackUpDependency(this IServiceCollection service)
        {
            service.AddScoped<IJobBackupRepository, JobBackupRepository>();

            service.AddScoped<IJobBackupService, JobBackupService>();
        }

    }
}
