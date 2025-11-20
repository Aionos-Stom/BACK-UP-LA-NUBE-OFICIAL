using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Aplication.Services.PoliticaBackupService;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.IOC1.Dependencies
{
    public static class PoliticaBackupDependency
    {
        public static void AddPoliticaBackupdependency(this IServiceCollection services)
        {
            services.AddScoped<IPoliticaBackupRepository, PoliticaBackupRepository>();
            services.AddScoped<IPoliticaBackupService, PoliticaBackupService>();
        }

    }
}
