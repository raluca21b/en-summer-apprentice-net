using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Repositories;
using TicketManagement.Models.DTO;

namespace TicketManagement.Controllers

{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public ActionResult<List<OrderDTO>> GetAll()
        {
            var orders = _orderRepository.GetAll().ToList();

            var dtoOrders = orders.Select(o => new OrderDTO()
            {
                EventID = o.TicketCategory?.EventId ?? -1,
                OrderedAt = o.OrderedAt ?? DateTime.MinValue,
                TicketCategoryID = o.TicketCategoryId ?? 0,
                NumberOfTickets = o.NumberOfTickets ?? 0,
                TotalPrice = o.TotalPrice ?? decimal.Zero,
            });

            return Ok(dtoOrders);
        }

        [HttpGet]
        public ActionResult<OrderDTO> GetById(int id)
        {
            var order = _orderRepository.GetById(id);
            if(order == null)
            {
                return NotFound();
            }
            var orderDTO = new OrderDTO()
            {
                EventID = order.TicketCategory?.EventId ?? -1,
                OrderedAt = order.OrderedAt ?? DateTime.MinValue,
                TicketCategoryID = order.TicketCategoryId ?? 0,
                NumberOfTickets = order.NumberOfTickets ?? 0,
                TotalPrice = order.TotalPrice ?? decimal.Zero,
            };
            return Ok(orderDTO);
        }
    }
}
