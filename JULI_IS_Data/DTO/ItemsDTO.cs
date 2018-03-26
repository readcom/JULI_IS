using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pozadavky.Data;

namespace Pozadavky.DTO
{
    public class ItemsDTO
    {
        public int ID { get; set; }

        public int PozadavekID { get; set; }

        public string FullPozadavekID { get; set; }

        public int CisloRadku { get; set; }

        public string Zalozil { get; set; }

        public string Popis { get; set; }

        public string InterniPoznamka { get; set; }

        //[Required(ErrorMessage = "prosím vyplňte KST")]
        public string KST { get; set; }

        public bool InvesticePlanovana { get; set; }
        public bool InvesticeNeplanovana { get; set; }
        public bool NakupOstatni { get; set; }

        public string CisloInvestice { get; set; }

        public string CisloKonta { get; set; }

        [Required(ErrorMessage = "prosím zadejte jednotku")]
        public string Jednotka { get; set; }

        public float Mnozstvi { get; set; }

        public float CenaZaJednotku { get; set; }

        public float CelkovaCena { get; set; }

        public string Mena { get; set; }

        public DateTime DatumZalozeni { get; set; }

        public DateTime? DatumObjednani { get; set; }

        public DateTime? TerminDodani { get; set; }

        public int DodavatelID { get; set; }

        public string DodavatelName { get; set; }
        public string FullDodavatelName { get; set; }
        public string FullDodavatelNumber { get; set; }
        

        public string Stredisko { get; set; }

        // poptany 3 firmy

        public bool Poptany3Firmy { get; set; }

        public int DuvodID { get; set; }

        public string Duvod { get; set; }


        // smazani polozky

        public bool Smazano { get; set; }

        public string SmazalUzivatel { get; set; }

        public DateTime? DatumSmazani { get; set; }

      // objednavka  

        public bool Objednano { get; set; }

        public int? ObjednavkaID { get; set; }

        public string ObjednavkaFullID { get; set; }

        public string ObjednalUzivatel { get; set; }

        public bool Specha { get; set; }

        public bool Neobjednavat { get; set; }

        public string NabidkaCislo { get; set; }


    }
}
