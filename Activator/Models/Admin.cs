using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Activator.Models
{
    class Admin
    {
        public string AId { get; set; }
        public string AName { get; set; }
        public string APassword { get; set; }
        public string APhone { get; set; }
        public BitmapImage AImage{ get; set; }
    }
}
