using AutoMapper;
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
        private readonly IMapper _mapper;

        public EventController(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;   
            _mapper = mapper;
        }
        [HttpGet]
       public ActionResult<List<EventDTO>> GetAll()
        {
            var events = _eventRepository.GetAll();
            var dtoEvents = _mapper.Map<List<EventDTO>>(events);

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

            var dtoEvent = _mapper.Map<EventDTO>(@event); 
            return Ok(dtoEvent);
        }
    }
    
   
}