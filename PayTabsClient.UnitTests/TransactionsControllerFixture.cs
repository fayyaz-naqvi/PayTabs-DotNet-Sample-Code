using Microsoft.EntityFrameworkCore;
using PayTabsClient.Controllers;
using PayTabsClient.Data;
using PayTabsClient.Models;

namespace PayTabsClient.xUnitTests;

public class TransactionsControllerFixture
{
    public TransactionsController Controller { get; }

    public TransactionsControllerFixture()
    {
        var transactions = new List<Transaction>
    {
        new Transaction { Id = 1, CartId = "C1", ProfileId = 123, TranType = "Sale", TranClass = "Ecom", CartCurrency = "USD", CartAmount = 100, CartDescription = "Test", PaypageLang = "en", HideShipping = false, IsFramed = false, ReturnURL = "http://return", CallbackURL = "http://callback",  Endpoint = "https://example.com", ServerKey = "test-key", Tran_Ref = "TRX001" },
        new Transaction { Id = 2, CartId = "C2", ProfileId = 456, TranType = "Sale", TranClass = "Ecom", CartCurrency = "EUR", CartAmount = 200, CartDescription = "Test2", PaypageLang = "en", HideShipping = true, IsFramed = true, ReturnURL = "http://return2", CallbackURL = "http://callback2", Endpoint = "https://example.com", ServerKey = "test-key", Tran_Ref = "TRX002" }
    };

        var options = new DbContextOptionsBuilder<PayTabsClientContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        var context = new PayTabsClientContext(options);
        context.Transaction.AddRange(transactions);
        context.SaveChanges();

        Controller = new TransactionsController(context);
    }
}
