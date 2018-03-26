using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.DTO
{
    public class CiselnikDTO
    {

        public int ID { get; set; }

        public int Typ { get; set; }

        public string Cislo { get; set; }

        public string Popis { get; set; }
   
        public string FullName { get; set; }

    }
}
