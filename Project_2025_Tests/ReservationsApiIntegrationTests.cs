using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project_2025_Web;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTOs;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Project_2025_Tests
{
    [TestClass]
    public class ReservationsApiIntegrationTests
    {
        private static WebApplicationFactory<Program> _factory!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.Single(d =>
                            d.ServiceType == typeof(DbContextOptions<DataContext>));
                        services.Remove(descriptor);

                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("IntegrationTestDb");
                        });

                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });

                        services.AddScoped<IPermissionService, DummyPermissionService>();

                        var sp = services.BuildServiceProvider();
                        using (var scope = sp.CreateScope())
                        {
                            var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                            ctx.Database.EnsureCreated();

                            ctx.Roles.Add(new Role { Id = 1, Name = "User" });

                            ctx.Users.Add(new User
                            {
                                Id = 1,
                                Username = "integrationUser",
                                Email = "int@example.com",
                                PasswordHash = new byte[0],
                                RoleId = 1
                            });
                            ctx.Plans.Add(new Plan
                            {
                                Id = 1,
                                Name = "Integration Plan",
                                Description = "Desc",
                                Basic_Price = 50m,
                                Type_Difficulty = 1,
                                Max_Persons = 10,
                                Distance = 5
                            });
                            ctx.SaveChanges();
                        }
                    });

                    builder.ConfigureWebHost(webHost =>
                    {
                        webHost.Configure(app =>
                        {
                            app.UseRouting();
                            app.UseAuthentication();
                            app.UseAuthorization();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                            });
                        });
                        webHost.UseEnvironment("Development");
                    });
                });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory.Dispose();
        }

        [TestMethod]
        public async Task GetAll_ReturnsEmptyArray_WhenNoReservations()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/Reservations");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var arr = await response.Content.ReadFromJsonAsync<ReservationDTO[]>();
            Assert.IsNotNull(arr);
            Assert.AreEqual(0, arr.Length);
        }

        [TestMethod]
        public async Task CreateThenGetById_ReturnsCreatedEntity()
        {
            var client = _factory.CreateClient();
            var newDto = new ReservationDTO
            {
                Id_Plan = 1,
                Date = DateTime.Today,
                Status = "OK",
                Person_Number = 2
            };

            var postResponse = await client.PostAsJsonAsync("/api/Reservations", newDto);
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
            var created = await postResponse.Content.ReadFromJsonAsync<ReservationDTO>();
            Assert.IsNotNull(created);
            Assert.IsTrue(created.Id > 0);

            var getResponse = await client.GetAsync($"/api/Reservations/{created.Id}");
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var fetched = await getResponse.Content.ReadFromJsonAsync<ReservationDTO>();
            Assert.IsNotNull(fetched);
            Assert.AreEqual(created.Id, fetched.Id);
            Assert.AreEqual("OK", fetched.Status);
        }

        [TestMethod]
        public async Task Delete_ReturnsBadRequest_WhenIdDoesNotExist()
        {
            var client = _factory.CreateClient();
            var response = await client.DeleteAsync("/api/Reservations/999");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var client = _factory.CreateClient();
            var newDto = new ReservationDTO
            {
                Id_Plan = 1,
                Date = DateTime.Today,
                Status = "X",
                Person_Number = 1
            };
            var post = await client.PostAsJsonAsync("/api/Reservations", newDto);
            var created = await post.Content.ReadFromJsonAsync<ReservationDTO>();

            var deleteResponse = await client.DeleteAsync($"/api/Reservations/{created!.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
