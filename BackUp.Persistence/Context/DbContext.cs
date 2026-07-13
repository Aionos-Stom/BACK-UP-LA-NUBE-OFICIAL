using Microsoft.EntityFrameworkCore;
using BackUp.Domainn.Entities;
using BackUp.Domainn.Entities.Users;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Domain.Entities.Bac;

namespace BackUp.Persistence.Context
{
    public class BackUpDbContext : DbContext, IApplicationDbContext
    {
        public BackUpDbContext(DbContextOptions<BackUpDbContext> options) : base(options)
        { }

        // Entidades existentes
        public DbSet<Organizacion> Organizacion { get; set; }
        public DbSet<CloudStorage> CloudStorage { get; set; }
        public DbSet<PoliticaBackup> PoliticaBackup { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<JobBackup> JobBackup { get; set; }
        public DbSet<Recuperacion> Recuperacion { get; set; }
        public DbSet<VerificacionIntegridad> VerificacionIntegridad { get; set; }
        public DbSet<Alerta> Alerta { get; set; }
        public DbSet<Sesion> Sesion { get; set; }

        // Nuevas entidades: planes, suscripciones, pagos, tokens
        public DbSet<Plan> Plan { get; set; }
        public DbSet<Suscripcion> Suscripcion { get; set; }
        public DbSet<Pago> Pago { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }

        Task<int> IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
            => base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Tablas existentes ---
            modelBuilder.Entity<Organizacion>().ToTable("Organizacion");
            modelBuilder.Entity<CloudStorage>().ToTable("CloudStorage");
            modelBuilder.Entity<PoliticaBackup>().ToTable("PoliticaBackup");
            modelBuilder.Entity<JobBackup>().ToTable("JobBackup");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Recuperacion>().ToTable("Recuperacion");
            modelBuilder.Entity<VerificacionIntegridad>().ToTable("VerificacionIntegridad");
            modelBuilder.Entity<Alerta>().ToTable("Alerta");
            modelBuilder.Entity<Sesion>().ToTable("Sesion");

            // --- Nuevas tablas ---
            modelBuilder.Entity<Plan>().ToTable("PlanSuscripcion");
            modelBuilder.Entity<Suscripcion>().ToTable("Suscripcion");
            modelBuilder.Entity<Pago>().ToTable("Pago");
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");

            // Mapear columnas para Organizacion
            modelBuilder.Entity<Organizacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Configuracion).HasColumnName("configuracion");
                entity.Property(e => e.LicenciaValidaHasta).HasColumnName("licencia_valida_hasta");
                entity.Property(e => e.MaxUsuarios).HasColumnName("max_usuarios");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            // CloudStorage
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
            });

            // PoliticaBackup
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
            });

            // JobBackup
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
            });

            // Usuario
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
                entity.Property(e => e.FotoPerfil).HasColumnName("FotoPerfil");
                entity.Property(e => e.Bio).HasColumnName("Bio").HasMaxLength(500);
                entity.Property(e => e.Ciudad).HasColumnName("Ciudad").HasMaxLength(100);
                entity.Property(e => e.Pais).HasColumnName("Pais").HasMaxLength(100);
                entity.Property(e => e.Cargo).HasColumnName("Cargo").HasMaxLength(150);
                entity.Property(e => e.Empresa).HasColumnName("Empresa").HasMaxLength(150);
                entity.Property(e => e.FechaNacimiento).HasColumnName("FechaNacimiento");
                entity.Property(e => e.LinkedIn).HasColumnName("LinkedIn").HasMaxLength(200);
            });

            // Recuperacion
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
            });

            // VerificacionIntegridad
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

            // Alerta
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
            });

            // Sesion
            modelBuilder.Entity<Sesion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.Token).HasColumnName("token");
                entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
                entity.Property(e => e.IpAddress).HasColumnName("ip_address");
                entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            });

            // --- Nuevas entidades ---
            modelBuilder.Entity<Plan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.PrecioMensual).HasColumnName("precio_mensual").HasPrecision(10, 2);
                entity.Property(e => e.LimiteAlmacenamientoBytes).HasColumnName("limite_almacenamiento_bytes");
                entity.Property(e => e.MaxJobsConcurrentes).HasColumnName("max_jobs_concurrentes");
                entity.Property(e => e.MaxPoliticas).HasColumnName("max_politicas");
                entity.Property(e => e.BackupAutomatico).HasColumnName("backup_automatico");
                entity.Property(e => e.SoportePrioritario).HasColumnName("soporte_prioritario");
                entity.Property(e => e.EsGratuito).HasColumnName("es_gratuito");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<Suscripcion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.PlanId).HasColumnName("plan_id");
                entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(30);
                entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
                entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
                entity.Property(e => e.EsGratisAdminGranted).HasColumnName("es_gratis_admin_granted");
                entity.Property(e => e.OtorgadaPorAdminId).HasColumnName("otorgada_por_admin_id");
                entity.Property(e => e.AlmacenamientoUsadoBytes).HasColumnName("almacenamiento_usado_bytes");
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SuscripcionId).HasColumnName("suscripcion_id");
                entity.Property(e => e.Monto).HasColumnName("monto").HasPrecision(10, 2);
                entity.Property(e => e.Moneda).HasColumnName("moneda").HasMaxLength(10);
                entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20);
                entity.Property(e => e.MetodoPago).HasColumnName("metodo_pago").HasMaxLength(50);
                entity.Property(e => e.ReferenciaExterna).HasColumnName("referencia_externa");
                entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(512);
                entity.Property(e => e.Expiracion).HasColumnName("expiracion");
                entity.Property(e => e.Revocado).HasColumnName("revocado");
                entity.Property(e => e.CreadoEn).HasColumnName("creado_en");
                entity.Property(e => e.ReemplazadoPor).HasColumnName("reemplazado_por");
                // Propiedad computada - no mapear
                entity.Ignore(e => e.EstaActivo);
            });

            ConfigureRelationships(modelBuilder);
            ConfigureAuditLog(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CloudStorage>()
                .HasOne(cs => cs.Organizacion)
                .WithMany(o => o.CloudStorages)
                .HasForeignKey(cs => cs.OrganizacionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PoliticaBackup>()
                .HasOne(pb => pb.Organizacion)
                .WithMany(o => o.PoliticasBackup)
                .HasForeignKey(pb => pb.OrganizacionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Organizacion)
                .WithMany(o => o.Usuarios)
                .HasForeignKey(u => u.OrganizacionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobBackup>()
                .HasOne(jb => jb.Politica)
                .WithMany(pb => pb.JobsBackup)
                .HasForeignKey(jb => jb.PoliticaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<JobBackup>()
                .HasOne(jb => jb.CloudStorage)
                .WithMany(cs => cs.JobsBackup)
                .HasForeignKey(jb => jb.CloudStorageId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VerificacionIntegridad>()
                .HasOne(vi => vi.JobBackup)
                .WithMany(jb => jb.VerificacionesIntegridad)
                .HasForeignKey(vi => vi.job_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recuperacion>()
                .HasOne(r => r.JobBackup)
                .WithMany(jb => jb.Recuperaciones)
                .HasForeignKey(r => r.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.JobBackup)
                .WithMany(jb => jb.Alertas)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Alertas)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Sesion>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Sesiones)
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recuperacion>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Recuperaciones)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            // Nuevas relaciones
            modelBuilder.Entity<Suscripcion>()
                .HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Suscripcion>()
                .HasOne(s => s.Plan)
                .WithMany(p => p.Suscripciones)
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Suscripcion)
                .WithMany(s => s.Pagos)
                .HasForeignKey(p => p.SuscripcionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Usuario)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureAuditLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>().ToTable("AuditLog");
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.Accion).HasColumnName("accion").HasMaxLength(100);
                entity.Property(e => e.Entidad).HasColumnName("entidad").HasMaxLength(100);
                entity.Property(e => e.EntidadId).HasColumnName("entidad_id").HasMaxLength(50);
                entity.Property(e => e.DatosAnteriores).HasColumnName("datos_anteriores");
                entity.Property(e => e.DatosNuevos).HasColumnName("datos_nuevos");
                entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasColumnName("user_agent");
                entity.Property(e => e.CreadoEn).HasColumnName("creado_en");
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
                entity.Property(e => e.Navegador).HasColumnName("Navegador").HasMaxLength(200);
                entity.Property(e => e.SistemaOperativo).HasColumnName("SistemaOperativo").HasMaxLength(100);
                entity.Property(e => e.Dispositivo).HasColumnName("Dispositivo").HasMaxLength(50);
                entity.Property(e => e.DescripcionLegible).HasColumnName("DescripcionLegible").HasMaxLength(500);
            });
        }
    }
}
