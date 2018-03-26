namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Objednavky")]
    public partial class Objednavky
    {
        public int ID { get; set; }

        public DateTime? Datum { get; set; }

        public DateTime? TerminDodani { get; set; }

        [StringLength(500)]
        public string CelkovyPopis { get; set; }

        [StringLength(50)]
        public string NabidkaCislo { get; set; }

        public bool Smazano { get; set; }

        public string SmazalUzivatel { get; set; }

        public DateTime? SmazanoDne { get; set; }

        [StringLength(60)]
        public string ObjednavatelName { get; set; }

        [StringLength(60)]
        public string FullObjednavkaID { get; set; }

        // new

        public int? ObjednavatelID { get; set; }
        public int DodavatelID { get; set; }
        public int? DodavatelS21ID { get; set; }

        public float CelkovaCena { get; set; }

        [StringLength(5)]
        public string Mena { get; set; }

        public bool Objednano { get; set; }
        public DateTime? DatumObjednani { get; set; }

        public int? KST1 { get; set; }
        public int? KST2 { get; set; }
        public int? KST3 { get; set; }

        public int TypInvestice { get; set; }

        [StringLength(50)]
        public string HlavniRada { get; set; }

        public int? TextCenaId { get; set; }

        public int? TextDodaciPodmId { get; set; }

        public int? TextPlatebniPodmId { get; set; }

        [StringLength(200)]
        public string TextCenaText { get; set; }

        [StringLength(200)]
        public string TextDodaciPodmText { get; set; }

        [StringLength(200)]
        public string TextPlatebniPodmText { get; set; }



        public int? SchvalovatelID { get; set; }
        public DateTime? OdeslanoNaSchvaleniDne { get; set; }
        public DateTime? SchvalenoDne { get; set; }
        public DateTime? ZamitnutoDne { get; set; }
        public bool Schvaleno { get; set; }
        public bool Zamitnuto { get; set; }
        public int? DuvodZamitnutiID { get; set; }

        [StringLength(200)]
        public string DuvodZamitnutiText { get; set; }

        public bool Odeslano { get; set; }
        public DateTime? OdeslanoDne { get; set; }

        public bool Dokonceno { get; set; }
        public DateTime? DokoncenoDne { get; set; }

        public bool Vytisknuto { get; set; }
        public DateTime? VytisknutoDne { get; set; }

        public bool Neodesilat { get; set; }
        public int PocetPolozek { get; set; }

        public bool AvizoDoruceni { get; set; }
        public DateTime? DatumDodani { get; set; }

        public string Poznamka { get; set; }

        public bool Stornovano { get; set; }

    }
}
