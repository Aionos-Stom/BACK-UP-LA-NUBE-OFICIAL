
using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Aplication.Services.OrganizacionService;
using BackUp.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;


namespace BackUp.IOC1.Dependencies1
{
    public static class OrganizacionDependency
    {
        public static void AddOrganizacionDependency(this IServiceCollection service)
        {
            service.AddScoped<IOrganizacionRepository, OrganizacionRepository>();

            service.AddScoped<IOrganizacionService, OrganizacionService>();

        }

    }
}
