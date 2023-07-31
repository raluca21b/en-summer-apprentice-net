using AutoMapper;
using Moq;
using TicketManagement.Models.DTO;
using TicketManagement.Models;
using TicketManagement.Repositories;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Controllers;
using TicketManagement.Exceptions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace TicketManagementSystem.UnitTests
{
    [TestClass]
    public class EventControllerTests
    {
        Mock<IEventRepository> _eventRepositoryMock;
        Mock<IMapper> _mapperMock;
        List<Event> _moqEvents;
        List<EventDTO> _moqEventsDto;

        [TestInitialize]
        public void SetupMoqData()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _mapperMock = new Mock<IMapper>();
            _moqEvents = new List<Event>
            {
                new Event
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
            };
            _moqEventsDto = new List<EventDTO>
            {
                new EventDTO
                {
                    EventId = 1,
                    EventName = "Moq name",
                    EventDescription = "Moq description",
                    EventType = "test event type",
                    Venue = "Mock location"
                }
            };
        }
        
        [TestMethod]
        public async Task GetAllEventsReturnListOfEvents()
        {
            _eventRepositoryMock.Setup(moq => moq.GetAll()).ReturnsAsync(_moqEvents);
            _mapperMock.Setup(mapper => mapper.Map<List<EventDTO>>(It.IsAny<List<Event>>()))
                                             .Returns(_moqEventsDto);
            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            var events = okResult?.Value as List<EventDTO>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(1, events.Count);
            CollectionAssert.AreEqual(_moqEventsDto, events);
        }

        [TestMethod]
        public async Task GetAllEventsReturnEmptyList()
        {
            // Arrange
            _eventRepositoryMock.Setup(moq => moq.GetAll()).ReturnsAsync(new List<Event>());
            _mapperMock.Setup(mapper => mapper.Map<List<EventDTO>>(It.IsAny<List<Event>>())).Returns(new List<EventDTO>());

            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            var events = okResult?.Value as List<EventDTO>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(events);
            Assert.AreEqual(0, events.Count);
        }
        [TestMethod]
        public async Task GetEventByIdReturnFirstRecord()
        {
            //Arrange
            _eventRepositoryMock.Setup(moq => moq.GetById(It.IsAny<int>())).Returns(Task.Run(() => _moqEvents.First()));
            _mapperMock.Setup(moq => moq.Map<EventDTO>(It.IsAny<Event>())).Returns(_moqEventsDto.First());

            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            //Act

            var result = await controller.GetById(1);
            var eventResult = result.Result as OkObjectResult;
            var eventFound = eventResult?.Value as EventDTO;

            //Assert
            Assert.IsNotNull(eventResult);
            Assert.AreEqual(200, eventResult.StatusCode);
            Assert.IsNotNull(eventFound);
            Assert.AreEqual(1, eventFound.EventId);
        }

        [TestMethod]
        public async Task GetEventByIdEntityNotFoundException()
        {
            // Arrange
            var eventID = 999; // A non-existing event ID
            _eventRepositoryMock.Setup(moq => moq.GetById(It.IsAny<int>()))
                                .ThrowsAsync(new EntityNotFoundException(eventID, nameof(Event)));

            var controller = new EventController(_eventRepositoryMock.Object,_mapperMock.Object);

            //Act and Assert
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () =>
            {
                var result = await controller.GetById(eventID);
                Assert.IsNull(result);

            });

            Assert.AreEqual("'Event' with id '999' was not found.", exception.Message);

        }

        [TestMethod]
        public async Task DeleteEventExistingOrderDeletesEvent()
        {
            // Arrange
            var eventID = 1;

            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.Delete(eventID);
            var noContentResult = result as NoContentResult;

            // Assert
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);


        }

        [TestMethod]
        public async Task DeleteEventNonExistingEventReturnsEntityNotFoundException()
        {
            // Arrange
            var nonExistingEventId = 999;
            _eventRepositoryMock.Setup(moq => moq.GetById(nonExistingEventId))
                                                 .ThrowsAsync(new EntityNotFoundException(nonExistingEventId, nameof(Event)));

            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            // Act and Assert
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () =>
            {
                await controller.Delete(nonExistingEventId);
            });

            // Assert
            Assert.AreEqual("'Event' with id '999' was not found.", exception.Message);
        }

        [TestMethod]
        public async Task PatchUpdatesEventExistsSuccessfulPatch()
        {
            // Arrange
            var eventId = 1;
            var eventEntity = new Event
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
            };
            var eventPatchDTO = new EventPatchDTO { EventId = 1, EventName = "MockUpdate", EventDescription = "DescrUpdate" };

            _eventRepositoryMock.Setup(moq => moq.GetById(eventPatchDTO.EventId)).ReturnsAsync(eventEntity);
            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.Patch(eventPatchDTO);

            // Assert
            Assert.IsNotNull(result);
            _eventRepositoryMock.Verify(moq => moq.Update(eventEntity), Times.Once);
            Assert.AreEqual(eventPatchDTO.EventName, eventEntity.EventName);
        }

        [TestMethod]
        public async Task PatchUpdatesEventNonExistentEventThrowEntityNotFoundExceptionNoUpdate()
        {
            // Arrange
            var eventId = 999;
            var eventEntity = new Event
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
            };
            var eventPatchDTO = new EventPatchDTO { EventId = 1, EventName = "MockUpdate", EventDescription = "DescrUpdate" };

            _eventRepositoryMock.Setup(moq => moq.GetById(eventPatchDTO.EventId))
                                                 .ThrowsAsync(new EntityNotFoundException(eventId, nameof(Event))); ;
    

            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            //Act 
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () =>
            {
                var result = await controller.Patch(eventPatchDTO);
            });

            // Assert
            Assert.AreEqual("'Event' with id '999' was not found.", exception.Message);
            _eventRepositoryMock.Verify(moq => moq.Update(eventEntity), Times.Never);
            Assert.AreNotEqual(eventPatchDTO.EventName, eventEntity.EventName);
        }

        [TestMethod]
        public async Task PatchUpdatesEventNullEventPatchDtoNoUpdate()
        {
            // Arrange
            var eventId = 1;
            var eventEntity = new Event
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
            };
            var eventPatchDTO = new EventPatchDTO { };

            _eventRepositoryMock.Setup(moq => moq.GetById(eventPatchDTO.EventId))
                                                 .ThrowsAsync(new EntityNotFoundException(eventPatchDTO.EventId, nameof(Event))); ;


            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            //Act 
            var exception = await Assert.ThrowsExceptionAsync<EntityNotFoundException>(async () =>
            {
                var result = await controller.Patch(eventPatchDTO);
            });

            // Assert
            Assert.AreEqual("'Event' with id '0' was not found.", exception.Message);
            _eventRepositoryMock.Verify(moq => moq.Update(eventEntity), Times.Never);
            Assert.AreNotEqual(eventPatchDTO.EventName, eventEntity.EventName);
        }

        [TestMethod]
        public async Task PatchEventUpdateJustEventNameSuccessfulUpdate()
        {
            // Arrange
            var eventId = 1;
            var eventEntity = new Event
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
            };
            var eventPatchDTO = new EventPatchDTO { EventId = 1, EventName = "MockUpdate" };

            _eventRepositoryMock.Setup(moq => moq.GetById(eventPatchDTO.EventId)).ReturnsAsync(eventEntity);
            var controller = new EventController(_eventRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.Patch(eventPatchDTO);

            // Assert
            Assert.IsNotNull(result);
            _eventRepositoryMock.Verify(moq => moq.Update(eventEntity), Times.Once);
            Assert.AreEqual(eventPatchDTO.EventName, eventEntity.EventName);
            Assert.AreNotEqual(eventPatchDTO.EventDescription, eventEntity.EventDescription);
        }

    }
}
