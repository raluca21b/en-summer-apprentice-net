using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TicketManagement.Models.DTO;
using TicketManagement.Repositories;

namespace TicketManagement.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;   
        }
        [HttpGet]
       public ActionResult<List<EventDTO>> GetAll()
        {
            var events = _eventRepository.GetAll();

            var dtoEvents = events.Select(e => new EventDTO()
            {
                EventId = e.EventId,
                EventName = e.EventName,
                EventDescription = e.EventDescription,
                EventType = e.EventType?.EventTypeName ?? string.Empty,
                Venue = e.Venue?.Location ?? string.Empty,
            });

            return Ok(dtoEvents);
        }

        [HttpGet]
        public ActionResult<EventDTO> GetById(int id)
        {
            var @event = _eventRepository.GetById(id);

            if (@event == null)
            {
                return NotFound();
            }
            var eventDTO = new EventDTO
            {
                EventId = @event.EventId,
                EventName = @event.EventName,
                EventDescription = @event.EventDescription,
                EventType = @event.EventType?.EventTypeName ?? string.Empty, 
                Venue = @event.Venue?.Location ?? string.Empty
            };
            return Ok(eventDTO);
        }
    }
    
   
}