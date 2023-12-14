using FluentAssertions;
using WebApp.Models;
using AutoFixture;
using AutoFixture.Xunit2;

namespace WebAppTest
{
    public class DepartmentTest
    {
        #region snippet_AddSeller_SingleSeller_DepartmentSellersShouldNotBeEmpty
        [Fact]
        public void AddSeller_SingleSeller_DepartmentSellersShouldNotBeEmpty()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = fixture.Create<Department>();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            department.Sellers.Should().NotBeEmpty();  
        }
        #endregion

        #region snippet_AddSeller_SingleSeller_DepartmentSellersContainSellerAdded
        [Fact]
        public void AddSeller_SingleSeller_DepartmentSellersContainSellerAdded()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = fixture.Create<Department>();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            department.Sellers.Should().Contain(seller);
        }
        #endregion

        #region snippet_AddSeller_SingleSeller_DepartmentShouldNotBeNull
        [Fact]
        public void AddSeller_SingleSeller_DepartmentShouldNotBeNull()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = fixture.Create<Department>();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            department.Should().NotBeNull();
        }
        #endregion

        #region snippet_AddSeller_SingleSeller_DepartmenteShouldBeDepartment
        [Fact]
        public void AddSeller_SingleSeller_DepartmenteShouldBeDepartment()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = fixture.Create<Department>();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            department.Should().BeOfType<Department>();
        }
        #endregion

        # region snippet_AddSeller_SingleSeller_SellerAddedShouldNotBeNull
        [Fact]
        public void AddSeller_SingleSeller_SellerAddedShouldNotBeNull()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = fixture.Create<Department>();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            seller.Should().NotBeNull();
        }
        #endregion

        #region snippet_AddSeller_SingleSeller_SellerAddedShouldBeSeller
        [Fact]
        public void AddSeller_SingleSeller_SellerAddedShouldBeSeller()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = fixture.Create<Department>();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            seller.Should().BeOfType<Seller>();
        }
        #endregion

        #region snippet_AddSeller_SingleSeller_SellerAddedShouldContainDepartment
        [Fact]
        public void AddSeller_SingleSeller_SellerAddedShouldContainDepartment()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = fixture.Create<Department>();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            seller.Department.Should().NotBeNull();
        }
        #endregion

        private Fixture CreateFixtureWithoutRecursion()
        {
            Fixture fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}