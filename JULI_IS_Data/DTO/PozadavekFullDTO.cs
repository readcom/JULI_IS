using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.DTO
{
    public class PozadavekFullDTO
    {
        public int ID { get; set; }
        public string Vlozil { get; set; }
        public Nullable<System.DateTime> Datum { get; set; }

        public string Popis { get; set; }
        public string Mena { get; set; }
        public Nullable<float> Cena { get; set; }


        //public string Nazev { get; set; }
        //public string Adresa { get; set; }
        //public string IC { get; set; }
        //public string DIC { get; set; }

        //public string FileName { get; set; }
        //public string FullPath { get; set; }
        //public string Description { get; set; }
    }
}
