using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.DTO
{
    public class ObjednavkyDTO
    {

        public int ID { get; set; }

        public string FullObjednavkaID { get; set; }

        public int? ObjednavatelID { get; set; }

        public string ObjednavatelName { get; set; }

        public DateTime? Datum { get; set; }

        public string CelkovyPopis { get; set; }

        public float CelkovaCena { get; set; }

        public string Mena { get; set; }

        public int? KST1 { get; set; }
        public int? KST2 { get; set; }
        public int? KST3 { get; set; }

        public string HlavniRada { get; set; }

        public int? TextCenaId { get; set; }

        public int? TextDodaciPodmId { get; set; }

        public int? TextPlatebniPodmId { get; set; }

        public bool Objednano { get; set; }

        public DateTime? DatumObjednani { get; set; }

        public DateTime? TerminDodani { get; set; }
        
        public string PozadavekZalozil { get; set; }

        public string FullDodavatelName { get; set; }

        public int? DodavatelID { get; set; }
        
        public bool Smazano { get; set; }

        public string SmazalUzivatel { get; set; }

        public DateTime? SmazanoDne { get; set; }

        public int? SchvalovatelID { get; set; }
        public DateTime? OdeslanoNaSchvaleniDne { get; set; }
        public DateTime? SchvalenoDne { get; set; }
        public DateTime? ZamitnutoDne { get; set; }
        public bool Schvaleno { get; set; }
        public bool Zamitnuto { get; set; }
        public int? DuvodZamitnutiID { get; set; }
        public string DuvodZamitnutiText { get; set; }

    }
}
