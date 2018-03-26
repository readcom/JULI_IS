using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.DTO
{
    public class UsersDTO
    {
      
        public int ID { get; set; }

        public int JULINumber { get; set; }

        public string Jmeno { get; set; }

        public string User { get; set; }

        public int Uroven { get; set; }

        public string Email { get; set; }

        public string Telefon { get; set; }
        public bool OdesilatMaily { get; set; }
        public bool NacitatDodavatele { get; set; }

    }
}
