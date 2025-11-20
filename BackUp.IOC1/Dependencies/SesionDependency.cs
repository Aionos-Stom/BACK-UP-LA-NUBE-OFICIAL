using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Services.RecuperacionService;
using BackUp.Application.Interfaces.IService;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Application.Services.SesionService;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.IOC1.Dependencies
{
    public static class SesionDependency
    {
        public static void AddSesionDependency(this IServiceCollection services)
        {
            services.AddScoped<ISesionRepository, SesionRepository>();
            services.AddScoped<ISesionService, SesionService>();
        }
    }
}