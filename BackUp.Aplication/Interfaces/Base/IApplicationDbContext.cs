using Microsoft.EntityFrameworkCore;
using BackUp.Domainn.Entities.Users;
using BackUp.Domain.Entities.Bac;


namespace BackUp.Aplication.Interfaces.Base
{
    public interface IApplicationDbContext
    {
        // Entidades del Sistema de Backup Multi-Cloud
        DbSet<Organizacion> Organizacion { get; }
        DbSet<CloudStorage> CloudStorage { get; }
        DbSet<PoliticaBackup> PoliticaBackup { get; }
        DbSet<Usuario> Usuario { get; }
        DbSet<JobBackup> JobBackup { get; }
        DbSet<Recuperacion> Recuperacion { get; }
        DbSet<VerificacionIntegridad> VerificacionIntegridad { get; }
        DbSet<Alerta> Alerta { get; }
        DbSet<Sesion> Sesion { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
