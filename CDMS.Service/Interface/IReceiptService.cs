using CDMS.Model;
using CDMS.Model.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CDMS.Service
{
    public interface IReceiptService
    {
        void Create(Receipt info);
        void Update(Receipt info);
        void Delete(Receipt info);

        Receipt Get(string id);
        
        IQueryable<Receipt> GetAll();

        bool IsUsed(Receipt info);

        bool IsCheckNumExists(Receipt info);

        IQueryable<ReceiptListViewModel> GetListView();
    }
}
