using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
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
            dbContext.SetupAllProperties();
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
            dbContext.VerifyNoOtherCalls();
        }

        // Exemplo de um método que força uma exceção, tende a erro, conforme o tipo de método (FirstAsync, FirstOrDefaultAsync, etc)
        [Theory]
        [InlineData(1, 2)]
        public async Task NotUpdateInexistentId(int notPersistedId, int persistedId)
        {
            var seller = new Seller(notPersistedId);
            var sellers = fixture.Build<Seller>()
                .With(s => s.Id, persistedId)
                .CreateMany()
                .AsQueryable()
                .BuildMockDbSet();

            dbContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => sellers.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => sut.UpdateAsync(seller));

            ex.Should().BeOfType<Exception>();
            sellers.Object.Should().NotContain(seller);
            ex.Message.Should().Be("Id not found");
        }

        // Exemplo de um método que lança uma exceção, Mock não tem suporte para SingleOrDefaultAsync method
        [Fact]
        public async Task ThrowExceptionOnFirstOrDefaultAsync()
        {
            var seller = fixture.Create<Seller>();
            seller.Id = 1;

            var sellers = fixture.Build<Seller>()
                .With(s => s.Id, 1)
                .CreateMany()
                .AsQueryable()
                .BuildMockDbSet();

            dbContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => sellers.Object);

            sellers.Setup(s => s.SingleOrDefaultAsync(It.IsAny<Expression<Func<Seller, bool>>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("Throwing a exception"));

            var ex = await Assert.ThrowsAsync<Exception>(() => sut.UpdateAsync(seller));

            ex.Should().BeOfType<Exception>();
            dbContext.Verify(c => c.Instance.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        //[Fact]
        //public async Task UpdateIfValidId()
        //{

        //}
    }
}
