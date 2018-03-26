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
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }

        public static List<DodavateleS21> GetS21DodavateleOrderedByName()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                return db.DodavateleS21
                    .Select(d => new DodavateleS21()
                    {
                        Id = d.ID,
                        SUPN05 = d.SUPN05.Trim(),
                        SNAM05 = d.SNAM05.Trim(),
                        SAD105 = d.SAD105.Trim(),
                        SAD205 = d.SAD205.Trim(),
                        SAD305 = d.SAD305.Trim(),
                        SAD405 = d.SAD405.Trim(),
                        SAD505 = d.SAD505.Trim(),
                        PCD105 = d.PCD105.Trim(),
                        PCD205 = d.PCD205.Trim(),
                        PHON05 = d.PHON05.Trim(),
                        SPIN05 = d.SPIN05.Trim(),
                        CURN05 = d.CURN05.Trim(),
                        ALPH05 = d.ALPH05.Trim(),
                        FAXN05 = d.FAXN05.Trim(),
                        LGCD05 = d.LGCD05.Trim(),
                        COMP05 = d.COMP05,
                        COMC05 = d.COMC05,
                        TOCD05 = d.TOCD05.Trim(),
                        TTTP05 = d.TTTP05.Trim(),
                        TTPR05 = d.TTPR05,
                        TTTD05 = d.TTTD05,
                        BAN105 = d.BAN105.Trim(),
                        BAN205 = d.BAN205.Trim(),
                        COCD05 = d.COCD05.Trim(),
                        VTID05 = d.VTID05.Trim(),
                        WURL05 = d.WURL05.Trim(),
                        IBAN05 = d.IBAN05.Trim(),
                        CisloNazev = d.SUPN05.Trim() + " | " + d.SNAM05.Trim() + " | " + d.CURN05.Trim(),
                        NazevCislo = d.SNAM05.Trim() + " | " + d.SUPN05.Trim() + " | " + d.CURN05.Trim()
                        //,PozadavkyId = d.Pozadavky.Select(t => t.ID)
                    })
                    .OrderBy(a => a.SNAM05)
                    .ToList();
            }

        }

        public static List<DodavateleS21> GetS21DodavateleOrderedByNumb()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                return db.DodavateleS21
                    .Select(d => new DodavateleS21()
                    {
                        Id = d.ID,
                        SUPN05 = d.SUPN05.Trim(),
                        SNAM05 = d.SNAM05.Trim(),
                        SAD105 = d.SAD105.Trim(),
                        SAD205 = d.SAD205.Trim(),
                        SAD305 = d.SAD305.Trim(),
                        SAD405 = d.SAD405.Trim(),
                        SAD505 = d.SAD505.Trim(),
                        PCD105 = d.PCD105.Trim(),
                        PCD205 = d.PCD205.Trim(),
                        PHON05 = d.PHON05.Trim(),
                        SPIN05 = d.SPIN05.Trim(),
                        CURN05 = d.CURN05.Trim(),
                        ALPH05 = d.ALPH05.Trim(),
                        FAXN05 = d.FAXN05.Trim(),
                        LGCD05 = d.LGCD05.Trim(),
                        COMP05 = d.COMP05,
                        COMC05 = d.COMC05,
                        TOCD05 = d.TOCD05.Trim(),
                        TTTP05 = d.TTTP05.Trim(),
                        TTPR05 = d.TTPR05,
                        TTTD05 = d.TTTD05,
                        BAN105 = d.BAN105.Trim(),
                        BAN205 = d.BAN205.Trim(),
                        COCD05 = d.COCD05.Trim(),
                        VTID05 = d.VTID05.Trim(),
                        WURL05 = d.WURL05.Trim(),
                        IBAN05 = d.IBAN05.Trim(),
                        CisloNazev = d.SUPN05.Trim() + " | " + d.SNAM05.Trim() + " | " + d.CURN05.Trim(),
                        NazevCislo = d.SNAM05.Trim() + " | " + d.SUPN05.Trim() + " | " + d.CURN05.Trim()
                        //,PozadavkyId = d.Pozadavky.Select(t => t.ID)
                    })
                    .OrderBy(a => a.SUPN05)
                    .ToList();
            }

        }

        public static List<DodavateleDTO> GetDodavateleOrderedByName2()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                var dodavatele = db.Database.SqlQuery<DodavateleDTO>(
                    "SELECT min(Id) as Id, PozadavekId, ItemId, OdpOsobaId, SUPN05, SNAM05, SAD105, SAD205, SAD305, SAD405,"+
                    "SAD505, PSC, PHON05, SPIN05, CURN05, ALPH05, FAXN05, LGCD05, COMP05, COMC05,"+
                    "TOCD05, TTTP05, TTPR05, TTTD05, BAN105, BAN205, COCD05, VTID05, WURL05, IBAN05,"+
                    "TPAC1A, CNTN1A, CTNU1A, CNJT1A, CRNM1A, PFNB1A, OFNB1A, MBNB1A, EMIL1A," +
                    "(ISNULL(SNAM05,'') + ' | ' + ISNULL(SUPN05,'') + ' | ' + ISNULL(CURN05,'')) AS NazevCislo" + 
                    " FROM POZ_OBJ.dbo.Dodavatele" +
                    " Group by PozadavekId, ItemId, OdpOsobaId, SUPN05, SNAM05, SAD105, SAD205, SAD305, SAD405, SAD505, PSC, PHON05, SPIN05, CURN05, ALPH05, FAXN05, LGCD05, COMP05, COMC05," +
                    "TOCD05, TTTP05, TTPR05, TTTD05, BAN105, BAN205, COCD05, VTID05, WURL05, IBAN05, TPAC1A, CNTN1A, CTNU1A, CNJT1A, CRNM1A, PFNB1A, OFNB1A, MBNB1A, EMIL1A" +
                    " ORDER BY NazevCislo"
                    ).ToList();

                //var query = db.Dodavatele
                //   .Select(d => new DodavateleDTO()
                //   {
                //       Id = d.Id,
                //       SUPN05 = d.SUPN05,
                //       SNAM05 = d.SNAM05,
                //       SAD105 = d.SAD105,
                //       SAD205 = d.SAD205,
                //       SAD305 = d.SAD305,
                //       SAD405 = d.SAD405,
                //       SAD505 = d.SAD505,
                //       PSC = d.PSC,
                //       PHON05 = d.PHON05,
                //       SPIN05 = d.SPIN05,
                //       CURN05 = d.CURN05,
                //       ALPH05 = d.ALPH05,
                //       FAXN05 = d.FAXN05,
                //       LGCD05 = d.LGCD05,
                //       COMP05 = d.COMP05,
                //       COMC05 = d.COMC05,
                //       TOCD05 = d.TOCD05,
                //       TTTP05 = d.TTTP05,
                //       TTPR05 = d.TTPR05,
                //       TTTD05 = d.TTTD05,
                //       BAN105 = d.BAN105,
                //       BAN205 = d.BAN205,
                //       COCD05 = d.COCD05,
                //       VTID05 = d.VTID05,
                //       WURL05 = d.WURL05,
                //       IBAN05 = d.IBAN05,
                //       CisloNazev = d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05,
                //       NazevCislo = d.SNAM05 + " | " + d.SUPN05 + " | " + d.CURN05
                //   })         
                //    .OrderBy(a => a.SNAM05).
                //    AsEnumerable().Distinct()
                //    .ToList();

                return dodavatele;

            }
        }

        public static List<DodavateleDTO> GetDodavateleOrderedByName()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {


                var query = db.Dodavatele
                   .Select(d => new DodavateleDTO()
                   {
                       Id = d.Id,
                       SUPN05 = d.SUPN05,
                       SNAM05 = d.SNAM05,
                       SAD105 = d.SAD105,
                       SAD205 = d.SAD205,
                       SAD305 = d.SAD305,
                       SAD405 = d.SAD405,
                       SAD505 = d.SAD505,
                       PSC = d.PSC,
                       PHON05 = d.PHON05,
                       SPIN05 = d.SPIN05,
                       CURN05 = d.CURN05,
                       ALPH05 = d.ALPH05,
                       FAXN05 = d.FAXN05,
                       LGCD05 = d.LGCD05,
                       COMP05 = d.COMP05,
                       COMC05 = d.COMC05,
                       TOCD05 = d.TOCD05,
                       TTTP05 = d.TTTP05,
                       TTPR05 = d.TTPR05,
                       TTTD05 = d.TTTD05,
                       BAN105 = d.BAN105,
                       BAN205 = d.BAN205,
                       COCD05 = d.COCD05,
                       VTID05 = d.VTID05,
                       WURL05 = d.WURL05,
                       IBAN05 = d.IBAN05,
                       CisloNazev = d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05,
                       NazevCislo = d.SNAM05 + " | " + d.SUPN05 + " | " + d.CURN05
                   })
                    .OrderBy(a => a.SNAM05).
                    AsEnumerable().Distinct()
                    .ToList();

                return query;

            }
        }

        public static List<DodavateleDTO> GetDodavateleOrderedByNumb()
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                return db.Dodavatele
                   .Select(d => new DodavateleDTO()
                   {
                       Id = d.Id,
                       SUPN05 = d.SUPN05,
                       SNAM05 = d.SNAM05,
                       SAD105 = d.SAD105,
                       SAD205 = d.SAD205,
                       SAD305 = d.SAD305,
                       SAD405 = d.SAD405,
                       SAD505 = d.SAD505,
                       PSC = d.PSC,
                       PHON05 = d.PHON05,
                       SPIN05 = d.SPIN05,
                       CURN05 = d.CURN05,
                       ALPH05 = d.ALPH05,
                       FAXN05 = d.FAXN05,
                       LGCD05 = d.LGCD05,
                       COMP05 = d.COMP05,
                       COMC05 = d.COMC05,
                       TOCD05 = d.TOCD05,
                       TTTP05 = d.TTTP05,
                       TTPR05 = d.TTPR05,
                       TTTD05 = d.TTTD05,
                       BAN105 = d.BAN105,
                       BAN205 = d.BAN205,
                       COCD05 = d.COCD05,
                       VTID05 = d.VTID05,
                       WURL05 = d.WURL05,
                       IBAN05 = d.IBAN05,
                       CisloNazev = d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05,
                       NazevCislo = d.SNAM05 + " | " + d.SUPN05 + " | " + d.CURN05
                   })
                    .Distinct()
                    .OrderBy(a => a.SUPN05)
                    .ToList();

            }
        }

        public static void FillGridViewS21Dodavatele(GridViewDataSet<DodavateleS21> dataSet)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from d in db.DodavateleS21
                             select new DodavateleS21()
                             {
                                 Id = d.ID,
                                 SUPN05 = d.SUPN05.Trim(),
                                 SNAM05 = d.SNAM05.Trim(),
                                 SAD105 = d.SAD105.Trim(),
                                 SAD205 = d.SAD205.Trim(),
                                 SAD305 = d.SAD305.Trim(),
                                 SAD405 = d.SAD405.Trim(),
                                 SAD505 = d.SAD505.Trim(),
                                 PCD105 = d.PCD105.Trim(),
                                 PCD205 = d.PCD205.Trim(),
                                 PHON05 = d.PHON05.Trim(),
                                 SPIN05 = d.SPIN05.Trim(),
                                 CURN05 = d.CURN05.Trim(),
                                 ALPH05 = d.ALPH05.Trim(),
                                 FAXN05 = d.FAXN05.Trim(),
                                 LGCD05 = d.LGCD05.Trim(),
                                 COMP05 = d.COMP05,
                                 COMC05 = d.COMC05,
                                 TOCD05 = d.TOCD05.Trim(),
                                 TTTP05 = d.TTTP05.Trim(),
                                 TTPR05 = d.TTPR05,
                                 TTTD05 = d.TTTD05,
                                 BAN105 = d.BAN105.Trim(),
                                 BAN205 = d.BAN205.Trim(),
                                 COCD05 = d.COCD05.Trim(),
                                 VTID05 = d.VTID05.Trim(),
                                 WURL05 = d.WURL05.Trim(),
                                 IBAN05 = d.IBAN05.Trim(),
                                 CisloNazev = d.SUPN05.Trim() + " | " + d.SNAM05.Trim()

                             });

                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                dataSet.LoadFromQueryable(query);
            }
        }

        public static DodavateleS21 GetS21DodavatelById(int DodID)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = (from d in db.DodavateleS21
                                     // where p.Id == id
                                 select new DodavateleS21()
                                 {
                                     Id = d.ID,
                                     SUPN05 = d.SUPN05.Trim(),
                                     SNAM05 = d.SNAM05.Trim(),
                                     SAD105 = d.SAD105.Trim(),
                                     SAD205 = d.SAD205.Trim(),
                                     SAD305 = d.SAD305.Trim(),
                                     SAD405 = d.SAD405.Trim(),
                                     SAD505 = d.SAD505.Trim(),
                                     PCD105 = d.PCD105.Trim(),
                                     PCD205 = d.PCD205.Trim(),
                                     PHON05 = d.PHON05.Trim(),
                                     SPIN05 = d.SPIN05.Trim(),
                                     CURN05 = d.CURN05.Trim(),
                                     ALPH05 = d.ALPH05.Trim(),
                                     FAXN05 = d.FAXN05.Trim(),
                                     LGCD05 = d.LGCD05.Trim(),
                                     COMP05 = d.COMP05,
                                     COMC05 = d.COMC05,
                                     TOCD05 = d.TOCD05.Trim(),
                                     TTTP05 = d.TTTP05.Trim(),
                                     TTPR05 = d.TTPR05,
                                     TTTD05 = d.TTTD05,
                                     BAN105 = d.BAN105.Trim(),
                                     BAN205 = d.BAN205.Trim(),
                                     COCD05 = d.COCD05.Trim(),
                                     VTID05 = d.VTID05.Trim(),
                                     WURL05 = d.WURL05.Trim(),
                                     IBAN05 = d.IBAN05.Trim(),
                                     CisloNazev = d.SUPN05.Trim() + " | " + d.SNAM05.Trim()
                                 }).SingleOrDefault(a => a.Id == DodID);

                return dodavatel;

            }
        }

        public static DodavateleS21 GetS21DodavatelBySUPN05(string supn05)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = (from d in db.DodavateleS21
                                  where d.SUPN05 == supn05
                                 select new DodavateleS21()
                                 {
                                     Id = d.ID,
                                     SUPN05 = d.SUPN05.Trim(),
                                     SNAM05 = d.SNAM05.Trim(),
                                     SAD105 = d.SAD105.Trim(),
                                     SAD205 = d.SAD205.Trim(),
                                     SAD305 = d.SAD305.Trim(),
                                     SAD405 = d.SAD405.Trim(),
                                     SAD505 = d.SAD505.Trim(),
                                     PCD105 = d.PCD105.Trim(),
                                     PCD205 = d.PCD205.Trim(),
                                     PHON05 = d.PHON05.Trim(),
                                     SPIN05 = d.SPIN05.Trim(),
                                     CURN05 = d.CURN05.Trim(),
                                     ALPH05 = d.ALPH05.Trim(),
                                     FAXN05 = d.FAXN05.Trim(),
                                     LGCD05 = d.LGCD05.Trim(),
                                     COMP05 = d.COMP05,
                                     COMC05 = d.COMC05,
                                     TOCD05 = d.TOCD05.Trim(),
                                     TTTP05 = d.TTTP05.Trim(),
                                     TTPR05 = d.TTPR05,
                                     TTTD05 = d.TTTD05,
                                     BAN105 = d.BAN105.Trim(),
                                     BAN205 = d.BAN205.Trim(),
                                     COCD05 = d.COCD05.Trim(),
                                     VTID05 = d.VTID05.Trim(),
                                     WURL05 = d.WURL05.Trim(),
                                     IBAN05 = d.IBAN05.Trim(),
                                     CisloNazev = d.SUPN05.Trim() + " | " + d.SNAM05.Trim()
                                 }).SingleOrDefault();

                return dodavatel;

            }
        }

        public static DodavateleS21 GetDodavatelByPozadavekId(int PozID)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = (from p in db.Pozadavky
                                 join d in db.Dodavatele on p.DodavatelID equals d.Id
                                 where p.ID == PozID
                                 select new DodavateleS21()
                                 {
                                     Id = d.Id,
                                     SUPN05 = d.SUPN05,
                                     SNAM05 = d.SNAM05,
                                     SAD105 = d.SAD105,
                                     SAD205 = d.SAD205,
                                     SAD305 = d.SAD305,
                                     SAD405 = d.SAD405,
                                     SAD505 = d.SAD505,
                                     PHON05 = d.PHON05,
                                     SPIN05 = d.SPIN05,
                                     CURN05 = d.CURN05,
                                     PSC = d.PSC,
                                     ALPH05 = d.ALPH05,
                                     FAXN05 = d.FAXN05,
                                     LGCD05 = d.LGCD05,
                                     COMP05 = d.COMP05,
                                     COMC05 = d.COMC05,
                                     TOCD05 = d.TOCD05,
                                     TTTP05 = d.TTTP05,
                                     TTPR05 = d.TTPR05,
                                     TTTD05 = d.TTTD05,
                                     BAN105 = d.BAN105,
                                     BAN205 = d.BAN205,
                                     COCD05 = d.COCD05,
                                     VTID05 = d.VTID05,
                                     WURL05 = d.WURL05,
                                     IBAN05 = d.IBAN05,

                                     TPAC1A = d.TPAC1A,
                                     CNTN1A = d.CNTN1A,
                                     CTNU1A = d.CTNU1A,
                                     CNJT1A = d.CNJT1A,
                                     CRNM1A = d.CRNM1A,
                                     PFNB1A = d.PFNB1A,
                                     OFNB1A = d.OFNB1A,
                                     MBNB1A = d.MBNB1A,
                                     EMIL1A = d.EMIL1A,
                                     GTX11A = d.GTX11A,

                                     CisloNazev = d.SUPN05 + " | " + d.SNAM05,
                                     NazevCislo = d.SNAM05 + " | " + d.SUPN05
                                 }).FirstOrDefault();

                return dodavatel;

            }
        }


        // vrati dodavatele ve formatu S21
        public static DodavateleS21 GetDodavatelByIdAsS21(int DodID)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = (from d in db.Dodavatele
                                     // where p.Id == id
                                 select new DodavateleS21()
                                 {
                                     Id = d.Id,
                                     SUPN05 = d.SUPN05.Trim(),
                                     SNAM05 = d.SNAM05.Trim(),
                                     SAD105 = d.SAD105.Trim(),
                                     SAD205 = d.SAD205.Trim(),
                                     SAD305 = d.SAD305.Trim(),
                                     SAD405 = d.SAD405.Trim(),
                                     SAD505 = d.SAD505.Trim(),
                                     PHON05 = d.PHON05.Trim(),
                                     SPIN05 = d.SPIN05.Trim(),
                                     CURN05 = d.CURN05.Trim(),
                                     PSC = d.PSC,
                                     ALPH05 = d.ALPH05.Trim(),
                                     FAXN05 = d.FAXN05.Trim(),
                                     LGCD05 = d.LGCD05.Trim(),
                                     COMP05 = d.COMP05,
                                     COMC05 = d.COMC05,
                                     TOCD05 = d.TOCD05.Trim(),
                                     TTTP05 = d.TTTP05.Trim(),
                                     TTPR05 = d.TTPR05,
                                     TTTD05 = d.TTTD05,
                                     BAN105 = d.BAN105.Trim(),
                                     BAN205 = d.BAN205.Trim(),
                                     COCD05 = d.COCD05.Trim(),
                                     VTID05 = d.VTID05.Trim(),
                                     WURL05 = d.WURL05.Trim(),
                                     IBAN05 = d.IBAN05.Trim(),

                                     TPAC1A = d.TPAC1A.Trim(),
                                     CNTN1A = d.CNTN1A.Trim() ?? "",
                                     CTNU1A = d.CTNU1A,
                                     CNJT1A = d.CNJT1A.Trim(),
                                     CRNM1A = d.CRNM1A.Trim(),
                                     PFNB1A = d.PFNB1A.Trim(),
                                     OFNB1A = d.OFNB1A.Trim(),
                                     MBNB1A = d.MBNB1A.Trim(),
                                     EMIL1A = d.EMIL1A.Trim() ?? "",
                                     GTX11A = d.GTX11A.Trim() ?? "",

                                     CisloNazev = d.SUPN05.Trim() + " | " + d.SNAM05.Trim(),
                                     NazevCislo = d.SNAM05.Trim() + " | " + d.SUPN05.Trim()
                                 }).SingleOrDefault(a => a.Id == DodID);

                return dodavatel;

            }
        }

        public static int DodavatelInsert(DodavateleS21 d, OdpOsoby o)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = new Dodavatele()
                {
    
                    PozadavekId = d.PozadavekId,
                    ItemId = d.ItemId,
                    OdpOsobaId = d.OdpOsobaId,
                    SUPN05 = d.SUPN05 != null ? d.SUPN05.Trim() : "",
                    SNAM05 = d.SNAM05 != null ? d.SNAM05.Trim() : "",
                    SAD105 = d.SAD105 != null ? d.SAD105.Trim() : "",
                    SAD205 = d.SAD205 != null ? d.SAD205.Trim() : "",
                    SAD305 = d.SAD305 != null ? d.SAD305.Trim() : "",
                    SAD405 = d.SAD405 != null ? d.SAD405.Trim() : "",
                    SAD505 = d.SAD505 != null ? d.SAD505.Trim() : "",
                    PHON05 = d.PHON05 != null ? d.PHON05.Trim() : "",
                    SPIN05 = d.SPIN05 != null ? d.SPIN05.Trim() : "",
                    CURN05 = d.CURN05 != null ? d.CURN05.Trim() : "",
                    PSC    = d.PSC    != null ? d.PSC.Trim() : "",
                    ALPH05 = d.ALPH05 != null ? d.ALPH05.Trim() : "",
                    FAXN05 = d.FAXN05 != null ? d.FAXN05.Trim() : "",
                    LGCD05 = d.LGCD05 != null ? d.LGCD05.Trim() : "",
                    COMP05 = d.COMP05,          
                    COMC05 = d.COMC05,          
                    TOCD05 = d.TOCD05 != null ? d.TOCD05.Trim() : "",
                    TTTP05 = d.TTTP05 != null ? d.TTTP05.Trim() : "",
                    TTPR05 = d.TTPR05,
                    TTTD05 = d.TTTD05,
                    BAN105 = d.BAN105 != null ? d.BAN105.Trim() : "",
                    BAN205 = d.BAN205 != null ? d.BAN205.Trim() : "",
                    COCD05 = d.COCD05 != null ? d.COCD05.Trim() : "",
                    VTID05 = d.VTID05 != null ? d.VTID05.Trim() : "",
                    WURL05 = d.WURL05 != null ? d.WURL05.Trim() : "",
                    IBAN05 = d.IBAN05 != null? d.IBAN05.Trim() : ""
                };

                db.Dodavatele.Add(dodavatel);
                db.SaveChanges();

                if (o != null)
                {
                    dodavatel.TPAC1A = o.TPAC1A != null ? o.TPAC1A.Trim() : "";
                    dodavatel.CNTN1A = o.CNTN1A != null ? o.CNTN1A.Trim() : "";
                    dodavatel.CTNU1A = o.CTNU1A;
                    dodavatel.CNJT1A = o.CNJT1A != null ? o.CNJT1A.Trim() : "";
                    dodavatel.CRNM1A = o.CRNM1A != null ? o.CRNM1A.Trim() : "";
                    dodavatel.PFNB1A = o.PFNB1A != null ? o.PFNB1A.Trim() : "";
                    dodavatel.OFNB1A = o.OFNB1A != null ? o.OFNB1A.Trim() : "";
                    dodavatel.MBNB1A = o.MBNB1A != null ? o.MBNB1A.Trim() : "";
                    dodavatel.EMIL1A = o.EMIL1A != null ? o.EMIL1A.Trim() : "";
                    dodavatel.GTX11A = o.GTX11A != null ? o.GTX11A.Trim() : "";

                    db.SaveChanges();
                }

                return dodavatel.Id;
            }
        }


        /// <summary>
        /// Ulozi dodavatele ve formatu DodavateleDTO jako noveho
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int DodavatelInsert(DodavateleDTO d)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = new Dodavatele()
                {
                    Id = d.Id,
                    SUPN05 = d.SUPN05,
                    SNAM05 = d.SNAM05,
                    SAD105 = d.SAD105,
                    SAD205 = d.SAD205,
                    SAD305 = d.SAD305,
                    SAD405 = d.SAD405,
                    SAD505 = d.SAD505,
                    PSC = d.PSC,
                    PHON05 = d.PHON05,
                    SPIN05 = d.SPIN05,
                    CURN05 = d.CURN05,
                    ALPH05 = d.ALPH05,
                    FAXN05 = d.FAXN05,
                    LGCD05 = d.LGCD05,
                    COMP05 = d.COMP05,
                    COMC05 = d.COMC05,
                    TOCD05 = d.TOCD05,
                    TTTP05 = d.TTTP05,
                    TTPR05 = d.TTPR05,
                    TTTD05 = d.TTTD05,
                    BAN105 = d.BAN105,
                    BAN205 = d.BAN205,
                    COCD05 = d.COCD05,
                    VTID05 = d.VTID05,
                    WURL05 = d.WURL05,
                    IBAN05 = d.IBAN05,

                    TPAC1A = d.TPAC1A,
                    CNTN1A = d.CNTN1A,
                    CTNU1A = d.CTNU1A,
                    CNJT1A = d.CNJT1A,
                    CRNM1A = d.CRNM1A,
                    PFNB1A = d.PFNB1A,
                    OFNB1A = d.OFNB1A,
                    MBNB1A = d.MBNB1A,
                    EMIL1A = d.EMIL1A,
                    GTX11A = d.GTX11A,
                };

                db.Dodavatele.Add(dodavatel);
                db.SaveChanges();
                
                return dodavatel.Id;
            }
        }


        public static void DodavatelSave(int id, DodavateleS21 d, OdpOsoby o)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                var dodav = db.Dodavatele.Find(id);

                dodav.PozadavekId = d.PozadavekId;
                dodav.ItemId = d.ItemId;
                dodav.OdpOsobaId = d.OdpOsobaId;
                dodav.SUPN05 = d.SUPN05 != null ? d.SUPN05.Trim() : "";
                dodav.SNAM05 = d.SNAM05 != null ? d.SNAM05.Trim() : "";
                dodav.SAD105 = d.SAD105 != null ? d.SAD105.Trim() : "";
                dodav.SAD205 = d.SAD205 != null ? d.SAD205.Trim() : "";
                dodav.SAD305 = d.SAD305 != null ? d.SAD305.Trim() : "";
                dodav.SAD405 = d.SAD405 != null ? d.SAD405.Trim() : "";
                dodav.SAD505 = d.SAD505 != null ? d.SAD505.Trim() : "";
                dodav.PHON05 = d.PHON05 != null ? d.PHON05.Trim() : "";
                dodav.SPIN05 = d.SPIN05 != null ? d.SPIN05.Trim() : "";
                dodav.CURN05 = d.CURN05 != null ? d.CURN05.Trim() : "";
                dodav.PSC = d.PSC != null ? d.PSC.Trim() : "";
                dodav.ALPH05 = d.ALPH05 != null ? d.ALPH05.Trim() : "";
                dodav.FAXN05 = d.FAXN05 != null ? d.FAXN05.Trim() : "";
                dodav.LGCD05 = d.LGCD05 != null ? d.LGCD05.Trim() : "";
                dodav.COMP05 = d.COMP05;
                dodav.COMC05 = d.COMC05;
                dodav.TOCD05 = d.TOCD05 != null ? d.TOCD05.Trim() : "";
                dodav.TTTP05 = d.TTTP05 != null ? d.TTTP05.Trim() : "";
                dodav.TTPR05 = d.TTPR05;
                dodav.TTTD05 = d.TTTD05;
                dodav.BAN105 = d.BAN105 != null ? d.BAN105.Trim() : "";
                dodav.BAN205 = d.BAN205 != null ? d.BAN205.Trim() : "";
                dodav.COCD05 = d.COCD05 != null ? d.COCD05.Trim() : "";
                dodav.VTID05 = d.VTID05 != null ? d.VTID05.Trim() : "";
                dodav.WURL05 = d.WURL05 != null ? d.WURL05.Trim() : "";
                dodav.IBAN05 = d.IBAN05 != null ? d.IBAN05.Trim() : "";

                db.SaveChanges();

                if (o != null)
                {                    
                    dodav.TPAC1A = o.TPAC1A != null ? o.TPAC1A.Trim() : "";
                    dodav.CNTN1A = o.CNTN1A != null ? o.CNTN1A.Trim() : "";
                    dodav.CTNU1A = o.CTNU1A;
                    dodav.CNJT1A = o.CNJT1A != null ? o.CNJT1A.Trim() : "";
                    dodav.CRNM1A = o.CRNM1A != null ? o.CRNM1A.Trim() : "";
                    dodav.PFNB1A = o.PFNB1A != null ? o.PFNB1A.Trim() : "";
                    dodav.OFNB1A = o.OFNB1A != null ? o.OFNB1A.Trim() : "";
                    dodav.MBNB1A = o.MBNB1A != null ? o.MBNB1A.Trim() : "";
                    dodav.EMIL1A = o.EMIL1A != null ? o.EMIL1A.Trim() : "";
                    dodav.GTX11A = o.GTX11A != null ? o.GTX11A.Trim() : "";

                    db.SaveChanges();
                }
                else
                {
                    dodav.OdpOsobaId = 0;
                    dodav.TPAC1A = "";
                    dodav.CNTN1A = "";
                    dodav.CTNU1A = 0;
                    dodav.CNJT1A = "";
                    dodav.CRNM1A = "";
                    dodav.PFNB1A = "";
                    dodav.OFNB1A = "";
                    dodav.MBNB1A = "";
                    dodav.EMIL1A = "";
                    dodav.GTX11A = "";

                    db.SaveChanges();
                }
            }
        }

        public static void DeleteDodavatel(int id)
        {
            //using (var db = new PozadavkyContext(DtbConxString))
            //{
            //    var dodavatel = db.Dodavatele.Find(id);
            //    db.Dodavatele.Remove(dodavatel);
            //    db.SaveChanges();
            //}
        }

        public static OdpOsoby GetOdpOsobaByDodavatelId(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var osoba = (from o in db.OdpOsoby
                             join d in db.DodavateleS21 on o.TPAC1A equals d.SUPN05
                             where d.ID == id & d.PYCT05 == o.CTNU1A
                             select o).FirstOrDefault();

                if (osoba != null)
                {
                    osoba.CNTN1A = osoba.CNTN1A.Trim() ?? "";
                    osoba.GTX11A = osoba.GTX11A.Trim() ?? "";

                }

                return osoba;
            }
        }

        public static DodavateleDTO GetDodavatelById(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var dodavatel = (from d in db.Dodavatele  
                                 select new DodavateleDTO()
                                 {
                                     Id = d.Id,
                                     SUPN05 = d.SUPN05,
                                     SNAM05 = d.SNAM05,
                                     SAD105 = d.SAD105,
                                     SAD205 = d.SAD205,
                                     SAD305 = d.SAD305,
                                     SAD405 = d.SAD405,
                                     SAD505 = d.SAD505,
                                     PSC = d.PSC,
                                     PHON05 = d.PHON05,
                                     SPIN05 = d.SPIN05,
                                     CURN05 = d.CURN05,
                                     ALPH05 = d.ALPH05,
                                     FAXN05 = d.FAXN05,
                                     LGCD05 = d.LGCD05,
                                     COMP05 = d.COMP05,
                                     COMC05 = d.COMC05,
                                     TOCD05 = d.TOCD05,
                                     TTTP05 = d.TTTP05,
                                     TTPR05 = d.TTPR05,
                                     TTTD05 = d.TTTD05,
                                     BAN105 = d.BAN105,
                                     BAN205 = d.BAN205,
                                     COCD05 = d.COCD05,
                                     VTID05 = d.VTID05,
                                     WURL05 = d.WURL05,
                                     IBAN05 = d.IBAN05,

                                     TPAC1A = d.TPAC1A,
                                     CNTN1A = d.CNTN1A,
                                     CTNU1A = d.CTNU1A,
                                     CNJT1A = d.CNJT1A,
                                     CRNM1A = d.CRNM1A,
                                     PFNB1A = d.PFNB1A,
                                     OFNB1A = d.OFNB1A,
                                     MBNB1A = d.MBNB1A,
                                     EMIL1A = d.EMIL1A,
                                     GTX11A = d.GTX11A,


                                     CisloNazev = d.SUPN05 + " | " + d.SNAM05
                                 }).SingleOrDefault(a => a.Id == id);

                return dodavatel;

            }
        }

        /// <summary>
        /// zkopiruje dodavatele podle ID do noveho a vrati nove ID
        /// </summary>
        /// <param name="dodavatelID"></param>
        /// <returns></returns>
        public static int DodavatelCopy(int dodavatelID)
        {
            var DodavatelZpozadavku = GetDodavatelById(dodavatelID);
            return DodavatelInsert(DodavatelZpozadavku);
        }

    }

}

