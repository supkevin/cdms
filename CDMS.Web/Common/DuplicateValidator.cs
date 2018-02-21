using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDMS.Web.Common
{
    public class DuplicateValidator
    {
        public List<string> Message { get; set; }

        private void Validate(List<KeyValuePair<string, int>> info)
        {
            if (info.Count > 0)
            {
                foreach (var item in info)
                {
                    this.Message.Add(string.Format("產品不可重複，產品編號:{0}<br/>", item.Key));
                }
            }
        }

        public DuplicateValidator(List<KeyValuePair<string, int>> info)
        {
            this.Message = new List<string>();
            Validate(info);
        }
    }
}