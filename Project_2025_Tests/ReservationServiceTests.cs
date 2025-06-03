using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//  ← ESTA LÍNEA NECESARIA
using AutoMapper;

// ← Estos must match exactamente el namespace donde definiste cada clase en tu proyecto principal:
using Project_2025_Web.Data;           // DataContext
using Project_2025_Web.Data.Entities;  // Role, User, Plan, MappingProfile
using Project_2025_Web.DTOs;           // ReservationDTO, ReservationFilterDTO
using Project_2025_Web.Services;       // IReservationService, ReservationService

namespace Project_2025_Tests
{
    [TestClass]
    public class ReservationServiceTests
    {
        private IMapper _mapper!;

        [TestInitialize]
        public void TestInitialize()
        {
            // Configuramos AutoMapper con tu MappingProfile real:
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();
        }

        private DataContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var ctx = new DataContext(options);

            // Semilla mínima: Role, User y Plan
            ctx.Roles.Add(new Role { Id = 1, Name = "User" });
            ctx.Users.Add(new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                RoleId = 1
            });
            ctx.Plans.Add(new Plan
            {
                Id = 1,
                Name = "PlanTest",
                Description = "Desc test",
                Basic_Price = 100m,
                Type_Difficulty = 1,
                Max_Persons = 5,
                Distance = 10
            });

            ctx.SaveChanges();
            return ctx;
        }

        [TestMethod]
        public async Task GetListAsync_WhenNoReservations_ReturnsEmptyResponse()
        {
            // Arrange
            var ctx = CreateInMemoryContext();
            var service = new ReservationService(ctx, _mapper);

            // Act
            var response = await service.GetListAsync();

            // Assert
            Assert.IsTrue(response.IsSucess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(0, response.Result.Count);
        }

        // ... aquí van el resto de tus TestMethod ...
    }
}
