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
    public static class SettingsService
    {
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
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
    }
}
