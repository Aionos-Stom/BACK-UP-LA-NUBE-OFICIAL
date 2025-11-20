using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Aplication.Services.CloudStorageService;
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
    public static class CloudStorageDependency
    {
        public static void AddCloudStorageDependency(this IServiceCollection service)
        {
            service.AddScoped<ICloudStorageService, CloudStorageService>();
            service.AddScoped<ICloudStorageRepository, CloudStorageRepository>();
        }

    }
}
