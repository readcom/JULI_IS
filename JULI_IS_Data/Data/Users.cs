namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Users")]
    public partial class Users
    {
        public int ID { get; set; }

        public int JULINumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Jmeno { get; set; }

        [Required]
        [StringLength(100)]
        public string User { get; set; }

        [Required]
        public int Uroven { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Telefon { get; set; }
        
        public bool OdesilatMaily { get; set; }
        public bool NacitatDodavatele { get; set; }
    }
}
