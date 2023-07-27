using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
       public async Task<ActionResult<List<EventDTO>>> GetAll()
        {
            var events = await _eventRepository.GetAll();
            var dtoEvents = _mapper.Map<List<EventDTO>>(events);

            return Ok(dtoEvents);
        }

        [HttpGet]
        public async Task<ActionResult<EventDTO>> GetById(int id)
        {
            var @event = await _eventRepository.GetById(id);

            if (@event == null)
            {
                return NotFound();
            }

            var dtoEvent = _mapper.Map<EventDTO>(@event); 
            return Ok(dtoEvent);
        }

        [HttpPatch]
        public async Task<ActionResult<EventPatchDTO>> Patch(EventPatchDTO eventPatchDTO)
        {
            var eventEntity = await _eventRepository.GetById(eventPatchDTO.EventId);

            if (eventEntity == null)
            {
                return NotFound();
            }

            if (!eventPatchDTO.EventName.IsNullOrEmpty())
                eventEntity.EventName = eventPatchDTO.EventName;

            if (!eventPatchDTO.EventDescription.IsNullOrEmpty())
                eventEntity.EventDescription = eventPatchDTO.EventDescription;
            
            _eventRepository.Update(eventEntity);
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var eventEntity = await _eventRepository.GetById(id);

            if (eventEntity == null)
            {
                return NotFound();
            }

            _eventRepository.Delete(eventEntity);
            return NoContent();
        }
    
    
    
    }
    
   
}