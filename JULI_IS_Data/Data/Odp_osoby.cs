namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Odp_osoby")]
    public partial class OdpOsoby
    {
        public int Id { get; set; }
        public string TPAC1A { get; set; }
        public string CNTN1A { get; set; }
        public decimal? CTNU1A { get; set; }
        public string CNJT1A { get; set; }
        public string CRNM1A { get; set; }
        public string PFNB1A { get; set; }
        public string OFNB1A { get; set; }
        public string MBNB1A { get; set; }
        public string EMIL1A { get; set; }
        public string GTX11A { get; set; }
    }
}
