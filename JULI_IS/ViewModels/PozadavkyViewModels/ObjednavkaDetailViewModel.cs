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
using DotVVM.Framework.Storage;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
//using System.Drawing.Printing;
using DotVVM.Framework.Hosting;

namespace ViewModels.PozadavkyViewModels
{
    public class ObjednavkaDetailViewModel : AppViewModel
    {

        public int? ObjId
        {
            get { return Convert.ToInt32(Context.Parameters["Id"]); }
        }

        public bool Editovatelny { get; set; } = true;

        public bool Stornovatelne { get; set; } = false;

        public bool NothingFound { get; set; } = false;

        public bool ZrusitelnyPodpis { get; set; } = false;

        public UsersDTO Schvalovatel { get; set; }

        //public string ActiveUserName { get; private set; } = 
        //   UserServices.GetNameByUserName( Constants.ActiveUser);

        //public int ActiveUserLevel { get; private set; } = Constants.ActiveUserLevel;

        public bool Neodesilat { get; set; } = false;
        
        public ObjednavkaDTO ObjData { get; set; } = new ObjednavkaDTO();

        public List<CiselnikDTO> KSTlist { get; set; } = new List<CiselnikDTO>();
        public int KST1 { get; set; }
        public int KST2 { get; set; }
        public int KST3 { get; set; }

        public  List<ObjednavkaTexty> TextCena { get; set; } = new List<ObjednavkaTexty>();

        public  List<ObjednavkaTexty> TextDodaciPodm { get; set; } = new List<ObjednavkaTexty>();

        public  List<ObjednavkaTexty> TextPlatebniPodm { get; set; } = new List<ObjednavkaTexty>();

        public List<DodavateleDTO> Dodavatele { get; set; } = new List<DodavateleDTO>();

        public List<DodavateleDTO> DodavateleNumbOrd { get; set; } = new List<DodavateleDTO>();

        public List<string> HlavniRada { get; set; } = new List<string> {"04 - Ostatní", "02 - Modelové zařízení", "10 - Investice"};
  
        public DodavateleDTO Dodavatel { get; set; } = new DodavateleDTO();
        
        public string SelectedDodavatel { get; set; }

        public string Vytisknuto { get; set; }

        public List<string> DodavateleS21List { get; set; } = new List<string>();
        public string DodavatelSelected { get; set; }

        //----------------------------------------------------------------------

        public List<string> MenaList { get; set; } = new List<string> { "CZK", "EUR", "USD", "CHF", "GBP" };
        public string MenaClass { get; set; } = "form-control";
        public bool DruhCenyVlastniText { get; set; }
        public bool DodPodmVlastniText { get; set; }
        public bool PlatPodmVlastniText { get; set; }

        public void SaveKoncept()
        {
            if (Editovatelny)
            {
                try
                {
                    ObjData.PocetPolozek = ItemsService.GetPocetItemsByObjId(ObjData.ID);
                    ObjednavkyService.SaveObj(ObjData);
                    ConfirmText = $"Koncept objednávky uložen...";
                }
                catch (Exception e)
                {
                    AlertText = e.Message;
                }

                //foreach (var file in Files)
                //{
                //    FilesService.SaveFileDescription(file.Description, file.ID);
                //}

            }
            else AlertText = "Objednávku nelze uložit ani upravovat";
        }




        /// <summary>
        /// kontroluje menu, jestli odpovida dodavatel x objednavka
        /// </summary>
        public void ChangeMena()
        {
            if (ObjData.Mena == "CZK") MenaClass = "form-control";
            if (ObjData.Mena != "CZK") MenaClass = "form-control mena-cizy";
            if (DodavatelS21.CURN05 != ObjData.Mena) MenaClass = "form-control mena-error";
        }

        public void DodPodmyChange()
        {
            if (ObjData.TextDodaciPodmId == 16)
                DodPodmVlastniText = true;            
            else
            {
                DodPodmVlastniText = false;
                if (ObjData.TextDodaciPodmId != null)
                ObjData.TextDodaciPodmText = TextDodaciPodm.Find(t => t.ID == ObjData.TextDodaciPodmId).Text;
            }                            
        }

        public void DruhCenyChange()
        {
            if (ObjData.TextCenaId == 6)
                DruhCenyVlastniText = true;
             else
            {
                DruhCenyVlastniText = false;
                if (ObjData.TextCenaId != null)
                    ObjData.TextCenaText = TextCena.Find(t => t.ID == ObjData.TextCenaId).Text;
            }
        }

        public void PlatPodmChange()
        {
            if (ObjData.TextPlatebniPodmId == 20)
                PlatPodmVlastniText = true;
            else
            {
                PlatPodmVlastniText = false;
                if (ObjData.TextPlatebniPodmId != null && ObjData.TextPlatebniPodmId != 0)
                    ObjData.TextPlatebniPodmText = TextPlatebniPodm.Find(t => t.ID == ObjData.TextPlatebniPodmId).Text;
            }
        }

        public void NastavHlavniRada()
        {
            switch (ObjData.TypInvestice)
            {
                case 1:
                    ObjData.HlavniRada = "02 - Modelové zařízení";
                    break;
                case 2:
                    ObjData.HlavniRada = "10 - Investice";
                    break;
                case 3:
                    ObjData.HlavniRada = "04 - Ostatní";
                    break;
                default:
                    break;
            }
        }


        public void NeodesilatChange()
        {
            if (ObjData.Neodesilat) ObjData.Neodesilat = false;
            else ObjData.Neodesilat = true;
            SaveKoncept();
        }
        //------------------------------ ITEMS ----------------------------------------

        public bool JinyDuvod { get; set; } = false;
        public ObjItemsDTO ItemData { get; set; } = new ObjItemsDTO();
        public GridViewDataSet<ObjItemsDTO> ItemsGv { get; set; } = new GridViewDataSet<ObjItemsDTO>()
        {
            PageSize = 20,
            SortExpression = nameof(PozadavekDTO.ID),
            SortDescending = true
        };
        public bool DuvodHvezdicka { get; set; } = false;
        public string CelkovaCenaMena { get; set; }

        public void Prepocitat()
        {
            ItemData.CelkovaCena = ItemData.CenaZaJednotku * ItemData.Mnozstvi;
            CelkovaCenaMena = ItemData.CelkovaCena.ToString("n2");
            CelkovaCenaMena += (" " +  ObjData.Mena);

        }

        public void DeleteItem(int id)
        {
            if (Editovatelny)
            {
                try
                {
                    ItemsService.DeleteItemFromObj(id);
                    ObjednavkyService.CelkovaCenaPrepocitat(ObjData.ID);
                    ConfirmText = $"Položka vyřazena z objednávky a vrácena zpět";
                }
                catch (Exception e)
                {
                    AlertText = e.Message;
                }
            }

            else AlertText = $"Změny v objednávce nejsou povoleny!";
        }

        public void EditItem(int? ItemId)
        {
            ClearAlerts();
            SaveKoncept();
            ItemData = ItemsService.GetObjItemById(ItemId.Value);
            Files = FilesService.GetFilesListByItemID(ItemData.OrigItemID);
            DuvodChange();
            Prepocitat();
            Context.ResourceManager.AddStartupScript("$('div[data-id=item-detail]').modal({backdrop: 'static'});");
        }

        public void SaveItem()
        {
            string LastKST = ItemData.KST;
            bool LastInvesticeNeplanovana = ItemData.InvesticeNeplanovana;
            bool LastInvesticePlanovana = ItemData.InvesticePlanovana;
            bool LastNakupOstatni = ItemData.NakupOstatni;
            string LastCisloInvestice = ItemData.CisloInvestice;
            string LastCisloKonta = ItemData.CisloKonta;


            ClearAlerts();
            Prepocitat();
            ItemData.ObjednavkaID = ObjData.ID;
            ItemData.DodavatelID = ObjData.DodavatelID;

            try
            {
                ItemsService.ObjItemSave(ItemData);
                ObjData.CelkovaCena = ItemsService.GetCelkovaCenaByObjId(ObjData.ID);
                ObjData.PocetPolozek = ItemsService.GetPocetItemsByObjId(ObjData.ID);
                SaveKoncept();
            }
            catch (Exception ex)
            {
                if (ItemData.Jednotka == null)
                    AlertText = "Prosím vyplňte jednotku! ";

                if (ex.InnerException == null) AlertText += ex.Message;
                else
                {
                    AlertText += ex.InnerException.Message;
                    if (ex.InnerException.InnerException.Message != null)
                        AlertText += ex.InnerException.InnerException.Message;
                }
            }

            if (AlertText == null)
                {
                    ConfirmText = "Položka v pořádku uložena, můžete psát další nebo kliknout na tl. \"Ukončit\"...";
                    Context.ResourceManager.AddStartupScript("$('div[data-id=item-detail]').modal('hide');");
                }

            //if(NewPozadavek)
            //    Context.RedirectToRoute("PozadavekEdit", new { Id = PozadavekData.ID });

        }

        public void DuvodChange()
        {
            if ((ItemData.Poptany3Firmy == true && ItemData.DuvodID == 6)
                || (ItemData.Poptany3Firmy == false && ItemData.DuvodID == 7))
            {
                JinyDuvod = true;
            }
            else JinyDuvod = false;

            if (((ItemData.Poptany3Firmy == true) && (ItemData.DuvodID == 4 || ItemData.DuvodID == 5)) || ((ItemData.Poptany3Firmy == false) && (ItemData.DuvodID == 5 || ItemData.DuvodID == 6)))
                DuvodHvezdicka = true;
            else DuvodHvezdicka = false;
        }

        public Reasons[] DuvodyPoptavky { get; set; } =
           {
            new Reasons() { ID = 1, Text = "Nejlepší cena"},
            new Reasons() { ID = 2, Text = "Nejlepší kvalita"},
            new Reasons() { ID = 3, Text = "Nejrychlejší dodávka"},
            new Reasons() { ID = 4, Text = "(*) Speciální Dodavatel"},
            new Reasons() { ID = 5, Text = "(*) Speciální materiál"},
            new Reasons() { ID = 6, Text = "Jiný důvod: "}
        };

        public Reasons[] DuvodyNepoptani { get; set; } =
        {
            new Reasons() { ID = 1, Text = "Celková výše požadavku (objednávky) do 15.000,- Kč bez DPH"},
            new Reasons() { ID = 2, Text = "Servisní zásah"},
            new Reasons() { ID = 3, Text = "Havárie"},
            new Reasons() { ID = 4, Text = "Porucha"},
            new Reasons() { ID = 5, Text = "(*) Speciální Dodavatel"},
            new Reasons() { ID = 6, Text = "(*) Speciální materiál"},
            new Reasons() { ID = 7, Text = "Jiný důvod: "}
        };

        public class Reasons
        {
            public Reasons() { }
            public int ID { get; set; }
            public string Text { get; set; }
        }

        public void LinkToPozadavek(int id)
        {
            ClearAlerts();
            SaveKoncept();
            Context.RedirectToRoute("PozadavekEdit", new { Id = id });
        }


    public void VytvoritObjednavku()
    {
            //VALIDACE
            ClearAlerts();

            try
            {
                var items = ItemsGv.Items.ToList();
                if (items.Count == 0) throw new ValidationException("items");

                if (ObjData.TextCenaId == null || ObjData.TextCenaId == 0)
                {
                    throw new ValidationException("cena");
                }

                if (string.IsNullOrEmpty(ObjData.TextDodaciPodmText))
                {
                    throw new ValidationException("dodpodm");
                }


                if (string.IsNullOrEmpty(ObjData.TextPlatebniPodmText))
                {
                    throw new ValidationException("platpodm");
                }
       
                if (string.IsNullOrEmpty(SelectedDodavatel))
                {
                    throw new ValidationException("dodavatel");
                }

                if (UserServices.GetActiveUser() == "miroslav.dvoracek")
                {
                    throw new ValidationException("opravneni");
                }

                string email = DodavatelService.GetDodavatelById(ObjData.DodavatelID).WURL05;
                if (string.IsNullOrEmpty(email))
                {
                    throw new ValidationException("email");
                }

                if (ObjData.KST1 == 0 || ObjData.KST1 is null)
                {
                    throw new ValidationException("KST");
                }


                ObjData.Objednano = true;
                ObjData.ObjednavatelName = ActiveUser;
                ObjData.DatumObjednani = DateTime.Now;
                string kst = ObjData.KST1.ToString();
                string rok = DateTime.Now.Year.ToString();
                int lastNumberOfYear = ObjednavkyService.GetLastNumberOfYear(rok);
                int newNumber = lastNumberOfYear + 1;
                if (string.IsNullOrEmpty(ObjData.FullObjednavkaID))
                    ObjData.FullObjednavkaID = rok + "-" + newNumber.ToString().PadLeft(4, '0') + "/" + kst;
                SaveKoncept();
                ObjednavkyService.ObjednavkaOdeslatNaSchvaleni(ObjData);
                ConfirmText = $"Objednávka uložena a odeslána na schválení...";

            }
            catch (ValidationException exc)
            {
                if (exc.Message == "items")
                    AlertText = "V objednávce musí být alespoň jedna položka!";

                if (exc.Message == "dodpodm")
                    AlertText = "Nejsou vyplněny \"Dodací podmínky\"!";

                if (exc.Message == "cena")
                    AlertText = "Není vyplněn \"Druh ceny\"!";

                if (exc.Message == "platpodm")
                    AlertText = "Nejsou vyplněny \"Platební podmínky\"!";

                if (exc.Message == "platpodm")
                    AlertText = "Nejsou vyplněny \"Platební podmínky\"!";

                if (exc.Message == "dodavatel")
                    AlertText = "V objednávce chybí Dodavatel!";

                if (exc.Message == "opravneni")
                    AlertText = "Nemáte oprávnění k podpisu objednávky!";

                if (exc.Message == "email")
                    AlertText = "Není vyplněna emailová adresa dodavatele!";

                if (exc.Message == "KST")
                    AlertText = "Nenalezeno KST v požadavku, prosím zvolte správné středisko!";
            }
            catch (Exception exc)
            {
                AlertText = exc.Message;
            }

            Context.ResourceManager.AddStartupScript(" $('html, body').animate({ scrollTop: $(document).height() }, 'slow');");
        }

        public void StornovatObjednavku()
        {
            ObjednavkyService.StornoObjednavky(ObjData.ID);
            SaveKoncept();
            Context.RedirectToRoute("ObjednavkaEdit", new { Id = ObjData.ID });
        }

        //------------------------------ DODAVATELE ----------------------------------------

        public List<DodavateleS21> DodavateleS21 { get; set; } = new List<DodavateleS21>();
        public List<DodavateleS21> DodavateleS21ByNumb { get; set; } = new List<DodavateleS21>();
        public DodavateleS21 DodavatelS21 { get; set; } = new DodavateleS21();
        public OdpOsoby Osoba { get; set; } = new OdpOsoby();

        public void DodavateleInit()
        {
            DodavateleS21 = DodavatelService.GetS21DodavateleOrderedByName().Where(w => w.NazevCislo != null && w.NazevCislo.Length > 3).ToList();
            DodavateleS21List = DodavateleS21.Select(s => s.NazevCislo).ToList();
            //DodavateleS21 = DodavatelService.GetS21DodavateleOrderedByName();
            //DodavateleS21ByNumb = DodavatelService.GetS21DodavateleOrderedByNumb();
        }


        /// <summary>
        /// ulozi nebo update zaznam v tab. dbo.Dodavatele, do ObjData.DodavatelID ulozi ID dodavatele, pokud je novy
        /// </summary>
        public void SaveDodavatel()
        {
            ClearAlerts();
            if (Osoba != null) DodavatelS21.OdpOsobaId = Osoba.Id;

            try
            {
                // nema dodavatele
                if (ObjData.DodavatelID == 0)
                {
                    ObjData.DodavatelID = DodavatelService.DodavatelInsert(DodavatelS21, Osoba);
                }
                else
                {
                    //DodavatelS21.Id = PozadavekData.DodavatelID;
                    DodavatelService.DodavatelSave(ObjData.DodavatelID, DodavatelS21, Osoba);
                }

                ConfirmText = "Změna dodavatele uložena...";
            }
            catch (Exception ex)
            {
                AlertText = "Chyba při ukládání dodavatele: ";

                if (ex.InnerException == null) AlertText += ex.Message;
                else
                {
                    AlertText += ex.InnerException.Message;

                    if (ex.InnerException.InnerException.Message != null)
                        AlertText += ex.InnerException.InnerException.Message;
                }
            }

        }
    
        public void EditDodavatel()
        {
            ClearAlerts();
            SaveKoncept();
            if (DodavateleS21.Count == 0) DodavateleInit();

            //if (Dodavatel.SUPN05 != null && Dodavatel.SUPN05 != "")
            //{
            //    ObjData.DodavatelID = DodavatelService.GetS21DodavatelBySUPN05(Dodavatel.SUPN05).Id;
            //}

            Context.ResourceManager.AddStartupScript("$('div[data-id=dodavatel-detail]').modal({backdrop: 'static'});");
        }

        public void DodavatelReset()
        {
            Dodavatel = new DodavateleDTO();
        }

        public void SaveDodavatelModal()
        {
            ClearAlerts();

            if (DodavatelS21 != null)
                SelectedDodavatel = DodavatelS21.SNAM05 + ", " + DodavatelS21.SUPN05
                 + "\n"
                    + (String.IsNullOrEmpty(DodavatelS21.SAD105) ? "" : DodavatelS21.SAD105 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD205) ? "" : DodavatelS21.SAD205 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD305) ? "" : DodavatelS21.SAD305 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD405) ? "" : DodavatelS21.SAD405 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD505) ? "" : DodavatelS21.SAD505 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.PSC) ? "" : "\nPSČ: " + DodavatelS21.PSC)
                 + $"\nměna: {DodavatelS21.CURN05}"
                 + ""
                 ;
            SelectedDodavatel += $"\nemail: { DodavatelS21.WURL05}\n";

            if (Osoba != null)
            {
                SelectedDodavatel += Osoba.CNTN1A;            
            }

            SaveDodavatel();
            SaveKoncept();
            ChangeMena();
            Context.ResourceManager.AddStartupScript("$('div[data-id=dodavatel-detail]').modal('hide');");

        }

        //public void LoadDodavatele(int dodId)
        // nacte dodavatele a osoby z S21 podle kombinace Nazev | Cislo
        public void LoadDodavatele(string dodavatel, bool editovany = false)
        {
            ClearAlerts();
            //Dodavatel = DodavatelService.GetS21DodavatelById()
            DodavatelS21 = DodavateleS21.Where(w => w.NazevCislo == dodavatel).SingleOrDefault();
            //DodavatelS21 = DodavateleS21.Find(d => d.Id == dodId);
            DodavatelS21.PSC = DodavatelS21.PCD105 + DodavatelS21.PCD205;

            Osoba = DodavatelService.GetOdpOsobaByDodavatelId(DodavatelS21.Id);
            SelectedDodavatel = DodavatelS21.SNAM05 + ", " + DodavatelS21.SUPN05
               + "\n"
               + (String.IsNullOrEmpty(DodavatelS21.SAD105) ? "" : DodavatelS21.SAD105 + ", ")
               + (String.IsNullOrEmpty(DodavatelS21.SAD205) ? "" : DodavatelS21.SAD205 + ", ")
               + (String.IsNullOrEmpty(DodavatelS21.SAD305) ? "" : DodavatelS21.SAD305 + ", ")
               + (String.IsNullOrEmpty(DodavatelS21.SAD405) ? "" : DodavatelS21.SAD405 + ", ")
               + (String.IsNullOrEmpty(DodavatelS21.SAD505) ? "" : DodavatelS21.SAD505 + ", ")
               + (String.IsNullOrEmpty(DodavatelS21.PSC) ? "" : "\nPSČ: " + DodavatelS21.PSC)
               + $"\nměna: {DodavatelS21.CURN05}"
               + ""
               ;

            SelectedDodavatel += $"\nemail: { DodavatelS21.WURL05}\n";

            if (Osoba != null)
            {
                Osoba.CNTN1A = Osoba.CNTN1A + (Osoba.GTX11A == "" ? "" : Osoba.GTX11A);                
                SelectedDodavatel += Osoba.CNTN1A;            
            }

            SaveDodavatel();
            SaveKoncept();
            ChangeMena();

        }

        public void CloseModal(string dataid)
        {
            ClearAlerts();
            string closescript = "$('div[data-id=" + dataid + "]').modal('hide');";
            Context.ResourceManager.AddStartupScript(closescript);
        }

        // ------------------------  TISK ------------------------------------------------------

        public void Tisk()
        {
            SaveKoncept();
            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "O" + DateTime.Now.Year + ObjData.ID;

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

            MemoryStream stream;
            int id = 0;

            try
            {
                stream = TiskServices.CreatePdfObjednavkovyListByObjId(ObjData.ID);
                string objId = ObjednavkyService.GeObjNumber(ObjData.FullObjednavkaID);
                if (objId == "") objId = ObjData.ID.ToString();
                id = FilesService.SaveFileGetId(stream, fullPath, "Tiskova_sestava_objednavka_" + objId + ".pdf", 0, 0, ObjData.ID, "Tisková sestava objednávky.");
            }
            catch (Exception ex)
            {
                AlertText = ex.Message;
            }

            System.Threading.Thread.Sleep(1000);
            Context.RedirectToRoute("FileDownloadPDF", new { Id = id });

        }

        public void TiskPotrzeni()
        {
            SaveKoncept();
            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "O" + DateTime.Now.Year + ObjData.ID;

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

            MemoryStream stream;
            int id = 0;

            try
            {
                stream = TiskServices.CreatePdfPotvrzeníObjednavkyByObjId(ObjData.ID);

                string objId = ObjednavkyService.GeObjNumber(ObjData.FullObjednavkaID);
                if (objId == "") objId = ObjData.ID.ToString();

                id = FilesService.SaveFileGetId(stream, fullPath, "Potvrzeni_objednavka_" + objId + ".pdf", 0, 0, ObjData.ID, "Potvrzení objednávky.");
            }
            catch (Exception ex)
            {
                AlertText = ex.Message;
            }

            System.Threading.Thread.Sleep(1000);
            Context.RedirectToRoute("FileDownloadPDF", new { Id = id });
        }

        public void Vytisteno()
        {
            if (ActiveUserLevel.Contains(3))
            {
                ObjData.Vytisknuto = true;
                ObjData.VytisknutoDne = DateTime.Now;
                try
                {
                    ObjednavkyService.SaveObj(ObjData);

                }
                catch (Exception e)
                {
                    AlertText = e.Message;
                }

                SaveKoncept();
                string juliFile = @"\\juli-app\Pozadavky";
                string mainDirectory = DateTime.Now.Year.ToString();
                string subDir = "O" + DateTime.Now.Year + ObjData.ID;

                string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

                MemoryStream stream;
                int id = 0;

                try
                {
                    stream = TiskServices.CreatePdfObjednavkovyListByObjId(ObjData.ID);

                    string objId;
                    if (ObjednavkyService.GeObjNumber(ObjData.FullObjednavkaID) != "")
                        objId = ObjednavkyService.GeObjNumber(ObjData.FullObjednavkaID);
                    else objId = ObjData.ID.ToString();

                    id = FilesService.SaveFileGetId(stream, fullPath, "Tiskova_sestava_objednavka_" + objId + ".pdf", 0, 0, 0, "Tisková sestava objednávky.");
                }
                catch (Exception ex)
                {
                    AlertText = ex.Message;
                }

                System.Threading.Thread.Sleep(1000);

                var PDFfile = FilesService.GetFileByID(id);
                TiskServices.PrintFile(PDFfile.FullPath);

                //Vytisknuto = Tools.ReadTextResourceFromAssembly("JULI_IS.Resources.Pozadavky.PageObjednavkaDetail.Vytisteno");
                //if (string.IsNullOrEmpty(Vytisknuto)) Vytisknuto = "Nezdarilo se";
            }

            //string juliFile = @"\\juli-app\Pozadavky";
            //string mainDirectory = DateTime.Now.Year.ToString();
            //string subDir = "P" + DateTime.Now.Year + ObjData.ID;

            //string fullPath = Path.Combine(juliFile, mainDirectory, subDir);
   
            //MemoryStream stream;
            //int id = 0;

            //try
            //{
            //    stream = TiskServices.CreatePdfObjednavkovyListByObjId(ObjData.ID);
                
            //    id = FilesService.SaveFileGetId(stream, fullPath, "Tiskova_sestava_objednavka_" + ObjData.ID + ".pdf", 0, 0, 0, "Tisková sestava objednávky.");
            //}
            //catch (Exception ex)
            //{
            //    AlertText = ex.Message;
            //}

            //System.Threading.Thread.Sleep(1000);

            //string fullPathWithFile = FilesService.GetFileByID(id).FullPath;

      

           // ProcessStartInfo info = new ProcessStartInfo();
           // info.Verb = "";
           // info.FileName = fullPathWithFile;
           // info.CreateNoWindow = true;
           // info.WindowStyle = ProcessWindowStyle.Hidden;
           //// info.UseShellExecute = false;
           // info.CreateNoWindow = true;

            // Process p = new Process();
            // p.StartInfo = info;
            // p.Start();

            // if (p.HasExited == false)
            // {
            //     p.WaitForExit(10000);
            //     //proc.Kill();
            // }

            // p.EnableRaisingEvents = true;

            // p.CloseMainWindow();
            // p.Close();

            //p.WaitForInputIdle();
            //System.Threading.Thread.Sleep(3000);
            //if (false == p.CloseMainWindow())
            //    p.Kill();

        }


        // ------------------------  FILES ------------------------------------------------------

        public UploadedFilesCollection UploadedFiles { get; set; } = new UploadedFilesCollection();
        public List<FilesDTO> Files { get; set; } = new List<FilesDTO>();
        public bool FileDescrptChange { get; set; } = false;

        public void DeleteFile(int id)
        {
           // if (Editovatelny)
            {
                try
                {
                    FilesService.DeleteFile(id);
                }
                catch (Exception ex)
                {
                    AlertText = ex.Message;
                }
            }

           // else AlertText = $"Změny v objednávce nejsou povoleny!";



        }

        public void SaveDescription(string popis, int id)
        {
            try
            {
                FilesService.SaveFileDescription(popis, id);
                ConfirmText = "Změny uloženy...";
            }
            catch (Exception ex)
            {
                AlertText = ex.Message;
            }
        }

        public void FilesUploadedComplete()
        {
            var storage = Context.Configuration.ServiceLocator.GetService<IUploadedFileStorage>();

            //if (NewPozadavek)
            //{
            //    PozadavkyService.PozadavekInsert(PozadavekData);
            //    PozadavekData.ID = PozadavkyService.LastPozId;
            //    NewPozadavek = false;
            //}

            foreach (var file in UploadedFiles.Files)
            {
                var stream = storage.GetFile(file.FileId);


                string juliFile = @"\\juli-app\Pozadavky";
                string mainDirectory = DateTime.Now.Year.ToString();
                string subDir = "O" + DateTime.Now.Year + ObjData.ID;

                string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

                string filename = Path.GetFileName(file.FileName);

                FilesService.SaveFile(stream, fullPath, filename, 0, 0, ObjData.ID);
            }
            UploadedFiles.Clear();
            SaveKoncept();
            //Context.RedirectToRoute("PozadavekEdit", new { Id = ObjData.ID });
        }


        public void ZrusitPodpisyPotvrzeni()
        {
            ClearAlerts();
            if (ObjData.Objednano && !ObjData.Schvaleno && !ObjData.Zamitnuto) 
                Context.ResourceManager.AddStartupScript("$('div[data-id=confirm]').modal({backdrop: 'static'});");

        }

        public void ZrusitPodpisy()
        {
            ObjData.Objednano = false;
            Editovatelny = true;
            SaveKoncept();
            Context.ResourceManager.AddStartupScript("$('div[data-id=confirm]').modal('hide');");
        }

        // ------------------------  PRERENDER ------------------------------------------------------

        public override Task PreRender()
        {
            if (!Context.IsPostBack)  // prvni nacteni stranky
            {
                try
                {
                    ObjData = ObjednavkyService.GetObjById(ObjId.Value);
                    ObjednavkyService.CelkovaCenaPrepocitat(ObjData.ID);
                    ActiveUserLevel = UserServices.GetActiveUserLevels();
                    ActiveUser = UserServices.GetActiveUser();
                    Schvalovatel = UserServices.GetUserById(ObjData.SchvalovatelID);
                    KSTlist = ObjednavkyService.GetKSTList();
                    TextCena = ObjednavkyService.GetObjednavkaTexty(1);
                    TextDodaciPodm = ObjednavkyService.GetObjednavkaTexty(2);
                    TextPlatebniPodm = ObjednavkyService.GetObjednavkaTexty(3);
                    //Dodavatele = DodavatelService.GetDodavateleOrderedByName2();
                    //DodavateleNumbOrd = DodavatelService.GetDodavateleOrderedByNumb();
                    DodPodmyChange();
                    DruhCenyChange();
                    PlatPodmChange();
                    NastavHlavniRada();

                    if (ObjData.DodavatelID > 0)
                    {
                        DodavatelS21 = DodavatelService.GetDodavatelByIdAsS21(ObjData.DodavatelID);

                        SelectedDodavatel = DodavatelS21.SNAM05 + ", " + DodavatelS21.SUPN05
                            + "\n"
                            + (String.IsNullOrEmpty(DodavatelS21.SAD105) ? "" : DodavatelS21.SAD105 + ", ")
                            + (String.IsNullOrEmpty(DodavatelS21.SAD205) ? "" : DodavatelS21.SAD205 + ", ")
                            + (String.IsNullOrEmpty(DodavatelS21.SAD305) ? "" : DodavatelS21.SAD305 + ", ")
                            + (String.IsNullOrEmpty(DodavatelS21.SAD405) ? "" : DodavatelS21.SAD405 + ", ")
                            + (String.IsNullOrEmpty(DodavatelS21.SAD505) ? "" : DodavatelS21.SAD505 + ", ")
                            + (String.IsNullOrEmpty(DodavatelS21.PSC) ? "" : "\nPSČ: " + DodavatelS21.PSC)
                            + $"\nměna: {DodavatelS21.CURN05}"
                           + "\n"
                            + (String.IsNullOrEmpty(DodavatelS21.CNTN1A) ? "" : DodavatelS21.CNTN1A + ", ")
                            + (String.IsNullOrEmpty(DodavatelS21.WURL05) ? "" : DodavatelS21.WURL05 + ", ")
                           ;

                        Osoba.CNTN1A = DodavatelS21.CNTN1A;
                        Osoba.EMIL1A = DodavatelS21.EMIL1A;
                        Osoba.GTX11A = DodavatelS21.GTX11A;

                        if (ObjData.Mena == "CZK") MenaClass = "form-control";
                        if (ObjData.Mena != "CZK") MenaClass = "form-control mena-cizy";
                        if (DodavatelS21.CURN05 != ObjData.Mena) MenaClass = "form-control mena-error";
                    }


                    Editovatelny =
                        (UploadedFiles.IsBusy == false) &&
                        (
                            (ObjData.Zamitnuto == false && ObjData.Schvaleno == false)
                            && ((ObjData.Objednano == false && UserServices.GetActiveUserLevels().Contains(3))
                            || (UserServices.GetActiveUserLevels().Contains(4)))
                        )
                        || (UserServices.GetActiveUserLevels().Contains(99))                        
                        ;

                    Stornovatelne =
                       (UploadedFiles.IsBusy == false && !ObjData.Stornovano) &&
                       (
                           ((ObjData.Zamitnuto == false)
                           && ((UserServices.GetActiveUserLevels().Contains(3))
                           || (UserServices.GetActiveUserLevels().Contains(4))))
                           || (UserServices.GetActiveUserLevels().Contains(99))
                       );
               
                    DuvodChange();

                }
                catch (Exception ex)
                {
                    AlertText = "Chyba při nahrávání objednávky: ";

                    if (ex.InnerException == null) AlertText += ex.Message;
                    else
                    {
                        AlertText += ex.InnerException.Message;

                        if (ex.InnerException.InnerException.Message != null)
                            AlertText += ex.InnerException.InnerException.Message;
                    }
                }
         
            }

            // uplatni se vzdy pri kazdem PostBacku

            Files = FilesService.GetFilesListByObjID(ObjId.Value);
            ObjednavkyService.FillGridViewItemsByObjID(ItemsGv, ObjId.Value);
            NothingFound = ItemsGv.PagingOptions.TotalItemsCount == 0;
            ObjData.PocetPolozek = ItemsGv.Items.Count();

            if (String.IsNullOrEmpty(ObjData.KST1.ToString()))
                if (!String.IsNullOrEmpty(ItemsGv.Items[0].KST))
                    ObjData.KST1 = Convert.ToInt32(ItemsGv.Items[0].KST);
                else ObjData.KST1 = 0;

            ZrusitelnyPodpis = (ObjData.Objednano && !ObjData.Schvaleno && !ObjData.Zamitnuto) &&
                (UserServices.GetActiveUserLevels().Contains(3)
                            || (UserServices.GetActiveUserLevels().Contains(4)));


            //SeznamObjednavek = ObjednavkyService.GetListObjednavek();

            return base.PreRender();


        }

      


    }
}

