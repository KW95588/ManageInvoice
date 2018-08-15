using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvoiceManager.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace InvoiceManager.DAL
{
    public class InvoiceManagerContext : DbContext
    {
        public InvoiceManagerContext(): base("InvoiceManagerContext")
        {

        }

        public DbSet<Invoice> Invoice { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }


    }
}