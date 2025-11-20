
using BackUp.Application.Interfaces.IService;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Application.Services.AlertaService;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.IOC1.Dependencies
{
    public static class AlertaDependency
    {
        public static void AddAlertaDependency(this IServiceCollection service)
        {
            service.AddScoped<IAlertaRepository, AlertaRepository>();

            service.AddScoped<IAlertaService, AlertaService>();
        }

    }
}
