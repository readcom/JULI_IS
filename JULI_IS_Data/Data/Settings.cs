namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Settings")]
    public partial class Setting
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string User { get; set; }
        
        public int Skupina { get; set; }

        public string PopisNastaveni { get; set; }
        public string VlastniNastaveni { get; set; }


        public bool OdesilatMaily { get; set; }

        public bool IsTextDoObjednavky { get; set; }

        [StringLength(500)]
        public string TextDoObjednavky { get; set; }
    }
}
