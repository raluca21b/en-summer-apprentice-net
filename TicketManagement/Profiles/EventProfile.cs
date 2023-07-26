using AutoMapper;
using TicketManagement.Models.DTO;
using TicketManagement.Models;

namespace TicketManagement.Profiles
{
    public class EventProfile:Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDTO>()
               .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType.EventTypeName))
               .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue.Location))
               .ReverseMap();
        }
    }
}
