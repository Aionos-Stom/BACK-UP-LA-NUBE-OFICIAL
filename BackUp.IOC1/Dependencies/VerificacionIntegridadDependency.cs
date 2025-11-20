using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Application.Services.VerificacionIntegridadService;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.IOC1.Dependencies
{
    public static class VerificacionIntegridadDependency
    {
        public static void AddVerificacionIntegridaddependency(this IServiceCollection services)
        {
            services.AddScoped<IVerificacionIntegridadRepository, VerificacionIntegridadRepository>();
            services.AddScoped<IVerificacionIntegridadService, VerificacionIntegridadService>();
        }
    }
}
