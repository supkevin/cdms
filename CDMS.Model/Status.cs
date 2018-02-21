using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class Status : CodeValue<Status>
    {
        public int Type { get; set; }

        static List<Status> _Source = null;
        static List<Status> _UserAllow = null;

        public Status(int value, string text, int type) : base(value, text)
        {
            this.Type = type; 
        }

        public static List<Status> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.Status>();
                _Source.Add(Status.Draft);
                _Source.Add(Status.Wait);
                _Source.Add(Status.WIP);
                _Source.Add(Status.Done);
                _Source.Add(Status.Close);
            }

            return _Source;
        }

        public static List<Status> GetProcessStatus()
        {
            if (null == _UserAllow)
            {
                _UserAllow = new List<Model.Status>();
                _UserAllow.Add(Status.Wait);
                _UserAllow.Add(Status.WIP);
                _UserAllow.Add(Status.Done);
                _UserAllow.Add(Status.Close);
            }

            return _UserAllow;
        }
        public static List<Status> GetAdministrtorStatus()
        {
            if (null == _Source)
            {
                _Source = new List<Model.Status>();
                _Source.Add(Status.Draft);
                _Source.Add(Status.Wait);           
                _Source.Add(Status.Close);
            }

            return _Source;
        }
        public static Status FromValue(int value)
        {
            if (value == 0)
                return Draft;
            if (value == 10)
                return Wait;
            if (value == 20)
                return WIP;
            if (value == 50)
                return Done;
            if (value == 99)
                return Close;
            return null;
        }

        public static Status Draft => new Status(0, "草稿", 3 );
        public static Status Wait => new Status(10, "待處理", 3);
        public static Status WIP => new Status(20, "處理中",1);
        public static Status Done => new Status(50, "已處理",1);
        public static Status Close => new Status(99, "結案",3);
    }
}
