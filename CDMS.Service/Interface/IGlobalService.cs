using CDMS.Model.ViewModel;
using CDMS.Model;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CDMS.Service
{
    public interface IGlobalService
    {
        HashSet<MyCodeValue> GetBankAccountList();

        HashSet<MyTextValue> GetBrandList();

        HashSet<MyTextValue> GetCheckStatusList();

        Dictionary<string, String> GetCompanyShortNameList();

        Dictionary<string, String> GetConditionKindList();

        Dictionary<string, String> GetCurrencyKindList();

        HashSet<MyTextValue> GetCustomerList();

        HashSet<MyTextValue> GetDealItemList();

        HashSet<MyTextValue> GetDepartmentList();

        HashSet<MyTextValue> GetDepositSummaryList();

        Dictionary<string, MenuViewModel> GetMenuList();
                
        HashSet<MyTextValue> GetNewsTypeList();

        Dictionary<string, String> GetPriceKindList();

        HashSet<MyTextValue> GetProductList();
        
        HashSet<MyTextValue> GetProductKindList();

        HashSet<MyTextValue> GetQuotationLevelList();

        Dictionary<string, String> GetShippingModeList();
        
        HashSet<MyTextValue> GetSupplierList();

        HashSet<MyTextValue> GetTitleList();

        HashSet<MyTextValue> GetTicketTypeList();

        HashSet<MyTextValue> GetUnitKindList();

        HashSet<MyTextValue> GetUserList();

        Dictionary<string, String> GetWarehouseList();
    }
}
