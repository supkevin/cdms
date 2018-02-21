using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface IInquiryComplexService
    {
        InquiryComplex Create(InquiryComplex model);

        void Update(InquiryComplex model);
        void Delete(InquiryComplex model);

        void RemoveChild(long id);

        InquiryComplex Get(string id);
        
        IQueryable<InquiryComplex> GetAll();

        InquiryHistoryViewModel GetHistory(string productID);

        bool IsUsed(InquiryComplex info);
    }
}
