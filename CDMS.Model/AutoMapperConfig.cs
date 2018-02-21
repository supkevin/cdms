using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AutoMapper;
using CDMS.Model.ViewModel;

namespace CDMS.Model
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(
                cfg =>
                {
                    // AccountID
                    cfg.CreateMap<BankAccount, BankAccountViewModel>();
                    cfg.CreateMap<BankAccountViewModel, BankAccount>();
                    
                    // BankDeposit
                    cfg.CreateMap<BankDeposit, SimpleBankDepositViewModel>();
                    cfg.CreateMap<SimpleBankDepositViewModel, BankDeposit>();

                    // Brand
                    cfg.CreateMap<Brand, BrandViewModel>();
                    cfg.CreateMap<BrandViewModel, Brand>();

                    // Code
                    cfg.CreateMap<Code, CodeViewModel>();
                    cfg.CreateMap<CodeViewModel, Code>();
                                    
                    // Company
                    cfg.CreateMap<Company, CompanyViewModel>();
                    cfg.CreateMap<CompanyViewModel, Company>();

                    cfg.CreateMap<Alternative, AlternativeViewModel>();
                    cfg.CreateMap<AlternativeViewModel, Alternative>();

                    // Inquiry
                    cfg.CreateMap<Inquiry, InquiryViewModel>();
                    cfg.CreateMap<InquiryViewModel, Inquiry>();

                    // InquiryDetail
                    cfg.CreateMap<InquiryDetail, InquiryDetailViewModel>();
                    cfg.CreateMap<InquiryDetailViewModel, InquiryDetail>();
                    
                    // News
                    cfg.CreateMap<News, NewsViewModel>();
                    cfg.CreateMap<NewsViewModel, News>();
                    
                    // Payment
                    cfg.CreateMap<Payment, PaymentViewModel>();
                    cfg.CreateMap<PaymentViewModel, Payment>();

                    // Product
                    cfg.CreateMap<Product, ProductViewModel>();
                    cfg.CreateMap<ProductViewModel, Product>();

                    // Purchase
                    cfg.CreateMap<Purchase, PurchaseViewModel>();
                    cfg.CreateMap<PurchaseViewModel, Purchase>();

                    // PurchaseDetail
                    cfg.CreateMap<PurchaseDetail, PurchaseDetailViewModel>();
                    cfg.CreateMap<PurchaseDetailViewModel, PurchaseDetail>();
                    
                    // PurchaseInvoice
                    cfg.CreateMap<PurchaseInvoice, PurchaseInvoiceViewModel>();
                    cfg.CreateMap<PurchaseInvoiceViewModel, PurchaseInvoice>();

                    // PurchaseInvoiceDetail
                    cfg.CreateMap<PurchaseInvoiceDetail, PurchaseInvoiceDetailViewModel>();
                    cfg.CreateMap<PurchaseInvoiceDetailViewModel, PurchaseInvoiceDetail>();

                    // ProductImage
                    cfg.CreateMap<ProductImage, ProductImageViewModel>();
                    cfg.CreateMap<ProductImageViewModel, ProductImage>();
                    
                    // Receipt
                    cfg.CreateMap<Receipt, ReceiptViewModel>();
                    cfg.CreateMap<ReceiptViewModel, Receipt>();

                    // Quotation
                    cfg.CreateMap<Quotation, QuotationViewModel>()
                    .ForMember(x => x.Auditor, y => y.Ignore())
                    .ForMember(x => x.ReviewDate, y => y.Ignore())
                    .ForMember(x => x.Result, y => y.Ignore())
                    .ForMember(x => x.SalesID, y => y.Ignore())
                    .ForMember(x => x.SalesDate, y => y.Ignore());

                    cfg.CreateMap<QuotationViewModel, Quotation>();

                    // QuotationDetail
                    cfg.CreateMap<QuotationDetail, QuotationDetailViewModel>();
                    cfg.CreateMap<QuotationDetailViewModel, QuotationDetail>();

                    // Sales
                    cfg.CreateMap<Sales, SalesViewModel>();
                    cfg.CreateMap<SalesViewModel, Sales>();

                    // SalesDetail
                    cfg.CreateMap<SalesDetail, SalesDetailViewModel>();
                    cfg.CreateMap<SalesDetailViewModel, SalesDetail>();

                    // SalesInvoice
                    cfg.CreateMap<SalesInvoice, SalesInvoiceViewModel>();
                    cfg.CreateMap<SalesInvoiceViewModel, SalesInvoice>();

                    // SalesInvoiceDetail
                    cfg.CreateMap<SalesInvoiceDetail, SalesInvoiceDetailViewModel>();
                    cfg.CreateMap<SalesInvoiceDetailViewModel, SalesInvoiceDetail>();

                    // StockChange
                    cfg.CreateMap<StockChange, StockChangeViewModel>();
                    cfg.CreateMap<StockChangeViewModel, StockChange>();

                    // StockChangeDetail
                    cfg.CreateMap<StockChangeDetail, StockChangeDetailViewModel>();
                    cfg.CreateMap<StockChangeDetailViewModel, StockChangeDetail>();

                    // User
                    cfg.CreateMap<User, UserViewModel>();
                    cfg.CreateMap<UserViewModel, User>();


                    //// Inquiry
                    //cfg.CreateMap<Inquiry, InquiryComplex>()
                    //  .ForMember(dest => dest.ChildList, 
                    //             dest => dest.Ignore());


                    //// 直接下條件
                    //cfg.CreateMap<InspectionViewModel, Inspection>()
                    // .ForMember(dest => dest.Inspection_Mistake,
                    //            opt => opt
                    //    .MapFrom(src => src.InspectionMistakeList
                    //                    .Where(x => x.IsDeleted == false && x.ID_Inspection_Mistake == 0).ToList()));                                      
                });
        }

        //private static void AfterMap2(InspectionViewModel dto, Inspection client, ResolutionContext resolutionContext)
        //{
        //    if (dto.InspectionMistakeList == null)
        //    {
        //        return;
        //    }

        //    foreach (var vm in dto.InspectionMistakeList.Where(x => x.IsDeleted == false))
        //    {
        //        if (client.Inspection_Mistake == null)
        //        {
        //            client.Inspection_Mistake = new HashSet<Inspection_Mistake>();
        //        }

        //        var mistake = resolutionContext.Mapper.Map<Inspection_Mistake>(vm);
        //        client.Inspection_Mistake.Add(mistake);
        //    }
        //}

        //private static void AfterMap(
        //    Inspection client, InspectionViewModel dto, ResolutionContext resolutionContext)
        //{
        //    if (client.Inspection_Mistake == null)
        //    {
        //        return;
        //    }

        //    //IQueryable<IdentityRole> roles = null;
        //    //CreateMap<User, UserDto>()
        //    //    .ForMember(x => x.Roles, opt =>
        //    //        opt.MapFrom(src =>
        //    //            src.Roles
        //    //                .Join(roles, a => a.RoleId, b => b.Id, (a, b) => b.Name)
        //    //                .ToList()
        //    //        )
        //    //    );            
        //    foreach (var mistake in client.Inspection_Mistake)
        //    {
        //        if (dto.InspectionMistakeList == null)
        //        {
        //            dto.InspectionMistakeList = new HashSet<InspectionMistakeViewModel>();
        //        }

        //        var vm = resolutionContext.Mapper.Map<InspectionMistakeViewModel>(mistake);
        //        dto.InspectionMistakeList.Add(vm);
        //    }
        //}
    }
}
