using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
// Debe coincidir exactamente con el namespace de tu controlador API:
using Project_2025_Web.Controllers.Api;
// Entidades (solo si necesitas devolver o comparar Reservation u objetos similares):
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTO;
// DTOs:
using Project_2025_Web.DTOs;
// Interfaz del servicio de reservas:
using Project_2025_Web.Services;
// Interfaz del servicio de permisos:
using Project_2025_Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_2025_Tests
{
    [TestClass]
    public class ReservationsApiControllerTests
    {
        // 1) Declaramos los mocks como campos privados:
        private Mock<IReservationService> _mockService!;
        private Mock<IPermissionService> _mockPermissionService!;

        // 2) Declaramos el controlador a probar:
        private ReservationsController _controller!;

        // 3) En lugar de un constructor con parámetros, usamos TestInitialize:
        [TestInitialize]
        public void Setup()
        {
            // Creamos los mocks:
            _mockService = new Mock<IReservationService>();
            _mockPermissionService = new Mock<IPermissionService>();

            // Inyectamos ambos mocks en el constructor del controlador:
            _controller = new ReservationsController(
                _mockService.Object,
                _mockPermissionService.Object
            );
        }

        [TestMethod]
        public async Task GetReservations_ReturnsOk_WithDTOList()
        {
            // Arrange
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

            // El controlador, en GetReservations, llama a _reservationService.GetFilteredAsync(...)
            _mockService
                .Setup(s => s.GetFilteredAsync(It.IsAny<ReservationFilterDTO>()))
                .ReturnsAsync(fakeDtoList);

            // Act
            var actionResult = await _controller.GetReservations();

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Se esperaba OkObjectResult");
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(fakeDtoList, okResult.Value);
        }

        [TestMethod]
        public async Task GetReservation_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetOneAsync(5))
                .ReturnsAsync(new Response<ReservationDTO>
                {
                    IsSucess = false,
                    Message = "La reserva no existe"
                });

            // Act
            var result = await _controller.GetReservation(5);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetReservation_ReturnsOk_WhenServiceReturnsDTO()
        {
            // Arrange
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

            // Act
            var actionResult = await _controller.GetReservation(1);
            var okResult = actionResult.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(fakeDto, okResult.Value);
        }

        [TestMethod]
        public async Task CreateReservation_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange: forzamos ModelState inválido
            _controller.ModelState.AddModelError("Status", "Required");
            var dto = new ReservationDTO();

            // Act
            var result = await _controller.CreateReservation(dto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task CreateReservation_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
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

            // Act
            var result = await _controller.CreateReservation(dto);
            var badReq = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(badReq);
            Assert.AreEqual(400, badReq.StatusCode);
            Assert.AreEqual("No pudo crear", badReq.Value);
        }

        [TestMethod]
        public async Task CreateReservation_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
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

            // Act
            var result = await _controller.CreateReservation(dto);
            var createdAtAction = result.Result as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(createdAtAction);
            Assert.AreEqual(nameof(_controller.GetReservation), createdAtAction.ActionName);
            Assert.AreEqual(createdEntity, createdAtAction.Value);
            Assert.AreEqual(10, createdAtAction.RouteValues["id"]);
        }

        [TestMethod]
        public async Task UpdateReservation_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var dto = new ReservationDTO { Id = 2 };

            // Act
            var result = await _controller.UpdateReservation(1, dto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateReservation_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
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

            // Act
            var result = await _controller.UpdateReservation(1, dto);

            // Assert
            var badReq = result as BadRequestObjectResult;
            Assert.IsNotNull(badReq);
            Assert.AreEqual(400, badReq.StatusCode);
            Assert.AreEqual("No existe", badReq.Value);
        }

        [TestMethod]
        public async Task UpdateReservation_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
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

            // Act
            var result = await _controller.UpdateReservation(1, dto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteReservation_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAsync(5))
                .ReturnsAsync(new Response<object>
                {
                    IsSucess = false,
                    Message = "No existe"
                });

            // Act
            var result = await _controller.DeleteReservation(5);

            // Assert
            var badReq = result as BadRequestObjectResult;
            Assert.IsNotNull(badReq);
            Assert.AreEqual(400, badReq.StatusCode);
            Assert.AreEqual("No existe", badReq.Value);
        }

        [TestMethod]
        public async Task DeleteReservation_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAsync(3))
                .ReturnsAsync(new Response<object>
                {
                    IsSucess = true
                });

            // Act
            var result = await _controller.DeleteReservation(3);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
