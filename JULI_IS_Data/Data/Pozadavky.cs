namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pozadavky")]
    public partial class Pozadavky
    {
        public int ID { get; set; }

        [StringLength(400)]
        public string Popis { get; set; }

        public float CelkovaCena { get; set; }

        [StringLength(5)]
        public string Mena { get; set; }


        public int? ZpusobPlatbyId { get; set; }

        [StringLength(1000)]
        public string ZpusobPlatbyText { get; set; }

        [StringLength(50)]
        public string NabidkaCislo { get; set; }


        public int PocetPolozek { get; set; }

        public bool? Smazano { get; set; }

        [StringLength(50)]
        public string SmazalUzivatel { get; set; }

        [StringLength(50)]
        public string Zalozil { get; set; }

        [StringLength(50)]
        public string Zastoupeno { get; set; }

        public DateTime? Datum { get; set; }

        [StringLength(20)]
        public string FullPozadavekID { get; set; }

        public DateTime? DatumSmazani { get; set; }

        [StringLength(10)]
        public string KST { get; set; }

        public bool InvesticePlanovana { get; set; }

        public bool InvesticeNeplanovana { get; set; }

        public bool NakupOstatni { get; set; }

        [StringLength(12)]
        public string CisloInvestice { get; set; }

        [StringLength(12)]
        public string CisloKonta { get; set; }

        [StringLength(400)]
        public string Poznamka { get; set; }

        public int PodpisLevel { get; set; }
        public int SchvalovatelID { get; set; }
        
        public bool Level1Schvaleno { get; set; }

        public bool Level2Schvaleno { get; set; }

        public bool Level3Schvaleno { get; set; }

        public int? Level1SchvalovatelID { get; set; }

        public DateTime? Level1SchvalenoDne { get; set; }

        public DateTime? Level2SchvalenoDne { get; set; }

        public DateTime? Level3SchvalenoDne { get; set; }

        public int? Level2SchvalovatelID { get; set; }

        public int? Level3SchvalovatelID { get; set; }

        public bool Level1Odeslano { get; set; }

        public bool Level2Odeslano { get; set; }

        public bool Level3Odeslano { get; set; }

        public DateTime? Level1OdeslanoDne { get; set; }

        public DateTime? Level2OdeslanoDne { get; set; }

        public DateTime? Level3OdeslanoDne { get; set; }

        public bool Zamitnuto { get; set; }

        public bool Objednano { get; set; }
        
        public int? DuvodZamitnutiID { get; set; }

        public DateTime? ZamitnutoDne { get; set; }

        [StringLength(200)]
        public string DuvodZamitnutiText { get; set; }

        public int? DodavatelID { get; set; }
        public int DodavatelS21ID { get; set; }

        public bool Neverejny { get; set; }

        [StringLength(50)]
        public string Stav { get; set; }
    }
}
