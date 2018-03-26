namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Investice")]
    public partial class Investice
    {
        public int ID { get; set; }

        [StringLength(9)]
        public string INV_NUM { get; set; }

        [StringLength(3)]
        public string KST { get; set; }

        [StringLength(255)]
        public string DESC { get; set; }
    }
}
