using Microsoft.EntityFrameworkCore;
using BackUp.Domainn.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Domain.Entities.Bac;

namespace BackUp.Persistence.Context
{
    public class BackUpDbContext : DbContext, IApplicationDbContext
    {
        public BackUpDbContext(DbContextOptions<BackUpDbContext> options) : base(options)
        { }

        // Implementación de la interfaz IApplicationDbContext
        public DbSet<Organizacion> Organizacion { get; set; }
        public DbSet<CloudStorage> CloudStorage { get; set; }
        public DbSet<PoliticaBackup> PoliticaBackup { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<JobBackup> JobBackup { get; set; }
        public DbSet<Recuperacion> Recuperacion { get; set; }
        public DbSet<VerificacionIntegridad> VerificacionIntegridad { get; set; }
        public DbSet<Alerta> Alerta { get; set; }
        public DbSet<Sesion> Sesion { get; set; }

        Task<int> IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
            => base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear las entidades a los nombres reales de las tablas en la BD
            modelBuilder.Entity<Organizacion>().ToTable("Organizacion");
            modelBuilder.Entity<CloudStorage>().ToTable("CloudStorage");
            modelBuilder.Entity<PoliticaBackup>().ToTable("PoliticaBackup");
            modelBuilder.Entity<JobBackup>().ToTable("JobBackup");
            modelBuilder.Entity<Usuario>().ToTable("Usuario"); // La tabla se llama USERS, no Usuario
            modelBuilder.Entity<Recuperacion>().ToTable("Recuperacion");
            modelBuilder.Entity<VerificacionIntegridad>().ToTable("VerificacionIntegridad");
            modelBuilder.Entity<Alerta>().ToTable("Alerta");
            modelBuilder.Entity<Sesion>().ToTable("Sesion");


            // Mapear columnas específicas para Organizacion
            modelBuilder.Entity<Organizacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Configuracion).HasColumnName("configuracion");
                entity.Property(e => e.LicenciaValidaHasta).HasColumnName("licencia_valida_hasta");
                entity.Property(e => e.MaxUsuarios).HasColumnName("max_usuarios");
                entity.Property(e => e.Activo).HasColumnName("activo");
                // fecha_creacion no se mapea ya que no existe en la entidad
            });

            // Configurar CloudStorage
            modelBuilder.Entity<CloudStorage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrganizacionId).HasColumnName("organizacion_id");
                entity.Property(e => e.Proveedor).HasColumnName("proveedor");
                entity.Property(e => e.Configuration).HasColumnName("configuration");
                entity.Property(e => e.EndpointUrl).HasColumnName("endpoint_url");
                entity.Property(e => e.TierActual).HasColumnName("tier_actual");
                entity.Property(e => e.CostoMensual).HasColumnName("costo_mensual");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                // fecha_creacion no se mapea ya que no existe en la entidad
            });

            // Configurar PoliticaBackup
            modelBuilder.Entity<PoliticaBackup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrganizacionId).HasColumnName("organizacion_id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Frecuencia).HasColumnName("frecuencia");
                entity.Property(e => e.TipoBackup).HasColumnName("tipo_backup");
                entity.Property(e => e.RetencionDias).HasColumnName("retencion_dias");
                entity.Property(e => e.RpoMinutes).HasColumnName("rpo_minutes");
                entity.Property(e => e.RtoMinutes).HasColumnName("rto_minutes");
                entity.Property(e => e.VentanaEjecucion).HasColumnName("ventana_ejecucion");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                // fecha_creacion y descripcion no se mapean ya que no existen en la entidad
            });

            // Configurar JobBackup
            modelBuilder.Entity<JobBackup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PoliticaId).HasColumnName("politica_id");
                entity.Property(e => e.CloudStorageId).HasColumnName("cloud_storage_id");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.FechaProgramada).HasColumnName("fecha_programada");
                entity.Property(e => e.FechaEjecucion).HasColumnName("fecha_ejecucion");
                entity.Property(e => e.FechaCompletado).HasColumnName("fecha_completado");
                entity.Property(e => e.TamanoBytes).HasColumnName("tamano_bytes");
                entity.Property(e => e.DuracionSegundos).HasColumnName("duracion_segundos");
                entity.Property(e => e.SourceData).HasColumnName("source_data");
                entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
                // created_at no se mapea ya que no existe en la entidad
            });

            // Configurar Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrganizacionId).HasColumnName("organizacion_id");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Rol).HasColumnName("rol");
                entity.Property(e => e.MfaHabilitado).HasColumnName("mfa_habilitado");
                entity.Property(e => e.MfaSecret).HasColumnName("mfa_secret");
                entity.Property(e => e.LastLogin).HasColumnName("last_login");
                entity.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
            });

            // Configurar Recuperacion
            modelBuilder.Entity<Recuperacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.JobId).HasColumnName("job_id");
                entity.Property(e => e.TipoRecuperacion).HasColumnName("tipo_recuperacion");
                entity.Property(e => e.PuntoTiempo).HasColumnName("punto_tiempo");
                entity.Property(e => e.InputPath).HasColumnName("input_path");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.IsSimulacion).HasColumnName("is_simulacion");
                entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
                // created_at no se mapea ya que no existe en la entidad
            });

            // Configurar VerificacionIntegridad
            modelBuilder.Entity<VerificacionIntegridad>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.job_id).HasColumnName("job_id");
                entity.Property(e => e.ChecksumSha256).HasColumnName("checksum_sha256");
                entity.Property(e => e.Resultado).HasColumnName("resultado");
                entity.Property(e => e.FechaVerificacion).HasColumnName("fecha_verificacion");
                entity.Property(e => e.Detalles).HasColumnName("detalles");
                entity.Property(e => e.IntegrityScore).HasColumnName("integrity_score");
            });

            // Configurar Alerta
            modelBuilder.Entity<Alerta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.JobId).HasColumnName("job_id");
                entity.Property(e => e.Tipo).HasColumnName("tipo");
                entity.Property(e => e.Severidad).HasColumnName("severidad");
                entity.Property(e => e.Mensaje).HasColumnName("mensaje");
                entity.Property(e => e.IsAcknowledged).HasColumnName("is_acknowledged");
                // created_at no se mapea ya que no existe en la entidad
            });

            // Configurar Sesion
            modelBuilder.Entity<Sesion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.Token).HasColumnName("token");
                entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
                entity.Property(e => e.IpAddress).HasColumnName("ip_address");
                entity.Property(e => e.UserAgent).HasColumnName("user_agent");
                // created_at no se mapea ya que no existe en la entidad
            });

            // Configurar relaciones
            ConfigureRelationships(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CloudStorage>()
       .HasOne(cs => cs.Organizacion)
       .WithMany(o => o.CloudStorages)
       .HasForeignKey(cs => cs.OrganizacionId)
       .OnDelete(DeleteBehavior.Cascade);

            // Organizacion -> PoliticaBackup (1 a muchos)
            modelBuilder.Entity<PoliticaBackup>()
                .HasOne(pb => pb.Organizacion)
                .WithMany(o => o.PoliticasBackup)
                .HasForeignKey(pb => pb.OrganizacionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Organizacion -> Usuario (1 a muchos) - CORREGIDO
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Organizacion)  // Usar la propiedad de navegación
                .WithMany(o => o.Usuarios)
                .HasForeignKey(u => u.OrganizacionId)
                .OnDelete(DeleteBehavior.Cascade);

            // PoliticaBackup -> JobBackup (1 a muchos)
            modelBuilder.Entity<JobBackup>()
                .HasOne(jb => jb.Politica)
                .WithMany(pb => pb.JobsBackup)
                .HasForeignKey(jb => jb.PoliticaId)
                .OnDelete(DeleteBehavior.NoAction);

            // CloudStorage -> JobBackup (1 a muchos)
            modelBuilder.Entity<JobBackup>()
                .HasOne(jb => jb.CloudStorage)
                .WithMany(cs => cs.JobsBackup)
                .HasForeignKey(jb => jb.CloudStorageId)
                .OnDelete(DeleteBehavior.NoAction);

            // JobBackup -> VerificacionIntegridad (1 a muchos) - CORREGIDO
            modelBuilder.Entity<VerificacionIntegridad>()
                .HasOne(vi => vi.JobBackup)
                .WithMany(jb => jb.VerificacionesIntegridad)
                .HasForeignKey(vi => vi.job_id)
                .OnDelete(DeleteBehavior.Cascade);

            // JobBackup -> Recuperacion (1 a muchos) - CORREGIDO
            modelBuilder.Entity<Recuperacion>()
                .HasOne(r => r.JobBackup)
                .WithMany(jb => jb.Recuperaciones)
                .HasForeignKey(r => r.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            // JobBackup -> Alerta (1 a muchos)
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.JobBackup)
                .WithMany(jb => jb.Alertas)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            // Usuario -> Alerta (1 a muchos) - CORREGIDO
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Alertas)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            // Usuario -> Sesion (1 a muchos) - CORREGIDO
            modelBuilder.Entity<Sesion>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Sesiones)
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario -> Recuperacion (1 a muchos) - CORREGIDO
            modelBuilder.Entity<Recuperacion>()
                .HasOne(r => r.Usuario)  // Usar la propiedad de navegación
                .WithMany(u => u.Recuperaciones)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}


