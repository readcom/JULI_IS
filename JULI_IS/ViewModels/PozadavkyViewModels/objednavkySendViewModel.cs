using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.Services;
using Pozadavky.DTO;
using DotVVM.Framework.Controls;
//using AutoMapper;
using Pozadavky.Data;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.PozadavkyViewModels
{
	public class objednavkySendViewModel : AppViewModel
    {
        public GridViewDataSet<ObjednavkaDTO> SeznamObjednavekGV { get; set; } = new GridViewDataSet<ObjednavkaDTO>();

        public GridViewDataSet<ObjItemsDTO> SeznamItemsGv { get; set; } = new GridViewDataSet<ObjItemsDTO>();
        

        public List<PozadavekDTO> SeznamPozadavku { get; set; } = new List<PozadavekDTO>();

        public List<ObjednavkaDTO> SeznamObjednavek { get; set; } = new List<ObjednavkaDTO>();

        public int SelectedObjId { get; set; }

        public List<ObjItemsDTO> SeznamCheckedItems { get; set; } = new List<ObjItemsDTO>();

        public PozadavekDTO PozadavekData { get; set; } = new PozadavekDTO();

        public bool AddToObj { get; set; } = false;
        public string info { get; set; }


        public bool NothingFound { get; set; } = false;

        public bool JenPozadavky { get; set; } = true;

        public List<string> EmailoveAdresy { get; set; } = new List<string>() { "jiri.fejfusa@juli.cz", "sarka.coupkova@juli.cz", "objednavky@juli.cz", "marek.novak@juli.cz" };
        public bool zmenitEmail { get; set; } = false;
        public string vybranyEmail { get; set; }


        public void Objednat(int id, bool upravitFirst = false)
        {
            ClearAlerts();
            // vytvorit objednavku a potvrzeni a odeslat to vse na email dodavatele
            try
            {
                var objednavka = ObjednavkyService.GetObjById(id);

                string email = "";
                if (!upravitFirst)
                {
                    email = DodavatelService.GetDodavatelById(objednavka.DodavatelID).WURL05;
                    email += "; objednavky@juli.cz";
                }
                else email += "objednavky@juli.cz";

                if (string.IsNullOrEmpty(email))
                    throw new ValidationException("email");

                int Priloha1 = Tisk(objednavka);
                int Priloha2 = TiskPotrzeni(objednavka);
                System.Threading.Thread.Sleep(1000);

                if (Priloha1 == 0 || Priloha2 == 0 )
                    throw new ValidationException("tisk");

                List<string> prilohy = new List<string>();
                prilohy.Add(FilesService.GetFileByID(Priloha1).FullPath);
                prilohy.Add(FilesService.GetFileByID(Priloha2).FullPath);

                string objId = ObjednavkyService.GeObjNumber(objednavka.FullObjednavkaID);
                ConfirmText = MailServices.SendMail(email, $"OBJEDNÁVKA / ORDER {objId} – JULI Motorenwerk, s.r.o.",               
                    "Dobrý den,<br><br>" +
                    "v příloze Vám zasíláme naši objednávku.<br>" +
                    "Prosíme potvrzení objednávky zaslat na: objednavky@juli.cz<br>" +
                    "Elektronické faktury zasílejte na email: faktury@juli.cz<br>" +
                    "<br>" +
                    "Enclosed find our order.<br>" +
                    "Please order confirmation send to: objednavky@juli.cz<br>" +
                    "E-invoices send to: faktury@juli.cz<br>" +
                    "<br>" +
                    "<span style='color:red'>UPOZORNĚNÍ: Faktury posílejte výhradně na mailovou adresu: faktury@juli.cz</span><br>" +
                    "Jinak nemůžeme garantovat řádné zpracování Vašich faktur.<br><br>" +
                    "*** Automaticky generovaná zpráva. ***<br>" +
                    "*** This is an automatically generated email. *** "
                    , prilohy, "objednavky"); //"objednavky"

                if (ConfirmText == "Email byl odeslán. ")
                {
                    

                    objednavka.Odeslano = true;
                    objednavka.OdeslanoDne = DateTime.Now;
                    objednavka.Dokonceno = true;
                    objednavka.DokoncenoDne = DateTime.Now;
                    ObjednavkyService.SaveObj(objednavka);

                    List<PozadavekDTO> pozadavky = ObjednavkyService.GetListPozadavkyByObj(id);
                    foreach (var item in pozadavky)
                    {

                        PozadavekDTO pozadavek = PozadavkyService.GetPozadavekById(item.ID);
                        pozadavek.Stav = "Dokončeno";
                        PozadavkyService.PozadavekSave(pozadavek);
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "email")
                    AlertText += "Dodavatel nemá zadanou emailovou adresu!";
                else
                    if (ex.Message == "tisk")
                    AlertText += "Chyba při generování příloh (tiskové sestavy)!";
                else
                    if (ex.InnerException == null) AlertText += ex.Message;
                else AlertText += ex.InnerException.Message;
            }

        }
 
        public void Neodeslat(int id)
        {
            // Nastavit priznak "Neodesilat" a "Dokonceno" a vyradit ze seznamu
            ClearAlerts();
            try
            {
                var objednavka = ObjednavkyService.GetObjById(id);
                objednavka.Neodesilat = true;
                objednavka.Dokonceno = true;
                objednavka.DokoncenoDne = DateTime.Now;
                ObjednavkyService.SaveObj(objednavka);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null) AlertText += ex.Message;
                else AlertText += ex.InnerException.Message;
            }

        }

        public void Upravit(int id)
        {
            // vytvorit objednavku a potvrzeni a odeslat to vse na email dle vyberu
            ClearAlerts();
            // vytvorit objednavku a potvrzeni a odeslat to vse na email dodavatele
            try
            {
                var objednavka = ObjednavkyService.GetObjById(id);

                string email = vybranyEmail;             

                int Priloha1 = Tisk(objednavka);
                int Priloha2 = TiskPotrzeni(objednavka);
                System.Threading.Thread.Sleep(1000);

                if (Priloha1 == 0 || Priloha2 == 0)
                    throw new ValidationException("tisk");

                List<string> prilohy = new List<string>();
                prilohy.Add(FilesService.GetFileByID(Priloha1).FullPath);
                prilohy.Add(FilesService.GetFileByID(Priloha2).FullPath);

                string objId = ObjednavkyService.GeObjNumber(objednavka.FullObjednavkaID);


                ConfirmText = MailServices.SendMail(email, $"OBJEDNÁVKA / ORDER {objId} – JULI Motorenwerk, s.r.o.",
                    "Dobrý den,<br><br>" +
                    "V příloze Vám zasíláme naši objednávku.<br>" +
                    "Prosíme potvrzení objednávky zaslat na: objednavky@juli.cz<br><br>" +
                    "Enclosed find our order.<br>" +
                    "Please order confirmation send to: objednavky@juli.cz<br><br>", prilohy, "objednavky"); //"objednavky"

                if (ConfirmText == "Email byl odeslán. ")
                {
                    objednavka.Odeslano = true;
                    objednavka.OdeslanoDne = DateTime.Now;
                    objednavka.Dokonceno = true;
                    objednavka.DokoncenoDne = DateTime.Now;
                    ObjednavkyService.SaveObj(objednavka);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "email")
                    AlertText += "Dodavatel nemá zadanou emailovou adresu!";
                else
                    if (ex.Message == "tisk")
                    AlertText += "Chyba při generování příloh (tiskové sestavy)!";
                else
                    if (ex.InnerException == null) AlertText += ex.Message;
                else AlertText += ex.InnerException.Message;
            }

        }

        public int Tisk(ObjednavkaDTO ObjData)
        {
            
            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "P" + DateTime.Now.Year + ObjData.ID;

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

            MemoryStream stream;
            int id = 0;

            try
            {
                stream = TiskServices.CreatePdfObjednavkovyListByObjId(ObjData.ID);

                string objId = ObjednavkyService.GeObjNumber(ObjData.FullObjednavkaID);
                if (objId == "") objId = ObjData.ID.ToString();

                id = FilesService.SaveFileGetId(stream, fullPath, "Objednavka_" + objId + ".pdf", 0, 0, 0, "Tisková sestava objednávky.");
                return (id);
            }           
            catch (Exception ex)
            {
                AlertText += ex.Message;
                return (id);
            }



        }

        public int TiskPotrzeni(ObjednavkaDTO ObjData)
        {
            
            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "P" + DateTime.Now.Year + ObjData.ID;

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

            MemoryStream stream;
            int id = 0;

            try
            {
                stream = TiskServices.CreatePdfPotvrzeníObjednavkyByObjId(ObjData.ID);

                string objId = ObjednavkyService.GeObjNumber(ObjData.FullObjednavkaID);
                if (objId == "") objId = ObjData.ID.ToString();

                id = FilesService.SaveFileGetId(stream, fullPath, "Potvrzeni_objednavky_" + objId + ".pdf", 0, 0, 0, "Potvrzení objednávky.");
                return (id);
            }
            catch (Exception ex)
            {
                AlertText += ex.Message;
                return (id);
            }


        }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                ActiveUserLevel = UserServices.GetActiveUserLevels();
                ActiveUser = UserServices.GetActiveUser();
       
              
            }

            ObjednavkyService.FillGridViewObjednavkyNaOdeslani(SeznamObjednavekGV);
            

            NothingFound = SeznamObjednavekGV.PagingOptions.TotalItemsCount == 0;            
            
            return base.PreRender();
        }


    }
}

