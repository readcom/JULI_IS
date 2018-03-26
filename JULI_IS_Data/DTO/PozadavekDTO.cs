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
    public class PozadavekDTO
    {

        public int ID { get; set; }
        public string IDstr { get; set; }

        public string FullPozadavekID { get; set; }

        public string NabidkaCislo { get; set; }

        public string Zalozil { get; set; }

        public string Zastoupeno { get; set; }

        public DateTime? Datum { get; set; }

        public string Popis { get; set; }

        public string Poznamka { get; set; }

        public float CelkovaCena { get; set; }

        public string Mena { get; set; }

        public int? ZpusobPlatbyId { get; set; }
        public string ZpusobPlatbyText { get; set; }

        public string DodavatelName { get; set; }

        public int DodavatelID { get; set; }
        public int DodavatelS21ID { get; set; }

        public bool Smazano { get; set; }

        public string SmazalUzivatel { get; set; }

        public DateTime? DatumSmazani { get; set; }

        // [Required(ErrorMessage ="prosím vyplňte KST")]
        public string KST { get; set; }

        public bool InvesticePlanovana { get; set; }
        public bool InvesticeNeplanovana { get; set; }
        public bool NakupOstatni { get; set; }

        public string CisloInvestice { get; set; }

        public string CisloKonta { get; set; }

        public int PocetPolozek { get; set; }


        public int PodpisLevel { get; set; }

        /// <summary>
        /// ID vedoucího, kterému byl požadavek určen na schválení
        /// </summary>
        public int SchvalovatelID { get; set; }

        public string Stav { get; set; }


        // vedouci
        public bool Level1Odeslano { get; set; }
        public DateTime? Level1OdeslanoDne { get; set; }
        public string Level1OdeslanoJmeno { get; set; }

        /// <summary>
        /// ID vedoucího, který požadavek skutečně podepsal
        /// </summary>
        public int Level1SchvalovatelID { get; set; }
        public string Level1SchvalovatelJmeno { get; set; }
        public string Level1SchvalovatelUserName { get; set; }
        public bool Level1Schvaleno { get; set; }
        public DateTime? Level1SchvalenoDne { get; set; }

        // reditel, controling
        public bool Level2Odeslano { get; set; }
        public DateTime? Level2OdeslanoDne { get; set; }
        public int Level2SchvalovatelID { get; set; }
        public string Level2SchvalovatelJmeno { get; set; }
        public string Level2SchvalovatelUserName { get; set; }
        public bool Level2Schvaleno { get; set; }
        public DateTime? Level2SchvalenoDne { get; set; }

        //vedouci nakupu
        public bool Level3Odeslano { get; set; }
        public DateTime? Level3OdeslanoDne { get; set; }
        public int Level3SchvalovatelID { get; set; }
        public string Level3SchvalovatelJmeno { get; set; }
        public string Level3SchvalovatelUserName { get; set; }
        public bool Level3Schvaleno { get; set; }
        public DateTime? Level3SchvalenoDne { get; set; }

        // zamitnuti pozadavku
        public bool Zamitnuto { get; set; }
        public DateTime? ZamitnutoDne { get; set; }
        public int? DuvodZamitnutiID { get; set; }
        public string DuvodZamitnutiText { get; set; }

        public bool Objednano { get; set; }
        public bool Neverejny { get; set; }

    }

}
