using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.Services
{
    public static class DBService
    {
      public static void ChangeDatabase(
      this DbContext source,
      string configConnectionStringName = ""
          )
        {
            try
            {
                // use the const name if it's not null, otherwise
                // using the convention of connection string = EF contextname
                // grab the type name and we're done
                var configNameEf = configConnectionStringName;
                    
                // add a reference to System.Configuration
                var entityCnxStringBuilder = new EntityConnectionStringBuilder
                    (ConfigurationManager
                        .ConnectionStrings[configNameEf].ConnectionString);

                // init the sqlbuilder with the full EF connectionstring cargo
                var sqlCnxStringBuilder = new SqlConnectionStringBuilder
                    (entityCnxStringBuilder.ProviderConnectionString);

                // only populate parameters with values if added




                // now flip the properties that were changed
                source.Database.Connection.ConnectionString
                         = sqlCnxStringBuilder.ConnectionString;
            }
            catch (Exception ex)
            {
                
            }
        }
    }


}

