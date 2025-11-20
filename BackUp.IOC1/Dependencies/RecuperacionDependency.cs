using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Services.RecuperacionService;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.IOC1.Dependencies
{
    public static class RecuperacionDependency
    {
        public static void AddRecuperacionDependency(this IServiceCollection services)
        {
            services.AddScoped<IRecuperacionRepository, RecuperaciónRepository>();
            services.AddScoped<IRecuperacionService, RecuperacionService>();
        }
    }
}
