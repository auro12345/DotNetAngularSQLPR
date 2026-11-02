using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.Application.Interfaces;
using Xunit;

namespace ProductManagement.Tests;

public class ProductDeleteControllerTests
{
    [Fact]
    public void AIK_005_AC_1_Delete_existingProduct_returnsNoContentAndRepositoryDeletion()
    {
        var service = new Mock<IProductService>();
        service.Setup(item => item.Delete(7)).Returns(true);
        var controller = new ProductsController(service.Object);

        var result = controller.Delete(7);

        Assert.IsType<NoContentResult>(result);
        service.Verify(item => item.Delete(7), Times.Once);
    }

    [Fact]
    public void AIK_005_AC_2_Delete_missingProduct_returnsNotFound()
    {
        var service = new Mock<IProductService>();
        service.Setup(item => item.Delete(404)).Returns(false);
        var controller = new ProductsController(service.Object);

        var result = controller.Delete(404);

        Assert.IsType<NotFoundResult>(result);
    }
}
