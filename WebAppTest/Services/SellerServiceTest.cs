using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;

namespace WebAppTest.Services
{
    public class SellerServiceTest
    {
        #region snippet_AddSeller_SingleSeller_DepartmentSellersShouldNotBeEmpty
        [Fact]
        public void FindAllAsync_ReturnAllSellersAsync()
        {
            var fixture = CreateFixtureWithoutRecursion();


            // preciso de um moq para dbContext 
            // preciso de um seller service

            //moq
            //var department = fixture.Create<Department>();
            //var seller = fixture.Create<Seller>();

            // act
            //department.AddSeller(seller);

            // assert
            //department.Sellers.Should().NotBeEmpty();
        }
        #endregion

        private IFixture CreateFixtureWithoutRecursion()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}
