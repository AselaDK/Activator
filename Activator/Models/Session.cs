using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activator.Models
{

    class Session
    {
        private readonly bool adminStatus = false;
        private readonly string adminId = "";
        public string MyStatus { get; set; }
        public string MyId { get; set; }
        public Session(bool astatus, string aid)
        {
            this.adminStatus = astatus;
            this.adminId = aid;
        }
    }
}
