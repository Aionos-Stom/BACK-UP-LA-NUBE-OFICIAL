using BackUp.Domainn.Base;
using BackUp.Domainn.Entities.Users;

namespace BackUp.Domain.Entities.Bac
{
    public sealed class Organizacion 
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Configuracion { get; set; }
        public DateTime? LicenciaValidaHasta { get; set; }
        public int MaxUsuarios { get; set; }
        public bool Activo { get; set; } = true;

        // Navigation properties
        public List<CloudStorage> CloudStorages { get; set; } = new();
        public List<PoliticaBackup> PoliticasBackup { get; set; } = new();
       public List<Usuario> Usuarios { get; set; } = new();
      
    }
}
