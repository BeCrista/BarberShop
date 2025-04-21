using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Barbearia.Models
{
    public class AvailableTimeSlot
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        [Required]
        public DateTime DataHoraInicio { get; set; }
        [Required]
        public DateTime DataHoraFim { get; set; }
        public bool Ocupado { get; set; }

        [ValidateNever]
        public Barber? Barber { get; set; }
    }
}