namespace Pozadavky.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PozadavkyContext : DbContext
    {
        public PozadavkyContext(string CnxString)
            : base($"name={CnxString}")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

    }
}
