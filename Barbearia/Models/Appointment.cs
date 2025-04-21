using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Barbearia.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        [Required]
        public string ClienteNome { get; set; }
        [Required]
        public string ClienteEmail { get; set; }
        public int BarberId { get; set; }
        public int ServiceId { get; set; }
        [Required]
        public DateTime DataHora { get; set; }

        [ValidateNever]
        public Barber? Barber { get; set; }
        [ValidateNever]
        public Service? Service { get; set; }
    }
}
