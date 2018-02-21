using CDMS.Model;
using System.Collections.Generic;
namespace CDMS.Service
{
    public interface IFeedbackService
    {
        void Create(Feedback model);
        void Update(Feedback model);
        void Delete(Feedback model);

        Feedback Get(int id_feedback );

        Feedback GetById(int id_feedback);

        IEnumerable<Feedback> GetAll();
    }
}
