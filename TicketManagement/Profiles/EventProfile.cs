using AutoMapper;
using TicketManagement.Models;
using TicketManagement.Models.DTO;

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

            CreateMap<Event, EventPatchDTO>().ReverseMap();
        }
    }
}
