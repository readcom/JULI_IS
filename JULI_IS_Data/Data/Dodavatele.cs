namespace Pozadavky.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Dodavatele")]
    public partial class Dodavatele
    {
        public int Id { get; set; }

        public int PozadavekId { get; set; }
        public int ItemId { get; set; }
        public int OdpOsobaId { get; set; }

        public string SUPN05 { get; set; }
        public string SNAM05 { get; set; }
        public string SAD105 { get; set; }
        public string SAD205 { get; set; }
        public string SAD305 { get; set; }
        public string SAD405 { get; set; }
        public string SAD505 { get; set; }
        public string PSC { get; set; }
        public string PHON05 { get; set; }
        public string SPIN05 { get; set; }
        public string CURN05 { get; set; }
        public string ALPH05 { get; set; }
        public string FAXN05 { get; set; }
        public string LGCD05 { get; set; }
        public decimal COMP05 { get; set; }
        public decimal COMC05 { get; set; }
        public string TOCD05 { get; set; }
        public string TTTP05 { get; set; }
        public decimal TTPR05 { get; set; }
        public decimal TTTD05 { get; set; }
        public string BAN105 { get; set; }
        public string BAN205 { get; set; }
        public string COCD05 { get; set; }
        public string VTID05 { get; set; }
        public string WURL05 { get; set; }
        public string IBAN05 { get; set; }

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
