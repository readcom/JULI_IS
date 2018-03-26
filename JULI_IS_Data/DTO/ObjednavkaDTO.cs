using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.DTO
{
    public class ObjednavkaDTO
    {

        public int ID { get; set; }

        public string FullObjednavkaID { get; set; }

        public int? ObjednavatelID { get; set; }

        public int DodavatelID { get; set; }
        public int? DodavatelS21ID { get; set; }
        public string NabidkaCislo { get; set; }
        public int PocetPolozek { get; set; }

        public string ObjednavatelName { get; set; }

        public DateTime? Datum { get; set; }
        public DateTime? TerminDodani { get; set; }

        public string CelkovyPopis { get; set; }

        public float CelkovaCena { get; set; }

        public string Mena { get; set; }

        public int? KST1 { get; set; }
        public int? KST2 { get; set; }
        public int? KST3 { get; set; }

        public string HlavniRada { get; set; }

        public int TypInvestice { get; set; }

        public int? TextCenaId { get; set; }

        public int? TextDodaciPodmId { get; set; }

        public int? TextPlatebniPodmId { get; set; }

        public string TextCenaText { get; set; }

        public string TextDodaciPodmText { get; set; }

        public string TextPlatebniPodmText { get; set; }

        /// <summary>
        /// odeslano na schvaleni
        /// </summary>
        public bool Objednano { get; set; }

        public DateTime? DatumObjednani { get; set; }
        
        public string PozadavekZalozil { get; set; }

        public string FullDodavatelName { get; set; }
       
        public bool Smazano { get; set; }

        public string SmazalUzivatel { get; set; }

        public DateTime? SmazanoDne { get; set; }

        public int? SchvalovatelID { get; set; }
        public DateTime? OdeslanoNaSchvaleniDne { get; set; }
        public DateTime? SchvalenoDne { get; set; }
        public DateTime? ZamitnutoDne { get; set; }
        
        /// <summary>
        /// schvalena obj. ved. nakupu
        /// </summary>
        public bool Schvaleno { get; set; }
        public bool Zamitnuto { get; set; }
        public int? DuvodZamitnutiID { get; set; }
        public string DuvodZamitnutiText { get; set; }

        public bool Odeslano { get; set; }
        public DateTime? OdeslanoDne { get; set; }

        public bool Dokonceno { get; set; }
        public DateTime? DokoncenoDne { get; set; }


        public bool Vytisknuto { get; set; }
        public DateTime? VytisknutoDne { get; set; }

        public bool Neodesilat { get; set; }
        public bool AvizoDoruceni { get; set; }
        public DateTime? DatumDodani { get; set; }
        public string Poznamka { get; set; }
        public bool Stornovano { get; set; }
    }
}
