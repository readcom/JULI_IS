using Pozadavky.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pozadavky.DTO;
using DotVVM.Framework.Controls;

namespace Pozadavky.Services
{
    public static class FilterService
    {
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }




        public static List<string> GetListObjFullId()
        {
            return ObjednavkyService.GetListObjFullId();                                   
        }



        public static List<InvesticeDTO> GetInvesticeList()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                string sql =
                "SELECT *, (INV_NUM + ' | ' + [DESC]) AS CisloANazev from Investice " +
                "WHERE (ISNUMERIC(TRY_CAST(LEFT(INV_NUM, 1) AS INT)) = 1 " +
                "AND TRY_CAST(RIGHT(INV_NUM, 2) as INT) > right(year(getdate()), 2) - 3  " +
                "AND TRY_CAST(RIGHT(INV_NUM, 2) as INT) <= right(year(getdate()), 2) + 1) " +
                "OR [DESC] = 'Neplánovaná Investice' " +
                "ORDER BY INV_NUM";

                return db.Database.SqlQuery<InvesticeDTO>(sql).ToList();

                //return db.Investice
                //    .Select(d => new InvesticeDTO()
                //    {
                //        ID = d.ID,
                //        INV_NUM = d.INV_NUM,
                //        KST = d.KST,
                //        DESC = d.DESC,
                //        CisloANazev = d.INV_NUM + " | " + d.DESC
                //    })
                //    .Where(d => String. d.ToString.  d.INV_NUM.Length )
                //    .OrderBy(a => a.INV_NUM)
                //    .ToList();
            }

        }

        public static string GetSetting(string nastaveni)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var setting = (from s in db.Settings
                               where s.PopisNastaveni == nastaveni
                               select s.VlastniNastaveni);

                return "";

            }
        }


        public static List<string> GetKSTList()
        {            
            using (var db = new PozadavkyContext(DtbConxString))
            {
                List<string> q = (from k in db.Ciselnik
                            where k.Typ == 5 && k.Cislo != null
                            select k.Cislo)
                            .Distinct()
                            .OrderBy(o => o)
                            .ToList();
                return q;
            }            
        }


        /// <summary>
        /// Vrati seznam zakladatelů z pozadavku
        /// </summary>
        /// <returns></returns>
        public static List<string> PozadavkyGetCreatorList()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                List<string> query = (from p in db.Pozadavky
                                      where p.Zalozil != null
                                      select p.Zalozil)
                                      .Distinct()
                                      .OrderBy(c => c)
                                      .ToList();
                return query;
            }
        }


        /// <summary>
        /// Vrati seznam dodavatelů z pozadavku
        /// </summary>
        /// <returns></returns>
        public static List<string> PozadavkyGetDodavateleList()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                List<string> q = (from p in db.Pozadavky
                                  join d in db.Dodavatele on p.DodavatelID equals d.Id
                                  where d.SNAM05 != null && d.SNAM05 != ""
                                  select d.SNAM05)
                                  .Distinct()
                                  .OrderBy(c => c)
                                  .ToList();
                return q;
            }
        }

        /// <summary>
        /// vrati seznam sloupcu tabulky pro polozky nebo pozadavky
        /// </summary>
        /// <param name="sloupce"></param>
        /// <returns></returns>
        public static List<string> GetSeznamSloupcu(string sloupce)
        {
            var list = new List<string>();

            if (sloupce == "I")
            {
                Items items = new Items();
                list = items.GetType().GetProperties().Select(s => "i." + s.Name).ToList();
            }
            else
            {
                Pozadavky.Data.Pozadavky poz = new Pozadavky.Data.Pozadavky();
                list = poz.GetType().GetProperties().Select(s => "p." + s.Name).ToList();
            }

            return list;
        }


        /// <summary>
        /// Vrati seznam pozadavku (FullId)
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPozFullId()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                List<string> q = (from p in db.Pozadavky
                                  where p.FullPozadavekID != null && p.FullPozadavekID != ""
                                  select p.FullPozadavekID)
                                  .Distinct()
                                  .OrderByDescending(c => c)
                                  //.Take(100)
                                  .ToList();
                return q;
            }
        }

    }
}
