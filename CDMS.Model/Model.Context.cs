﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CDMS.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }
        public virtual DbSet<Inspection> Inspection { get; set; }
        public virtual DbSet<Inspection_Image> Inspection_Image { get; set; }
        public virtual DbSet<Observation> Observation { get; set; }
        public virtual DbSet<OverSea> OverSea { get; set; }
        public virtual DbSet<OverType> OverType { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Track> Track { get; set; }
        public virtual DbSet<Workplace> Workplace { get; set; }
        public virtual DbSet<PurchaseInvoiceDetail> PurchaseInvoiceDetail { get; set; }
        public virtual DbSet<SalesInvoiceDetail> SalesInvoiceDetail { get; set; }
        public virtual DbSet<MenuPermission> MenuPermission { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<Alternative> Alternative { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<Code> Code { get; set; }
        public virtual DbSet<Inquiry> Inquiry { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<v_Price> v_Price { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<v_CodeUsed> v_CodeUsed { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public virtual DbSet<InquiryDetail> InquiryDetail { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Quotation> Quotation { get; set; }
        public virtual DbSet<QuotationDetail> QuotationDetail { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<v_CustomerLatestSales> v_CustomerLatestSales { get; set; }
        public virtual DbSet<v_LatestSales> v_LatestSales { get; set; }
        public virtual DbSet<v_StockTrack> v_StockTrack { get; set; }
        public virtual DbSet<StockChange> StockChange { get; set; }
        public virtual DbSet<StockChangeDetail> StockChangeDetail { get; set; }
        public virtual DbSet<v_Stock> v_Stock { get; set; }
        public virtual DbSet<SalesDetail> SalesDetail { get; set; }
        public virtual DbSet<Receivable> Receivable { get; set; }
        public virtual DbSet<v_ReceivableSummary> v_ReceivableSummary { get; set; }
        public virtual DbSet<v_ReceivableDetail> v_ReceivableDetail { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Receipt> Receipt { get; set; }
        public virtual DbSet<BankDeposit> BankDeposit { get; set; }
        public virtual DbSet<PurchaseInvoice> PurchaseInvoice { get; set; }
        public virtual DbSet<SalesInvoice> SalesInvoice { get; set; }
        public virtual DbSet<v_ProductSalesRanking> v_ProductSalesRanking { get; set; }
    }
}
