using AutoFixture;
using AutoFixture.Xunit2;
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
        private readonly Mock<IDbContext> mockContext;
        private readonly SellerService sut;

        public SellerServiceShould()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            mockContext = new Mock<IDbContext>();
            mockContext.SetupAllProperties();
            sut = new SellerService(mockContext.Object);
        }

        public void Dispose()
        {
            
        }

        [Fact]
        public async Task FindAndReturnAllSellersAsync()
        {
            var sellers = fixture.CreateMany<Seller>().AsQueryable().BuildMockDbSet();

            // Populei esses dados no meu dbset
            mockContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => sellers.Object);

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

            mockContext.Setup(c => c.Instance.Set<Seller>().AddAsync(seller, It.IsAny<CancellationToken>()));

            await sut.InsertAsync(seller);

            mockContext.Verify(c => c.Instance.Set<Seller>().AddAsync(seller, It.IsAny<CancellationToken>()), Times.Once);
            mockContext.Verify(c => c.Instance.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockContext.VerifyNoOtherCalls();
        }

        // Exemplo de um método que força uma exceção, tende a erro, conforme o tipo de método (FirstAsync, FirstOrDefaultAsync, etc)
        [Theory]
        [InlineData(1, 2)]
        public async Task NotUpdateInexistentIdWithMockSetup(int notPersistedId, int persistedId)
        {
            var seller = new Seller(notPersistedId);
            var sellers = fixture.Build<Seller>()
                .With(s => s.Id, persistedId)
                .CreateMany()
                .AsQueryable()
                .BuildMockDbSet();

            mockContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => sellers.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => sut.UpdateAsync(seller));

            ex.Should().BeOfType<Exception>();
            sellers.Object.Should().NotContain(seller);
            ex.Message.Should().Be("Id not found");
        }

        // Exemplo de um método que lança uma exceção, Mock não tem suporte para SingleOrDefaultAsync method
        [Fact]
        public async Task ThrowExceptionOnFirstOrDefaultAsyncWithMockSetup()
        {
            var seller = fixture.Create<Seller>();
            seller.Id = 1;

            var sellers = fixture.Build<Seller>()
                .With(s => s.Id, 1)
                .CreateMany()
                .AsQueryable()
                .BuildMockDbSet();

            mockContext.Setup(c => c.Instance.Set<Seller>()).Returns(() => sellers.Object);

            sellers.Setup(s => s.SingleOrDefaultAsync(It.IsAny<Expression<Func<Seller, bool>>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("Throwing a exception"));

            var ex = await Assert.ThrowsAsync<Exception>(() => sut.UpdateAsync(seller));

            ex.Should().BeOfType<Exception>();
            mockContext.Verify(c => c.Instance.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // Exemplo de um método sem setup dos mocks, para throws, é uma solução pobre, pois tem que forçar um cenário para o teste passar
        [Fact]
        public async Task ThrowExceptionManyEqualsId()
        {
            var seller = fixture.Create<Seller>();
            seller.Id = 1;

            var mockSetSeller = fixture.Build<Seller>()
                .With(s => s.Id, 1)
                .CreateMany()
                .AsQueryable()
                .BuildMockDbSet();

            mockContext.Setup(c => c.Instance.Set<Seller>()).Returns(mockSetSeller.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => sut.UpdateAsync(seller));

            ex.Should().BeOfType<Exception>();
            mockContext.Verify(c => c.Instance.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        // Exemplo de um método sem setup dos mocks
        [Fact]
        public async Task UpdateOnSingleOrDefaultId()
        {
            var seller = fixture.Create<Seller>();
            seller.Id = 1;

            var mockSetSeller = fixture.Build<Seller>()
                .With(s => s.Id, 1)
                .CreateMany(1)
                .AsQueryable()
                .BuildMockDbSet();

            mockContext.Setup(c => c.Instance.Set<Seller>()).Returns(mockSetSeller.Object);

            await sut.UpdateAsync(seller);

            mockSetSeller.Object.Should().Contain(s => s.Id == seller.Id);
            mockSetSeller.Verify(s => s.Update(It.IsAny<Seller>()), Times.Once);
            mockContext.Verify(c => c.Instance.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Esse tipo de verificação nao é possível fazer, pois o mockContext não foi preparado para tal
            //mockContext.Verify(c => c.Instance.Set<Seller>().Update(seller), Times.Once);
            //mockContext.Verify(c => c.Instance.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        //[Fact]
        //public async Task UpdateIfValidId()
        //{

        //}
    }
}
