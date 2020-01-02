using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAssembly.Models
{
    class Message
    {
        private string _sender;
        private string _content;
        private DateTime _time;

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }
    }
}
