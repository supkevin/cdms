using CDMS.Model;
using System.Collections.Generic;
using System.Linq;

namespace CDMS.Service
{
    public interface INewsService
    {
        void Create(News model);
        void Update(News model);
        void Delete(News model);

        News Get(long id);
        
        IQueryable<News> GetAll();

        bool IsUsed(News info);
    }
}
