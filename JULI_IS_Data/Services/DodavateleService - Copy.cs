/*
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
    public static class DodavatelService
    {
        public static List<DodavateleS21DTO> GetDodavatelOrderedByName()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                return db.Dodavatele
                    .Select(d => new DodavateleS21DTO()
                    {
                        ID = d.ID,
                        Nazev = d.Nazev,
                        Adresa = d.Adresa,
                        Adresa2 = d.Adresa2,
                        Adresa3 = d.Adresa3,
                        AdresaDodatek = d.AdresaDodatek,
                        Mena = d.Mena,
                        Zeme = d.Zeme,
                        Telefon = d.Telefon,
                        JULINumber = d.JULINumber,
                        CisloNazev = d.JULINumber + " | " + d.Nazev

                        //,PozadavkyId = d.Pozadavky.Select(t => t.ID)
                    })
                    .OrderBy(a => a.Nazev)
                    .ToList();
            }

        }

        internal static void FillGridViewAllDodavatel(GridViewDataSet<DodavateleS21DTO> dataSet)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from d in db.Dodavatele        
                    select new DodavateleS21DTO()
                    {
                        ID = (int?) d.ID ?? 0,
                        JULINumber = d.JULINumber,
                        Nazev = d.Nazev,
                        Adresa = d.Adresa,
                        Adresa2 = d.Adresa2,
                        Adresa3 = d.Adresa3,
                        AdresaDodatek = d.AdresaDodatek,
                        Mena = d.Mena,
                        Zeme = d.Zeme,
                        Telefon = d.Telefon
                    });

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                dataSet.LoadFromQueryable(query);
            }
        }

        public static DodavateleS21DTO GetDodavatelById(int DodID)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = (from d in db.Dodavatele
                                     // where p.Id == id
                                 select new DodavateleS21DTO()
                                 {
                                     ID = d.ID,
                                     Nazev = d.Nazev,
                                     Adresa = d.Adresa,
                                     Adresa2 = d.Adresa2,
                                     Adresa3 = d.Adresa3,
                                     AdresaDodatek = d.AdresaDodatek,
                                     Mena = d.Mena,
                                     Zeme = d.Zeme,
                                     Telefon = d.Telefon,
                                     JULINumber = d.JULINumber
                                 }).SingleOrDefault(a => a.ID == DodID);

                return dodavatel;

            }
        }

        public static void DodavatelInsert(DodavateleS21DTO d)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = new Dodavatele_OLD()
                                 {
                                    ID = d.ID,
                                    Nazev = d.Nazev,
                                    Adresa = d.Adresa,
                                    Adresa2 = d.Adresa2,
                                    Adresa3 = d.Adresa3,
                                    AdresaDodatek = d.AdresaDodatek,
                                    Mena = d.Mena,
                                    Zeme = d.Zeme,
                                    Telefon = d.Telefon,
                                    JULINumber = d.JULINumber
                };

                db.Dodavatele.Add(dodavatel);
                db.SaveChanges();

            }
        }

        public static void DodavatelSave(DodavateleS21DTO data)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                
                var dodavatel = db.Dodavatele.Find((data.ID));

                dodavatel.Nazev = data.Nazev;
                dodavatel.Adresa = data.Adresa;
                dodavatel.Adresa2 = data.Adresa2;
                dodavatel.Adresa3 = data.Adresa3;
                dodavatel.AdresaDodatek = data.AdresaDodatek;
                dodavatel.Mena = data.Mena;
                dodavatel.Zeme = data.Zeme;
                dodavatel.Telefon = data.Telefon;
                dodavatel.JULINumber = data.JULINumber;
                db.SaveChanges();                
            }
        }

        public static void DeleteDodavatel(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = db.Dodavatele.Find(id);
                db.Dodavatele.Remove(dodavatel);
                db.SaveChanges();
            }
        }
    }
}
*/