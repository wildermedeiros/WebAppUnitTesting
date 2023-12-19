using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Security.Cryptography.Xml;
using WebApp.DatabaseContext;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.Exceptions;


namespace WebAppTest.Services
{
    public class SellerServiceShould : IDisposable
    {
        private readonly Fixture fixture;
        private readonly Mock<IDbContext> dbContext;
        private readonly SellerService sut;

        public SellerServiceShould()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            dbContext = new Mock<IDbContext>(); 
            sut = new SellerService(dbContext.Object);
        }

        public void Dispose()
        {
            
        }

        [Fact]
        public async Task FindAndReturnAllSellersAsync()
        {
            var sellers = fixture.CreateMany<Seller>().AsQueryable().BuildMockDbSet();

            // Populei esses dados no meu dbset
            dbContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => sellers.Object);

            // Quando eu executar esse comando
            var result = await sut.FindAllAsync();

            // O resultado deverá ser o mesmo conteúdo que foi preparado no dbset
            result.Should().BeEquivalentTo(sellers.Object);
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task InsertSellerAsync()
        {
            var seller = fixture.Create<Seller>();

            dbContext.Setup(c => c.Instance.Set<Seller>().AddAsync(seller, It.IsAny<CancellationToken>()));

            await sut.InsertAsync(seller);

            dbContext.Verify(c => c.Instance.Set<Seller>().AddAsync(seller, It.IsAny<CancellationToken>()), Times.Once);
            dbContext.Verify(c => c.Instance.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task NotUpdateInexistentId()
        {
            var seller = new Seller(1);
            var sellers = fixture.Build<Seller>()
                .With(s => s.Id, 2)
                .CreateMany(1)
                .AsQueryable()
                .BuildMockDbSet();

            dbContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => sellers.Object);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => sut.UpdateAsync(seller));

            ex.Should().BeOfType<NotFoundException>();
            sellers.Object.Should().NotContain(seller);
            ex.Message.Should().Be("Id not found");
        }

        [Fact]
        public async UpdateIfValidId()
        {

        }

        [Fact]
        public async NotUpdateIfHasConcurrencyOnSaving()
        {

        }
    }
}
