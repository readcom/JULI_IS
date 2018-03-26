namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Files")]
    public partial class Files
    {
        public int ID { get; set; }

        public int? ItemID { get; set; }

        public int? PozadavekID { get; set; }
        public int? ObjID { get; set; }

        [StringLength(200)]
        public string FileName { get; set; }

        [StringLength(300)]
        public string FullPath { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public bool Smazano { get; set; }

        public string SmazalUzivatel { get; set; }

        public DateTime? DatumSmazani { get; set; }
    }
}
