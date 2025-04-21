using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barbearia.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")] // Especifica o tipo de dado para o preço
        public decimal Preco { get; set; }
    }
}
