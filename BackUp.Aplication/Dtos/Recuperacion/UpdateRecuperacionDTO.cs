

namespace BackUp.Aplication.Dtos.Recuperacion
{
    public record UpdateRecuperacionDTO
    {
        public int Id { get; init; }
        public string? Estado { get; init; }
        public DateTime? CompletedAt { get; init; }
    }
}
