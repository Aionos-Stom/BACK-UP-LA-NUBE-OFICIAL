using BackUp.Domainn.Base;
using BackUp.Domainn.Entities.Users;

namespace BackUp.Domain.Entities.Bac
{
    public sealed class Alerta 
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int? JobId { get; set; }
        public string? Tipo { get; set; } // 'integridad', 'rendimiento', 'costo', 'seguridad'
        public string? Severidad { get; set; } // 'baja', 'media', 'alta', 'critica'
        public string? Mensaje { get; set; }
        public bool IsAcknowledged { get; set; } = false;

        // Navigation properties
        public Usuario? Usuario { get; set; }
        public JobBackup? JobBackup { get; set; }
        
        // Business methods
        public bool EsCritica() => Severidad == "critica";
        public bool RequiereAccionInmediata() => Severidad == "alta" || Severidad == "critica";

        public void MarcarComoReconocida()
        {
            IsAcknowledged = true;
        }
    }
}
