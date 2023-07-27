using AutoMapper;
using TicketManagement.Models;
using TicketManagement.Models.DTO;

namespace TicketManagement.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.EventID, opt => opt.MapFrom(src => GetEventIdOrDefault(src)))
                .ForMember(dest => dest.TicketCategoryID, opt => opt.MapFrom(src => src.TicketCategoryId))
                .ReverseMap();
        }

        private int GetEventIdOrDefault(Order source)
        {
            if (source.TicketCategory?.Event != null)
            {
                return source.TicketCategory.Event.EventId;
            }

            return -1; 
        }
    }
}
