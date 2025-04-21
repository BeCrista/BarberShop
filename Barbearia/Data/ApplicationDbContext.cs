using Barbearia.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Barbearia.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AvailableTimeSlot> AvailableTimeSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações adicionais do modelo, se necessário
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Barber)
                .WithMany() // Correção: Barber não tem Appointments, mas AvailableTimeSlots
                .HasForeignKey(a => a.BarberId)
                .OnDelete(DeleteBehavior.Restrict); // Impede a exclusão de barbeiro com agendamentos

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);  // Impede a exclusão de serviço com agendamentos

            modelBuilder.Entity<AvailableTimeSlot>()
                .HasOne(t => t.Barber)
                .WithMany(b => b.HorariosDisponiveis)
                .HasForeignKey(t => t.BarberId)
                .OnDelete(DeleteBehavior.Cascade); // Permite exclusão em cascata de TimeSlots.

            base.OnModelCreating(modelBuilder);
        }

    }
}
