using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Barbearia.Data;
using Barbearia.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Barbearia.Services;
using MimeKit;

namespace Barbearia.Controllers
{
    public class AgendamentosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly GoogleCalendarService _calendarService;

        public AgendamentosController(ApplicationDbContext context, IEmailSender emailSender, GoogleCalendarService calendarService)
        {
            _context = context;
            _emailSender = emailSender;
            _calendarService = calendarService;
        }

        public IActionResult EscolherBarbeiro()
        {
            var barbeiros = _context.Barbers.ToList();
            return View(barbeiros);
        }

        public IActionResult EscolherHorario(int barberId)
        {
            var horarios = _context.AvailableTimeSlots
                .Where(h => h.BarberId == barberId && !h.Ocupado && h.DataHoraInicio > DateTime.Now)
                .OrderBy(h => h.DataHoraInicio)
                .ToList();

            ViewBag.BarberId = barberId;
            return View(horarios);
        }

        public IActionResult EscolherServico(int slotId, int barberId)
        {
            var servicos = _context.Services.ToList();
            ViewBag.SlotId = slotId;
            ViewBag.BarberId = barberId;
            return View(servicos);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarAgendamento(string ClienteNome, string ClienteEmail, int SlotId, int BarberId, int ServiceId)
        {
            var slot = await _context.AvailableTimeSlots.FindAsync(SlotId);
            if (slot == null || slot.Ocupado)
            {
                return NotFound("Horário não disponível.");
            }

            var agendamento = new Appointment
            {
                ClienteNome = ClienteNome,
                ClienteEmail = ClienteEmail,
                BarberId = BarberId,
                ServiceId = ServiceId,
                DataHora = slot.DataHoraInicio
            };

            slot.Ocupado = true;
            _context.Appointments.Add(agendamento);

            var barbeiro = await _context.Barbers.FindAsync(BarberId);
            var servico = await _context.Services.FindAsync(ServiceId);

            var subject = "Confirmação de Agendamento - Barbearia";
            var message = $@"
            <h2>Olá, {ClienteNome}!</h2>
            <p>Seu agendamento foi confirmado com sucesso.</p>
            <p><strong>Data:</strong> {slot.DataHoraInicio:dd/MM/yyyy HH:mm}</p>
            <p><strong>Serviço:</strong> {servico?.Nome}</p>
            <p><strong>Barbeiro:</strong> {barbeiro?.Nome}</p>
            <br/>
            <p>Qualquer dúvida, estamos à disposição!</p>
            <p><em>Equipe da Barbearia</em></p>";

            // Garantir codificação correta para UTF-8
            var mailMessage = new MimeMessage();
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("html")
            {
                Text = message
            };
            mailMessage.Headers.Add("Content-Type", "text/html; charset=utf-8");

            await _emailSender.SendEmailAsync(ClienteEmail, subject, message);
            await _context.SaveChangesAsync();

            // Cria evento no Google Calendar
            string titulo = $"Agendamento com {barbeiro?.Nome}";
            string descricao = $"Cliente: {ClienteNome}\nServiço: {servico?.Nome}";
            DateTime dataInicio = slot.DataHoraInicio;
            DateTime dataFim = dataInicio.AddMinutes(30); // pode ajustar com base no tempo do serviço
            string calendarId = "9efa42649f2e3c7abdc852fe953e30605b6fab1c995d661933abeb936e9d0aea@group.calendar.google.com"; //ID do calendário que criei para a barbearia

            await _calendarService.CreateEventAsync(dataInicio, dataFim, titulo, descricao, calendarId);

            return RedirectToAction("Sucesso");
        }


        public IActionResult Sucesso()
        {
            return View();
        }


        // GET: Agendamentos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.Barber).Include(a => a.Service);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Agendamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Agendamentos/Create
        public IActionResult Create()
        {
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Nome");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nome");
            return View();
        }

        // POST: Agendamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteNome,ClienteEmail,BarberId,ServiceId,DataHora")] Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Especialidade", appointment.BarberId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nome", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Agendamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Especialidade", appointment.BarberId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nome", appointment.ServiceId);
            return View(appointment);
        }

        // POST: Agendamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteNome,ClienteEmail,BarberId,ServiceId,DataHora")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Especialidade", appointment.BarberId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nome", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Agendamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Agendamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
