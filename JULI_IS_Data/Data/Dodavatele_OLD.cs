namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Dodavatele_")]
    public partial class Dodavatele_OLD
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nazev { get; set; }

        [StringLength(100)]
        public string Adresa { get; set; }

        [StringLength(10)]
        public string IC { get; set; }

        [StringLength(12)]
        public string DIC { get; set; }

        [StringLength(100)]
        public string Adresa2 { get; set; }

        [StringLength(100)]
        public string Adresa3 { get; set; }

        [StringLength(100)]
        public string AdresaDodatek { get; set; }

        [StringLength(5)]
        public string Mena { get; set; }

        [StringLength(10)]
        public string Zeme { get; set; }

        [StringLength(30)]
        public string Telefon { get; set; }

        [StringLength(10)]
        public string JULINumber { get; set; }
    }
}
