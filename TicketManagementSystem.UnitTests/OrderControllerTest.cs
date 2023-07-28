using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicketManagement.Controllers;
using TicketManagement.Exceptions;
using TicketManagement.Models;
using TicketManagement.Models.DTO;
using TicketManagement.Repositories;

namespace TicketManagementSystem.UnitTests
{
    [TestClass]
    public class OrderControllerTest
    {
        Mock<IOrderRepository> _orderRepositoryMock;
        Mock<ITicketCategoryRepository> _ticketCategoryRepositoryMock;
        Mock<IMapper> _mapperMoq;
        List<Order> _moqOrders;
        List<OrderDTO> _moqOrdersDto;

        [TestInitialize]
        public void SetupMoqData()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _ticketCategoryRepositoryMock = new Mock<ITicketCategoryRepository>();
            _mapperMoq = new Mock<IMapper> { };
            _moqOrders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    NumberOfTickets = 2,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 1200,
                    CustomerId = 1,
                    TicketCategoryId = 1,
                    Customer = new Customer { CustomerId = 1, CustomerName = "CustomerName moq", Email = "maq@mail.com" },
                    TicketCategory = new TicketCategory
                    {
                        TicketCategoryId = 1,
                        Description = "Ticket Mock",
                        Price = 600,
                        EventId = 1,
                        Event = new Event
                        {
                            EventId = 1,
                            EventName = "Moq name",
                            EventDescription = "Moq description",
                            EndDate = DateTime.Now,
                            StartDate = DateTime.Now,
                            EventType = new EventType { EventTypeId = 1, EventTypeName = "test event type" },
                            EventTypeId = 1,
                            Venue = new Venue { VenueId = 1, Capacity = 12, Location = "Mock location", Type = "mock type" },
                            VenueId = 1
                        }
                    }
                }
            };
            _moqOrdersDto = new List<OrderDTO>
            {
                new OrderDTO
                {
                    EventID = 1,
                    OrderedAt = DateTime.Now,
                    TicketCategoryID = 1,
                    NumberOfTickets = 2,
                    TotalPrice = 1200

                }
            };
        }

        [TestMethod]
        public async Task GetAllOrdersReturnListOfOrders()
        {

            _orderRepositoryMock.Setup(moq => moq.GetAll()).ReturnsAsync(_moqOrders);
            _mapperMoq.Setup(mapper => mapper.Map<List<OrderDTO>>(It.IsAny<List<Order>>()))
                                             .Returns(_moqOrdersDto);
            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);

            // Act
            var result = await controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            var orders = okResult?.Value as List<OrderDTO>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(1, orders.Count);
            CollectionAssert.AreEqual(_moqOrdersDto, orders);
        }

        [TestMethod]
        public async Task GetAllOrdersReturnWithEmptyList()
        {
            // Arrange
            _orderRepositoryMock.Setup(moq => moq.GetAll()).ReturnsAsync(new List<Order>());
            _mapperMoq.Setup(mapper => mapper.Map<List<OrderDTO>>(It.IsAny<List<Order>>())).Returns(new List<OrderDTO>());

            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);

            // Act
            var result = await controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            var orders = okResult?.Value as List<OrderDTO>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(orders);
            Assert.AreEqual(0, orders.Count);
        }

        [TestMethod]
        public async Task GetOrderByIdReturnFirstRecord()
        {
            //Arrange
            _orderRepositoryMock.Setup(moq => moq.GetById(It.IsAny<int>())).Returns(Task.Run(() => _moqOrders.First()));
            _mapperMoq.Setup(moq => moq.Map<OrderDTO>(It.IsAny<Order>())).Returns(_moqOrdersDto.First());

            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);

            //Act

            var result = await controller.GetById(1);
            var orderResult = result.Result as OkObjectResult;
            var orderFound = orderResult?.Value as OrderDTO;

            //Assert
            Assert.IsNotNull(orderResult);
            Assert.AreEqual(200, orderResult.StatusCode);
            Assert.IsNotNull(orderFound);
            Assert.AreEqual(1200, orderFound.TotalPrice);
        }

        [TestMethod]
        public async Task GetOrderByIdEntityNotFoundException()
        {
            // Arrange
            var orderId = 999; // A non-existing order ID
            _orderRepositoryMock.Setup(moq => moq.GetById(It.IsAny<int>()))
                                .ThrowsAsync(new EntityNotFoundException(orderId, nameof(Order)));

            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);

            //Act and Assert
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () =>
            {
                var result = await controller.GetById(orderId);
                Assert.IsNull(result);

            });

            Assert.AreEqual("'Order' with id '999' was not found.", exception.Message);
            
        }

        [TestMethod]
        public async Task DeleteOrderExistingOrderDeletesOrder()
        {
            // Arrange
            var orderId = 1;

            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);

            // Act
            var result = await controller.Delete(orderId);
            var noContentResult = result as NoContentResult;

            // Assert
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);


        }

        [TestMethod]
        public async Task DeleteOrderNonExistingOrderReturnsEntityNotFoundException()
        {
            // Arrange
            var nonExistingOrderId = 999;
            _orderRepositoryMock.Setup(moq => moq.GetById(nonExistingOrderId))
                                                 .ThrowsAsync(new EntityNotFoundException(nonExistingOrderId, nameof(Order)));

            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);

            // Act and Assert
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () =>
            {
                await controller.Delete(nonExistingOrderId);
            });

            // Assert
            Assert.AreEqual("'Order' with id '999' was not found.", exception.Message);
        }

        [TestMethod]
        public async Task PatchOrderExistsSuccessfulPatch()
        {
            // Arrange
            var orderId = 1;
            var orderEntity = new Order
            {
                OrderId = 1,
                NumberOfTickets = 2,
                OrderedAt = DateTime.Now,
                TotalPrice = 1200,
                CustomerId = 1,
                TicketCategoryId = 1,
                Customer = new Customer { CustomerId = 1, CustomerName = "CustomerName moq", Email = "maq@mail.com" },
                TicketCategory = new TicketCategory
                {
                    TicketCategoryId = 1,
                    Description = "Ticket Mock",
                    Price = 600,
                    EventId = 1,
                    Event = new Event
                    {
                        EventId = 1,
                        EventName = "Moq name",
                        EventDescription = "Moq description",
                        EndDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        EventType = new EventType { EventTypeId = 1, EventTypeName = "test event type" },
                        EventTypeId = 1,
                        Venue = new Venue { VenueId = 1, Capacity = 12, Location = "Mock location", Type = "mock type" },
                        VenueId = 1
                    }
                }
            };

            var orderPatchDTO = new OrderPatchDTO { OrderID = 1,EventID = 1, TicketDescription = "Ticket Mock",  NumberOfTickets = 5};

            _orderRepositoryMock.Setup(moq => moq.GetById(orderPatchDTO.OrderID)).ReturnsAsync(orderEntity);
            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);

            // Act
            var result = await controller.Patch(orderPatchDTO);

            // Assert
            Assert.IsNotNull(result);
            _orderRepositoryMock.Verify(moq => moq.Update(orderEntity), Times.Once);
            Assert.AreEqual(orderPatchDTO.NumberOfTickets, orderEntity.NumberOfTickets);
            Assert.AreEqual(orderPatchDTO.NumberOfTickets * orderEntity.TicketCategory.Price, orderEntity.TotalPrice);
        }

        [TestMethod]
        public async Task PatchInvalidNumberOfTicketsThrowsArgumentExceptionNoUpdate()
        {
            var orderId = 1;
            var orderEntity = new Order
            {
                OrderId = 1,
                NumberOfTickets = 2,
                OrderedAt = DateTime.Now,
                TotalPrice = 1200,
                CustomerId = 1,
                TicketCategoryId = 1,
                Customer = new Customer { CustomerId = 1, CustomerName = "CustomerName moq", Email = "maq@mail.com" },
                TicketCategory = new TicketCategory
                {
                    TicketCategoryId = 1,
                    Description = "Ticket Mock",
                    Price = 600,
                    EventId = 1,
                    Event = new Event
                    {
                        EventId = 1,
                        EventName = "Moq name",
                        EventDescription = "Moq description",
                        EndDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        EventType = new EventType { EventTypeId = 1, EventTypeName = "test event type" },
                        EventTypeId = 1,
                        Venue = new Venue { VenueId = 1, Capacity = 12, Location = "Mock location", Type = "mock type" },
                        VenueId = 1
                    }
                }
            };

            var orderPatchDTO = new OrderPatchDTO { OrderID = 1, EventID = 1, TicketDescription = "Ticket Mock", NumberOfTickets = -5 };

            _orderRepositoryMock.Setup(moq => moq.GetById(orderPatchDTO.OrderID)).ReturnsAsync(orderEntity);
            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);
            // Act and Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await controller.Patch(orderPatchDTO));
            Assert.AreEqual("Number of Tickets cannot be negative or null!", exception.Message);
            
        }

        [TestMethod]
        public async Task PatchInexistentEventIDThrowsEntityNotFoundExceptionNoUpdate()
        {
            var orderId = 1;
            var orderEntity = new Order
            {
                OrderId = 1,
                NumberOfTickets = 2,
                OrderedAt = DateTime.Now,
                TotalPrice = 1200,
                CustomerId = 1,
                TicketCategoryId = 1,
                Customer = new Customer { CustomerId = 1, CustomerName = "CustomerName moq", Email = "maq@mail.com" },
                TicketCategory = new TicketCategory
                {
                    TicketCategoryId = 1,
                    Description = "Ticket Mock",
                    Price = 600,
                    EventId = 1,
                    Event = new Event
                    {
                        EventId = 1,
                        EventName = "Moq name",
                        EventDescription = "Moq description",
                        EndDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        EventType = new EventType { EventTypeId = 1, EventTypeName = "test event type" },
                        EventTypeId = 1,
                        Venue = new Venue { VenueId = 1, Capacity = 12, Location = "Mock location", Type = "mock type" },
                        VenueId = 1
                    }
                }
            };

            var orderPatchDTO = new OrderPatchDTO { OrderID = 1, EventID = 99, TicketDescription = "Ticket Mock", NumberOfTickets = 5 };

            _orderRepositoryMock.Setup(moq => moq.GetById(orderPatchDTO.OrderID))
                                                 .ThrowsAsync(new EntityNotFoundException("TicketCategory not found with eventId "
                                                                                          + orderPatchDTO.EventID.ToString()
                                                                                          + " and description "
                                                                                          + orderPatchDTO.TicketDescription));

            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);
            // Act and Assert
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () => await controller.Patch(orderPatchDTO));
            Assert.AreEqual("TicketCategory not found with eventId 99 and description Ticket Mock", exception.Message);
        }

        [TestMethod]
        public async Task PatchInexistentTicketCategoryThrowsEntityNotFoundExceptionNoUpdate()
        {
            var orderId = 1;
            var orderEntity = new Order
            {
                OrderId = 1,
                NumberOfTickets = 2,
                OrderedAt = DateTime.Now,
                TotalPrice = 1200,
                CustomerId = 1,
                TicketCategoryId = 1,
                Customer = new Customer { CustomerId = 1, CustomerName = "CustomerName moq", Email = "maq@mail.com" },
                TicketCategory = new TicketCategory
                {
                    TicketCategoryId = 1,
                    Description = "Ticket Mock",
                    Price = 600,
                    EventId = 1,
                    Event = new Event
                    {
                        EventId = 1,
                        EventName = "Moq name",
                        EventDescription = "Moq description",
                        EndDate = DateTime.Now,
                        StartDate = DateTime.Now,
                        EventType = new EventType { EventTypeId = 1, EventTypeName = "test event type" },
                        EventTypeId = 1,
                        Venue = new Venue { VenueId = 1, Capacity = 12, Location = "Mock location", Type = "mock type" },
                        VenueId = 1
                    }
                }
            };

            var orderPatchDTO = new OrderPatchDTO { OrderID = 1, EventID = 99, TicketDescription = "bad descript", NumberOfTickets = 5 };

            _orderRepositoryMock.Setup(moq => moq.GetById(orderPatchDTO.OrderID))
                                                 .ThrowsAsync(new EntityNotFoundException("TicketCategory not found with eventId "
                                                                                          + orderPatchDTO.EventID.ToString()
                                                                                          + " and description "
                                                                                          + orderPatchDTO.TicketDescription));

            var controller = new OrderController(_orderRepositoryMock.Object, _ticketCategoryRepositoryMock.Object, _mapperMoq.Object);
            // Act and Assert
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () => await controller.Patch(orderPatchDTO));
            Assert.AreEqual("TicketCategory not found with eventId 99 and description bad descript", exception.Message);
        }
    }



}