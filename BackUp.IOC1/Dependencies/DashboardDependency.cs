
using BackUp.Application.Interfaces.IService;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Application.Services.Dashboard;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackUp.IOC1.Dependencies
{
    public static class DashboardDependency
    {
        public static void AddDashboardDependency(this IServiceCollection service)
        {
            service.AddScoped<IDashboardRepository, DashboardRepository>();

            service.AddScoped<IDashboardService, DashboardService>();
        }
    }
}
