using DotVVM.Framework.Controls;
using Pozadavky.Data;
using Pozadavky.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Security;
using DotVVM.Framework.ViewModel;
using AutoMapper;
using System.Data.SqlClient;

namespace Pozadavky.Services
{
    public static class ItemsService
    {
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }

        public static int LastItemId { get; set; } = 1;

        //static ItemsService()
        //{
        //    Mapper.Initialize(cfg => cfg.CreateMap<Items, ItemsDTO>());
        //}


        public static void FillGridViewItems(GridViewDataSet<ItemsDTO> dataSet)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.Items
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false
                             select new ItemsDTO()
                             {
                                 ID = (int?)i.ID ?? 0,
                                 PozadavekID = p.ID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis,
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka,
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani,
                                 ObjednavkaID = i.ObjednavkaID,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 Mena = p.Mena,
                                 Stredisko = p.KST,
                                 NabidkaCislo = i.NabidkaCislo,
                                 ObjednavkaFullID = i.ObjednavkaID != 0 ? i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString() : "není"
                             });

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                dataSet.LoadFromQueryable(query);
            }
        }

        public static void FillGridViewItemsNaObjednani(GridViewDataSet<ObjItemsDTO> datagv)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && i.Objednano == false && p.Level2Schvaleno == true && (i.ObjednavkaID == null || i.ObjednavkaID == 0)
                             select new ObjItemsDTO()
                             {
                                 ID = i.ID,
                                 PozadavekID = i.PozadavekID,
                                 DodavatelID = i.DodavatelID,
                                 OrigItemID = i.OrigItemID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis ?? "",
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka ?? "",
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani ?? DateTime.Now,
                                 ObjednavkaID = i.ObjednavkaID ?? 0,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString(),
                                 Mena = p.Mena,
                                 Stredisko = p.KST,
                                 NabidkaCislo = i.NabidkaCislo
                             });

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                datagv.LoadFromQueryable(query);
            }
        }

        public static void FillGridViewItemsFiltered(GridViewDataSet<ObjItemsDTO> datagv, string column = "", string filter = "", DateTime? datumod = null, DateTime? datumdo = null)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && i.Objednano == false && p.Level2Schvaleno == true && (i.ObjednavkaID == null || i.ObjednavkaID == 0)
                             && (datumod != null ? (i.DatumZalozeni > datumod.Value && i.DatumZalozeni <= datumdo) : true)
                             select new ObjItemsDTO()
                             {
                                 ID = i.ID,
                                 PozadavekID = i.PozadavekID,
                                 DodavatelID = i.DodavatelID,
                                 OrigItemID = i.OrigItemID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis ?? "",
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka ?? "",
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani ?? DateTime.Now,
                                 ObjednavkaID = i.ObjednavkaID ?? 0,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString(),
                                 Mena = p.Mena,
                                 Stredisko = p.KST,
                                 NabidkaCislo = i.NabidkaCislo
                             });

                var list = query;
                //var list2 = query;

                switch (column)
                {
                    case "FullPozadavekID":
                        list = query.Where(w => w.FullPozadavekID == filter);
                        break;
                    case "FullDodavatelName":
                        list = query.Where(w => w.FullDodavatelName == filter);
                        break;
                    case "KST":
                        list = query.Where(w => w.Stredisko == filter);
                        break;
                    case "Zalozil":
                        list = query.Where(w => w.Zalozil == filter);
                        break;
                    case "Polozka":
                        list = query.Where(w => w.Popis.Contains(filter));
                        break;
                    default:
                        list = query;
                        break;
                }

                //if (datumod != null)
                //{
                //    list = query.Where(w => w.DatumZalozeni <= datumdo.Value);
                //    list2 = query.Where(w => w.DatumZalozeni > datumod.Value);

                //}

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                datagv.LoadFromQueryable(list);
            }
        }

        public static void FillGridViewItemsFiltered(GridViewDataSet<ItemsDTO> datagv, string column = "", string filter = "", DateTime? datumod = null, DateTime? datumdo = null)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && i.Objednano == false && p.Level2Schvaleno == true && (i.ObjednavkaID == null || i.ObjednavkaID == 0)
                             && (datumod != null ? (i.DatumZalozeni > datumod.Value && i.DatumZalozeni < datumdo) : true)
                             select new ItemsDTO()
                             {
                                 ID = i.ID,
                                 PozadavekID = i.PozadavekID,
                                 DodavatelID = i.DodavatelID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis ?? "",
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka ?? "",
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani ?? DateTime.Now,
                                 ObjednavkaID = i.ObjednavkaID ?? 0,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString(),
                                 Mena = p.Mena,
                                 Stredisko = p.KST,
                                 NabidkaCislo = i.NabidkaCislo
                             });

                var list = query;
                //var list2 = query;

                switch (column)
                {
                    case "FullPozadavekID":
                        list = query.Where(w => w.FullPozadavekID == filter);
                        break;
                    case "FullDodavatelName":
                        list = query.Where(w => w.FullDodavatelName == filter);
                        break;
                    case "KST":
                        list = query.Where(w => w.Stredisko == filter);
                        break;
                    case "Zalozil":
                        list = query.Where(w => w.Zalozil == filter);
                        break;
                    case "Polozka":
                        list = query.Where(w => w.Popis.Contains(filter));
                        break;
                    default:
                        list = query;
                        break;
                }

                //if (datumod != null)
                //{
                //    list = query.Where(w => w.DatumZalozeni <= datumdo.Value);
                //    list2 = query.Where(w => w.DatumZalozeni > datumod.Value);

                //}

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                datagv.LoadFromQueryable(list);
            }
        }

        public static void FillGridViewObjednavkyWithItems(GridViewDataSet<ObjItemsDTO> datagv)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             join o in db.Objednavky on i.ObjednavkaID equals o.ID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == o.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && o.Smazano == false
                             select new ObjItemsDTO()
                             {
                                 ID = o.ID,
                                 IDstr = o.ID.ToString(),
                                 PozadavekID = i.PozadavekID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 DatumZalozeni = o.Datum.Value,

                                 DodavatelID = i.DodavatelID,
                                 OrigItemID = i.OrigItemID,
                                 Zalozil = p.Zalozil,
                                 Zastoupeno = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis ?? "",

                                 InterniPoznamka = i.InterniPoznamka ?? "",
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani ?? DateTime.Now,
                                 ObjednavkaID = i.ObjednavkaID ?? 0,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = o.FullObjednavkaID,
                                 Mena = o.Mena,
                                 Objednano = o.Objednano,
                                 Specha = i.Specha,
                                 Neobjednavat = i.Neobjednavat,
                                 Schvaleno = o.Schvaleno,
                                 Zamitnuto = o.Zamitnuto,
                                 NabidkaCislo = i.NabidkaCislo,
                                 Stav = ((o.Objednano && !o.Schvaleno) ? "Odesláno na schálení" : o.Schvaleno ? "Objednáno" : o.Zamitnuto ? "Zamítnuto" : "")

                             });

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                datagv.LoadFromQueryable(query);
            }
        }

        public static void FillGridViewObjednavkyWithItemsFiltered(GridViewDataSet<ObjItemsDTO> datagv, string column = "", string filter = "", DateTime? datumod = null, DateTime? datumdo = null)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             join o in db.Objednavky on i.ObjednavkaID equals o.ID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == o.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && o.Smazano == false && o.Zamitnuto == false
                             select new ObjItemsDTO()
                             {
                                 ID = i.ID,
                                 IDstr = o.ID.ToString(),                                
                                 PozadavekID = i.PozadavekID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 DatumZalozeni = o.Datum.Value,
                                 DodavatelID = i.DodavatelID,
                                 OrigItemID = i.OrigItemID,
                                 Zalozil = p.Zalozil,
                                 Zastoupeno = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis ?? "",
                                 HlavniRada = o.HlavniRada,
                                 InterniPoznamka = i.InterniPoznamka ?? "",
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani ?? DateTime.Now,
                                 ObjednavkaID = i.ObjednavkaID ?? 0,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = (o.FullObjednavkaID == null || o.FullObjednavkaID == "") ? o.ID.ToString() : o.FullObjednavkaID,
                                 Mena = o.Mena,
                                 Objednano = o.Objednano,
                                 Specha = i.Specha,
                                 Neobjednavat = i.Neobjednavat,
                                 Schvaleno = o.Schvaleno,
                                 Zamitnuto = o.Zamitnuto,
                                 NabidkaCislo = i.NabidkaCislo,
                                 Dodano = i.Dodano,
                                 // objednano = odeslano na schvaleni
                                 Stav = (
                                    (o.Stornovano) ? "Storno" :
                                    (o.Dokonceno & i.Dodano) ? "Zboží dodáno" :
                                    (o.Dokonceno & o.Odeslano & o.AvizoDoruceni) ? "Avízo zasláno" :
                                    (o.Dokonceno & o.Odeslano) ? "Objednávka odeslána" : 
                                    (o.Dokonceno & o.Neodesilat) ? "Objednávku neodesílat" : 
                                    (o.Objednano && !o.Schvaleno) ? "Odesláno na schválení" : 
                                    o.Schvaleno ? "Schváleno" : o.Zamitnuto ? "Zamítnuto" : "Koncept")
                             });

                var list = query;

                switch (column)
                {
                    case "ObjednavkaFullID":
                        list = query.Where(w => w.ObjednavkaFullID == filter);
                        break;
                    case "FullDodavatelName":
                        list = query.Where(w => w.FullDodavatelName.Contains(filter.Substring(0,5)));
                        break;
                    case "Stav":
                        list = query.Where(w => w.Stav == filter);
                        break;
                    case "Zalozil":
                        list = query.Where(w => w.Zalozil == filter);
                        break;
                    case "Polozka":
                        list = query.Where(w => w.Popis.Contains(filter));
                        break;
                    case "HlavniRada":
                        list = query.Where(w => w.HlavniRada.Contains(filter));
                        break;
                    default:
                        list = query;
                        break;
                }


                datagv.LoadFromQueryable(list);
            }
        }

        public static void FillGridViewObjednavkyWithItemsFilteredNeschvalene(GridViewDataSet<ObjItemsDTO> datagv, string column = "", string filter = "", DateTime? datumod = null, DateTime? datumdo = null)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             join o in db.Objednavky on i.ObjednavkaID equals o.ID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == o.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && o.Smazano == false && o.Zamitnuto == false && o.Schvaleno == false && o.Stornovano == false
                             select new ObjItemsDTO()
                             {
                                 ID = i.ID,
                                 IDstr = o.ID.ToString(),
                                 PozadavekID = i.PozadavekID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 DatumZalozeni = o.Datum.Value,
                                 DodavatelID = i.DodavatelID,
                                 OrigItemID = i.OrigItemID,
                                 Zalozil = p.Zalozil,
                                 Zastoupeno = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis ?? "",
                                 HlavniRada = o.HlavniRada,
                                 InterniPoznamka = i.InterniPoznamka ?? "",
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani ?? DateTime.Now,
                                 ObjednavkaID = i.ObjednavkaID ?? 0,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = (o.FullObjednavkaID == null || o.FullObjednavkaID == "") ? o.ID.ToString() : o.FullObjednavkaID,
                                 Mena = o.Mena,
                                 Objednano = o.Objednano,
                                 Specha = i.Specha,
                                 Neobjednavat = i.Neobjednavat,
                                 Schvaleno = o.Schvaleno,
                                 Zamitnuto = o.Zamitnuto,
                                 NabidkaCislo = i.NabidkaCislo,
                                 Dodano = i.Dodano,
                                 // objednano = odeslano na schvaleni
                                 Stav = (
                                    (o.Stornovano) ? "Storno" :
                                    (o.Dokonceno & i.Dodano) ? "Zboží dodáno" :
                                    (o.Dokonceno & o.Odeslano & o.AvizoDoruceni) ? "Avízo zasláno" :
                                    (o.Dokonceno & o.Odeslano) ? "Objednávka odeslána" :
                                    (o.Dokonceno & o.Neodesilat) ? "Objednávku neodesílat" :
                                    (o.Objednano && !o.Schvaleno) ? "Odesláno na schválení" :
                                    o.Schvaleno ? "Schváleno" : o.Zamitnuto ? "Zamítnuto" : "Koncept")
                             });

                var list = query;

                switch (column)
                {
                    case "ObjednavkaFullID":
                        list = query.Where(w => w.ObjednavkaFullID == filter);
                        break;
                    case "FullDodavatelName":
                        list = query.Where(w => w.FullDodavatelName.Contains(filter.Substring(0, 5)));
                        break;
                    case "Stav":
                        list = query.Where(w => w.Stav == filter);
                        break;
                    case "Zalozil":
                        list = query.Where(w => w.Zalozil == filter);
                        break;
                    case "Polozka":
                        list = query.Where(w => w.Popis.Contains(filter));
                        break;
                    case "HlavniRada":
                        list = query.Where(w => w.HlavniRada.Contains(filter));
                        break;
                    default:
                        list = query;
                        break;
                }


                datagv.LoadFromQueryable(list);
            }
        }

        public static void FillGridViewObjednavkyWithItemsFilteredSchvalene(GridViewDataSet<ObjItemsDTO> datagv, string column = "", string filter = "", DateTime? datumod = null, DateTime? datumdo = null)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             join o in db.Objednavky on i.ObjednavkaID equals o.ID
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == o.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && o.Smazano == false && (o.Schvaleno == true || (o.Schvaleno == false && o.Zamitnuto == true))
                             select new ObjItemsDTO()
                             {
                                 ID = i.ID,
                                 IDstr = o.ID.ToString(),
                                 PozadavekID = i.PozadavekID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 DatumZalozeni = o.Datum.Value,
                                 DodavatelID = i.DodavatelID,
                                 OrigItemID = i.OrigItemID,
                                 Zalozil = p.Zalozil,
                                 Zastoupeno = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis ?? "",
                                 HlavniRada = o.HlavniRada,
                                 InterniPoznamka = i.InterniPoznamka ?? "",
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani ?? DateTime.Now,
                                 ObjednavkaID = i.ObjednavkaID ?? 0,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = (o.FullObjednavkaID == null || o.FullObjednavkaID == "") ? o.ID.ToString() : o.FullObjednavkaID,
                                 Mena = o.Mena,
                                 Objednano = o.Objednano,
                                 Specha = i.Specha,
                                 Neobjednavat = i.Neobjednavat,
                                 Schvaleno = o.Schvaleno,
                                 Zamitnuto = o.Zamitnuto,
                                 NabidkaCislo = i.NabidkaCislo,
                                 Dodano = i.Dodano,
                                 // objednano = odeslano na schvaleni
                                 Stav = (
                                    (o.Stornovano) ? "Storno" :
                                    (o.Dokonceno & i.Dodano) ? "Zboží dodáno" :
                                    (o.Dokonceno & o.Odeslano & o.AvizoDoruceni) ? "Avízo zasláno" :
                                    (o.Dokonceno & o.Odeslano) ? "Objednávka odeslána" :
                                    (o.Dokonceno & o.Neodesilat) ? "Objednávku neodesílat" :
                                    (o.Objednano && !o.Schvaleno && !o.Zamitnuto) ? "Odesláno na schválení" :
                                    o.Schvaleno ? "Schváleno" : o.Zamitnuto ? "Zamítnuto" : "Koncept")
                             });

                var list = query;

                switch (column)
                {
                    case "ObjednavkaFullID":
                        list = query.Where(w => w.ObjednavkaFullID == filter);
                        break;
                    case "FullDodavatelName":
                        list = query.Where(w => w.FullDodavatelName.Contains(filter.Substring(0, 5)));
                        break;
                    case "Stav":
                        list = query.Where(w => w.Stav == filter);
                        break;
                    case "Zalozil":
                        list = query.Where(w => w.Zalozil == filter);
                        break;
                    case "Polozka":
                        list = query.Where(w => w.Popis.Contains(filter));
                        break;
                    case "HlavniRada":
                        list = query.Where(w => w.HlavniRada.Contains(filter));
                        break;
                    default:
                        list = query;
                        break;
                }


                datagv.LoadFromQueryable(list);
            }
        }


        // pro DotVVM 1.1.
        public static GridViewDataSetLoadedData<ItemsDTO> FillGridViewItemsNaObjednaniv11(IGridViewDataSetLoadOptions gridViewDataSetLoadOptions)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.Items
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where i.Smazano == false && i.Objednano == false && p.Level2Schvaleno == true && i.ObjednavkaID == 0
                             select new ItemsDTO()
                             {
                                 ID = (int?)i.ID ?? 0,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 Popis = i.Popis,
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka,
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani,
                                 ObjednavkaID = i.ObjednavkaID,
                                 FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                 ObjednavkaFullID = i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString(),
                                 Mena = p.Mena,
                                 Stredisko = p.KST
                             }).OrderBy(order => order.ID);

                return query.GetDataFromQueryable(gridViewDataSetLoadOptions);

            }
        }

        /// <summary>
        /// nastavi trideni a velikost stranky GridView
        /// </summary>
        /// <param name="data">GridView</param>
        public static void GridViewSetSort(GridViewDataSet<ItemsDTO> data)
        {
            data.PagingOptions.PageSize = 20;
            data.SortingOptions.SortExpression = nameof(ItemsDTO.PozadavekID);
            data.SortingOptions.SortDescending = true;
        }


        /// <summary>
        /// nastavi trideni a velikost stranky GridView
        /// </summary>
        /// <param name="data">GridView</param>
        public static void GridViewSetSort(GridViewDataSet<ObjItemsDTO> data)
        {
            data.PagingOptions.PageSize = 20;
            data.SortingOptions.SortExpression = nameof(PozadavekDTO.Stav);
            data.SortingOptions.SortDescending = false;
        }

        public static void GridViewSetSortByID(GridViewDataSet<ObjItemsDTO> data)
        {
            data.PagingOptions.PageSize = 15;
            data.SortingOptions.SortExpression = nameof(ObjItemsDTO.ObjednavkaID);
            data.SortingOptions.SortDescending = true;
        }

        public static void FillGridViewItemsByUser(GridViewDataSet<ItemsDTO> dataSet, string name = "", string column = "", string filter = "", DateTime? datumod = null, DateTime? datumdo = null)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                if (name != "")
                {
                    //var query = (from i in db.Items
                    //             join p in db.Pozadavky on i.PozadavekID equals p.ID
                    //             from d in db.Dodavatele.Where(dod => dod.Id == i.DodavatelID).DefaultIfEmpty()
                    //             where p.Zalozil == name & i.Smazano == false
                    //             select new ItemsDTO()
                    //             {
                    //                 ID = (int?)i.ID ?? 0,
                    //                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                    //                 DatumZalozeni = i.DatumZalozeni,
                    //                 InterniPoznamka = i.InterniPoznamka,
                    //                 Jednotka = i.Jednotka,
                    //                 Mnozstvi = i.Mnozstvi,
                    //                 CenaZaJednotku = i.CenaZaJednotku,
                    //                 CelkovaCena = i.CelkovaCena,
                    //                 TerminDodani = i.TerminDodani,
                    //                 ObjednavkaID = i.ObjednavkaID,
                    //                 ObjednavkaFullID = i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString(),
                    //                 Mena = i.Mena
                    //             });

                    var query = (from i in db.Items
                                 join p in db.Pozadavky on i.PozadavekID equals p.ID
                                 from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                                 from o in db.Objednavky.Where(obj => obj.ID == i.ObjednavkaID).DefaultIfEmpty()
                                 where i.Smazano == false && p.Neverejny == false && p.Zalozil == name
                                 select new ItemsDTO()
                                 {
                                     ID = (int?)i.ID ?? 0,
                                     PozadavekID = p.ID,
                                     FullPozadavekID = p.FullPozadavekID,
                                     Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                     Popis = i.Popis,
                                     DatumZalozeni = i.DatumZalozeni,
                                     InterniPoznamka = i.InterniPoznamka,
                                     Jednotka = i.Jednotka,
                                     Mnozstvi = i.Mnozstvi,
                                     CenaZaJednotku = i.CenaZaJednotku,
                                     CelkovaCena = i.CelkovaCena,
                                     TerminDodani = i.TerminDodani,
                                     ObjednavkaID = i.ObjednavkaID,
                                     FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                     FullDodavatelNumber = d.SUPN05 + " | " + d.SNAM05,
                                     Mena = p.Mena,
                                     Stredisko = p.KST,
                                     NabidkaCislo = i.NabidkaCislo,
                                     ObjednavkaFullID = (string.IsNullOrEmpty(o.FullObjednavkaID) ? (i.ObjednavkaID == 0 ? "není" : i.ObjednavkaID.ToString()) : o.FullObjednavkaID)
                                     //ObjednavkaFullID = i.ObjednavkaID != 0 ? i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString() : "není"
                                 });

                    var list = query;
                    //var list2 = query;

                    switch (column)
                    {
                        case "FullPozadavekID":
                            list = query.Where(w => w.FullPozadavekID == filter);
                            break;
                        case "FullDodavatelName":
                            list = query.Where(w => w.FullDodavatelName.Contains(filter.Substring(0, 5)));
                            break;
                        case "DodavatelNumber":
                            list = query.Where(w => w.FullDodavatelNumber == filter);
                            break;
                        case "KST":
                            list = query.Where(w => w.Stredisko == filter);
                            break;
                        case "Zalozil":
                            list = query.Where(w => w.Zalozil == filter);
                            break;
                        case "Polozka":
                            list = query.Where(w => w.Popis.Contains(filter));
                            break;
                        default:
                            list = query;
                            break;
                    }

                    //// vlastni SQL
                    //if (column == "" & datumdo == null & datumod == null & filter != "")
                    //{
                    //    list = query.Where(w => w.))
                    //}


                    // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                    dataSet.LoadFromQueryable(list.OrderByDescending(o => o.DatumZalozeni));
                }
                else
                {
                    //var query = (from i in db.Items
                    //             join p in db.Pozadavky on i.PozadavekID equals p.ID
                    //             from d in db.Dodavatele.Where(dod => dod.Id == i.DodavatelID).DefaultIfEmpty()
                    //             where i.Smazano == false
                    //             select new ItemsDTO()
                    //             {
                    //                 ID = (int?)i.ID ?? 0,
                    //                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                    //                 DatumZalozeni = i.DatumZalozeni,
                    //                 InterniPoznamka = i.InterniPoznamka,
                    //                 Jednotka = i.Jednotka,
                    //                 Mnozstvi = i.Mnozstvi,
                    //                 CenaZaJednotku = i.CenaZaJednotku,
                    //                 CelkovaCena = i.CelkovaCena,
                    //                 TerminDodani = i.TerminDodani,
                    //                 ObjednavkaID = i.ObjednavkaID,
                    //                 ObjednavkaFullID = i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString(),
                    //                 Mena = i.Mena
                    //             });


                    var query = (from i in db.Items
                                 join p in db.Pozadavky on i.PozadavekID equals p.ID
                                 from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                                 from o in db.Objednavky.Where(obj => obj.ID == i.ObjednavkaID).DefaultIfEmpty()
                                 where i.Smazano == false && p.Neverejny == false
                                 select new ItemsDTO()
                                 {
                                     ID = (int?)i.ID ?? 0,
                                     PozadavekID = p.ID,
                                     FullPozadavekID = p.FullPozadavekID,
                                     Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                     Popis = i.Popis,
                                     DatumZalozeni = i.DatumZalozeni,
                                     InterniPoznamka = i.InterniPoznamka,
                                     Jednotka = i.Jednotka,
                                     Mnozstvi = i.Mnozstvi,
                                     CenaZaJednotku = i.CenaZaJednotku,
                                     CelkovaCena = i.CelkovaCena,
                                     TerminDodani = i.TerminDodani,
                                     ObjednavkaID = i.ObjednavkaID,
                                     FullDodavatelName = d.SNAM05 + " | " + d.SUPN05,
                                     FullDodavatelNumber = d.SUPN05 + " | " + d.SNAM05,
                                     Mena = p.Mena,
                                     Stredisko = p.KST,
                                     NabidkaCislo = i.NabidkaCislo,
                                     ObjednavkaFullID = (string.IsNullOrEmpty(o.FullObjednavkaID) ? (i.ObjednavkaID == 0 ? "není" : i.ObjednavkaID.ToString()) : o.FullObjednavkaID)
                                     //ObjednavkaFullID = i.ObjednavkaID != 0 ? i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString() : "není"
                                 });

                    var list = query;
                    //var list2 = query;

                    switch (column)
                    {
                        case "FullPozadavekID":
                            list = query.Where(w => w.FullPozadavekID == filter);
                            break;
                        case "FullDodavatelName":
                            list = query.Where(w => w.FullDodavatelName.Contains(filter.Substring(0, 5)));            
                            break;
                        case "DodavatelNumber":
                            list = query.Where(w => w.FullDodavatelNumber == filter);
                            break;
                        case "KST":
                            list = query.Where(w => w.Stredisko == filter);
                            break;
                        case "Zalozil":
                            list = query.Where(w => w.Zalozil == filter);
                            break;
                        case "Polozka":
                            list = query.Where(w => w.Popis.Contains(filter));
                            break;
                        default:
                            list = query;
                            break;
                    }

                    // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                    dataSet.LoadFromQueryable(list.OrderByDescending(o => o.DatumZalozeni));
                }
            }
        }

        //        select Popis, Jednotka, sum(Mnozstvi) as Mnozstvi, CenaZaJednotku, sum(CelkovaCena) as CelkovaCena, Mena, TerminDodani from  Items
        //        where ObjednavkaID = 29
        //        group by Popis, Jednotka, CenaZaJednotku, Popis, Jednotka, CenaZaJednotku, Mena, TerminDodani



        public static void FillGridViewItemsBySQL(GridViewDataSet<ItemsDTO> dataSet, string wherecond = "")
        {
            /*
                select i.id as ID, p.ID as PozadavekID, p.FullPozadavekID as FullPozadavekID, 
                IIF(((p.Zastoupeno IS NULL) OR(p.Zastoupeno = '')), p.Zalozil, p.Zastoupeno) as Zalozil, 
                i.Popis, i.InterniPoznamka, i.Jednotka, i.Mnozstvi, i.CenaZaJednotku, i.CelkovaCena, i.TerminDodani, 
                (d.SNAM05 + ' | ' + d.SUPN05) as FullDodavatelName, (d.SUPN05 + ' | ' + d.SNAM05) as FullDodavatelNumber, 
                DatumZalozeni = i.DatumZalozeni, p.Mena, p.KST, i.NabidkaCislo, i.ObjednavkaID,  
                ISNULL(o.FullObjednavkaID, '') as ObjednavkaFullID
                FROM Items i join Pozadavky p on i.PozadavekID = p.ID
                left outer join Dodavatele d on p.DodavatelID = d.Id
                left outer join Objednavky o on i.ObjednavkaID = o.ID
                WHERE i.Smazano = 0 AND p.Neverejny = 0
            */
            string sqlcomm =
                "select i.id as ID, p.ID as PozadavekID, p.FullPozadavekID as FullPozadavekID, " +
                "IIF(((p.Zastoupeno IS NULL) OR(p.Zastoupeno = '')), p.Zalozil, p.Zastoupeno) as Zalozil, " +
                "i.Popis, i.InterniPoznamka, i.Jednotka, i.Mnozstvi, i.CenaZaJednotku, i.CelkovaCena, i.TerminDodani, " +
                "(d.SNAM05 + ' | ' + d.SUPN05) as FullDodavatelName, (d.SUPN05 + ' | ' + d.SNAM05) as FullDodavatelNumber, " +
                "DatumZalozeni = i.DatumZalozeni, p.Mena, p.KST, i.NabidkaCislo, i.ObjednavkaID,  " +
                "ISNULL(o.FullObjednavkaID, '') as ObjednavkaFullID " +
                "FROM Items i join Pozadavky p on i.PozadavekID = p.ID " +
                "left outer join Dodavatele d on p.DodavatelID = d.Id " +
                "left outer join Objednavky o on i.ObjednavkaID = o.ID " +
                "WHERE i.Smazano = 0 AND p.Neverejny = 0 AND " + wherecond;


            if (!checkForSQLInjection(wherecond))
            {
                using (var db = new PozadavkyContext(DtbConxString))
                {

                    // verze s parametry, nejdou nastavovat jmena sloupcu

                    //var parameters = new List<SqlParameter>();

                    //parameters.Clear();
                    //parameters.Add(new SqlParameter("@whereparam", wherecond));

                    //var query = db.Database.SqlQuery<ItemsDTO>(sqlcomm, parameters.Select(x => ((ICloneable)x).Clone()).ToArray());
                    //var query2 = query.ToList().AsQueryable();

                    //dataSet.LoadFromQueryable(query2);

                    var query = db.Database.SqlQuery<ItemsDTO>(sqlcomm);
                    var query2 = query.AsQueryable();

                    dataSet.LoadFromQueryable(query2);

                }
            }

            else
            {
                MailServices.SendMail("marek.novak@juli.cz; ko.helpdesk@juli.cz",
                    "SQL Injection Alert!",
                    "Zaznamenán podezřelý dotaz v požadavcích: \n" +
                    $"{wherecond} \n" +
                    $"uživatel: {UserServices.GetActiveUser()}");

                throw new Exception("sqlinjection");
            }
        }


        public static int GetPocetItemsByObjId(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var items = (from i in db.ObjItems
                             where i.ObjednavkaID == id && i.Smazano == false
                             select i);

                return items.Count();
            }
        }

        public static float GetCelkovaCenaByObjId(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var items = (from i in db.ObjItems
                             where i.ObjednavkaID == id && i.Smazano == false
                             select i.CelkovaCena).DefaultIfEmpty(0).Sum();

                return items;
            }
        }

        public static void ObjItemSave(ObjItemsDTO data)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = db.ObjItems.Find((data.ID));
                item.CisloRadku = data.CisloRadku;
                item.DatumZalozeni = data.DatumZalozeni;
                item.InterniPoznamka = data.InterniPoznamka;
                item.Jednotka = data.Jednotka;
                item.Mnozstvi = data.Mnozstvi;
                item.CenaZaJednotku = data.CenaZaJednotku;
                item.CelkovaCena = data.CelkovaCena;
                item.TerminDodani = data.TerminDodani ?? DateTime.Now;
                item.DodavatelID = data.DodavatelID;
                item.ObjednavkaID = data.ObjednavkaID ?? 0;
                item.Poptany3Firmy = data.Poptany3Firmy;
                item.DuvodID = data.DuvodID;
                item.Duvod = data.Duvod;
                item.PozadavekID = data.PozadavekID;
                item.Popis = data.Popis;
                item.DatumObjednani = data.DatumObjednani;
                item.ObjednalUzivatel = data.ObjednalUzivatel;
                item.KST = data.KST;
                item.InvesticeNeplanovana = data.InvesticeNeplanovana;
                item.InvesticePlanovana = data.InvesticePlanovana;
                item.NakupOstatni = data.NakupOstatni;
                item.CisloInvestice = data.CisloInvestice;
                item.CisloKonta = data.CisloKonta;
                item.Specha = data.Specha;
                item.Neobjednavat = data.Neobjednavat;
                item.ObjednalUzivatel = data.ObjednalUzivatel;
                item.NabidkaCislo = data.NabidkaCislo;
                item.Dodano = data.Dodano;
                db.SaveChanges();
            }
        }

        public static void ObjItemSave(ObjItems data)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = db.ObjItems.Find((data.ID));

                item.DodavatelID = data.DodavatelID;

                //db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void DeleteItem(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = db.Items.Find(id);

                item.Smazano = true;
                item.SmazalUzivatel = UserServices.GetActiveUser();
                item.DatumSmazani = DateTime.Now;
                //db.Pozadavky.Remove(pozadavek);
                db.SaveChanges();
            }
        }

        public static void DeleteItemsByPozadavekId(int id, bool podepsano = false)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                List<Items> items = (from i in db.Items
                                     where i.PozadavekID == id
                                     select i).ToList();

                if (podepsano)
                    items.ForEach(i =>
                    {
                        i.Smazano = true;
                        i.SmazalUzivatel = UserServices.GetActiveUser();
                        i.DatumSmazani = DateTime.Now;
                    });
                else
                    items.ForEach(i => db.Items.Remove(i));

                db.SaveChanges();
            }
        }

        /// <summary>
        /// zkopiruje polozky z pozadavku do objednavek
        /// </summary>
        /// <param name="data"></param>
        public static void CopyItemsToObjByPoz(PozadavekDTO data)
        {
            //delete all old items and copy new
            DeleteObjItemsByPozadavekId(data.ID);

            using (var db = new PozadavkyContext(DtbConxString))
            {
                // GROUP je z duvodu spojeni rozuctovanych polozek
                string sqlcomm =
                "INSERT INTO dbo.ObjItems(PozadavekID, OrigItemID, CisloRadku, Popis, InterniPoznamka, KST, CisloInvestice," +
                                        "CisloKonta, InvesticePlanovana, InvesticeNeplanovana, NakupOstatni, Jednotka, Mnozstvi," +
                                        "CenaZaJednotku, CelkovaCena, DatumZalozeni, DatumObjednani, TerminDodani, Smazano," +
                                        "SmazalUzivatel, DatumSmazani, Mena, DodavatelID, Poptany3Firmy, DuvodID," +
                                        "Duvod, ObjednavkaID, ObjednalUzivatel, Objednano, Specha, Neobjednavat, NabidkaCislo) " +
                "SELECT i.PozadavekID, min(i.ID) as ID, null, i.Popis, InterniPoznamka, iif(min(isnull(i.KST,'')) = '', iif(min(isnull(p.KST,'')) = '', min(SUBSTRING(i.CisloInvestice,1,3)), min(i.KST)), min(i.KST)) as KST, min(i.CisloInvestice)," +
                                       "min(i.CisloKonta), null, null, null, Jednotka, sum(i.Mnozstvi) as Mnozstvi," +
                                       "i.CenaZaJednotku as CenaZaJednotku, sum(i.CelkovaCena) as CelkovaCena, min(i.DatumZalozeni) as DatumZalozeni, null, min(i.TerminDodani) as TerminDodani, 0," +
                                       "null, null, p.Mena, iif(max(isnull(i.DodavatelID, 0)) = 0, max(p.DodavatelID), max(i.DodavatelID)) as DodavatelID, i.Poptany3Firmy, i.DuvodID," +
                                       "Duvod, null, null, i.Objednano, Specha, Neobjednavat, i.NabidkaCislo  " +
                "FROM dbo.Items i left join Pozadavky p on i.PozadavekID = p.ID " +
                "WHERE i.Smazano = 0 AND i.Objednano = 0 AND PozadavekID = @id " +
                "GROUP BY i.PozadavekID,  i.Popis, InterniPoznamka, Jednotka,  CenaZaJednotku, p.Mena, Poptany3Firmy, DuvodID, Duvod,  i.Objednano, Specha, Neobjednavat, i.NabidkaCislo  " +
                "ORDER BY min(i.ID)";

                SqlParameter parameter = new SqlParameter("@id", data.ID);
                db.Database.ExecuteSqlCommand(sqlcomm, parameter);
            }

            //using (var db = new PozadavkyContext(DtbConxString))
            //{
            //    // vsem novym polozkam priradit kopii dodavatele
            //    //var objitems = (from o in db.ObjItems
            //    //                where o.PozadavekID == data.ID
            //    //                select o);


            //    //int DodNewId = DodavatelService.DodavatelCopy(objitems.First().DodavatelID);
                      
            //    //foreach (var item in objitems)
            //    //{
            //    //    item.DodavatelID = DodNewId;
            //    //    ObjItemSave(item);
            //    //}
            //}
        }

        private static void DeleteObjItemsByPozadavekId(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                string sqlcomm =
                    "DELETE FROM dbo.ObjItems WHERE PozadavekID = @id";

                SqlParameter parameter = new SqlParameter("@id", id);

                db.Database.ExecuteSqlCommand(sqlcomm, parameter);
            }
        }


        /// <summary>
        /// odstrani polozku z objednavky, nemaze fyzicky
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteItemFromObj(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var i = db.ObjItems.Find(id);
                i.ObjednavkaID = null;
                db.SaveChanges();
            }
        }




        public static void FillGridViewItemsByPozadavekId(GridViewDataSet<ItemsDTO> dataSet, int pozadavekId)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.Items
                             join p in db.Pozadavky on i.PozadavekID equals p.ID
                             from d in db.Dodavatele.Where(dod => dod.Id == i.DodavatelID).DefaultIfEmpty()
                             from o in db.Objednavky.Where(obj => obj.ID == i.ObjednavkaID).DefaultIfEmpty()
                             where i.PozadavekID == pozadavekId && i.Smazano == false
                             select new ItemsDTO()
                             {
                                 ID = i.ID,
                                 CisloRadku = i.CisloRadku,
                                 Popis = i.Popis,
                                 Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka,
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani,
                                 ObjednavkaID = i.ObjednavkaID,
                                 ObjednavkaFullID = (string.IsNullOrEmpty(o.FullObjednavkaID) ? (i.ObjednavkaID == 0 ? "není" : i.ObjednavkaID.ToString()) : o.FullObjednavkaID),
                                 Mena = i.Mena,
                                 KST = i.KST,
                                 CisloKonta = i.CisloKonta,
                                 CisloInvestice = i.CisloInvestice
                             });

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                dataSet.LoadFromQueryable(query);
            }
        }

        public static List<ItemsDTO> GetItemsByPozadavekId(int pozadavekId)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.Items
                             where i.PozadavekID == pozadavekId && i.Smazano == false
                             select new ItemsDTO()
                             {
                                 ID = i.ID,
                                 Popis = i.Popis,
                                 CisloRadku = i.CisloRadku,
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka,
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani,
                                 ObjednavkaID = i.ObjednavkaID,
                                 KST = i.KST,
                                 CisloKonta = i.CisloKonta,
                                 CisloInvestice = i.CisloInvestice,
                                 ObjednavkaFullID = i.ObjednavkaID != 0 ? i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString() : "není",
                                 Mena = i.Mena
                             });

                return query.ToList();
            }
        }

        public static List<ItemsDTO> GetItemsByObjId(int objId)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from i in db.ObjItems
                             where i.ObjednavkaID == objId && i.Smazano == false
                             select new ItemsDTO()
                             {
                                 ID = i.ID,
                                 Popis = i.Popis,
                                 CisloRadku = i.CisloRadku ?? 1,
                                 DatumZalozeni = i.DatumZalozeni,
                                 InterniPoznamka = i.InterniPoznamka,
                                 Jednotka = i.Jednotka,
                                 Mnozstvi = i.Mnozstvi,
                                 CenaZaJednotku = i.CenaZaJednotku,
                                 CelkovaCena = i.CelkovaCena,
                                 TerminDodani = i.TerminDodani,
                                 ObjednavkaID = i.ObjednavkaID,
                                 KST = i.KST,
                                 CisloKonta = i.CisloKonta,
                                 CisloInvestice = i.CisloInvestice,
                                 ObjednavkaFullID = i.ObjednavkaID != 0 ? i.DatumZalozeni.Year.ToString() + i.ObjednavkaID.ToString() : "není",
                                 Mena = i.Mena,
                                 NabidkaCislo = i.NabidkaCislo
                             });

                return query.OrderBy(o => o.ID).ToList();
            }
        }

        public static int ItemInsert(ItemsDTO itemData)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var i = new Items()
                {
                    PozadavekID = itemData.PozadavekID,
                    Popis = itemData.Popis,
                    DodavatelID = itemData.DodavatelID,
                    CisloRadku = itemData.CisloRadku,
                    DatumZalozeni = itemData.DatumZalozeni,
                    DatumObjednani = itemData.DatumObjednani,
                    InterniPoznamka = itemData.InterniPoznamka,
                    Jednotka = itemData.Jednotka,
                    Mnozstvi = itemData.Mnozstvi,
                    CenaZaJednotku = itemData.CenaZaJednotku,
                    CelkovaCena = itemData.CelkovaCena,
                    TerminDodani = itemData.TerminDodani ?? DateTime.Now,
                    Poptany3Firmy = itemData.Poptany3Firmy,
                    DuvodID = itemData.DuvodID,
                    Duvod = itemData.Duvod,
                    ObjednavkaID = itemData.ObjednavkaID ?? 0,
                    ObjednalUzivatel = itemData.ObjednalUzivatel,
                    KST = itemData.KST,
                    InvesticeNeplanovana = itemData.InvesticeNeplanovana,
                    InvesticePlanovana = itemData.InvesticePlanovana,
                    NakupOstatni = itemData.NakupOstatni,
                    CisloInvestice = itemData.CisloInvestice,
                    CisloKonta = itemData.CisloKonta
                };

                db.Items.Add(i);
                db.SaveChanges();

                LastItemId = i.ID;
                return i.ID;
            }
        }

        public static ItemsDTO GetItemById(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = (from i in db.Items
                            select new ItemsDTO()
                            {
                                ID = i.ID,
                                PozadavekID = i.PozadavekID,
                                Popis = i.Popis,
                                CisloRadku = i.CisloRadku,
                                DatumZalozeni = i.DatumZalozeni,
                                DatumObjednani = i.DatumObjednani,
                                TerminDodani = i.TerminDodani,

                                InterniPoznamka = i.InterniPoznamka,
                                Jednotka = i.Jednotka,
                                Mnozstvi = i.Mnozstvi,
                                CenaZaJednotku = i.CenaZaJednotku,
                                CelkovaCena = i.CelkovaCena,
                                Poptany3Firmy = i.Poptany3Firmy,
                                DuvodID = i.DuvodID,
                                Duvod = i.Duvod,
                                ObjednavkaID = i.ObjednavkaID,
                                ObjednalUzivatel = i.ObjednalUzivatel,
                                KST = i.KST,
                                InvesticeNeplanovana = i.InvesticeNeplanovana,
                                InvesticePlanovana = i.InvesticePlanovana,
                                NakupOstatni = i.NakupOstatni,
                                CisloInvestice = i.CisloInvestice,
                                CisloKonta = i.CisloKonta,
                                NabidkaCislo = i.NabidkaCislo
                            }).SingleOrDefault(a => a.ID == id);

                return item;
            }
        }


        public static ObjItemsDTO GetObjItemById(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = (from i in db.ObjItems
                            select new ObjItemsDTO()
                            {
                                ID = i.ID,
                                PozadavekID = i.PozadavekID,
                                Popis = i.Popis,
                                DodavatelID = i.DodavatelID,
                                OrigItemID = i.OrigItemID,
                                ObjednavkaID = i.ObjednavkaID,
                                CisloRadku = i.CisloRadku,
                                DatumZalozeni = i.DatumZalozeni,
                                DatumObjednani = i.DatumObjednani,
                                TerminDodani = i.TerminDodani,

                                InterniPoznamka = i.InterniPoznamka,
                                Jednotka = i.Jednotka,
                                Mnozstvi = i.Mnozstvi,
                                CenaZaJednotku = i.CenaZaJednotku,
                                CelkovaCena = i.CelkovaCena,
                                Poptany3Firmy = i.Poptany3Firmy,
                                DuvodID = i.DuvodID,
                                Duvod = i.Duvod,
                                Specha = i.Specha,
                                Neobjednavat = i.Neobjednavat,
                                ObjednalUzivatel = i.ObjednalUzivatel,
                                KST = i.KST,

                                CisloInvestice = i.CisloInvestice,
                                CisloKonta = i.CisloKonta,
                                NabidkaCislo = i.NabidkaCislo,
                                Dodano = i.Dodano,
                            }).SingleOrDefault(a => a.ID == id);

                return item;
            }
        }

        public static float GetCelkovaCenaByPozadavekId(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var items = (from i in db.Items
                             where i.PozadavekID == id && i.Smazano == false
                             select i.CelkovaCena).DefaultIfEmpty(0).Sum();

                return items;
            }
        }

        // zjednoduseni je AutoMapper

        public static void ItemSave(ItemsDTO data)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var item = db.Items.Find((data.ID));
                item.CisloRadku = data.CisloRadku;
                item.DatumZalozeni = data.DatumZalozeni;
                item.InterniPoznamka = data.InterniPoznamka;
                item.Jednotka = data.Jednotka;
                item.Mnozstvi = data.Mnozstvi;
                item.CenaZaJednotku = data.CenaZaJednotku;
                item.CelkovaCena = data.CelkovaCena;
                item.TerminDodani = data.TerminDodani ?? DateTime.Now;
                item.DodavatelID = data.DodavatelID;
                item.ObjednavkaID = data.ObjednavkaID ?? 0;
                item.Poptany3Firmy = data.Poptany3Firmy;
                item.DuvodID = data.DuvodID;
                item.Duvod = data.Duvod;
                item.PozadavekID = data.PozadavekID;
                item.Popis = data.Popis;
                item.DatumObjednani = data.DatumObjednani;
                item.ObjednalUzivatel = data.ObjednalUzivatel;
                item.KST = data.KST;
                item.InvesticeNeplanovana = data.InvesticeNeplanovana;
                item.InvesticePlanovana = data.InvesticePlanovana;
                item.NakupOstatni = data.NakupOstatni;
                item.CisloInvestice = data.CisloInvestice;
                item.CisloKonta = data.CisloKonta;
                item.NabidkaCislo = data.NabidkaCislo;
                db.SaveChanges();
            }

        }

        public static int GetPocetItemsByPozadavekId(int id)
        {

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var items = (from i in db.Items
                             where i.PozadavekID == id && i.Smazano == false
                             select i);

                return items.Count();
            }
        }


        //public static void DeletePozadavkyByItemId(int id)
        //{
        //    using (var db = new PozadavkyContext(DtbConxString))
        //    {
        //        var ListPozadavkuKeSmazani = new List<Pozadavek>();

        //        ListPozadavkuKeSmazani = 
        //            (from p in db.Pozadavky
        //            where p.ObjednavkaID == id
        //            select p).ToList();

        //        foreach (var item in ListPozadavkuKeSmazani)
        //        {
        //            var pozadavek = db.Pozadavky.Find(item.Id);

        //            pozadavek.Smazano = true;
        //            pozadavek.SmazalUzivatel = UserServices.GetActiveUser();
        //            pozadavek.SmazanoDne = DateTime.Now;
        //            db.SaveChanges();
        //        }    
        //    }
        //}


        private static Boolean checkForSQLInjection(string userInput)

        {

            bool isSQLInjection = false;

            string[] sqlCheckList = { "--",
                                       ";--",
                                       ";",
                                       "/*",
                                       "*/",
                                        "@@",
                                        "@",
                                        "char",
                                        "nchar",
                                        "varchar",
                                        "nvarchar",
                                        "alter",
                                        "begin",
                                        "cast",
                                        "create",
                                        "cursor",
                                        "declare",
                                        "delete",
                                        "drop",
                                        "end",
                                        "exec",
                                        "execute",
                                        "fetch",
                                        "insert",
                                        "kill",
                                        "select",
                                        "sys",
                                        "sysobjects",
                                        "syscolumns",
                                        "table",
                                        "update"
                                       };

            string CheckString = userInput.Replace("'", "''");
            for (int i = 0; i <= sqlCheckList.Length - 1; i++)
            {
                if ((CheckString.IndexOf(sqlCheckList[i],
                    StringComparison.OrdinalIgnoreCase) >= 0))
                   { isSQLInjection = true; }
            }
            return isSQLInjection;
        }
    }


}

