using CDMS.Model;
using System.Collections.Generic;

namespace CDMS.Service
{
    public interface IInspectionImageService
    {

        Inspection_Image Get(int id_inspection_image);

        void Delete(Inspection_Image model);

        void DeleteWithOutSaveChange(int id_inspection);

        IEnumerable<Inspection_Image> GetForInspection(int id_inspection);
      
    }
}
