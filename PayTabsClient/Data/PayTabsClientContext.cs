using System;
using Microsoft.EntityFrameworkCore;
using PayTabsClient.Models;

namespace PayTabsClient.Data
{
    public class PayTabsClientContext : DbContext
    {
        public PayTabsClientContext(DbContextOptions<PayTabsClientContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Transaction> Transaction { get; set; }
    }
}
