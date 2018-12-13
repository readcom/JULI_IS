using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using DotVVM.Framework.Controls;
using Pozadavky.Data;
using Pozadavky.Services;
using Pozadavky.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Data.SqlClient;

namespace Pozadavky.Services
{
    public static class ObjednavkyService
    {
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }

        public static string InfoText { get; set; }  = "";

        public static class Logger
        {
            public static void Log(string s)
            {
                InfoText += s;
            }
        }



        static ObjednavkyService()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Objednavky, ObjednavkaDTO>());
        }

        public static int? LastObjId { get; set; } = 1;       

        public static void DeleteObj(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = db.Objednavky.Find(id);

                if (item.Schvaleno == true) { throw new ApplicationException("Objednávku nelze smazat, už je schválená!"); }
                if (item.Objednano == true)
                {
                    item.Smazano = true;
                    item.SmazalUzivatel = UserServices.GetActiveUser();
                    item.SmazanoDne = DateTime.Now;
                    db.SaveChanges();
                }
                
                if (!item.Objednano && !item.Schvaleno)
                {
                    db.Objednavky.Remove(item);
                    db.SaveChanges();
                }

                    // vymazat odkaz na objednavku z item

                    var query =
                    (from i in db.ObjItems
                     where i.ObjednavkaID == id
                     select i).ToList();

                    query.ForEach(i => i.ObjednavkaID = null);
                    db.SaveChanges();
                                
            }
        }

        public static string GetObjPopisById(int itemId)
        {            
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = db.Objednavky.Find(itemId);
                return item.CelkovyPopis;

            }
        }

        public static void FillGridViewNeobjednaneObjednavky(GridViewDataSet<ObjednavkaDTO> dataSet)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                //db.Objednavky.ProjectTo<ObjednavkyDTO>().ToList();
                var query = (from o in db.Objednavky
                             join i in db.ObjItems on o.ID equals i.ObjednavkaID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == i.DodavatelID).DefaultIfEmpty()
                             where o.Smazano == false && o.Objednano == false
                             select new ObjednavkaDTO()
                             {
                                 ID = o.ID,
                                 FullObjednavkaID = o.FullObjednavkaID,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05.ToString(),
                                 PozadavekZalozil = p.Zalozil,                                 
                                 CelkovyPopis = o.CelkovyPopis,
                                 CelkovaCena = o.CelkovaCena,
                                 Mena = o.Mena,
                                 Datum = o.Datum,
                                 TerminDodani = i.TerminDodani
                             });

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                dataSet.LoadFromQueryable(query);
            }
        }

        public static void DeleteItemFromObj(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var objItem = db.ObjItems.Find(id);

                var origItem = db.Items.Find(objItem.OrigItemID);
                origItem.Objednano = false;
                origItem.ObjednavkaID = 0;
               

                var origPoz = db.Pozadavky.Find(objItem.PozadavekID);
                origPoz.Objednano = false;
                origPoz.PodpisLevel = 2;

                db.SaveChanges();

                string sqlcomm =
                    "DELETE FROM dbo.ObjItems WHERE ID = @id";

                SqlParameter parameter1 = new SqlParameter("@id", id);

                db.Database.ExecuteSqlCommand(sqlcomm, parameter1);
            }
        }

        public static void FillGridViewObjednavkyNaOdeslani(GridViewDataSet<ObjednavkaDTO> dataSet)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                /*
                    select o.ID, o.FullObjednavkaID, (isnull(d.SNAM05, '') + ' | ' + isnull(d.SUPN05, '')) as FullDodavatelName, 
                    o.CelkovyPopis, o.CelkovaCena, o.Mena, o.Datum, min(i.TerminDodani) as TerminDodani, o.Neodesilat
                    from Objednavky o
                    join ObjItems i on o.ID = i.ObjednavkaID
                    LEFT OUTER JOIN Dodavatele d on o.DodavatelID = d.Id
                    WHERE o.Smazano = 0 AND o.Objednano = 1 AND o.Schvaleno = 1 AND o.Dokonceno = 0
                    GROUP BY o.ID, o.FullObjednavkaID, (isnull(d.SNAM05, '') + ' | ' + isnull(d.SUPN05, '')), o.CelkovyPopis, o.CelkovaCena, o.Mena, o.Datum, o.Neodesilat
                    ORDER BY 1
                */


                var obj = db.Database.SqlQuery<ObjednavkaDTO>
                    (
                    "select o.ID, o.FullObjednavkaID, (isnull(d.SNAM05,'') + ' | ' + isnull(d.SUPN05,'')) as FullDodavatelName, " +
                    "o.CelkovyPopis, o.CelkovaCena, o.Mena, o.Datum, min(i.TerminDodani) as TerminDodani, o.Neodesilat " +
                    "from Objednavky o " +
                    "join ObjItems i on o.ID = i.ObjednavkaID " +
                    "LEFT OUTER JOIN Dodavatele d on o.DodavatelID = d.Id " +
                    "WHERE o.Smazano = 0 AND o.Objednano = 1 AND o.Schvaleno = 1 AND o.Dokonceno = 0 " +
                    "GROUP BY o.ID, o.FullObjednavkaID, (isnull(d.SNAM05,'') + ' | ' + isnull(d.SUPN05,'')), o.CelkovyPopis, o.CelkovaCena, o.Mena, o.Datum, o.Neodesilat " +
                    "ORDER BY 1"
                    ).AsQueryable();

                /* 
                 var query = (from o in obj
                              select new ObjednavkaDTO()
                              {
                                  ID = o.ID,
                                  FullObjednavkaID = o.FullObjednavkaID,
                                  FullDodavatelName = o.FullDodavatelName,
                                  CelkovyPopis = o.CelkovyPopis,
                                  CelkovaCena = o.CelkovaCena,
                                  Mena = o.Mena,
                                  Datum = o.Datum,
                                  TerminDodani = o.TerminDodani
                              }).AsQueryable();
                 */

                dataSet.LoadFromQueryable(obj);

                //db.Objednavky.ProjectTo<ObjednavkyDTO>().ToList();
                //var query = (from o in db.Objednavky
                //             join i in db.ObjItems on o.ID equals i.ObjednavkaID
                //             from d in db.Dodavatele.Where(dod => dod.Id == o.DodavatelID).DefaultIfEmpty()                             
                //             where o.Smazano == false && o.Objednano == true && o.Schvaleno == true && o.Odeslano == false
                //             group new { o, i, d } by new
                //             {
                //                 ID = o.ID,
                //                 o.FullObjednavkaID,
                //                 d.SNAM05,
                //                 d.SUPN05,
                //                 o.CelkovyPopis,
                //                 o.CelkovaCena,
                //                 o.Mena,
                //                 o.Datum,
                //                 i.TerminDodani
                //             } into g
                //             select new ObjednavkaDTO()
                //             {
                //                 ID = g.Key.ID,
                //                 FullObjednavkaID = g.Key.FullObjednavkaID,
                //                 FullDodavatelName = g.Key.SNAM05 + " | " + g.Key.SUPN05.ToString(),
                //                 CelkovyPopis = g.Key.CelkovyPopis,
                //                 CelkovaCena = g.Key.CelkovaCena,
                //                 Mena = g.Key.Mena,
                //                 Datum = g.Key.Datum,                                 
                //                 TerminDodani =  g.Min(t => t.i.TerminDodani)
                //             });

                //// lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                //dataSet.LoadFromQueryable(query);
            }
        }

        public static void GridViewSetSortByID(GridViewDataSet<ObjednavkaDTO> data)
        {
            data.PagingOptions.PageSize = 15;
            data.SortingOptions.SortExpression = nameof(ObjednavkaDTO.ID);
            data.SortingOptions.SortDescending = true;
        }

        public static void FillGridViewObjednavkyNaPodpis(GridViewDataSet<ObjednavkaDTO> dataSet)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from o in db.Objednavky
                             join i in db.ObjItems on o.ID equals i.ObjednavkaID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where o.Smazano == false && o.Objednano == true && o.Schvaleno == false && o.Zamitnuto == false
                             group new { o, i, p, d } by new
                             {
                                 o.ID,
                                 o.FullObjednavkaID,
                                 o.CelkovyPopis,
                                 o.CelkovaCena,
                                 o.Datum,
                                 o.Objednano,
                                 o.DatumObjednani,
                                 o.Mena,
                                 o.DodavatelID

                             } into g
                             orderby
                               g.Key.ID descending
                             select new ObjednavkaDTO()
                             {
                                 ID = g.Key.ID,
                                 FullObjednavkaID = g.Key.FullObjednavkaID,
                                 FullDodavatelName = (g.Min(p => p.d.SNAM05) + " | " + g.Min(p => p.d.SUPN05)),
                                 CelkovyPopis = g.Key.CelkovyPopis,
                                 CelkovaCena = g.Key.CelkovaCena,
                                 Datum = g.Key.Datum,
                                 Objednano = g.Key.Objednano,
                                 DatumObjednani = g.Key.DatumObjednani,
                                 Mena =  g.Key.Mena, // g.Min(p => p.i.Mena),
                                 TerminDodani = (DateTime?)g.Min(p => p.i.TerminDodani),
                                 DodavatelID = g.Key.DodavatelID //g.Min(p => p.p.DodavatelID)
                             });


                dataSet.LoadFromQueryable(query);
            }

        }     


        public static void CelkovaCenaPrepocitat(int ObjId)
        {

            using (var db = new PozadavkyContext(DtbConxString))
            {

                float suma = 0;
                try
                {
                    suma = (from i in db.ObjItems
                                where i.ObjednavkaID == ObjId
                                select i.CelkovaCena).Sum();
                }
                catch (Exception e)
                {
                    suma = 0;
                }
                    
                var obj = db.Objednavky.Find(ObjId);
                obj.CelkovaCena = suma;
                db.SaveChanges();                                   
            }
        }

        public static ObjednavkaDTO GetObjById(int id)  // automapper
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                Mapper.Initialize(cfg => cfg.CreateMap<Objednavky, ObjednavkaDTO>());
                
                var o = db.Objednavky.Find(id);
                ObjednavkaDTO obj = Mapper.Map<ObjednavkaDTO>(o); 

                return obj;                    
            }
        }

        public static void FillGridViewObjednavky(GridViewDataSet<ObjednavkaDTO> dataSet)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                //db.Database.Log = str => Logger.Log(str);

                bool NeverejnyPristup = false;
                IQueryable<ObjednavkaDTO> query;
                var User = UserServices.GetActiveUser();

                if (!string.IsNullOrEmpty(User))
                {
                    List<UsersDTO> ActivUsers = UserServices.GetUsersByUserName(User);
                    foreach (var user in ActivUsers)
                    {
                        if (user.NeverejnyPristup == true) NeverejnyPristup = true;
                    }
                }

                if (NeverejnyPristup)
                {
                    //db.Objednavky.ProjectTo<ObjednavkyDTO>().ToList();
                    query = (from o in db.Objednavky
                             join i in db.ObjItems on o.ID equals i.ObjednavkaID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             //join i in db.ObjItems on new { ID = o.ID } equals new { ID = i.ObjednavkaID }
                             //join p in db.Pozadavky on new { ID = i.PozadavekID } equals new { ID = p.ID }
                             //join d in db.Dodavatele on new { ID = Convert.ToInt32(p.DodavatelID) } equals new { ID = d.ID } into d_join
                             //from d in d_join.DefaultIfEmpty()
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where o.Smazano == false
                             group new { o, i, p, d } by new
                             {
                                 o.ID,
                                 o.CelkovyPopis,
                                 o.CelkovaCena,
                                 o.Datum,
                                 o.Objednano,
                                 o.DatumObjednani,
                                 o.Mena,
                                 o.DodavatelID,
                                 o.FullObjednavkaID,
                                 o.PocetPolozek,
                                 o.AvizoDoruceni,
                                 o.DatumDodani,
                                 p.Zalozil
                             } into g
                             orderby
                               g.Key.ID descending
                             select new ObjednavkaDTO()
                             {
                                 ID = g.Key.ID,
                                 FullObjednavkaID = string.IsNullOrEmpty(g.Key.FullObjednavkaID) ? g.Key.ID.ToString() : g.Key.FullObjednavkaID,
                                 FullDodavatelName = (g.Min(p => p.d.SNAM05) + " | " + g.Min(p => p.d.SUPN05)),
                                 CelkovyPopis = g.Key.CelkovyPopis,
                                 CelkovaCena = g.Key.CelkovaCena,
                                 Datum = g.Key.Datum,
                                 Objednano = g.Key.Objednano,
                                 DatumObjednani = g.Key.DatumObjednani,
                                 Mena = g.Key.Mena,  //g.Min(p => p.i.Mena),
                                 TerminDodani = (DateTime?)g.Min(p => p.i.TerminDodani),
                                 DodavatelID = g.Key.DodavatelID, //(int?)g.Min(p => p.p.DodavatelID)
                                 PocetPolozek = g.Key.PocetPolozek,
                                 AvizoDoruceni = g.Key.AvizoDoruceni,
                                 DatumDodani = g.Key.DatumDodani,
                                 PozadavekZalozil = g.Key.Zalozil

                             });

                    //select new ObjednavkyDTO()
                    //{
                    //    ID = o.ID,
                    //    FullObjednavkaID = o.Datum.Value.Year.ToString() + o.ID.ToString(),
                    //    FullDodavatelName = d.Nazev + " | " + d.JULINumber.ToString(),
                    //    PozadavekZalozil = p.Zalozil,
                    //    CelkovyPopis = o.CelkovyPopis,
                    //    CelkovaCena = o.CelkovaCena,
                    //    Mena = i.Mena,
                    //    Datum = o.Datum,
                    //    TerminDodani = i.TerminDodani
                    //});
                }
                else
                {
                    query = (from o in db.Objednavky
                             join i in db.ObjItems on o.ID equals i.ObjednavkaID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             //join i in db.ObjItems on new { ID = o.ID } equals new { ID = i.ObjednavkaID }
                             //join p in db.Pozadavky on new { ID = i.PozadavekID } equals new { ID = p.ID }
                             //join d in db.Dodavatele on new { ID = Convert.ToInt32(p.DodavatelID) } equals new { ID = d.ID } into d_join
                             //from d in d_join.DefaultIfEmpty()
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where o.Smazano == false && p.Neverejny == false
                             group new { o, i, p, d } by new
                             {
                                 o.ID,
                                 o.CelkovyPopis,
                                 o.CelkovaCena,
                                 o.Datum,
                                 o.Objednano,
                                 o.DatumObjednani,
                                 o.Mena,
                                 o.DodavatelID,
                                 o.FullObjednavkaID,
                                 o.PocetPolozek,
                                 o.AvizoDoruceni,
                                 o.DatumDodani,
                                 p.Zalozil
                             } into g
                             orderby
                               g.Key.ID descending
                             select new ObjednavkaDTO()
                             {
                                 ID = g.Key.ID,
                                 FullObjednavkaID = string.IsNullOrEmpty(g.Key.FullObjednavkaID) ? g.Key.ID.ToString() : g.Key.FullObjednavkaID,
                                 FullDodavatelName = (g.Min(p => p.d.SNAM05) + " | " + g.Min(p => p.d.SUPN05)),
                                 CelkovyPopis = g.Key.CelkovyPopis,
                                 CelkovaCena = g.Key.CelkovaCena,
                                 Datum = g.Key.Datum,
                                 Objednano = g.Key.Objednano,
                                 DatumObjednani = g.Key.DatumObjednani,
                                 Mena = g.Key.Mena,  //g.Min(p => p.i.Mena),
                                 TerminDodani = (DateTime?)g.Min(p => p.i.TerminDodani),
                                 DodavatelID = g.Key.DodavatelID, //(int?)g.Min(p => p.p.DodavatelID)
                                 PocetPolozek = g.Key.PocetPolozek,
                                 AvizoDoruceni = g.Key.AvizoDoruceni,
                                 DatumDodani = g.Key.DatumDodani,
                                 PozadavekZalozil = g.Key.Zalozil

                             });
                }

                dataSet.LoadFromQueryable(query);
            }
        }


        /// <summary>
        /// Vrátí poslední číslo objednávky v daném roce, jinak vrací 0
        /// </summary>
        /// <param name="rok"></param>
        /// <returns></returns>
        public static int GetLastNumberOfYear(string rok)
        {

            

            string sqlquery = "select top 1 * from Objednavky " +
                              "where substring(FullObjednavkaID, 1, 4) = {0} " +
                              "order by substring(FullObjednavkaID, 6, 4) desc";

            using (var db = new PozadavkyContext(DtbConxString))
            {
                //var parameters = new SqlParameter("@rok", rok);
                // SqlParameter rokParam = new SqlParameter("rok", rok);
                // object[] parameters = new object[] { rok };

                

        

                var query = db.Database.SqlQuery<Objednavky>(sqlquery, rok);

                var item = query.FirstOrDefault();
                if (item != null)                
                    return Int32.Parse(item.FullObjednavkaID.Substring(5, 4));
                else return 0;
            }            
        }

        public static void FillGridViewObjednavkyFiltered(GridViewDataSet<ObjednavkaDTO> datagv, string column = "", string filter = "", DateTime? datumod = null, DateTime? datumdo = null)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                bool NeverejnyPristup = false;
                IQueryable<ObjednavkaDTO> query;

                var User = UserServices.GetActiveUser();

                if (!string.IsNullOrEmpty(User))
                {
                    List<UsersDTO> ActivUsers = UserServices.GetUsersByUserName(User);
                    foreach (var user in ActivUsers)
                    {
                        if (user.NeverejnyPristup == true) NeverejnyPristup = true;
                    }
                }

                if (NeverejnyPristup)
                {

                    query = (from o in db.Objednavky
                             join i in db.ObjItems on o.ID equals i.ObjednavkaID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             //join i in db.ObjItems on new { ID = o.ID } equals new { ID = i.ObjednavkaID }
                             //join p in db.Pozadavky on new { ID = i.PozadavekID } equals new { ID = p.ID }
                             //join d in db.Dodavatele on new { ID = Convert.ToInt32(p.DodavatelID) } equals new { ID = d.ID } into d_join
                             //from d in d_join.DefaultIfEmpty()
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where o.Smazano == false
                             group new { o, i, p, d } by new
                             {
                                 o.ID,
                                 o.CelkovyPopis,
                                 o.CelkovaCena,
                                 o.Datum,
                                 o.Objednano,
                                 o.DatumObjednani,
                                 o.Mena,
                                 o.DodavatelID,
                                 o.FullObjednavkaID,
                                 o.PocetPolozek,
                                 o.AvizoDoruceni,
                                 o.DatumDodani,
                                 p.Zalozil
                             } into g
                             orderby
                               g.Key.ID descending
                             select new ObjednavkaDTO()
                             {
                                 ID = g.Key.ID,
                                 FullObjednavkaID = string.IsNullOrEmpty(g.Key.FullObjednavkaID) ? g.Key.ID.ToString() : g.Key.FullObjednavkaID,
                                 FullDodavatelName = (g.Min(p => p.d.SNAM05) + " | " + g.Min(p => p.d.SUPN05)),
                                 CelkovyPopis = g.Key.CelkovyPopis,
                                 CelkovaCena = g.Key.CelkovaCena,
                                 Datum = g.Key.Datum,
                                 Objednano = g.Key.Objednano,
                                 DatumObjednani = g.Key.DatumObjednani,
                                 Mena = g.Key.Mena,  //g.Min(p => p.i.Mena),
                                 TerminDodani = (DateTime?)g.Min(p => p.i.TerminDodani),
                                 DodavatelID = g.Key.DodavatelID, //(int?)g.Min(p => p.p.DodavatelID)
                                 PocetPolozek = g.Key.PocetPolozek,
                                 AvizoDoruceni = g.Key.AvizoDoruceni,
                                 DatumDodani = g.Key.DatumDodani,
                                 PozadavekZalozil = g.Key.Zalozil

                             });
                }
                else
                {
                    query = (from o in db.Objednavky
                             join i in db.ObjItems on o.ID equals i.ObjednavkaID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             //join i in db.ObjItems on new { ID = o.ID } equals new { ID = i.ObjednavkaID }
                             //join p in db.Pozadavky on new { ID = i.PozadavekID } equals new { ID = p.ID }
                             //join d in db.Dodavatele on new { ID = Convert.ToInt32(p.DodavatelID) } equals new { ID = d.ID } into d_join
                             //from d in d_join.DefaultIfEmpty()
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where o.Smazano == false && p.Neverejny == false
                             group new { o, i, p, d } by new
                             {
                                 o.ID,
                                 o.CelkovyPopis,
                                 o.CelkovaCena,
                                 o.Datum,
                                 o.Objednano,
                                 o.DatumObjednani,
                                 o.Mena,
                                 o.DodavatelID,
                                 o.FullObjednavkaID,
                                 o.PocetPolozek,
                                 o.AvizoDoruceni,
                                 o.DatumDodani,
                                 p.Zalozil
                             } into g
                             orderby
                               g.Key.ID descending
                             select new ObjednavkaDTO()
                             {
                                 ID = g.Key.ID,
                                 FullObjednavkaID = string.IsNullOrEmpty(g.Key.FullObjednavkaID) ? g.Key.ID.ToString() : g.Key.FullObjednavkaID,
                                 FullDodavatelName = (g.Min(p => p.d.SNAM05) + " | " + g.Min(p => p.d.SUPN05)),
                                 CelkovyPopis = g.Key.CelkovyPopis,
                                 CelkovaCena = g.Key.CelkovaCena,
                                 Datum = g.Key.Datum,
                                 Objednano = g.Key.Objednano,
                                 DatumObjednani = g.Key.DatumObjednani,
                                 Mena = g.Key.Mena,  //g.Min(p => p.i.Mena),
                                 TerminDodani = (DateTime?)g.Min(p => p.i.TerminDodani),
                                 DodavatelID = g.Key.DodavatelID, //(int?)g.Min(p => p.p.DodavatelID)
                                 PocetPolozek = g.Key.PocetPolozek,
                                 AvizoDoruceni = g.Key.AvizoDoruceni,
                                 DatumDodani = g.Key.DatumDodani,
                                 PozadavekZalozil = g.Key.Zalozil

                             });
                }


                var list = query;

                switch (column)
                {
                    case "ObjednavkaFullID":
                        list = query.Where(w => w.FullObjednavkaID == filter);
                        break;
                    case "FullDodavatelName":
                        list = query.Where(w => w.FullDodavatelName == filter);
                        break;
                    //case "Stav":
                    //    list = query.Where(w => w.Stav == filter);
                    //    break;
                    case "Zalozil":
                        list = query.Where(w => w.PozadavekZalozil == filter);
                        break;
                    case "Polozka":
                        list = query.Where(w => w.CelkovyPopis.Contains(filter));
                        break;
                    //case "HlavniRada":
                    //    list = query.Where(w => w.HlavniRada.Contains(filter));
                    //    break;
                    default:
                        list = query;
                        break;
                }


                datagv.LoadFromQueryable(list);
            }
        }

        public static float GetCelkovaCenaByObjId(int id)
        {

            //select sum(CelkovaCena) from Items where ObjednavkaID = id

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var items = (from i in db.ObjItems
                             where i.ObjednavkaID == id
                             select i.CelkovaCena).DefaultIfEmpty(0).Sum();

                return items;
            }
        }

        public static List<ObjednavkaDTO> GetListObjednavek()  // automapper
        {

            using (var db = new PozadavkyContext(DtbConxString))
            {

                var query = (from o in db.Objednavky
                             where o.Smazano == false && o.Zamitnuto == false && o.Objednano == false
                             select new ObjednavkaDTO()                             
                             {
                                 ID = o.ID,
                                 FullObjednavkaID = o.ID.ToString(),
                                 ObjednavatelID = o.ObjednavatelID,
                                 //Zalozil = o.Zalozil,
                                 Datum = o.Datum,
                                 CelkovyPopis = o.CelkovyPopis,
                                 CelkovaCena = o.CelkovaCena,
                                 Mena = o.Mena,
                                 Smazano = o.Smazano,
                                 SmazalUzivatel = o.SmazalUzivatel,
                                 SmazanoDne = o.SmazanoDne
                             }
                    );
                return query.ToList();
            }



            // verze AutoMapper

            //using (var context = new PozadavkyContext(DtbConxString))
            //{
            //    return context.Objednavky.ProjectTo<ObjednavkyDTO>().ToList();
            //}
        }

        /// <summary>
        /// Vytvoří novou objednávku ze zadaného seznamu
        /// </summary>
        /// <param name="seznamItems">seznam Items</param>
        /// <returns></returns>
        public static int VytvoritObj(List<ObjItemsDTO> seznamItems)
        {
            int ObjednavkaNewId;

            using (var db = new PozadavkyContext(DtbConxString))
            {

                var pozadavek = PozadavkyService.GetPozadavekById(seznamItems.First().PozadavekID);
                var activeuser = UserServices.GetUsersByUserName(UserServices.GetActiveUser()).Where(lvl => lvl.Uroven == 3).FirstOrDefault();

                int DodNewId = DodavatelService.DodavatelCopy(seznamItems.First().DodavatelID);
                // zaloz novou obj.
                var o = new Objednavky
                {
                    Datum = DateTime.Now,
                    ObjednavatelID = activeuser.ID,
                    ObjednavatelName = activeuser.Jmeno,
                    DodavatelID = DodNewId,
                    KST1 = Int32.Parse(seznamItems.First().Stredisko ?? "0"),
                    NabidkaCislo = seznamItems.First().NabidkaCislo ?? "",
                    Mena = seznamItems.First().Mena ?? "",
                    FullObjednavkaID = "",
                    TextPlatebniPodmId = pozadavek.ZpusobPlatbyId ?? 0,
                    TextPlatebniPodmText = pozadavek.ZpusobPlatbyText ?? "",
                    TypInvestice = pozadavek.InvesticeNeplanovana ? 1 : pozadavek.InvesticePlanovana ? 2 : 3
                };
                db.Objednavky.Add(o);
                db.SaveChanges();

                ObjednavkaNewId = o.ID;

                // kazde item prirad cislo objednavky
                foreach (var itemId in seznamItems)
                {
                    var item = db.ObjItems.Find(itemId.ID);
                    item.ObjednavkaID = ObjednavkaNewId;
                    item.Objednano = false;
                    db.SaveChanges();
                }

                // upozorni orig. polozky ze se s nema neco deje
                foreach (var objitem in seznamItems)
                {

                    var pozadavekid = objitem.PozadavekID;
                    var SeznamItems = ItemsService.GetItemsByPozadavekId(pozadavekid);
                    foreach (var item in SeznamItems)
                    {
                        if (string.IsNullOrEmpty(item.InterniPoznamka)) item.InterniPoznamka = "";
                        if
                        (
                            item.Popis == objitem.Popis
                            && item.InterniPoznamka == objitem.InterniPoznamka
                            && item.Jednotka == objitem.Jednotka && item.CenaZaJednotku == objitem.CenaZaJednotku
                           
                        )
                        {
                            var oItem = db.Items.Find(item.ID);
                            oItem.ObjednavkaID = ObjednavkaNewId;
                            oItem.Objednano = false;
                            db.SaveChanges();
                        }
                    }
                }

                // pridat poznamky z pozadavku
                List<int> SeznamPozadavku = new List<int>();
                string poznamka = "";
                seznamItems.ForEach(i => SeznamPozadavku.Add(i.PozadavekID));
                SeznamPozadavku = SeznamPozadavku.Distinct().ToList();
                SeznamPozadavku.ForEach(p => poznamka += (PozadavkyService.GetPozadavekById(p).Poznamka + "; "));

                // uloz celkovou cenu polozek v objednavce a poznamky
                var obj = db.Objednavky.Find(ObjednavkaNewId);
                obj.CelkovaCena = GetCelkovaCenaByObjId(ObjednavkaNewId);
                obj.Poznamka = poznamka;
                db.SaveChanges();

                //nakopirovat soubory k pozadavku do objednavky
                List<int> pozadavkyId = new List<int>();
                foreach (var itemId in seznamItems)
                {
                    if (!pozadavkyId.Contains(itemId.PozadavekID))
                    pozadavkyId.Add(itemId.PozadavekID);                                
                }
                FilesService.CopyFilesFromPozadavek2Obj(pozadavkyId, ObjednavkaNewId);
                pozadavkyId.Clear();


                
            }

            return ObjednavkaNewId;
        }





        /// <summary>
        /// Přidá položky do existující objednávky
        /// </summary>
        /// <param name="id">Id objednavky</param>
        /// <param name="seznamItems">seznam položek</param>
        public static void AddToObj(int id, List<ObjItemsDTO> seznamItems)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                foreach (var itemId in seznamItems)
                {
                    var item = db.ObjItems.Find(itemId.ID);
                    item.ObjednavkaID = id;
                    //item.Objednano = true;
                    //item.ObjednalUzivatel = UserServices.GetActiveUser();
                    //item.DatumObjednani = DateTime.Now;

                    db.SaveChanges();
                }

                // uloz celkovou cenu polozek v objednavce
                var obj = db.Objednavky.Find(id);
                obj.CelkovaCena = GetCelkovaCenaByObjId(id);
                db.SaveChanges();

                // upozorni orig. polozky ze se s nema neco deje
                foreach (var objitem in seznamItems)
                {

                    var pozadavekid = objitem.PozadavekID;
                    var SeznamItems = ItemsService.GetItemsByPozadavekId(pozadavekid);
                    foreach (var item in SeznamItems)
                    {
                        if (string.IsNullOrEmpty(item.InterniPoznamka)) item.InterniPoznamka = "";
                        if
                        (
                            item.Popis == objitem.Popis
                            && item.InterniPoznamka == objitem.InterniPoznamka
                            && item.Jednotka == objitem.Jednotka && item.CenaZaJednotku == objitem.CenaZaJednotku

                        )
                        {
                            var oItem = db.Items.Find(item.ID);
                            oItem.ObjednavkaID = id;
                            oItem.Objednano = false;
                            db.SaveChanges();
                        }
                    }
                }

                //nakopirovat soubory k pozadavku do objednavky
                List<int> pozadavkyId = new List<int>();
                foreach (var itemId in seznamItems)
                {
                    if (!pozadavkyId.Contains(itemId.PozadavekID))
                        pozadavkyId.Add(itemId.PozadavekID);
                }
                FilesService.CopyFilesFromPozadavek2Obj(pozadavkyId, id);
                pozadavkyId.Clear();
            }
        }

        public static void EditObjPopis(string NewPopis, int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = db.Objednavky.Find(id);
                item.CelkovyPopis = NewPopis;
                db.SaveChanges();
            }
        }

        public static void InsertObj(ObjednavkaDTO item)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var i = new Objednavky
                {
                    Datum = DateTime.Now,
                    CelkovyPopis = item.CelkovyPopis
                };
                db.Objednavky.Add(i);
                db.SaveChanges(); 
            }
        }

        public static void SaveObj(ObjednavkaDTO obj)  
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var o = db.Objednavky.Find((obj.ID));
                // o = Mapper.Map<Objednavky>(obj);  // nejde, spatna automapa

                o.FullObjednavkaID = obj.FullObjednavkaID;
                o.ObjednavatelID = obj.ObjednavatelID;
                o.DodavatelID = obj.DodavatelID;
                o.DodavatelS21ID = obj.DodavatelS21ID;
                o.CelkovyPopis = obj.CelkovyPopis;
                o.Datum = obj.Datum;
                o.CelkovaCena = obj.CelkovaCena;
                o.Mena = obj.Mena;
                o.KST1 = obj.KST1;
                o.KST2 = obj.KST2;
                o.KST3 = obj.KST3;
                o.ObjednavatelName = obj.ObjednavatelName;
                o.Objednano = obj.Objednano;
                o.DatumObjednani = obj.DatumObjednani;
                o.HlavniRada = obj.HlavniRada;
                o.TextCenaId = obj.TextCenaId;
                o.TextDodaciPodmId = obj.TextDodaciPodmId;
                o.TextPlatebniPodmId = obj.TextPlatebniPodmId;
                o.TextDodaciPodmText = obj.TextDodaciPodmText;
                o.TextPlatebniPodmText = obj.TextPlatebniPodmText;
                o.TextCenaText = obj.TextCenaText;
                o.Schvaleno = obj.Schvaleno;
                o.SchvalenoDne = obj.SchvalenoDne;
                o.Odeslano = obj.Odeslano;
                o.OdeslanoDne = obj.OdeslanoDne;
                o.Dokonceno = obj.Dokonceno;
                o.DokoncenoDne = obj.DokoncenoDne;
                o.Vytisknuto = obj.Vytisknuto;
                o.VytisknutoDne = obj.VytisknutoDne;
                o.Neodesilat = obj.Neodesilat;
                o.PocetPolozek = obj.PocetPolozek;
                o.AvizoDoruceni = obj.AvizoDoruceni;
                o.DatumDodani = obj.DatumDodani;
                db.SaveChanges();
            }

        }

        public static List<PozadavekDTO> GetListPozadavkyByObj(int objId)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from p in db.Pozadavky                                 
                             join i in db.ObjItems on p.ID equals i.PozadavekID
                             where i.ObjednavkaID == objId
                             select new PozadavekDTO
                             {
                                ID = p.ID,
                                Zalozil = p.Zalozil,
                                Level1SchvalovatelID = p.Level1SchvalovatelID ?? 0,
                                Level2SchvalovatelID = p.Level2SchvalovatelID ?? 0,
                                Level3SchvalovatelID = p.Level3SchvalovatelID ?? 0,
                                FullPozadavekID = p.FullPozadavekID
                             });

                return query.Distinct().ToList();
            }
        }

        public static void FillGridViewItemsByObjID(GridViewDataSet<ObjItemsDTO> dataSet, int objId)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                //db.Objednavky.ProjectTo<ObjednavkyDTO>().ToList();

                var query = (from i in db.ObjItems
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             join o in db.Objednavky on i.ObjednavkaID equals o.ID
                             where i.ObjednavkaID == objId
                             orderby i.ID
                             select new ObjItemsDTO
                             {
                                 ID = (int?)i.ID ?? 0,
                                 Popis = i.Popis,
                                 PozadavekID = i.PozadavekID,
                                 DodavatelID = i.DodavatelID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka,
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani,
                                 ObjednavkaID = i.ObjednavkaID,
                                 ObjednavkaFullID = o.FullObjednavkaID,
                                 Mena = i.Mena,
                                 NabidkaCislo = i.NabidkaCislo,
                                 KST = i.KST
                             });

                //query = query.OrderBy(o => o.ID);
                dataSet.SortingOptions.SortDescending = false;
                dataSet.LoadFromQueryable(query);
            }
        }

        public static string ObjednavkaOdeslatNaSchvaleni(ObjednavkaDTO data) //       int id, int data.PodpisLevel, int level1Id = 0)
        {
            // pridat priznak PodpisLevel = data.PodpisLevel
            // pridat priznak Level1Schvaleno, Level1SchvalenoDne,Level2Odeslano, Level2SchvalovatelID

            // odeslat mail zakladateli pozadavku o schvaleni
            // odeslat mail schvalovateli data.PodpisLevel se zadosti o schvaleni, ve druhem levelu to jde na oba, controling i reditel

            string vysledek = "";

            //UsersDTO schvalovatel = UserServices.GetUserByLevel(4).FirstOrDefault();

            string SchvalovatelEmail = "petr.smolka@juli.cz";

            List<PozadavekDTO> pozadavky = GetListPozadavkyByObj(data.ID);


            using (var db = new PozadavkyContext(DtbConxString))
            {
                var obj = db.Objednavky.Find(data.ID);
                //obj.SchvalovatelID = schvalovatel.ID;
                obj.OdeslanoNaSchvaleniDne = DateTime.Now;
                obj.Schvaleno = false;
                db.SaveChanges();



                // odeslat info zakladateli pozadavku

                foreach (var item in pozadavky)
                {
                    if (Constants.Test == true) item.Zalozil = "marek.novak";
                    if (!string.IsNullOrEmpty(item.Zalozil))
                    {
                        vysledek = MailServices.SendMail(item.Zalozil + "@juli.cz" + (Constants.TestEmaily ? ";marek.novak@juli.cz" : ""),

                            $"Z požadavku č. {item.FullPozadavekID} byla vytvořena objednávka",

                            $"Z vašeho požadavku č. {item.FullPozadavekID} byla vytvořena objednávka a byla odeslána ke schválení dne " +
                            $"{obj.OdeslanoNaSchvaleniDne:d.M.yyyy}" + " v " +
                            $"{obj.OdeslanoNaSchvaleniDne:HH:mm}" + " hodin" +
                            $"<br> uživatelem: {UserServices.GetNameByUserName(UserServices.GetActiveUser())}"
                            );
                    }

                    PozadavekDTO pozadavek = PozadavkyService.GetPozadavekById(item.ID);
                    pozadavek.Stav = "Schvalovaní objednávky";
                    PozadavkyService.PozadavekSave(pozadavek);                                          
                }

                // info schvalovateli objednavky
                vysledek += MailServices.SendMail(SchvalovatelEmail + (Constants.TestEmaily ? ";marek.novak@juli.cz" : ""),
                "Objednávka připravena ke schválení",
                $"Objednávka č. {obj.FullObjednavkaID} vytvořena uživatelem: {obj.ObjednavatelName} je připravena ke schválení."
                );
       

            }

            return vysledek;
        }

        public static string ObjednavkaZamitnout(ObjednavkaDTO data, string duvod = "")
        {
            // pridat vyrozumeni i zadavateli pozadavku
            
            string vysledek = "";

            UsersDTO schvalovatel = UserServices.GetUsersByUserName(UserServices.GetActiveUser()).First();
            //UserServices.GetUserByLevel(4).FirstOrDefault();


            if (Constants.Test == true) schvalovatel = UserServices.GetUsersByUserName("marek.novak").First();

            string SchvalovatelEmail = schvalovatel.Email;

      

            List<PozadavekDTO> pozadavky = GetListPozadavkyByObj(data.ID);

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var obj = db.Objednavky.Find(data.ID);
                
                //obj.FullObjednavkaID = obj.FullObjednavkaID;

                obj.SchvalovatelID = schvalovatel.ID;
                obj.Schvaleno = false;
                obj.Zamitnuto = true;
                obj.ZamitnutoDne = DateTime.Now;
                obj.DuvodZamitnutiID = data.DuvodZamitnutiID;
                obj.DuvodZamitnutiText = duvod;

                db.SaveChanges();

                string ObjednalEmail = "objednavky@juli.cz"; //UserServices.GetUserById(obj.ObjednavatelID).Email;
                if (Constants.Test == true) ObjednalEmail = "marek.novak@juli.cz";

                string mailText =
                    $"Vaše objednávka č. {obj.FullObjednavkaID} " +
                    $"byla zamítnuta uživatelem {schvalovatel.Jmeno}. <br />" +
                    $"Dne {obj.ZamitnutoDne:d.M.yyyy} " +
                    $"v {obj.ZamitnutoDne:HH:mm}" + " hodin";

                mailText += duvod != "" ? $"<br><br> Důvod: {duvod}" : "neuvedeno"
                    //+ $"<br> email pro uživatele: {pozadavek.Zalozil}@juli.cz; " +
                    //$"{UserServices.GetUserById(pozadavek.Level1SchvalovatelID.Value).Email}" +
                    //$"{UserServices.GetUserById(pozadavek.Level2SchvalovatelID.Value).Email}"
                    //+ $"<br>  mail pro uživatele: {ObjednalEmail}"
                    ;

                //email pro objednavatele
                vysledek = MailServices.SendMail(
                    ObjednalEmail,
                    $"Objednávka č. {obj.FullObjednavkaID} byla zamítnuta!",
                    mailText
                );


                //email pro vsechny zadavatele pozadavku (protoze obj. se muze skladat z vic pozadavku od vic lidi)

                foreach (var item in pozadavky)
                {

                    string email = item.Zalozil + "@juli.cz";
                    if (Constants.Test == true) email = "marek.novak@juli.cz";

                    mailText =
                        $"Objednávka č. {obj.FullObjednavkaID} vytvořená z vašeho požadavku č. {item.FullPozadavekID} " +
                        $"byla zamítnuta uživatelem {schvalovatel.Jmeno}. <br />" +
                        $"Dne {obj.ZamitnutoDne:d.M.yyyy} " +
                        $"v {obj.ZamitnutoDne:HH:mm}" + " hodin";

                    mailText += duvod != "" ? $"<br><br> Důvod: {duvod}" : "neuvedeno"
                                // + $"<br>  mail pro uživatele: {item.Zalozil}@juli.cz"
                                ;

                    vysledek = MailServices.SendMail(
                        email,
                        $"Objednávka č. {obj.FullObjednavkaID} byla zamítnuta!",
                        mailText
                    );
                }

                
            }

            return vysledek;
        }

        public static string ObjednavkaSchvalit(ObjednavkaDTO data) //       int id, int data.PodpisLevel, int level1Id = 0)
        {
            // pridat vyrozumeni i zadavateli pozadavku

            string vysledek = "";

            UsersDTO schvalovatel = UserServices.GetUsersByUserName(UserServices.GetActiveUser()).First();
            //UserServices.GetUserByLevel(4).FirstOrDefault();
            if (Constants.Test == true) schvalovatel = UserServices.GetUsersByUserName("marek.novak").First();

            string SchvalovatelEmail = schvalovatel.Email;
            List<PozadavekDTO> pozadavky = GetListPozadavkyByObj(data.ID);

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var obj = db.Objednavky.Find(data.ID);

                //obj.FullObjednavkaID = obj.FullObjednavkaID;

                obj.SchvalovatelID = schvalovatel.ID;
                obj.Schvaleno = true;
                obj.SchvalenoDne = DateTime.Now;
                db.SaveChanges();

                string ObjednalEmail = "objednavky@juli.cz"; //UserServices.GetUserById(obj.ObjednavatelID).Email;
                if (Constants.Test == true) ObjednalEmail = "marek.novak@juli.cz";

                vysledek = MailServices.SendMail(
                ObjednalEmail,
                                
                $"Objednávka č. {obj.FullObjednavkaID} byla schválena ",

                $"Vaše objednávka č. {obj.FullObjednavkaID} " +
                $"byla schválena uživatelem {schvalovatel.Jmeno}. <br />" +
                $"Dne {obj.SchvalenoDne:d.M.yyyy} " +
                $"v {obj.SchvalenoDne:HH:mm}" + " hodin"
                //+ $"<br> email pro uživatele: {pozadavek.Zalozil}@juli.cz; " +
                //$"{UserServices.GetUserById(pozadavek.Level1SchvalovatelID.Value).Email}" +
                //$"{UserServices.GetUserById(pozadavek.Level2SchvalovatelID.Value).Email}"
                //+ $"<br>  mail pro uživatele: {ObjednalEmail}"
                );

                foreach (var item in pozadavky)
                {
                    string email = item.Zalozil + "@juli.cz";
                    if (Constants.Test == true) email = "marek.novak@juli.cz";

                    vysledek += MailServices.SendMail(email,

                    $"Objednávka č. {obj.FullObjednavkaID} byla schválena!",

                    $"Objednávka č. {obj.FullObjednavkaID} vytvořená z vašeho požadavku č. {item.FullPozadavekID} " +
                    $"byla schválena uživatelem {schvalovatel.Jmeno}. <br />" +
                    $"Dne {obj.SchvalenoDne:d.M.yyyy} " +
                    $"v {obj.SchvalenoDne:HH:mm}" + " hodin" 
                   // + $"<br>  mail pro uživatele: {item.Zalozil}@juli.cz"
                    );

                    PozadavekDTO pozadavek = PozadavkyService.GetPozadavekById(item.ID);
                    pozadavek.Stav = "Objednávka schválena";

                    PozadavkyService.PozadavekSave(pozadavek);
                }

            }

            return vysledek;
             
        }

        public static List<CiselnikDTO> GetKSTList()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var q = (from k in db.Ciselnik
                         where k.Typ == 5
                         select new CiselnikDTO()
                         {
                             ID = k.ID,
                             Cislo = k.Cislo,
                             Popis = k.Popis,
                             FullName = k.Cislo + " - " + k.Popis
                         })                                                 
                         .OrderBy(o => o.Cislo).ToList();
                return q;
            }
        }

        public static List<CiselnikDTO> GetInvList()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var q = (from k in db.Ciselnik
                         where k.Typ == 1
                         select new CiselnikDTO()
                         {
                             ID = k.ID,
                             Cislo = k.Cislo,
                             Popis = k.Popis,
                             FullName = k.Cislo + " - " + k.Popis
                         })
                         .OrderBy(o => o.Cislo).ToList();
                return q;
            }
        }

        public static List<ObjednavkaTexty> GetObjednavkaTexty(int skupinaid)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var q = (from t in db.ObjTxt
                         where t.SkupinaID == skupinaid
                         select t).ToList();
                return q;
            }
        }

        public static void StornoObjednavky(int ObjId)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var obj = db.Objednavky.Find(ObjId);
                obj.Stornovano = true;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// vrati cislo obj. z FullObjednavkaID
        /// </summary>
        /// <param name="FullObjID"></param>
        /// <returns></returns>
        public static string GeObjNumber(string FullObjID)
        {
            string vysledek;
            if (string.IsNullOrEmpty(FullObjID)) vysledek = "";
            else vysledek = FullObjID.Substring(5, 4);

            return vysledek;
        }


        /// <summary>
        /// vrati List FullId objednavek
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListObjFullId()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                // var q = db.Objednavky.Distinct(x => x.FullObjednavkaID).

                List<string> q = (from id in db.Objednavky
                         select id.FullObjednavkaID)
                         .Distinct()
                         .OrderByDescending(c => c)
                         .ToList();
                return q;
            }
        }

        /// <summary>
        /// vrati List FullId objednavek
        /// </summary>
        /// <returns></returns>
        public static List<string> SearchObjFullId(string fullobj)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
    
                List<string> q = (from id in db.Objednavky
                                  where id.FullObjednavkaID.Contains(fullobj)
                                  select id.FullObjednavkaID)
                         .Distinct()
                         .OrderByDescending(c => c)
                         .ToList();
                return q;
            }
        }


        /// <summary>
        /// Vrati seznam objednávek (FullObjId)
        /// </summary>
        /// <returns></returns>
        public static List<string> GetObjFullId()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                List<string> q = (from id in db.Objednavky
                                  where id.FullObjednavkaID != null && id.FullObjednavkaID != ""
                                  select id.FullObjednavkaID)                                  
                                  .Distinct()
                                  .OrderByDescending(c => c)
                                  //.Take(100)
                                  .ToList();
                return q;
            }
        }

        /// <summary>
        /// Vrati seznam zakladatelů z pozadavku
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCreatorList()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {  
                List<string> query = (from i in db.ObjItems
                                      join p in db.Pozadavky on i.PozadavekID equals p.ID                       
                                      select p.Zalozil)
                                      .Distinct()
                                      .OrderBy(c => c)                          
                                      .ToList();

                return query;
            }
        }


        /// <summary>
        /// Vrati seznam dodavatelů z objednávek
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDodavateleList()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                List<string> q = (from o in db.Objednavky
                                  join d in db.Dodavatele on o.DodavatelID equals d.Id
                                  select d.SNAM05)
                                  .Distinct()
                                  .OrderBy(c => c)
                                  .ToList();
                return q;
            }
        }


    }
}