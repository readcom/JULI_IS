using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.DTO
{
    public class FilesDTO
    {
  
        public int ID { get; set; }

        public int ItemID { get; set; }

        public int PozadavekID { get; set; }

        public string FileName { get; set; }

        public string FullPath { get; set; }

        public string Description { get; set; }

        public bool Smazano { get; set; }

        public string SmazalUzivatel { get; set; }

        public DateTime? DatumSmazani { get; set; }

    }
}
