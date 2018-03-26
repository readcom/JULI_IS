using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.DTO
{
    public class DodavateleDTO_old
    {

        public int ID { get; set; }

        public string JULINumber { get; set; }

        public string Nazev { get; set; }

        public string Adresa { get; set; }

        public string Adresa2 { get; set; }

        public string Adresa3 { get; set; }

        public string AdresaDodatek { get; set; }

        public string Mena { get; set; }

        public string Zeme { get; set; }
    
        public string Telefon { get; set; }

        public string CisloNazev { get; set; }

        public IEnumerable<int> PozadavkyId { get; set; }



    }
}
