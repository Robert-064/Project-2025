using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project_2025_Web.Controllers.Api;

using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTO;

using Project_2025_Web.DTOs;

using Project_2025_Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_2025_Tests
{
    [TestClass]
    public class ReservationsApiControllerTests
    {
        private Mock<IReservationService> _mockService!;
        private Mock<IPermissionService> _mockPermissionService!;
        private ReservationsController _controller!;

        [TestInitialize]
        public void Setup()
        {

            _mockService = new Mock<IReservationService>();
            _mockPermissionService = new Mock<IPermissionService>();
            _controller = new ReservationsController(
                _mockService.Object,
                _mockPermissionService.Object
            );
        }

        [TestMethod]
        public async Task GetReservations_ReturnsOk_WithDTOList()
        {
            var fakeDtoList = new List<ReservationDTO>
            {
                new ReservationDTO
                {
                    Id = 1,
                    Id_Plan = 1,
                    Id_User = 1,
                    Date = System.DateTime.Today,
                    Status = "A",
                    Person_Number = 2
                },
                new ReservationDTO
                {
                    Id = 2,
                    Id_Plan = 1,
                    Id_User = 1,
                    Date = System.DateTime.Today,
                    Status = "B",
                    Person_Number = 3
                }
            };

            _mockService
                .Setup(s => s.GetFilteredAsync(It.IsAny<ReservationFilterDTO>()))
                .ReturnsAsync(fakeDtoList);

            var actionResult = await _controller.GetReservations();

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Se esperaba OkObjectResult");
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(fakeDtoList, okResult.Value);
        }

        [TestMethod]
        public async Task GetReservation_ReturnsNotFound_WhenServiceFails()
        {
            _mockService
                .Setup(s => s.GetOneAsync(5))
                .ReturnsAsync(new Response<ReservationDTO>
                {
                    IsSucess = false,
                    Message = "La reserva no existe"
                });

            var result = await _controller.GetReservation(5);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetReservation_ReturnsOk_WhenServiceReturnsDTO()
        {
            var fakeDto = new ReservationDTO
            {
                Id = 1,
                Id_Plan = 1,
                Id_User = 1,
                Date = System.DateTime.Today,
                Status = "OK",
                Person_Number = 2
            };
            _mockService
                .Setup(s => s.GetOneAsync(1))
                .ReturnsAsync(new Response<ReservationDTO>
                {
                    IsSucess = true,
                    Result = fakeDto,
                    Message = "Encontrada"
                });

            var actionResult = await _controller.GetReservation(1);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(fakeDto, okResult.Value);
        }

        [TestMethod]
        public async Task CreateReservation_ReturnsBadRequest_WhenModelInvalid()
        {
            _controller.ModelState.AddModelError("Status", "Required");
            var dto = new ReservationDTO();

            var result = await _controller.CreateReservation(dto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task CreateReservation_ReturnsBadRequest_WhenServiceFails()
        {
            var dto = new ReservationDTO
            {
                Id_Plan = 1,
                Date = System.DateTime.Today,
                Status = "OK",
                Person_Number = 2
            };
            _mockService
                .Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(new Response<Reservation>
                {
                    IsSucess = false,
                    Message = "No pudo crear"
                });

            var result = await _controller.CreateReservation(dto);
            var badReq = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(badReq);
            Assert.AreEqual(400, badReq.StatusCode);
            Assert.AreEqual("No pudo crear", badReq.Value);
        }

        [TestMethod]
        public async Task CreateReservation_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var dto = new ReservationDTO
            {
                Id_Plan = 1,
                Date = System.DateTime.Today,
                Status = "OK",
                Person_Number = 2
            };
            var createdEntity = new Reservation
            {
                Id = 10,
                Id_Plan = 1,
                Id_User = 1,
                Date = System.DateTime.Today,
                Status = "OK",
                Person_Number = 2
            };
            _mockService
                .Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(new Response<Reservation>
                {
                    IsSucess = true,
                    Result = createdEntity
                });

            var result = await _controller.CreateReservation(dto);
            var createdAtAction = result.Result as CreatedAtActionResult;

            Assert.IsNotNull(createdAtAction);
            Assert.AreEqual(nameof(_controller.GetReservation), createdAtAction.ActionName);
            Assert.AreEqual(createdEntity, createdAtAction.Value);
            Assert.AreEqual(10, createdAtAction.RouteValues["id"]);
        }

        [TestMethod]
        public async Task UpdateReservation_ReturnsBadRequest_WhenIdMismatch()
        {
            var dto = new ReservationDTO { Id = 2 };

            var result = await _controller.UpdateReservation(1, dto);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateReservation_ReturnsBadRequest_WhenServiceFails()
        {
            var dto = new ReservationDTO
            {
                Id = 1,
                Id_Plan = 1,
                Id_User = 1,
                Date = System.DateTime.Today,
                Status = "OK",
                Person_Number = 2
            };
            _mockService
                .Setup(s => s.EditAsync(dto))
                .ReturnsAsync(new Response<Reservation>
                {
                    IsSucess = false,
                    Message = "No existe"
                });

            var result = await _controller.UpdateReservation(1, dto);

            var badReq = result as BadRequestObjectResult;
            Assert.IsNotNull(badReq);
            Assert.AreEqual(400, badReq.StatusCode);
            Assert.AreEqual("No existe", badReq.Value);
        }

        [TestMethod]
        public async Task UpdateReservation_ReturnsNoContent_WhenSuccessful()
        {
            var dto = new ReservationDTO
            {
                Id = 1,
                Id_Plan = 1,
                Id_User = 1,
                Date = System.DateTime.Today,
                Status = "OK",
                Person_Number = 2
            };
            _mockService
                .Setup(s => s.EditAsync(dto))
                .ReturnsAsync(new Response<Reservation>
                {
                    IsSucess = true
                });

            var result = await _controller.UpdateReservation(1, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteReservation_ReturnsBadRequest_WhenServiceFails()
        {
            _mockService
                .Setup(s => s.DeleteAsync(5))
                .ReturnsAsync(new Response<object>
                {
                    IsSucess = false,
                    Message = "No existe"
                });

            var result = await _controller.DeleteReservation(5);

            var badReq = result as BadRequestObjectResult;
            Assert.IsNotNull(badReq);
            Assert.AreEqual(400, badReq.StatusCode);
            Assert.AreEqual("No existe", badReq.Value);
        }

        [TestMethod]
        public async Task DeleteReservation_ReturnsNoContent_WhenSuccessful()
        {
            _mockService
                .Setup(s => s.DeleteAsync(3))
                .ReturnsAsync(new Response<object>
                {
                    IsSucess = true
                });

            var result = await _controller.DeleteReservation(3);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
