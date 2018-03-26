namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ObjednavkaTexty")]
    public partial class ObjednavkaTexty
    {
        public int ID { get; set; }

        public int SkupinaID { get; set; }
        
        [StringLength(50)]
        public string Text { get; set; }
    }
}
