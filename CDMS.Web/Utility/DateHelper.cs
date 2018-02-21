using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDMS.Web
{
    public class DateHelper
    {
        public static int GetBetweenBirthdayCount(DateTime pFrom, DateTime pTo, DateTime pBirthday)
        {
            int mBirthdayCount = 0;//會過幾次生日

            int TotalYear = pTo.Year - pFrom.Year;//看區間有幾年
            
            for (int i = 0; i <= TotalYear; i++)
            {
                DateTime EveryBirthday = DateTime.Parse(pFrom.AddYears(i).Year + "-" + pBirthday.Month.ToString().PadLeft(2, '0') + "-" + pBirthday.Day.ToString().PadLeft(2, '0'));
                if (pFrom <= EveryBirthday && pTo >= EveryBirthday)
                {
                    mBirthdayCount++;
                }
            }

            return mBirthdayCount;
        }
    }
}