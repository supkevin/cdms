using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using CDMS.Language;

namespace CDMS.Model.ViewModel
{
    //[CreditCard]: Validates the property has a credit card format.
    //[Compare]: Validates two properties in a model match.
    //[EmailAddress]: Validates the property has an email format.
    //[Phone]: Validates the property has a telephone format.
    //[Range]: Validates the property value falls within the given range.
    //[RegularExpression]: Validates that the data matches the specified regular expression.
    //[Required]: Makes a property required.
    //[StringLength]: Validates that a string property has at most the given maximum length.
    //[Url]: Validates the property has a URL format.

    //public class RequiredAttribute : 
    //    System.ComponentModel.DataAnnotations.RequiredAttribute
    //{
    //    private String displayName;

    //    public RequiredAttribute()
    //    {
    //        this.ErrorMessage = "{0} is required";
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var attributes =
    //            validationContext.ObjectType.GetProperty(validationContext.MemberName)
    //            .GetCustomAttributes(typeof(DisplayNameAttribute), true);

    //        if (attributes != null)
    //            this.displayName = (attributes[0] as DisplayNameAttribute).DisplayName;
    //        else
    //            this.displayName = validationContext.DisplayName;

    //        return base.IsValid(value, validationContext);
    //    }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(this.ErrorMessageString, displayName);
    //    }
    //}

    // https://www.codeproject.com/Tips/780992/Asp-Net-MVC-Custom-Compare-Data-annotation-with-Cl
    public enum GenericCompareOperator
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public sealed class GenericCompareAttribute : ValidationAttribute, IClientValidatable
    {
        private GenericCompareOperator operatorname = GenericCompareOperator.GreaterThanOrEqual;

        public string CompareToPropertyName { get; set; }
        public GenericCompareOperator OperatorName { get { return operatorname; } set { operatorname = value; } }
        // public IComparable CompareDataType { get; set; }
    
        //private  static string GetDisplayName(PropertyInfo pInfo, Type metaDataType)
        //{
        //    if (null == pInfo)
        //    {
        //        return String.Empty;
        //    }

        //    string propertyName = pInfo.Name;

        //    DisplayAttribute attr = (DisplayAttribute)metaDataType.GetProperty(propertyName)
        //        .GetCustomAttributes(typeof(DisplayAttribute), true)
        //        .SingleOrDefault();

        //    if (attr == null)
        //    {
        //        MetadataTypeAttribute otherType =
        //            (MetadataTypeAttribute)metaDataType.GetCustomAttributes(typeof(MetadataTypeAttribute), true)
        //            .FirstOrDefault();

        //        if (otherType != null)
        //        {
        //            var property = otherType.MetadataClassType.GetProperty(propertyName);
        //            if (property != null)
        //            {
        //                attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
        //            }
        //        }
        //    }
        //    return (attr != null) ? attr.Name : String.Empty;
        //}

        private string GetDisplayName(string propertyName) {
           return propertyName.ToLocalized();
        }

        private string GetErrorMessage(GenericCompareOperator operatorName, string source, string target)
        {
            string result = "";

            switch (operatorName) {
                case GenericCompareOperator.GreaterThan:
                    result = $"{source} 必須大於 {target}";
                    break;
                case GenericCompareOperator.GreaterThanOrEqual:
                    result = $"{source} 必須大於等於 {target}";
                    break;
                case GenericCompareOperator.LessThan:
                    result = $"{source} 必須小於 {target}";
                    break;
                case GenericCompareOperator.LessThanOrEqual:
                    result = $"{source} 必須小於等於 {target}";
                    break;
            }

            return result;
        }

        public GenericCompareAttribute() : base() { }
        //Override IsValid
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string operstring = (OperatorName == GenericCompareOperator.GreaterThan ?
            "greater than " : (OperatorName == GenericCompareOperator.GreaterThanOrEqual ?
            "greater than or equal to " :
            (OperatorName == GenericCompareOperator.LessThan ? "less than " :
            (OperatorName == GenericCompareOperator.LessThanOrEqual ? "less than or equal to " : ""))));
            var basePropertyInfo = validationContext.ObjectType.GetProperty(CompareToPropertyName);
           
            var valOther = (IComparable)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);

            var valThis = (IComparable)value;
           
            if (valOther == null) {
                return null;
            }
            var compareDisplayName = GetDisplayName(CompareToPropertyName);

            if ((operatorname == GenericCompareOperator.GreaterThan && valThis.CompareTo(valOther) <= 0) ||
                (operatorname == GenericCompareOperator.GreaterThanOrEqual && valThis.CompareTo(valOther) < 0) ||
                (operatorname == GenericCompareOperator.LessThan && valThis.CompareTo(valOther) >= 0) ||
                (operatorname == GenericCompareOperator.LessThanOrEqual && valThis.CompareTo(valOther) > 0))
                //return new ValidationResult(base.ErrorMessage);
                return new ValidationResult(GetErrorMessage(operatorname, validationContext.DisplayName , compareDisplayName));
            return null;
        }
        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule>
        GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string errorMessage = this.FormatErrorMessage(metadata.DisplayName);
            ModelClientValidationRule compareRule = new ModelClientValidationRule();
            compareRule.ErrorMessage = errorMessage;
            compareRule.ValidationType = "genericcompare";
            compareRule.ValidationParameters.Add("comparetopropertyname", CompareToPropertyName);
            compareRule.ValidationParameters.Add("operatorname", OperatorName.ToString());
            yield return compareRule;
        }

        #endregion
    }
}

namespace CDMS.Model.ViewModel
{
    #region BaseBankDeposit
    public class BaseBankDeposit : BankDeposit
    {

        [MaxLength(20)]
        new public string AccountID { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [MaxLength(10)]
        new public string BankID { get; set; }

        [MaxLength(15)]
        new public string CheckNum { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string CheckStatus { get; set; }

        [MaxLength(6)]
        new public string CompanyID { get; set; }

        [MaxLength(10)]
        new public string SourceID { get; set; }

        [MaxLength(10)]
        new public string Summary { get; set; }
    }
    #endregion

    #region BaseCompany
    public class BaseCompany : Company
    {

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [MaxLength(100)]
        new public string Address { get; set; }

        [MaxLength(20)]
        new public string Clerk { get; set; }

        [MaxLength(30)]
        new public string ClerkMobile { get; set; }

        [Required]
        [MaxLength(6)]
        new public string CompanyID { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string CompanyKindID { get; set; }

        [MaxLength(20)]
        new public string ContactPerson { get; set; }

        [MaxLength(100)]
        new public string FactoryAddress { get; set; }

        [MaxLength(20)]
        new public string Fax { get; set; }

        [Required]
        [MaxLength(100)]
        new public string FullName { get; set; }

        [MaxLength(100)]
        new public string InvoiceAddress { get; set; }

        [Required]
        [Range(1, 30)]
        new public int? NextMonth { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [MaxLength(100)]
        new public string ShippingAddress { get; set; }

        [Required]
        [MaxLength(100)]
        new public string ShortName { get; set; }

        [MinLength(8)]
        [MaxLength(8)]
        new public string TaxID { get; set; }

        [Phone]
        [MaxLength(30)]
        new public string Telephone1 { get; set; }

        [Phone]
        [MaxLength(30)]
        new public string Telephone2 { get; set; }
    }
    #endregion

    #region BaseInquiry
    public class BaseInquiry : Inquiry
    {

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [Required]
        [MaxLength(6)]
        new public string CompanyID { get; set; }

        [MaxLength(10)]
        new public string ContactPerson { get; set; }

        [MaxLength(10)]
        new public string ContactPhone { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string CurrencyID { get; set; }

        [Required]
        new public decimal? ExchangeRate { get; set; }

        [Required]
        new public DateTime? InquiryDate { get; set; }

        [MaxLength(10)]
        new public string InquiryID { get; set; }

        [MaxLength(10)]
        new public string PurchaseID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        new public DateTime? ValidateDate { get; set; }
    }
    #endregion

    #region BaseInquiryDetail
    public class BaseInquiryDetail : InquiryDetail
    {

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string ConditionID { get; set; }

        [MaxLength(10)]
        new public string InquiryID { get; set; }

        [Required]
        new public decimal? Price { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string PriceKindID { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        new public int? Qty { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }
    }
    #endregion

    #region BasePayment
    public class BasePayment : Payment
    {

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        new public string AccountMonth { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [Required]
        new public int? BankAccountID { get; set; }

        [Required]
        new public decimal? CashAmount { get; set; }

        [Required]
        new public decimal? CheckAmount { get; set; }

        [Required]
        [MaxLength(15)]
        new public string CheckNum { get; set; }

        [Required]
        new public DateTime? DueDate { get; set; }

        [Required]
        new public DateTime? PayDate { get; set; }

        [MinLength(10)]
        [MaxLength(10)]
        new public string PaymentID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        [MaxLength(6)]
        new public string SupplierID { get; set; }
    }
    #endregion

    #region BaseProduct
    public class BaseProduct : Product
    {

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [MaxLength(200)]
        new public string BOM { get; set; }

        [Required]
        [MaxLength(4)]
        new public string BrandID { get; set; }

        [Required]
        [MaxLength(10)]
        new public string KindID { get; set; }

        [Required]
        new public decimal? LatestCost { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Linked { get; set; }

        [Required]
        new public decimal? ListPrice { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        [MaxLength(30)]
        new public string ProductName { get; set; }

        [Required]
        new public decimal? RealPrice { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        new public int? SafeStock { get; set; }

        [Required]
        new public decimal? SetPrice { get; set; }

        [MaxLength(200)]
        new public string SPEC { get; set; }

        [MaxLength(6)]
        new public string SupplierID { get; set; }

        [Required]
        [MaxLength(10)]
        new public string UnitID { get; set; }
    }
    #endregion

    #region BasePurchase
    public class BasePurchase : Purchase
    {

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        new public string AccountMonth { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [MaxLength(10)]
        new public string ContactPerson { get; set; }

        [MaxLength(10)]
        new public string ContactPhone { get; set; }

        [Required]
        [MaxLength(6)]
        new public string CurrencyID { get; set; }

        [Required]
        new public decimal? ExchangeRate { get; set; }

        [MaxLength(12)]
        new public string InvoiceID { get; set; }

        [Required]
        new public DateTime? PurchaseDate { get; set; }

        [MaxLength(10)]
        new public string PurchaseID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        [MaxLength(6)]
        new public string SupplierID { get; set; }

        [Required]
        [MaxLength(6)]
        new public string WarehouseID { get; set; }
    }
    #endregion

    #region BasePurchaseDetail
    public class BasePurchaseDetail : PurchaseDetail
    {

        [MinLength(1)]
        [MaxLength(1)]
        new public string ConditionID { get; set; }

        [Required]
        new public decimal? Price { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string PriceKindID { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [MaxLength(10)]
        new public string PurchaseID { get; set; }

        [Required]
        new public int? Qty { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }
    }
    #endregion

    #region BasePurchaseInvoice
    public class BasePurchaseInvoice : PurchaseInvoice
    {

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        new public string AccountMonth { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [Required]
        new public DateTime? InvoiceDate { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        new public string InvoiceID { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string InvoiceStatusID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        [MaxLength(6)]
        new public string SupplierID { get; set; }

        [Required]
        new public decimal? Tax { get; set; }

        [Required]
        new public decimal? TaxExcluded { get; set; }

        [MaxLength(8)]
        new public string TaxID { get; set; }

        [Required]
        new public decimal? TaxIncluded { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string TaxLevelID { get; set; }

        [Required]
        new public int? TaxRate { get; set; }

        [MaxLength(100)]
        new public string Title { get; set; }
    }
    #endregion

    #region BasePurchaseInvoiceDetail
    public class BasePurchaseInvoiceDetail : PurchaseInvoiceDetail
    {

        [Required]
        new public decimal? Amount { get; set; }

        [MaxLength(10)]
        new public string InvoiceID { get; set; }

        [Required]
        new public decimal? Price { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        new public int? Qty { get; set; }
    }
    #endregion

    #region BaseQuotation
    public class BaseQuotation : Quotation
    {

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [MaxLength(10)]
        new public string Auditor { get; set; }

        [MaxLength(20)]
        new public string ContactPerson { get; set; }

        [MaxLength(20)]
        new public string ContactPhone { get; set; }

        [Required]
        [MaxLength(6)]
        new public string CustomerID { get; set; }

        [MaxLength(100)]
        new public string InvoiceAddress { get; set; }

        [Required]
        new public DateTime? QuotationDate { get; set; }

        [MaxLength(10)]
        new public string QuotationID { get; set; }

        [Required]
        [MaxLength(10)]
        new public string QuotePerson { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [MaxLength(6)]
        new public string Result { get; set; }

        [MaxLength(10)]
        new public string SalesID { get; set; }

        [MaxLength(100)]
        new public string ShippingAddress { get; set; }

        [Required]
        [MaxLength(6)]
        new public string ShippingModeID { get; set; }

        [MinLength(8)]
        [MaxLength(8)]
        new public string TaxID { get; set; }

        [Required]
        new public int? Total { get; set; }

        [Required]
        new public DateTime? ValidateDate { get; set; }

        [MaxLength(6)]
        new public string WarehouseID { get; set; }
    }
    #endregion

    #region BaseQuotationDetail
    public class BaseQuotationDetail : QuotationDetail
    {

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string ConditionID { get; set; }

        [Required]
        new public decimal? Price { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string PriceKindID { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        new public int? Qty { get; set; }

        [MaxLength(10)]
        new public string QuotationID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }
    }
    #endregion

    #region BaseReceipt
    public class BaseReceipt : Receipt
    {

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        new public string AccountMonth { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [Required]
        new public int? BankAccountID { get; set; }

        [Required]
        new public decimal? CashAmount { get; set; }

        [Required]
        new public decimal? CheckAmount { get; set; }

        [MaxLength(15)]
        new public string CheckNum { get; set; }

        [Required]
        [MaxLength(6)]
        new public string CustomerID { get; set; }

        [Required]
        new public DateTime? ReceiptDate { get; set; }

        [MinLength(10)]
        [MaxLength(10)]
        new public string ReceiptID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }
    }
    #endregion

    #region BaseReceivable
    public class BaseReceivable : Receivable
    {

        [MinLength(4)]
        [MaxLength(4)]
        new public string AccountMonth { get; set; }

        [MaxLength(6)]
        new public string CompanyID { get; set; }

        [MaxLength(10)]
        new public string DealItem { get; set; }

        [MaxLength(10)]
        new public string VoucherID { get; set; }
    }
    #endregion

    #region BaseSales
    public class BaseSales : Sales
    {

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        new public string AccountMonth { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [MaxLength(20)]
        new public string ContactPerson { get; set; }

        [Phone]
        [MaxLength(20)]
        new public string ContactPhone { get; set; }

        [Required]
        [MaxLength(6)]
        new public string CustomerID { get; set; }

        [MaxLength(100)]
        new public string InvoiceAddress { get; set; }

        [MaxLength(10)]
        new public string InvoiceID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        new public DateTime? SalesDate { get; set; }

        [MaxLength(10)]
        new public string SalesID { get; set; }

        [MaxLength(100)]
        new public string ShippingAddress { get; set; }

        [Required]
        [MaxLength(6)]
        new public string ShippingModeID { get; set; }

        [MinLength(8)]
        [MaxLength(8)]
        new public string TaxID { get; set; }

        [Required]
        [MaxLength(6)]
        new public string WarehouseID { get; set; }
    }
    #endregion

    #region BaseSalesDetail
    public class BaseSalesDetail : SalesDetail
    {

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string ConditionID { get; set; }

        [Required]
        new public decimal? Price { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string PriceKindID { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        new public int? Qty { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        [MaxLength(10)]
        new public string SalesID { get; set; }
    }
    #endregion

    #region BaseSalesInvoice
    public class BaseSalesInvoice : SalesInvoice
    {

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        new public string AccountMonth { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [Required]
        [MaxLength(6)]
        new public string CustomerID { get; set; }

        [Required]
        new public DateTime? InvoiceDate { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        new public string InvoiceID { get; set; }

        [MinLength(1)]
        [MaxLength(1)]
        new public string InvoiceStatusID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        new public decimal? Tax { get; set; }

        [Required]
        new public decimal? TaxExcluded { get; set; }

        [MaxLength(8)]
        new public string TaxID { get; set; }

        [Required]
        new public decimal? TaxIncluded { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string TaxLevelID { get; set; }

        [Required]
        new public int? TaxRate { get; set; }

        [MaxLength(100)]
        new public string Title { get; set; }
    }
    #endregion

    #region BaseSalesInvoiceDetail
    public class BaseSalesInvoiceDetail : SalesInvoiceDetail
    {

        [Required]
        new public decimal? Amount { get; set; }

        [MaxLength(10)]
        new public string InvoiceID { get; set; }

        [Required]
        new public decimal? Price { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        new public int? Qty { get; set; }
    }
    #endregion

    #region BaseStock
    public class BaseStock : Stock
    {

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        new public int? Qty { get; set; }

        [Required]
        new public int? SeqNo { get; set; }

        [Required]
        [MaxLength(6)]
        new public string WarehouseID { get; set; }
    }
    #endregion

    #region BaseStockChange
    public class BaseStockChange : StockChange
    {

        [Required]
        new public DateTime? ChangeDate { get; set; }

        [Required]
        [MaxLength(20)]
        new public string ChangePersonID { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string ChangeReasonID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [MaxLength(10)]
        new public string StockChangeID { get; set; }

        [Required]
        [MaxLength(6)]
        new public string WarehouseNewID { get; set; }

        [Required]
        [MaxLength(6)]
        new public string WarehouseOldID { get; set; }
    }
    #endregion

    #region BaseStockChangeDetail
    public class BaseStockChangeDetail : StockChangeDetail
    {

        [Required]
        [MaxLength(20)]
        new public string ProductID { get; set; }

        [Required]
        new public int? Qty { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [MaxLength(10)]
        new public string StockChangeID { get; set; }
    }
    #endregion

    #region BaseUser
    public class BaseUser : User
    {

        [MinLength(1)]
        [MaxLength(1)]
        new public string Activate { get; set; }

        [MaxLength(100)]
        new public string Address { get; set; }

        [Required]
        new public DateTime? BeginDate { get; set; }

        [Required]
        [MaxLength(10)]
        new public string DepartmentID { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        new public string Email { get; set; }

        [Required]
        new public DateTime? OnboardDate { get; set; }

        [Required]
        [MaxLength(20)]
        new public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(1)]
        new public string PermissionID { get; set; }

        [MaxLength(10)]
        new public string QuotationLevelID { get; set; }

        [MaxLength(200)]
        new public string Remarks { get; set; }

        [Required]
        [MaxLength(10)]
        new public string SupervisorID { get; set; }

        [Phone]
        [MaxLength(30)]
        new public string Telephone { get; set; }

        [Required]
        [MaxLength(20)]
        new public string TitleID { get; set; }

        [Required]
        [MaxLength(10)]
        new public string UserID { get; set; }

        [Required]
        [MaxLength(20)]
        new public string UserName { get; set; }
    }
    #endregion

}
