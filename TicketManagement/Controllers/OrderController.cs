using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Repositories;
using TicketManagement.Models.DTO;
using AutoMapper;
using TicketManagement.Models;

namespace TicketManagement.Controllers

{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITicketCategoryRepository _ticketCategoryRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, ITicketCategoryRepository ticketCategoryRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _ticketCategoryRepository = ticketCategoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<OrderDTO>> GetAll()
        {
            var orders = _orderRepository.GetAll().ToList();
            var dtoOrders = _mapper.Map<List<OrderDTO>>(orders);

            return Ok(dtoOrders);
        }

        [HttpGet]
        public async Task<ActionResult<OrderDTO>> GetById(int id)
        {
            var order = await _orderRepository.GetById(id);

            if(order == null)
            {
                return NotFound();
            }

            var orderDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderDTO);
        }

        [HttpPatch]
        public async Task<ActionResult<OrderPatchDTO>> Patch(OrderPatchDTO orderPatchDTO)
        {
            var orderEntity = await _orderRepository.GetById(orderPatchDTO.OrderID);

            if (orderEntity == null)
            {
                return NotFound();
            }

            orderEntity.OrderedAt = DateTime.Now;
            
            if (orderPatchDTO.NumberOfTickets >= 0)
            {
                orderEntity.NumberOfTickets = orderPatchDTO.NumberOfTickets;
            }
            else
            {
                return BadRequest("Number of Ticket cannot be negative or null!");
            }

            if (orderPatchDTO.EventID != 0 && orderPatchDTO.TicketDescription != null 
                                           && orderPatchDTO.TicketDescription != orderEntity.TicketCategory.Description)
            {
                TicketCategory ticketCategory = _ticketCategoryRepository
                                     .GetTicketCategoryByEventIdAndDescription
                                     (orderPatchDTO.EventID, orderPatchDTO.TicketDescription);
                ticketCategory.Event = orderEntity.TicketCategory.Event;
                orderEntity.TicketCategory = ticketCategory;
            }

            orderEntity.TotalPrice = orderEntity.NumberOfTickets * orderEntity.TicketCategory.Price;
            
            _orderRepository.Update(orderEntity);

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var orderEntity = await _orderRepository.GetById(id);

            if(orderEntity == null)
            {
                return NotFound();
            }

            _orderRepository.Delete(orderEntity);
            return NoContent();

        }
    }
}
