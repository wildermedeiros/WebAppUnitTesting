using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
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


// todo colocar os mocks no construtor, está ocupando espaço como argumento dos metodos
namespace WebAppTest.Controllers
{
    public class ServiceControllerShould
    {
        private readonly IFixture fixture;
        private readonly Mock<ISellerService> mockSellerService;
        private readonly Mock<IDepartmentService> mockDepartmentService;


        public ServiceControllerShould()
        {
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            mockSellerService = new Mock<ISellerService>();
            mockDepartmentService = new Mock<IDepartmentService>();
        }

        [Theory]
        [AutoDataAttributeWebApp]
        public async Task ReturnViewForIndex(List<Seller> sellers)
        {
            SellersController sut = new SellersController(mockSellerService.Object, mockDepartmentService.Object);
            
            mockSellerService.Setup(x => x.FindAllAsync()).Returns(Task.FromResult(sellers));

            var result = await sut.Index();

            result.Should().BeOfType<ViewResult>();
            mockSellerService.Verify(x => x.FindAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ReturnViewForCreateGet()
        {
            SellersController sut = new SellersController(mockSellerService.Object, mockDepartmentService.Object);

            var result = await sut.Create();

            result.Should().BeOfType<ViewResult>();
            mockDepartmentService.Verify(x => x.FindAllAsync(), Times.Once);
        }


        [Theory]
        [AutoDataAttributeWebApp]
        public async Task ReturnViewWhenInvalidModelStateForCreatePost(string key, Seller seller, string testError)
        {
            SellersController sut = new SellersController(mockSellerService.Object, mockDepartmentService.Object);

            sut.ModelState.AddModelError(key, testError);
            var result = await sut.Create(seller);

            var viewResult = result.Should().BeOfType<ViewResult>();

            //result.Should().BeOfType<RedirectToActionResult>();
            //mockSellerService.Verify(x => x.InsertAsync(It.IsAny<Seller>()), Times.Once);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Seller seller)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var departments = await _departmentService.FindAllAsync();
        //        var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
        //        return View(viewModel);
        //    }
        //    await _mockSellerService.InsertAsync(seller);
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
