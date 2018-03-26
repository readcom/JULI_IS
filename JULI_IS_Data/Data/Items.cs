namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Items")]
    public partial class Items
    {
        public int ID { get; set; }

        public int PozadavekID { get; set; }

        public int CisloRadku { get; set; }

        [StringLength(4000)]
        public string Popis { get; set; }

        [StringLength(2000)]
        public string InterniPoznamka { get; set; }

        [StringLength(50)]
        public string NabidkaCislo { get; set; }

        [StringLength(10)]
        public string KST { get; set; }

        public bool InvesticePlanovana { get; set; }

        public bool InvesticeNeplanovana { get; set; }

        public bool NakupOstatni { get; set; }

        [StringLength(12)]
        public string CisloInvestice { get; set; }

        [StringLength(12)]
        public string CisloKonta { get; set; }

        [StringLength(10)]
        public string Jednotka { get; set; }

        public float Mnozstvi { get; set; }

        public float CenaZaJednotku { get; set; }

        public float CelkovaCena { get; set; }

        public DateTime DatumZalozeni { get; set; }

        public DateTime? DatumObjednani { get; set; }

        public DateTime? TerminDodani { get; set; }

        public bool Smazano { get; set; }

        [StringLength(40)]
        public string SmazalUzivatel { get; set; }

        public DateTime? DatumSmazani { get; set; }

        [StringLength(5)]
        public string Mena { get; set; }

        public int DodavatelID { get; set; }

        public bool Poptany3Firmy { get; set; }

        public int DuvodID { get; set; }

        [StringLength(200)]
        public string Duvod { get; set; }

        public bool Objednano { get; set; }

        public int ObjednavkaID { get; set; }

        [StringLength(40)]
        public string ObjednalUzivatel { get; set; }

        public bool Specha { get; set; }
        public bool Neobjednavat { get; set; }

    }
}
