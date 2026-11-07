using Emk.Domain.DTOs;
using Emk.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emk.Gateway.Api.Controllers
{
    /// <summary>GET /v1/doctors/slots — свободное время приема.</summary>
    [ApiController]
    [Route("v1/doctors/slots")]
    public class SlotsController(IScheduleRepository repository) : ControllerBase
    {
        [HttpGet]
        public ActionResult<SlotsResponse> Get([FromQuery] DateOnly date, [FromQuery] Specialty? specialty) =>
            new SlotsResponse(date, repository.GetFreeSlots(date, specialty));
    }

    /// <summary>POST /v1/appointments/book — запись на прием.</summary>
    [ApiController]
    [Route("v1/appointments/book")]
    public class AppointmentsController(IScheduleRepository repository) : ControllerBase
    {
        [HttpPost]
        public ActionResult<Appointment> Book(AppointmentRequest request)
        {
            var appointment = repository.TryBook(request);
            return appointment is null
                ? Conflict(new ApiError("SLOT_ALREADY_BOOKED", "Выбранное время уже занято, выберите другой слот"))
                : StatusCode(StatusCodes.Status201Created, appointment);
        }
    }
}
