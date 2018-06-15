﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace X_SMS_DAL.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class XSmsEntities : DbContext
    {
        public XSmsEntities()
            : base("name=XSmsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<Transcation> Transcations { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Sector> Sectors { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Trend> Trends { get; set; }
        public virtual DbSet<PlayerStock> PlayerStocks { get; set; }
    
        public virtual int BuyStocks(Nullable<int> playerID, Nullable<int> playerAccID, Nullable<int> quantity, Nullable<int> stockID, Nullable<decimal> price)
        {
            var playerIDParameter = playerID.HasValue ?
                new ObjectParameter("playerID", playerID) :
                new ObjectParameter("playerID", typeof(int));
    
            var playerAccIDParameter = playerAccID.HasValue ?
                new ObjectParameter("playerAccID", playerAccID) :
                new ObjectParameter("playerAccID", typeof(int));
    
            var quantityParameter = quantity.HasValue ?
                new ObjectParameter("quantity", quantity) :
                new ObjectParameter("quantity", typeof(int));
    
            var stockIDParameter = stockID.HasValue ?
                new ObjectParameter("stockID", stockID) :
                new ObjectParameter("stockID", typeof(int));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("price", price) :
                new ObjectParameter("price", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BuyStocks", playerIDParameter, playerAccIDParameter, quantityParameter, stockIDParameter, priceParameter);
        }
    
        public virtual int SellStocks(Nullable<int> playerID, Nullable<int> playerAccID, Nullable<int> quantity, Nullable<int> stockID, Nullable<decimal> price)
        {
            var playerIDParameter = playerID.HasValue ?
                new ObjectParameter("playerID", playerID) :
                new ObjectParameter("playerID", typeof(int));
    
            var playerAccIDParameter = playerAccID.HasValue ?
                new ObjectParameter("playerAccID", playerAccID) :
                new ObjectParameter("playerAccID", typeof(int));
    
            var quantityParameter = quantity.HasValue ?
                new ObjectParameter("quantity", quantity) :
                new ObjectParameter("quantity", typeof(int));
    
            var stockIDParameter = stockID.HasValue ?
                new ObjectParameter("stockID", stockID) :
                new ObjectParameter("stockID", typeof(int));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("price", price) :
                new ObjectParameter("price", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SellStocks", playerIDParameter, playerAccIDParameter, quantityParameter, stockIDParameter, priceParameter);
        }
    }
}
