using System;
using Xunit;
using WebApplication1.Controllers;
using WebApplication1.Context;

namespace WebApplication1;

public class UnitTest1
{
    private readonly ProductContext product;
    public UnitTest1()
    {
        product = new ProductController();
    }

    [Fact]
    public void ProductControllerTests_ShouldReturnSuccess()
    {
        var controller = new ProductController();
        int ExpectedResult = 50;

        int result = controller.CheckoutProduct("A");

        Assert.Equal(ExpectedResult, result);

    }
}