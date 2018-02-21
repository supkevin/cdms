using CDMS.Model;
using System.Collections.Generic;
namespace CDMS.Service
{
    public interface IBankAccountService
    {
        void Create(BankAccount info);
        void Update(BankAccount info);
        void Delete(BankAccount info);

        BankAccount Get(long id);
        
        IEnumerable<BankAccount> GetAll();

        bool IsDataExists(BankAccount info);

        bool IsUsed(BankAccount info);
    }
}
