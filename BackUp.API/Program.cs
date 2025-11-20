using BackUp.Aplication.Interfaces.Base;
using BackUp.Application.Interfaces.IService;
using BackUp.Application.Services.BackUp;
using BackUp.IOC1.Dependencies;
using BackUp.IOC1.Dependencies1;
using BackUp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;


namespace BackUp.API
{
     public class Program
    {
          public static void Main(string[] args)
        {
                    var builder = WebApplication.CreateBuilder(args);
                                //  Swagger
                                builder.Services.AddEndpointsApiExplorer();
                                builder.Services.AddSwaggerGen(c =>
                                {
                                    c.SwaggerDoc("v1", new OpenApiInfo
                                    {
                                        Title = "BackUp API",
                                        Version = "v1",
                                        Description = "Documentación de la API de BackUp",
                                        Contact = new OpenApiContact
                                        {
                                            Name = "Soporte",
                                            Email = "soporte@BackUp.com"
                                        }
                                    });
                                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                                    c.IncludeXmlComments(xmlPath);
                                });

                                // Add services to the container.

                                builder.Services.AddCloudStorageDependency();
                                 builder.Services.AddJobBackUpDependency();
                                  builder.Services.AddOrganizacionDependency();
                                    builder.Services.AddPoliticaBackupdependency();
                                     builder.Services.AddDashboardDependency();
                                   builder.Services.AddScoped<IBackupService, BackupService>();
                                    builder.Services.AddAlertaDependency();
                                    builder.Services.AddRecuperacionDependency();
                                    builder.Services.AddSesionDependency();
                                      builder.Services.AddUsuariodependency();
                                         builder.Services.AddVerificacionIntegridaddependency();

            builder.Services.AddDbContext<BackUpDbContext>(options =>
                                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                                    builder.Services.AddScoped<IApplicationDbContext>(provider =>
                                    provider.GetRequiredService<BackUpDbContext>());


                                // Servicios
                                //builder.Services.AddScoped<IPedidoService,PedidoService>();

                                builder.Services.AddControllersWithViews();

                                var app = builder.Build();
          
         


                                // Middleware de Swagger
                                if (app.Environment.IsDevelopment())
                                {
                                    app.UseDeveloperExceptionPage();
                                    app.UseSwagger();
                                    app.UseSwaggerUI(c =>
                                    {

                                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackUp API v1");
                                        c.DisplayRequestDuration(); // Muestra duración de las peticiones
                                        c.EnableDeepLinking(); // Permite enlaces directos a secciones
                                        c.DefaultModelsExpandDepth(-1);
                                    });
                                }
                                else
                                {
                                    app.UseExceptionHandler("/Home/Error");
                                }

                                // Configure the HTTP request pipeline.
                                if (!app.Environment.IsDevelopment())
                                {
                                    app.UseExceptionHandler("/Home/Error");
                                }
                                app.UseStaticFiles();

                                app.UseRouting();

                                app.UseAuthorization();

                                app.MapControllerRoute(
                                    name: "default",
                                    pattern: "{controller=Home}/{action=Index}/{id?}");

                                app.Run();
           }

     }
}
