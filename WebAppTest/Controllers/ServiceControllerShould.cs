using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApp.Controllers;
using WebApp.Models;
using WebApp.Models.ViewModels;
using WebApp.Services;
using WebApp.Services.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppTest.Controllers
{
    public class ServiceControllerShould
    {
        private readonly IFixture fixture;
        private readonly Mock<ISellerService> mockSellerService;
        private readonly Mock<IDepartmentService> mockDepartmentService;
        private readonly SellersController sut;


        public ServiceControllerShould()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            mockSellerService = new Mock<ISellerService>();
            mockDepartmentService = new Mock<IDepartmentService>();
            sut = new SellersController(mockSellerService.Object, mockDepartmentService.Object);

        }

        [Theory]
        [AutoDataAttributeWebApp]
        public async Task ReturnViewForIndex(List<Seller> sellers)
        {
            mockSellerService.Setup(x => x.FindAllAsync()).Returns(Task.FromResult(sellers));

            var result = await sut.Index();

            result.Should().BeOfType<ViewResult>();
            mockSellerService.Verify(x => x.FindAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ReturnViewForCreate()
        {
            var result = await sut.Create();

            result.Should().BeOfType<ViewResult>();
            mockDepartmentService.Verify(x => x.FindAllAsync(), Times.Once);
        }


        [Theory]
        [AutoDataAttributeWebApp]
        public async Task ReturnViewWhenInvalidModelForCreate(string key, Seller seller, string testError)
        {
            sut.ModelState.AddModelError(key, testError);

            var result = await sut.Create(seller);

            var viewResult = result.Should().BeOfType<ViewResult>();
            var model = viewResult.Subject.Model.Should().BeOfType<SellerFormViewModel>();
            model.Subject.Should().BeOfType<SellerFormViewModel>();
            mockDepartmentService.Verify(x => x.FindAllAsync(), Times.Once);
        }

        [Theory]
        [AutoDataAttributeWebApp]
        public async Task ReturnRedirectToActionWhenValidModelForCreate(Seller seller)
        {
            Seller? savedSeller = null;
            mockSellerService.Setup(x => x.InsertAsync(It.IsAny<Seller>()))
                .Returns(Task.CompletedTask)
                .Callback<Seller>(x => savedSeller = x);


            var result = await sut.Create(seller);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>();
            redirectToActionResult.Subject.ActionName.Should().Be(nameof(SellersController.Index));
            mockSellerService.Verify(x => x.InsertAsync(It.IsAny<Seller>()), Times.Once);
            seller.Should().Be(savedSeller);
        }

        [Fact]
        public async Task ReturnRedirectToActionWhenNullIdForDelete()
        {
            var result = await sut.Delete(null);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>();
            redirectToActionResult.Subject.ActionName.Should().Be(nameof(SellersController.Error));
        }

        [Theory]
        [AutoDataAttributeWebApp]
        public async Task ReturnRedirectToActionWhenNotFoundIdForDelete(Seller seller)
        {
            seller.Id = 1;

            var result = await sut.Delete(seller?.Id);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>();
            redirectToActionResult.Subject.ActionName.Should().Be(nameof(SellersController.Error));
        }

        [Theory]
        [AutoDataAttributeWebApp]
        public async Task ReturnViewResultForDelete(Seller seller)
        {
            mockSellerService.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(seller)).Verifiable();

            var result = await sut.Delete(seller?.Id);

            var viewResult = result.Should().BeOfType<ViewResult>();
            var model = viewResult.Subject.Model.Should().BeOfType<Seller>();
            model.Subject.Should().Be(seller);
            mockSellerService.Verify();
        }
    }
}
