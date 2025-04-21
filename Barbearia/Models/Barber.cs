using System.ComponentModel.DataAnnotations;

namespace Barbearia.Models
{
    public class Barber
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        public string? FotoUrl { get; set; }
        [Required]
        public string Especialidade { get; set; }
        public ICollection<AvailableTimeSlot> HorariosDisponiveis { get; set; } = new List<AvailableTimeSlot>();
    }
}
