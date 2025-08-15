//-----------------------------------------------------------------------
// <copyright file="TransactionsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PayTabsClient.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayTabsClient.Data;
using PayTabsClient.Helpers;
using PayTabsClient.Models;

public class TransactionsController : Controller
{
    private readonly PayTabsClientContext context;

    public TransactionsController(PayTabsClientContext context)
    {
        this.context = context;
    }

    // GET: Transactions
    public async Task<IActionResult> Index()
    {
        return this.View(await this.context.Transaction.ToListAsync());
    }

    // GET: Transactions/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return this.NotFound();
        }

        var transaction = await this.context.Transaction
            .FirstOrDefaultAsync(m => m.Id == id);
        if (transaction is null)
        {
            return this.NotFound();
        }

        return this.View(transaction);
    }

    // GET: Transactions/Create
    public IActionResult Create()
    {
        return this.View();
    }

    // POST: Transactions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ProfileId,Endpoint,ServerKey,TranType,TranClass,CartId,CartCurrency,CartAmount,CartDescription,PaypageLang,HideShipping,IsFramed,ReturnURL,CallbackURL")] Transaction transaction)
    {
        if (this.ModelState.IsValid)
        {
            this.context.Add(transaction);
            await this.context.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }
        return this.View(transaction);
    }


    // GET: Transactions/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return this.NotFound();
        }

        var transaction = await this.context.Transaction
            .FirstOrDefaultAsync(m => m.Id == id);
        if (transaction is null)
        {
            return this.NotFound();
        }

        return this.View(transaction);
    }

    // POST: Transactions/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transaction = await this.context.Transaction.FindAsync(id);
        this.context.Transaction.Remove(transaction);
        await this.context.SaveChangesAsync();
        return this.RedirectToAction(nameof(this.Index));
    }

    private bool TransactionExists(int id)
    {
        return this.context.Transaction.Any(e => e.Id == id);
    }


    //

    public async Task<Transaction_Response> Pay(int id)
    {
        var transaction = await this.context.Transaction
            .FirstOrDefaultAsync(m => m.Id == id);
        if (transaction == null)
        {
            return null; // NotFound();
        }

        Connector c = new Connector();
        Transaction_Response r = c.Send(transaction);

        if (r.IsSuccess())
        {
            transaction.Tran_Ref = r.tran_ref;
            transaction.TriedToPay = true;

            this.context.Update(transaction);
            await this.context.SaveChangesAsync();

            this.Response.Redirect(r.redirect_url);
        }

        return r; // RedirectToAction(nameof(Details), new { id });
    }

    //

    [HttpPost]
    public async Task<IActionResult> Webhook([FromForm] Transaction_Result content)
    {
        var transaction = await this.context.Transaction
            .FirstOrDefaultAsync(m => m.CartId == content.cartId);

        if (transaction == null)
        {
            return this.NotFound();
        }

        bool valid = content.IsValid_Signature(transaction.ServerKey);

        transaction.IsValid_Signature = valid;
        transaction.IsSucceed = content.IsSucceed();

        this.context.Update(transaction);
        await this.context.SaveChangesAsync();


        return this.View(content);
    }

    //

    [HttpPost]
    public void IPN([FromBody] Transaction_IPN ipn)
    {
        //using var reader = new StreamReader(Request.Body);
        //var body = await reader.ReadToEndAsync();
        //System.Console.WriteLine(body);
        //return body;

        System.Console.WriteLine(ipn);

        return;
    }
}
