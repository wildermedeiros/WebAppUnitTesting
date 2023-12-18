using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using WebApp.Controllers;
using WebApp.DatabaseContext;
using WebApp.Models;
using WebApp.Services;


namespace WebAppTest.Services
{
    public class SellerServiceTest
    {
        private readonly Fixture fixture;
        private readonly Mock<IDbContext> dbContext;
        private readonly SellerService sellerService;

        public SellerServiceTest()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            dbContext = new Mock<IDbContext>(); 
            sellerService = new SellerService(dbContext.Object);
        }

        [Fact]
        public async Task FindAllAsync_ReturnAllSellersAsync()
        {
            var users = fixture.CreateMany<Seller>().AsQueryable().BuildMockDbSet();

            // Populei esses dados no meu dbset
            dbContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => users.Object);

            // Quando eu executar esse comando
            var result = await sellerService.FindAllAsync();

            // O resultado deverá ser o mesmo conteúdo que foi preparado no dbset
            result.Should().BeEquivalentTo(users.Object);
        }

        //#region snippet_AddSeller_SingleSeller_DepartmentSellersShouldNotBeEmpty
        //[Fact]
        //public void FindAllAsync_ReturnAllSellersAsync()
        //{

        //}

        // #endregion

        //private List<Seller> GetTestSellers()
        //{
        //    var sellers = new List<Seller>();
        //    sellers.Add(new Seller()
        //    {
        //        Name = "Test One"
        //    });
        //    sellers.Add(new Seller()
        //    {
        //        Name = "Test Two"
        //    });
        //    return sellers;
        //}

        //[Fact]
        //public async Task FindAllAsync_ReturnAllSellersAsyncWithoutFixtures()
        //{
        //    var mockDbContext = new Mock<IDbContext>();
        //    var mockDbSet = new Mock<DbSet<Seller>>();
        //    mockDbSet.Setup(s => s.ToListAsync()).ReturnsAsync();
        //    mockDbContext.Setup(context => context.Set<Seller>().ToListAsync(default)).Returns(GetTestSellers().As<Task>);
        //    var sellerService = new SellerService(mockDbContext.Object);

        //    var result = await sellerService.FindAllAsync();

        //    result.Should().NotBeEmpty();
        //}

        //public async Task Index_ReturnsAViewResult_WithAListOfBrainstormsellers()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<IBrainstormSessionRepository>();
        //    mockRepo.Setup(repo => repo.ListAsync())
        //        .ReturnsAsync(GetTestsellers());
        //    var controller = new HomeController(mockRepo.Object);

        //    // Act
        //    var result = await controller.Index();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<IEnumerable<StormSessionViewModel>>(
        //        viewResult.ViewData.Model);
        //    //assert.equal(2, model.count());
        //    Assert.True(model.Any());
        //}

        //private IFixture CreateFixtureWithoutRecursion()
        //    {
        //        IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
        //        //fixture.Behaviors
        //        //    .OfType<ThrowingRecursionBehavior>()
        //        //    .ToList()
        //        //    .ForEach(b => fixture.Behaviors.Remove(b));
        //        //fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        //        return fixture;
        //    }
    }
}
