using CDMS.Model;
using System.Collections.Generic;
using System;

namespace CDMS.Service
{
    public interface ITokenService
    {
        Guid GetToken();

        //Dictionary<string, String> GetConditionKind();
    }
}
