using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDMS.Language;

namespace CDMS.Web
{
    public class StringClass
    {

        public static string GetTrueOrFalse(bool mBool)
        {
            return mBool ? "TextTrue".ToLocalized() : "TestFalse".ToLocalized();

        }



    }
}