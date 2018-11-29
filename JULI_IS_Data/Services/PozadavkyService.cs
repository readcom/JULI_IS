using DotVVM.Framework.Controls;
using Pozadavky.Data;
using Pozadavky.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
//using System.Web.Security;
using DotVVM.Framework.ViewModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Pozadavky = Pozadavky.Data.Pozadavky;

namespace Pozadavky.Services
{
    public static class PozadavkyService
    {
        public static int LastPozId { get; set; } = 1;
        public static int LastS21DodId { get; set; } = 0;

        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { } 
        }

        //static PozadavkyService()
        //{
        //    DtbConxString = CookiesServices.GetCookieValue("DTB");
        //}

        public static List<PozadavekDTO> GetPozadavkyByName(string name = "")
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                if (name != "")
                {
                    var query = (from p in db.Pozadavky
                                 where p.Zalozil == name
                        select new PozadavekDTO()
                        {
                            ID = (int?) p.ID ?? 0,
                            Zalozil = p.Zalozil,
                            Datum = p.Datum,
                            Popis = p.Popis,
                            Mena = p.Mena,
                            CelkovaCena = p.CelkovaCena
                        }
                        );
                    return query.ToList();
                }
                else
                {
                    var query = (from p in db.Pozadavky
                                 select new PozadavekDTO()
                        {
                            ID = (int?)p.ID ?? 0,
                            Zalozil = p.Zalozil,
                            Datum = p.Datum,
                            Popis = p.Popis,
                            Mena = p.Mena,
                            CelkovaCena = p.CelkovaCena
                        }
                        );
                    return query.ToList();

                }
            }
        }

        public static string PozadavekZamitnout(PozadavekDTO data, string duvod = "")
        {
            // pridat priznak PodpisLevel = data.PodpisLevel - 1

            // zrusit priznak Level1Schvaleno, Level1SchvalenoDne,Level2Odeslano, Level2SchvalovatelID

            // odeslat mail zakladateli pozadavku o neschvaleni
            // ve druhem levelu odeslat mail schvalovateli levelu data.PodpisLevel - 1 o neschvaleni, 

            string vysledek = "";

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var pozadavek = db.Pozadavky.Find(data.ID);                
                pozadavek.ZamitnutoDne = DateTime.Now;
                pozadavek.Zamitnuto = true;
                pozadavek.DuvodZamitnutiID = data.DuvodZamitnutiID;
                pozadavek.DuvodZamitnutiText = duvod;
                pozadavek.Stav = "Zamínuto!";
                db.SaveChanges();

                UsersDTO user = UserServices.GetUsersByUserName(pozadavek.Zalozil).FirstOrDefault();
                if (user != null && user.OdesilatMaily == false)
                { }
                else
                {
                    if (data.PodpisLevel == 1)
                    {
                        //pozadavek.Level1Odeslano = false;
                        //pozadavek.Level1OdeslanoDne = null;
                        //pozadavek.PodpisLevel = 0;                    
                        //db.SaveChanges();

                        if (Constants.Test == true) pozadavek.Zalozil = "marek.novak";
                        vysledek = MailServices.SendMail(pozadavek.Zalozil + "@juli.cz" + (Constants.TestEmaily ? ";marek.novak@juli.cz" : ""),

                            $"Požadavek č. {pozadavek.FullPozadavekID} byl zamítnut ",

                            $"Váš požadavek č. {pozadavek.FullPozadavekID} " +                           
                            $"byl zamítnut uživatelem {UserServices.GetUsersByUserName(UserServices.GetActiveUser()).First().Jmeno}. <br />" +
                            $"Dne {pozadavek.ZamitnutoDne:d.M.yyyy} " +
                            $"v {pozadavek.ZamitnutoDne:HH:mm}" + " hodin" +
                            $"<br><br> Důvod: {duvod}"
                            );
                    }

                    if (data.PodpisLevel == 2)
                    {
                        //pozadavek.Level2Schvaleno = false;                    
                        //pozadavek.Level2Odeslano = false;
                        //pozadavek.Level2OdeslanoDne = null;
                        //pozadavek.Level2SchvalovatelID =
                        //    UserServices.GetUsersByUserName(UserServices.GetActiveUser()).First().ID;
                        //pozadavek.PodpisLevel = 1;
                        //db.SaveChanges();

                        if (Constants.Test == true) pozadavek.Zalozil = "marek.novak";
                        vysledek = MailServices.SendMail(
                            //komu
                            $"{pozadavek.Zalozil}@juli.cz" + (Constants.TestEmaily ? ";marek.novak@juli.cz" : ""),
                            // subject
                            $"Požadavek č. {pozadavek.FullPozadavekID} byl zamítnut.",
                            //mail
                            $"Váš požadavek č. {pozadavek.FullPozadavekID} " +
                            $"byl zamítnut uživatelem {UserServices.GetUsersByUserName(UserServices.GetActiveUser()).First().Jmeno}.<br>" +
                            $"dne {pozadavek.ZamitnutoDne:d.M.yyyy} " +
                            $"v {pozadavek.ZamitnutoDne:HH:mm}" + " hodin" +                         
                            $"<br><br> Důvod: {duvod}"
                            );

                    }

                    if (data.PodpisLevel == 3)
                    {
                        //pozadavek.Level3Odeslano = false;
                        //pozadavek.Level3OdeslanoDne = null;
                        //pozadavek.Level3Schvaleno = false;
                        //pozadavek.PodpisLevel = 2;
                        //db.SaveChanges();

                        if (Constants.Test == true) pozadavek.Zalozil = "marek.novak";
                        vysledek = MailServices.SendMail(
                            //komu
                            $"{pozadavek.Zalozil}@juli.cz" + (Constants.TestEmaily ? ";marek.novak@juli.cz" : ""),
                            // subject
                            $"Požadavek č. {pozadavek.FullPozadavekID} byl zamítnut objednavatelem.",
                            //mail
                            $"Váš požadavek č. {pozadavek.FullPozadavekID} " +
                            $"byl zamítnut uživatelem {UserServices.GetUsersByUserName(UserServices.GetActiveUser()).First().Jmeno}.<br>" +
                            $"dne {pozadavek.ZamitnutoDne:d.M.yyyy} " +
                            $"v {pozadavek.ZamitnutoDne:HH:mm}" + " hodin" +
                            $"<br><br> Důvod: {duvod}"
                            );
                    }
                }


                     
            }

            return vysledek;
        }

        public static string PozadavekSchvalit(PozadavekDTO data) //       int id, int data.PodpisLevel, int level1Id = 0)
        {
            // podpis level: 0 => novy pozadavek
            // podpis level: 1 => ceka na podpis vedoucim
            // podpis level: 2 => ceka na podpis reditel nebo control.
            // podpis level: 3 => vse podepsano, ceka na vytvoreni obj. a podpis nakupu
            // podpis level: 4 => vse podepsano, ceka na objednani


            // pridat priznak PodpisLevel = data.PodpisLevel
            // pridat priznak Level1Schvaleno, Level1SchvalenoDne,Level2Odeslano, Level2SchvalovatelID

            // odeslat mail zakladateli pozadavku o schvaleni
            // odeslat mail schvalovateli data.PodpisLevel se zadosti o schvaleni, ve druhem levelu to jde na oba, controling i reditel

            string vysledek = "";

            List < UsersDTO > schvalovatele = new List<UsersDTO>();

            string SchvalovateleEmail = "";

            // prvni odeslani na schvaleni
            if (data.PodpisLevel == 1)
            {
                SchvalovateleEmail = UserServices.GetUserById(data.SchvalovatelID).Email;
            }
            else
            {
                schvalovatele = UserServices.GetUserByLevel(data.PodpisLevel);
                foreach (var item in schvalovatele)
                {
                    if (item.OdesilatMaily == true)
                    SchvalovateleEmail += (item.Email + ";");
                }
            }

            if (Constants.TestEmaily && SchvalovateleEmail.Trim().LastOrDefault() == ';')
                SchvalovateleEmail += " marek.novak@juli.cz";
            else
            {
                if (Constants.TestEmaily && SchvalovateleEmail.Trim().LastOrDefault() != ';')
                SchvalovateleEmail += "; marek.novak@juli.cz";
            }

            if (Constants.Test) SchvalovateleEmail = "marek.novak@juli.cz";

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var pozadavek = db.Pozadavky.Find(data.ID);
                pozadavek.Zamitnuto = false;
                pozadavek.FullPozadavekID = data.FullPozadavekID;

                if (data.PodpisLevel == 1)
                {
                    pozadavek.Level1Odeslano = data.Level1Odeslano;
                    pozadavek.Level1OdeslanoDne = data.Level1OdeslanoDne;
                    pozadavek.PodpisLevel = data.PodpisLevel;
                    pozadavek.SchvalovatelID = data.SchvalovatelID;
                    pozadavek.Stav = "Odesláno na schválení";
                    db.SaveChanges();

                    UsersDTO user = UserServices.GetUsersByUserName(pozadavek.Zalozil).FirstOrDefault();
                    if (user != null && user.OdesilatMaily == false)
                    { }
                    else
                    { 
                        if (Constants.Test == true) pozadavek.Zalozil = "marek.novak";
                    vysledek = MailServices.SendMail(pozadavek.Zalozil + "@juli.cz" + (Constants.TestEmaily ? ";marek.novak@juli.cz" : ""),
                        $"Požadavek č. {pozadavek.FullPozadavekID} byl odeslán ke schválení.",
                        $"Váš požadavek č. {pozadavek.FullPozadavekID} byl odeslán ke schválení dne " +
                        $"{pozadavek.Level1OdeslanoDne:d.M.yyyy}" + " v " +
                        $"{pozadavek.Level1OdeslanoDne:HH:mm}" + " hodin"
                        );
                    }

                    if (UserServices.GetUserById(data.SchvalovatelID).OdesilatMaily == true)
                    { 
                        vysledek += MailServices.SendMail(SchvalovateleEmail, $"Požadavek č. {pozadavek.FullPozadavekID} je připraven ke schválení",
                        $"požadavek č. {pozadavek.FullPozadavekID} od uživatele " +
                        $"{pozadavek.Zalozil} je připraven ke schválení.");
                        // + $"<br>  mail pro uživatele: {SchvalovateleEmail}");
                    }

                }

                // schvaleno vedoucim a odesilam mail reditel / controling o pozadavku
                if (data.PodpisLevel == 2)
                {
                    pozadavek.Level1Schvaleno = true;
                    pozadavek.Level1SchvalenoDne = DateTime.Now;
                    pozadavek.Level2Odeslano = true;
                    pozadavek.Level2OdeslanoDne = DateTime.Now;
                    pozadavek.PodpisLevel = data.PodpisLevel;
                    pozadavek.Level1SchvalovatelID =
                        UserServices.GetUsersByUserName(UserServices.GetActiveUser()).Where(u => u.Uroven == 1).SingleOrDefault().ID;

                    pozadavek.Stav = "Čeká na podpis ředitele / kontrolingu";
                    db.SaveChanges();

                    UsersDTO user = UserServices.GetUsersByUserName(pozadavek.Zalozil).FirstOrDefault();
                    if (user != null && user.OdesilatMaily == false)
                    {
                    }
                    else
                    {
                        if (Constants.Test == true) pozadavek.Zalozil = "marek.novak";
                        vysledek = MailServices.SendMail(pozadavek.Zalozil + "@juli.cz" + (Constants.TestEmaily ? ";marek.novak@juli.cz" : ""),
                            $"Požadavek č. {pozadavek.FullPozadavekID} byl schválen.",
                            $"Váš požadavek č. {pozadavek.FullPozadavekID} byl schválen dne " +
                            $"{pozadavek.Level1SchvalenoDne:d.M.yyyy}" + " v " +
                            $"{pozadavek.Level1SchvalenoDne:HH:mm}" + " hodin"
                            + "<br> uživatelem: " + UserServices.GetUserById(pozadavek.Level1SchvalovatelID).Jmeno
                            );
                    }

                    vysledek += MailServices.SendMail(SchvalovateleEmail, "požadavek připraven ke schválení",
                            $"požadavek č. {pozadavek.FullPozadavekID} od uživatele " +
                            $"{pozadavek.Zalozil} je připraven ke schválení.");

                  

                }


                // schvaleno reditel nebo controling, do objednavek
                if (data.PodpisLevel == 3)
                {
                    pozadavek.Level2Schvaleno = true;
                    pozadavek.Level2SchvalenoDne = DateTime.Now;
                    pozadavek.Level3Odeslano = true;
                    pozadavek.Level3OdeslanoDne = DateTime.Now;
                    pozadavek.Level2SchvalovatelID =
                        UserServices.GetUsersByUserName(UserServices.GetActiveUser()).Where(u => u.Uroven == 2).SingleOrDefault().ID;
                    pozadavek.PodpisLevel = data.PodpisLevel;
                    pozadavek.Stav = "V tvorbě objednávky";
                    db.SaveChanges();


                    UsersDTO user = UserServices.GetUsersByUserName(pozadavek.Zalozil).FirstOrDefault();
                    if (user != null && user.OdesilatMaily == false)
                    {
                    }
                    else
                    {
                        if (Constants.Test == true) pozadavek.Zalozil = "marek.novak";
                        vysledek = MailServices.SendMail(pozadavek.Zalozil + "@juli.cz" + (Constants.TestEmaily ? "marek.novak@juli.cz" : ""),
                            $"Požadavek č. {pozadavek.FullPozadavekID} byl finálně schválen.",
                            $"Váš požadavek č. {pozadavek.FullPozadavekID} byl schválen dne " +
                            $"{pozadavek.Level2SchvalenoDne:d.M.yyyy}" + " v " +
                            $"{pozadavek.Level2SchvalenoDne:HH:mm}" + " hodin" +
                            $"<br>uživatelem: {UserServices.GetUserById(pozadavek.Level2SchvalovatelID).Jmeno}"
                            );
                    }

                    vysledek += MailServices.SendMail(SchvalovateleEmail, "požadavek připraven k objednání",
                    $"požadavek č. {pozadavek.FullPozadavekID} od uživatele " +
                    $"{pozadavek.Zalozil} byl schválen a je připraven k objednání.");


                    // zkopirovat polozky ze schvaleneho pozadavku do nove tabulky ObjItems
                    ItemsService.CopyItemsToObjByPoz(data);
                }             
            }

            return vysledek;
        }

        /// <summary>
        /// vrati cislo pozadavku z fullPozadavekID
        /// </summary>
        /// <param name="fullPozadavekID"></param>
        /// <returns></returns>
        public static string GePozNumber(string fullPozadavekID)
        {
            string vysledek;
            if (string.IsNullOrEmpty(fullPozadavekID)) vysledek = "";
            else vysledek = fullPozadavekID.Substring(6, 4);

            return vysledek;
        }
  

        public static void DeletePozadavek(int id, bool podepsano = false)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var pozadavek = db.Pozadavky.Find(id);

                if (pozadavek.Level1Schvaleno)
                {
                    pozadavek.Smazano = true;
                    pozadavek.SmazalUzivatel = UserServices.GetActiveUser();
                    pozadavek.DatumSmazani = DateTime.Now;
                }
                else
                {
                    db.Pozadavky.Remove(pozadavek);
                }                
                db.SaveChanges();
            }

            ItemsService.DeleteItemsByPozadavekId(id, podepsano);
            FilesService.DeleteFileByPozadavekId(id, podepsano);
        }
     
        public static void FillGridViewPozadavkyToSignByUserOrLevel(GridViewDataSet<PozadavekDTO> dataSet, string name = "", List<int> activeUserLevels = null)
        {
            if (name != "")  // je zadany uzivatel
            {
                using (var db = new PozadavkyContext(DtbConxString))
                {
                    List<UsersDTO> Users = UserServices.GetUsersByUserName(name);
                    List<int> usersId = new List<int>();
                    
                    string ActiveUser = UserServices.GetActiveUser();

                    Users.ForEach(u => usersId.Add(u.ID));
                    

                    var query = (from p in db.Pozadavky
                                 from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                                 where p.Smazano == false & p.Zamitnuto == false & p.PodpisLevel > 0 & p.PodpisLevel < 3 & usersId.Contains(p.Level1SchvalovatelID.Value) // & p.Zalozil != ActiveUser
                                 select new PozadavekDTO()
                                 {
                                     ID = p.ID,
                                     FullPozadavekID = p.FullPozadavekID,
                                     Zalozil = p.Zalozil,
                                     Datum = p.Datum,
                                     Popis = p.Popis,
                                     Mena = p.Mena,
                                     CelkovaCena = p.CelkovaCena,
                                     PocetPolozek = p.PocetPolozek,
                                     PodpisLevel = p.PodpisLevel,
                                     Level1SchvalovatelID = p.Level1SchvalovatelID.Value,
                                     SchvalovatelID = p.SchvalovatelID,
                                     DodavatelName = d.SNAM05.Length > 1 ? d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05 : d.SNAM05,
                                     DuvodZamitnutiID = p.DuvodZamitnutiID,
                                     DuvodZamitnutiText = p.DuvodZamitnutiText ?? ""
                                 });

                    
                    // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                    dataSet.LoadFromQueryable(query);                                        
                    
                }
            }
            else if (activeUserLevels != null) // neni uzivatel ale level
            {
                using (var db = new PozadavkyContext(DtbConxString))
                {
                    var query = (from p in db.Pozadavky
                                 from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                                 where p.Smazano == false  & p.Zamitnuto == false & p.PodpisLevel < 3 & activeUserLevels.Contains(p.PodpisLevel)
                                 select new PozadavekDTO()
                                 {
                                     ID = p.ID,
                                     FullPozadavekID = p.FullPozadavekID,
                                     Zalozil = p.Zalozil,
                                     Datum = p.Datum,
                                     Popis = p.Popis,
                                     Mena = p.Mena,
                                     CelkovaCena = p.CelkovaCena,
                                     PocetPolozek = p.PocetPolozek,
                                     PodpisLevel = p.PodpisLevel,
                                     SchvalovatelID = p.SchvalovatelID,
                                     Level1SchvalovatelID = p.Level1SchvalovatelID.Value,
                                     DodavatelName = d.SNAM05.Length > 1 ? d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05 : d.SNAM05,
                                     DuvodZamitnutiID = p.DuvodZamitnutiID,
                                     DuvodZamitnutiText = p.DuvodZamitnutiText ?? ""
                                 });

                    // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                    dataSet.LoadFromQueryable(query);
                }
            }
        }

        public static void FillGridViewPozadavkyToSignByUsersLevels(GridViewDataSet<PozadavekDTO> dataSet, string username)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                List<UsersDTO> Users = UserServices.GetUsersByUserName(username);
                List<int> userlevels = UserServices.GetActiveUserLevels();

                List<int> usersId = new List<int>();

                Users.ForEach(u => usersId.Add(u.ID));

                var query = (from p in db.Pozadavky
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where p.Smazano == false & p.Zamitnuto == false
                             & 
                             (
                                 (p.PodpisLevel == 1 & usersId.Contains(p.SchvalovatelID))
                                | (p.PodpisLevel == 2 & userlevels.Contains(2) & !usersId.Contains(p.Level1SchvalovatelID ?? 0))                                
                             )
                             select new PozadavekDTO()
                             {
                                 ID = p.ID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = p.Zalozil,
                                 Datum = p.Datum,
                                 Popis = p.Popis,
                                 Mena = p.Mena,
                                 CelkovaCena = p.CelkovaCena,
                                 PocetPolozek = p.PocetPolozek,
                                 PodpisLevel = p.PodpisLevel,
                                 SchvalovatelID = p.SchvalovatelID,
                                 Level1SchvalovatelID = p.Level1SchvalovatelID.Value,
                                 DodavatelName = d.SNAM05.Length > 1 ? d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05 : d.SNAM05,
                                 DuvodZamitnutiID = p.DuvodZamitnutiID,
                                 DuvodZamitnutiText = p.DuvodZamitnutiText ?? ""
                             });


                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                dataSet.LoadFromQueryable(query);


            }
        }

        public static void FillGridViewPozadavkyToSignLevel2(GridViewDataSet<PozadavekDTO> dataSet)
        {

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = (from p in db.Pozadavky
                             from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                             where p.Smazano == false & p.Zamitnuto == false & p.PodpisLevel == 2 & UserServices.GetActiveUserLevels().Contains(2)
                             select new PozadavekDTO()
                             {
                                 ID = p.ID,
                                 FullPozadavekID = p.FullPozadavekID,
                                 Zalozil = p.Zalozil,
                                 Datum = p.Datum,
                                 Popis = p.Popis,
                                 Mena = p.Mena,
                                 CelkovaCena = p.CelkovaCena,
                                 PocetPolozek = p.PocetPolozek,
                                 PodpisLevel = p.PodpisLevel,
                                 DodavatelName = d.SNAM05.Length > 1 ? d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05 : d.SNAM05,
                                 DuvodZamitnutiID = p.DuvodZamitnutiID,
                                 DuvodZamitnutiText = p.DuvodZamitnutiText ?? ""
                             });


                // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                dataSet.LoadFromQueryable(query);

            }

        }

        public static void FillGridViewPozadavkyByUser(GridViewDataSet<PozadavekDTO> dataSet, string name = "")
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                if (name != "")  // je zadany uzivatel
                {
                    var query = (from p in db.Pozadavky
                                 from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                                 where p.Zalozil == name & p.Smazano == false
                                 select new PozadavekDTO()
                                 {
                                     ID = (int?)p.ID ?? 0,
                                     IDstr = p.ID.ToString(),
                                     FullPozadavekID =  p.FullPozadavekID,
                                     Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,       
                                     NabidkaCislo = p.NabidkaCislo,
                                     PocetPolozek = p.PocetPolozek,                              
                                     KST = p.KST,
                                     Datum = p.Datum,
                                     Popis = p.Popis,
                                     Mena = p.Mena,
                                     CelkovaCena = p.CelkovaCena,
                                     PodpisLevel = p.PodpisLevel,
                                     SchvalovatelID = p.SchvalovatelID,
                                     Level1Odeslano = p.Level1Odeslano,
                                     Level1Schvaleno = p.Level1Schvaleno,
                                     Level2Schvaleno = p.Level2Schvaleno,
                                     Level3Schvaleno = p.Level3Schvaleno,
                                     Zamitnuto = p.Zamitnuto,
                                     Objednano = p.Objednano,
                                     DodavatelName = d.SNAM05.Length > 1 ? d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05 : d.SNAM05,
                                     DuvodZamitnutiID = p.DuvodZamitnutiID,
                                     DuvodZamitnutiText = p.DuvodZamitnutiText ?? "",
                                     Stav = p.Stav
                                 });

                    // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                    dataSet.LoadFromQueryable(query);
                }
                else
                {
                    var query = (from p in db.Pozadavky
                                 from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                                 where p.Smazano == false && p.PodpisLevel != 0 && p.Neverejny == false
                                 select new PozadavekDTO()
                                 {
                                     ID = (int?)p.ID ?? 0,
                                     IDstr = p.ID.ToString(),
                                     FullPozadavekID = p.FullPozadavekID,
                                     Zalozil = (p.Zastoupeno == null) ? p.Zalozil : p.Zastoupeno,
                                     NabidkaCislo = p.NabidkaCislo,
                                     PocetPolozek = p.PocetPolozek,
                                     KST = p.KST,
                                     Datum = p.Datum,
                                     Popis = p.Popis,
                                     Mena = p.Mena,
                                     CelkovaCena = p.CelkovaCena,
                                     PodpisLevel = p.PodpisLevel,
                                     SchvalovatelID = p.SchvalovatelID,
                                     Level1Odeslano = p.Level1Odeslano,
                                     Level1Schvaleno = p.Level1Schvaleno,
                                     Level2Schvaleno = p.Level2Schvaleno,
                                     Level3Schvaleno = p.Level3Schvaleno,
                                     Zamitnuto = p.Zamitnuto,
                                     Objednano = p.Objednano,
                                     DodavatelName = d.SNAM05.Length > 1 ? d.SUPN05 + " | " + d.SNAM05 + " | " + d.CURN05 : d.SNAM05,
                                     DuvodZamitnutiID = p.DuvodZamitnutiID,
                                     DuvodZamitnutiText = p.DuvodZamitnutiText ?? "",
                                     Stav = p.Stav
                                 });

                    // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
                    dataSet.LoadFromQueryable(query);
                }
            }
        }                  

        public static int PozadavekInsert(PozadavekDTO pozadavekData)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                //var i = new Item
                //{
                //    Datum = DateTime.Now,
                //    Vlozil = activeUser,
                //};
                //db.Items.Add(i);
                //db.SaveChanges();

                //// o.ID - ID noveho zaznamu

                var p = new Data.Pozadavky
                {
                    Zalozil = pozadavekData.Zalozil,
                    Zastoupeno = pozadavekData.Zastoupeno,
                    Datum = pozadavekData.Datum,
                    Popis = pozadavekData.Popis,
                    NabidkaCislo = pozadavekData.NabidkaCislo,
                    CelkovaCena = pozadavekData.CelkovaCena,
                    Mena = pozadavekData.Mena,
                    ZpusobPlatbyId = pozadavekData.ZpusobPlatbyId,
                    ZpusobPlatbyText = pozadavekData.ZpusobPlatbyText,
                    DodavatelID = pozadavekData.DodavatelID,
                    DodavatelS21ID = pozadavekData.DodavatelS21ID,
                    KST = pozadavekData.KST,            
                    InvesticeNeplanovana = pozadavekData.InvesticeNeplanovana,
                    InvesticePlanovana = pozadavekData.InvesticePlanovana,
                    NakupOstatni = pozadavekData.NakupOstatni,
                    CisloInvestice = pozadavekData.CisloInvestice,
                    CisloKonta = pozadavekData.CisloKonta,
                    Poznamka = pozadavekData.Poznamka,
                    PodpisLevel = pozadavekData.PodpisLevel,
                    Level1Odeslano = pozadavekData.Level1Odeslano,
                    Level2Odeslano = pozadavekData.Level2Odeslano,
                    Level3Odeslano = pozadavekData.Level3Odeslano,
                    Level1Schvaleno = pozadavekData.Level1Schvaleno,
                    Level2Schvaleno = pozadavekData.Level2Schvaleno,
                    Level3Schvaleno = pozadavekData.Level3Schvaleno,
                    Level1SchvalovatelID = pozadavekData.Level1SchvalovatelID,
                    Level2SchvalovatelID = pozadavekData.Level2SchvalovatelID,
                    Level3SchvalovatelID = pozadavekData.Level3SchvalovatelID,
                    Level1SchvalenoDne = pozadavekData.Level1SchvalenoDne,
                    Level2SchvalenoDne = pozadavekData.Level2SchvalenoDne,
                    Level3SchvalenoDne = pozadavekData.Level3SchvalenoDne,
                    Smazano = false,
                    Neverejny = pozadavekData.Neverejny,
                    Stav = pozadavekData.Stav
                };

                db.Pozadavky.Add(p);
                db.SaveChanges();

                LastPozId = p.ID;
                return p.ID;
            }
        }

        public static PozadavekDTO GetPozadavekById(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var pozadavek = (from p in db.Pozadavky
                                 select new PozadavekDTO()
                                 {
                                     ID = (int?)p.ID ?? 0,
                                     Popis = p.Popis,
                                     Mena = p.Mena,
                                     ZpusobPlatbyId = p.ZpusobPlatbyId,
                                     ZpusobPlatbyText = p.ZpusobPlatbyText,
                                     KST = p.KST,
                                     CelkovaCena = p.CelkovaCena,
                                     PocetPolozek = p.PocetPolozek,
                                     NabidkaCislo = p.NabidkaCislo,
                                     //FullPozadavekID = p.Datum.Value.Year.ToString() + p.ID.ToString(),
                                     FullPozadavekID = p.FullPozadavekID,
                                     InvesticeNeplanovana = p.InvesticeNeplanovana,
                                     InvesticePlanovana = p.InvesticePlanovana,
                                     NakupOstatni = p.NakupOstatni,
                                     CisloInvestice = p.CisloInvestice,
                                     CisloKonta = p.CisloKonta,
                                     Poznamka = p.Poznamka,
                                     PodpisLevel = p.PodpisLevel,
                                     Level1Schvaleno = p.Level1Schvaleno,
                                     Level1Odeslano = p.Level1Odeslano,
                                     Level2Odeslano = p.Level1Odeslano,
                                     Level3Odeslano = p.Level1Odeslano,
                                     Level1OdeslanoDne = p.Level1OdeslanoDne,
                                     Level2OdeslanoDne = p.Level2OdeslanoDne,
                                     Level3OdeslanoDne = p.Level3OdeslanoDne,
                                     Level2Schvaleno = p.Level2Schvaleno,
                                     Level3Schvaleno = p.Level3Schvaleno,
                                     Level1SchvalovatelID = p.Level1SchvalovatelID ?? 0,
                                     Level2SchvalovatelID = p.Level2SchvalovatelID ?? 0,
                                     Level3SchvalovatelID = p.Level3SchvalovatelID ?? 0,
                                     Level1SchvalenoDne = p.Level1SchvalenoDne,
                                     Level2SchvalenoDne = p.Level2SchvalenoDne,
                                     Level3SchvalenoDne = p.Level3SchvalenoDne,
                                     DodavatelID = p.DodavatelID ?? 0,
                                     DodavatelS21ID = p.DodavatelS21ID,
                                     Zamitnuto = p.Zamitnuto,
                                     ZamitnutoDne = p.ZamitnutoDne,
                                     DuvodZamitnutiID = p.DuvodZamitnutiID,
                                     DuvodZamitnutiText = p.DuvodZamitnutiText,
                                     Zalozil = p.Zalozil,
                                     Zastoupeno = p.Zastoupeno,
                                     Datum = p.Datum,
                                     Neverejny = p.Neverejny,
                                     SchvalovatelID = p.SchvalovatelID,
                                     Stav = p.Stav
                                 }).SingleOrDefault(a => a.ID == id);

                return pozadavek;
            }
        }

        // zjednoduseni je AutoMapper

        public static void PozadavekSave(PozadavekDTO pData)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var p = db.Pozadavky.Find((pData.ID));

                p.Popis = pData.Popis;
                p.FullPozadavekID = pData.FullPozadavekID;
                p.Zastoupeno = pData.Zastoupeno;
                p.PocetPolozek = pData.PocetPolozek;
                p.NabidkaCislo = pData.NabidkaCislo;
                p.Mena = pData.Mena;
                p.ZpusobPlatbyId = pData.ZpusobPlatbyId;
                p.ZpusobPlatbyText = pData.ZpusobPlatbyText;
                p.CelkovaCena = pData.CelkovaCena;
                p.KST = pData.KST;
                p.InvesticeNeplanovana = pData.InvesticeNeplanovana;
                p.InvesticePlanovana = pData.InvesticePlanovana;
                p.NakupOstatni = pData.NakupOstatni;
                p.CisloInvestice = pData.CisloInvestice;
                p.CisloKonta = pData.CisloKonta;
                p.Poznamka = pData.Poznamka;
                p.DodavatelID = pData.DodavatelID;
                p.DodavatelS21ID = pData.DodavatelS21ID;
                p.PodpisLevel = pData.PodpisLevel;
                p.Level1Schvaleno = pData.Level1Schvaleno;
                p.Level2Schvaleno = pData.Level2Schvaleno;
                p.Level3Schvaleno = pData.Level3Schvaleno;
                p.Level1SchvalovatelID = pData.Level1SchvalovatelID;
                p.Level2SchvalovatelID = pData.Level2SchvalovatelID;
                p.Level3SchvalovatelID = pData.Level3SchvalovatelID;
                p.Level1SchvalenoDne = pData.Level1SchvalenoDne;
                p.Level2SchvalenoDne = pData.Level2SchvalenoDne;
                p.Level3SchvalenoDne = pData.Level3SchvalenoDne;
                p.Level1Odeslano = pData.Level1Odeslano;
                p.Level2Odeslano = pData.Level2Odeslano;
                p.Level3Odeslano = pData.Level3Odeslano;
                p.Level1OdeslanoDne = pData.Level1OdeslanoDne;
                p.Level2OdeslanoDne = pData.Level2OdeslanoDne;
                p.Level3OdeslanoDne = pData.Level3OdeslanoDne;
                p.DuvodZamitnutiID = pData.DuvodZamitnutiID;
                p.DuvodZamitnutiText = pData.DuvodZamitnutiText;
                p.Neverejny = pData.Neverejny;
                p.Stav = pData.Stav;
                db.SaveChanges();
            }

        }

        public static int GetPocetPolozekByPozadavekId(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var items = (from i in db.Items
                    where (i.PozadavekID == id) && (i.Smazano == false)
                    select i);

                return items.Count();
            }
        }

        /// <summary>
        /// Vrátí poslední číslo požadavku v daném roce, jinak vrací 0
        /// </summary>
        /// <param name="rok"></param>
        /// <returns></returns>
        public static int GetLastNumberOfYear(string rok)
        {



            string sqlquery = "select top 1 * from Pozadavky  " +
                              "where substring(FullPozadavekID, 2, 4) = {0} " +
                              "order by substring(FullPozadavekID, 7, 4) desc";

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var query = db.Database.SqlQuery<Data.Pozadavky>(sqlquery, rok);

                var item = query.FirstOrDefault();
                if (item != null)
                    return Int32.Parse(item.FullPozadavekID.Substring(6, 4));
                else return 0;
            }
        }

        public static string GetYearFromFullId(string fullId)
        {
            if (!string.IsNullOrEmpty(fullId))
                return fullId.Substring(1, 4);
            else return "";
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
        //            pozadavek.SmazalUzivatel = ActiveUser;
        //            pozadavek.SmazanoDne = DateTime.Now;
        //            db.SaveChanges();
        //        }    
        //    }
        //}

        //public static void FillGridViewPozadavkyByUserAndObjId(GridViewDataSet<PozadavekDTO> dataSet, int? itemId, string name = "")
        //{
        //    using (var db = new PozadavkyContext(DtbConxString))
        //    {

        //        if (name != "")
        //        {
        //            var query = (from o in db.Objednavky
        //                         join p in db.Pozadavky on o.ID equals p.ObjednavkaID
        //                         join d in db.Dodavatel on p.DodavatelID equals d.ID
        //                         where o.Zalozil == name & p.Smazano == false & p.ObjednavkaID == itemId
        //                         select new PozadavekDTO()
        //                         {
        //                             ID = (int?)p.Id ?? 0,
        //                             ObjednavkaID = (int?)o.ID ?? 0,
        //                             Zalozil = o.Zalozil,
        //                             Datum = o.Datum,
        //                             Popis = p.Popis,
        //                             Mena = p.Mena,
        //                             CelkovaCena = p.CelkovaCena,
        //                             DodavatelName = d.Nazev,
        //                             ObjednavkaPopis = o.CelkovyPopis,
        //                             Poptany3Firmy = p.Poptany3firmy,
        //                             Duvod = p.Duvod
        //                         });

        //            // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
        //            dataSet.LoadFromQueryable(query);
        //        }
        //        else
        //        {
        //            var query = (from o in db.Objednavky
        //                         join p in db.Pozadavky on o.ID equals p.ObjednavkaID
        //                         join d in db.Dodavatel on p.DodavatelID equals d.ID
        //                         where p.Smazano == false & p.ObjednavkaID == itemId
        //                         select new PozadavekDTO()
        //                         {
        //                             ID = (int?)p.Id ?? 0,
        //                             ObjednavkaID = (int?)o.ID ?? 0,
        //                             Zalozil = o.Zalozil,
        //                             Datum = o.Datum,
        //                             Popis = p.Popis,
        //                             Mena = p.Mena,
        //                             CelkovaCena = p.CelkovaCena,
        //                             DodavatelName = d.Nazev,
        //                             ObjednavkaPopis = o.CelkovyPopis,
        //                             Poptany3Firmy = p.Poptany3firmy,
        //                             Duvod = p.Duvod
        //                         });

        //            // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
        //            dataSet.LoadFromQueryable(query);
        //        }
        //    }
        //}
    }
}
