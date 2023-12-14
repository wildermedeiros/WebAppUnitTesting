using AutoFixture.Xunit2;
using FluentAssertions;
using WebApp.Models;
using AutoFixture;

namespace WebAppTest
{
    public class DepartmentTest
    {
        [Fact]
        public void AddSeller_SingleSeller_SellerAdded()
        {
            var fixture = CreateFixtureWithoutRecursion();
            var department = CreateDefaultDepartment();
            var seller = fixture.Create<Seller>();

            department.AddSeller(seller);

            department.Should().NotBeNull();
        }

        private Department CreateDefaultDepartment()
        {
            var fixture = CreateFixtureWithoutRecursion();
            return fixture.Create<Department>();
        }

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