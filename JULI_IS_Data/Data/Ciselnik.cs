namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ciselniky")]
    public partial class Ciselnik
    {
        public int ID { get; set; }

        public int Typ { get; set; }

        [StringLength(10)]
        public string Cislo { get; set; }
        
        [StringLength(50)]
        public string Popis { get; set; }
    }
}
