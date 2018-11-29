namespace Pozadavky.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PozadavkyContext : DbContext
    {
        public PozadavkyContext()
            : base("name=SQLConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;           
        }

        public virtual DbSet<Dodavatele_S21> DodavateleS21 { get; set; }
        public virtual DbSet<Dodavatele> Dodavatele { get; set; }
        public virtual DbSet<OdpOsoby> OdpOsoby { get; set; }        
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<Investice> Investice { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<ObjItems> ObjItems { get; set; }
        public virtual DbSet<Objednavky> Objednavky { get; set; }
        public virtual DbSet<Pozadavky> Pozadavky { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Ciselnik> Ciselnik { get; set; }
        public virtual DbSet<ObjednavkaTexty> ObjTxt { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

    }
}
