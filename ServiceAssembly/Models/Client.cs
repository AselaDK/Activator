using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAssembly.Models
{
    class Client
    {
        private string _name;
        private int _avatarID;
        private DateTime _time;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int AvatarID
        {
            get { return _avatarID; }
            set { _avatarID = value; }
        }
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }
    }
}
