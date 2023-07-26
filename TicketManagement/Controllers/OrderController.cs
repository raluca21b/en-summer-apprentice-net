using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Repositories;
using TicketManagement.Models.DTO;
using AutoMapper;

namespace TicketManagement.Controllers

{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
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
        public ActionResult<OrderDTO> GetById(int id)
        {
            var order = _orderRepository.GetById(id);

            if(order == null)
            {
                return NotFound();
            }

            var orderDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderDTO);
        }
    }
}
