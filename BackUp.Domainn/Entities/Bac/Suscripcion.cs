using BackUp.Domainn.Entities.Users;

namespace BackUp.Domain.Entities.Bac
{
    public class Suscripcion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PlanId { get; set; }
        public string Estado { get; set; } = "activa"; // activa, cancelada, expirada, gratis_admin
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFin { get; set; }
        public bool EsGratisAdminGranted { get; set; } = false;
        public int? OtorgadaPorAdminId { get; set; }
        public long AlmacenamientoUsadoBytes { get; set; } = 0;
        public Usuario? Usuario { get; set; }
        public Plan? Plan { get; set; }
        public List<Pago> Pagos { get; set; } = new();
    }
}
