using Microsoft.AspNetCore.Mvc;
using PayTabsClient.Controllers;
using PayTabsClient.Models;

namespace PayTabsClient.xUnitTests;

public class TransactionsControllerTests : IClassFixture<TransactionsControllerFixture>
{
    private readonly TransactionsController _controller;

    //private Mock<PayTabsClientContext> _mockContext;
    //private Mock<DbSet<Transaction>> _mockSet;
    //private List<Transaction> _transactions;

    //public TransactionsControllerTests()
    //{
    //    _transactions = new List<Transaction>
    //    {
    //        new Transaction { Id = 1, CartId = "C1", ProfileId = 123, TranType = "Sale", TranClass = "Ecom", CartCurrency = "USD", CartAmount = 100, CartDescription = "Test", PaypageLang = "en", HideShipping = false, IsFramed = false, ReturnURL = "http://return", CallbackURL = "http://callback" },
    //        new Transaction { Id = 2, CartId = "C2", ProfileId = 456, TranType = "Sale", TranClass = "Ecom", CartCurrency = "EUR", CartAmount = 200, CartDescription = "Test2", PaypageLang = "en", HideShipping = true, IsFramed = true, ReturnURL = "http://return2", CallbackURL = "http://callback2" }
    //    };

    //    var queryable = _transactions.AsQueryable();
    //    _mockSet = new Mock<DbSet<Transaction>>();
    //    _mockSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(queryable.Provider);
    //    _mockSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(queryable.Expression);
    //    _mockSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
    //    _mockSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

    //    _mockContext = new Mock<PayTabsClientContext>(new DbContextOptions<PayTabsClientContext>());
    //    _mockContext.Setup(c => c.Transaction).Returns(_mockSet.Object);

    //    _controller = new TransactionsController(_mockContext.Object);
    //}

    public TransactionsControllerTests(TransactionsControllerFixture fixture)
    {
        _controller = fixture.Controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithTransactions()
    {
        var result = await _controller.Index();
        var viewResult = result as ViewResult;
        Assert.NotNull(viewResult);
        var model = viewResult.Model as List<Transaction>;
        Assert.Equal(2, model?.Count);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenIdIsNull()
    {
        var result = await _controller.Details(null);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenTransactionNotFound()
    {
        var result = await _controller.Details(999);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsView_WhenTransactionFound()
    {
        var result = await _controller.Details(1);
        var viewResult = result as ViewResult;
        Assert.NotNull(viewResult);
        var model = viewResult.Model as Transaction;
        Assert.Equal(1, model?.Id);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        var result = _controller.Create();
        Assert.IsType<ViewResult>(result);
    }
}
