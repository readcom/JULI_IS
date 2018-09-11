using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Storage;
using DotVVM.Framework.ViewModel;
using Pozadavky.Data;
using Pozadavky.DTO;
using Pozadavky.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using DotVVM.Framework.Hosting;

namespace ViewModels.PozadavkyViewModels
{
    // podpis level: 0 => novy pozadavek
    // podpis level: 1 => ceka na podpis vedoucim
    // podpis level: 2 => ceka na podpis reditel nebo control.
    // podpis level: 3 => vse podepsano, ceka na vytvoreni obj. a podpis nakupu
    // podpis level: 4 => vse podepsano, ceka na objednani

    public class PozadavekViewModel : AppViewModel
    {
        public PozadavekDTO PozadavekData { get; set; } = new PozadavekDTO();

        public List<InvesticeDTO> Investice { get; set; } = new List<InvesticeDTO>();
        public List<UsersDTO> Schvalovatele { get; set; } = new List<UsersDTO>();
        public List<FilesDTO> Files { get; set; } = new List<FilesDTO>();
        public string UploadText { get; set; } = "Nahrát soubory";
        public string SaveButtonText { get; set; } = "Uložit a další";
        public int LastItemId { get; set; }

        public bool vyplnenoKST { get; set; } = false;
        public bool Editovatelny { get; set; } = true;
        public bool NadrizenyUzivatel { get; set; } = false;
        public bool ZrusitelnyPodpis { get; set; } = false;
        public bool Podepsatelny { get; set; } = false;

        // FOCUS pokus
        //public System.Web.UI.Page txtItemPopis { get; set; } = new System.Web.UI.Page();

        public bool JinyDuvod { get; set; } = false;
        public bool OdeslatNaSchvaleni { get; set; } = false;

        public bool DuvodHvezdicka { get; set; } = false;
        public bool NewItem { get; set; } = false;

        // public int ParrentItemId { get; set; }


        public int InvesticeVyber { get; set; } = 3;

        public string Mena { get; set; }
        public List<string> MenaList { get; set; } = new List<string> { "CZK", "EUR", "USD", "CHF", "GBP", "JPY" };
        public string MenaClass { get; set; } = "form-control";




        //public bool VlastniPlatba { get; set; } = false;

        public List<ZpusobPlatby> ZpusobPlatbyLst { get; set; } = new List<ZpusobPlatby>()
        {
            new ZpusobPlatby() {id = 4,  text = "Faktura (bezhotovostní pøevod)" },
            new ZpusobPlatby() {id = 22,  text = "Zálohová faktura" },
            new ZpusobPlatby() {id = 17, text = "Hotovost" },
            new ZpusobPlatby() {id = 18, text = "Dobírka - Platební karta" },
            new ZpusobPlatby() {id = 19, text = "Dobírka - Hotovì" },
            new ZpusobPlatby() {id = 21, text = "Platební karta (EXPENSA)" },
            new ZpusobPlatby() {id = 20, text = "Vlastní text" }            
        };

        public class ZpusobPlatby
        {
            public int id { get; set; }
            public string text { get; set; }                   
        }           
    
        public string UserAlertText { get; set; }
        public List<CiselnikDTO> KSTlist { get; set; } = new List<CiselnikDTO>();
        public List<CiselnikDTO> InvList { get; set; } = new List<CiselnikDTO>();

        public OdpOsoby Osoba { get; set; } = new OdpOsoby();
        public int ItemsCount { get; set; }

        public bool IsCompany { get; set; }

        public bool Rozuctovani { get; set; } = false;

        public int? PozadavekId
        {
            get { return Convert.ToInt32(Context.Parameters["Id"]); }

        }

        // public string AlertText { get; set; }
        // public string ConfirmText { get; set; }

        public void ChangedKST()
        {
            if (PozadavekData.KST != null) vyplnenoKST = true;
        }

        public string CelkovaCenaMena { get; set; }


        // ------------------------  DODAVATELE ------------------------------------------------------

        //[Bind(Direction.ServerToClient)]
        public List<DodavateleS21> DodavateleS21 { get; set; } = new List<DodavateleS21>();
        public List<string> DodavateleS21List { get; set; } = new List<string>();
        public string DodavatelSelected { get; set; }


        public class DodavateleList
        {
            public string nazev { get; set; }
            public int id { get; set; }
        }

        //[Bind(Direction.ServerToClient)]
        public List<DodavateleS21> DodavateleS21ByNumb { get; set; } = new List<DodavateleS21>();

        public DodavateleS21 DodavatelS21 { get; set; } = new DodavateleS21();
        public bool NovyDodavatel { get; private set; } = false;
        public int SelectedDodavatelID { get; set; }
        public string SelectedDodavatel { get; set; }
        public bool NabidkaNeuvedena { get; set; } = false;

        public void PridatDodavatele()
        {
            SavePozadavek();
            Context.RedirectToRoute("DodavatelEdit", new { Id = 0 });
        }

        public void EditDodavatel()
        {
            ClearAlerts();
            SavePozadavek();
            //if (DodavatelS21.SUPN05 != null && DodavatelS21.SUPN05 != "")
            //{
            //    PozadavekData.DodavatelS21ID = DodavatelService.GetS21DodavatelBySUPN05(DodavatelS21.SUPN05).Id;
            //}

            Context.ResourceManager.AddStartupScript("$('div[data-id=dodavatel-detail]').modal({backdrop: 'static'});");
        }

        //public new void CloseModal(string dataid)
        //{
        //    ClearAlerts();
        //    string closescript = "$('div[data-id=" + dataid + "]').modal('hide');";
        //    Context.ResourceManager.AddStartupScript(closescript);
        //}

        // ulozi nebo update zaznam v tab. dbo.Dodavatele
        // do PozadavekData.DodavatelID ulozi ID dodavatele, pokud je novy
        public void SaveDodavatel()
        {
            ClearAlerts();
            if (Osoba != null) DodavatelS21.OdpOsobaId = Osoba.Id;

            try
            {
                // nema dodavatele
                if (PozadavekData.DodavatelID == 0)
                {
                    PozadavekData.DodavatelID = DodavatelService.DodavatelInsert(DodavatelS21, Osoba);
                }
                else
                {
                    //DodavatelS21.Id = PozadavekData.DodavatelID;
                    DodavatelService.DodavatelSave(PozadavekData.DodavatelID, DodavatelS21, Osoba);
                }

                ConfirmText = "Zmìna dodavatele uložena... ";
            }
            catch (Exception ex)
            {
                AlertText = "Chyba pøi ukládání dodavatele: ";

                if (ex.InnerException == null) AlertText += ex.Message;
                else
                {
                    AlertText += ex.InnerException.Message;
                    if (ex.InnerException.InnerException.Message != null)
                        AlertText += ex.InnerException.InnerException.Message;
                }

            }

        }

        // nacte dodavatele a osoby z S21 podle kombinace Nazev | Cislo
        public void LoadDodavatele(string dodavatel, bool editovany = false)
        {
          //  int dodId;
            ClearAlerts();
            if (!string.IsNullOrEmpty(dodavatel)) 
            {                
                DodavatelS21 = DodavateleS21.Where(w => w.NazevCislo == dodavatel).SingleOrDefault();
                DodavatelS21.PSC = (DodavatelS21.PCD105 ?? "") + (DodavatelS21.PCD205 ?? "");
                Osoba = DodavatelService.GetOdpOsobaByDodavatelId(DodavatelS21.Id);

                if (editovany) DodavatelS21.SUPN05 = "";

                    SelectedDodavatel = DodavatelS21.SNAM05 + ", " + DodavatelS21.SUPN05
                    + "\n"
                    + (String.IsNullOrEmpty(DodavatelS21.SAD105) ? "" : DodavatelS21.SAD105 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD205) ? "" : DodavatelS21.SAD205 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD305) ? "" : DodavatelS21.SAD305 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD405) ? "" : DodavatelS21.SAD405 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD505) ? "" : DodavatelS21.SAD505 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.PSC) ? "" : "\nPSÈ: " + DodavatelS21.PSC)
                    + $"\nmìna: {DodavatelS21.CURN05}"
                    + ""
                    ;

                SelectedDodavatel += $"\nemail: { DodavatelS21.WURL05}\n";

                if (Osoba != null)
                {
                    SelectedDodavatel += Osoba.CNTN1A;
                }
                
                SaveDodavatel();
                //PozadavekData.DodavatelS21ID = dodId;
                SavePozadavek();
                //PozadavkyService.LastS21DodId = dodId;
                ChangeMena();
            }
            else
            {
                AlertText = "Není zvolený dodavatel, nejprve jednoho vyberte nebo založte nového!";
                DodavateleInit();
            }
        }

        // nacte dodavatele a osoby z S21 podle dod. ID
        public void LoadDodavatele(int dodId)
        {
            ClearAlerts();
            if (dodId != 0)
            {
                DodavatelS21 = DodavateleS21.Find(d => d.Id == dodId);
                DodavatelS21.PSC = DodavatelS21.PCD105 + DodavatelS21.PCD205;
                Osoba = DodavatelService.GetOdpOsobaByDodavatelId(dodId);

                SelectedDodavatel = DodavatelS21.SNAM05 + ", " + DodavatelS21.SUPN05
                    + "\n"
                    + (String.IsNullOrEmpty(DodavatelS21.SAD105) ? "" : DodavatelS21.SAD105 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD205) ? "" : DodavatelS21.SAD205 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD305) ? "" : DodavatelS21.SAD305 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD405) ? "" : DodavatelS21.SAD405 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.SAD505) ? "" : DodavatelS21.SAD505 + ", ")
                    + (String.IsNullOrEmpty(DodavatelS21.PSC) ? "" : "\nPSÈ: " + DodavatelS21.PSC)
                    + $"\nmìna: {DodavatelS21.CURN05}"
                    + ""
                    ;

                SelectedDodavatel += $"\nemail: { DodavatelS21.WURL05}\n";

                if (Osoba != null)
                {
                    SelectedDodavatel += Osoba.CNTN1A;
                }

                SaveDodavatel();
                //PozadavekData.DodavatelS21ID = dodId;
                SavePozadavek();
                //PozadavkyService.LastS21DodId = dodId;
                ChangeMena();
            }
            else
            {
                AlertText = "Není zvolený dodavatel, nejprve jednoho vyberte nebo založte nového!";
                DodavateleInit();
            }
        }

        public void DodavateleInit()
        {
            if (DodavateleS21List.Count == 0)
            {
                DodavateleS21 = DodavatelService.GetS21DodavateleOrderedByName().Where(w => w.NazevCislo != null && w.NazevCislo.Length > 3).ToList();
                DodavateleS21List = DodavateleS21.Select(s => s.NazevCislo).ToList();
                //DodavateleS21List = DodavateleS21.Select(s => new DodavateleList() { id = s.Id, nazev = s.NazevCislo}).ToList();
               // DodavateleS21ByNumb = DodavatelService.GetS21DodavateleOrderedByNumb();
            }
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
                   + (String.IsNullOrEmpty(DodavatelS21.PSC) ? "" : "\nPSÈ: " + DodavatelS21.PSC)
                + $"\nmìna: {DodavatelS21.CURN05}"
                + ""
                ;
            SelectedDodavatel += $"\nemail: { DodavatelS21.WURL05}\n";

            if (Osoba != null)
            {
                SelectedDodavatel += Osoba.CNTN1A;
            }

            try
            {
                SaveDodavatel();
                SavePozadavek();
                ChangeMena();

            }
            catch (Exception ex)
            {
                if (ex.InnerException == null) AlertText = ex.Message;
                else AlertText = ex.InnerException.Message;
            }

            // PozadavkyService.LastS21DodId = PozadavekData.DodavatelS21ID;
            Context.ResourceManager.AddStartupScript("$('div[data-id=dodavatel-detail]').modal('hide');");

            //Context.RedirectToRoute("PozadavekEdit", new { Id = PozadavekData.ID });
        }

        public void DodavatelReset()
        {
            DodavatelS21 = new DodavateleS21();
            Osoba = new OdpOsoby();
        }

        public void NabidkaNeuvedenaCheck()
        {
            if (NabidkaNeuvedena) PozadavekData.NabidkaCislo = "Neuvedeno...";
        }


        // ------------------------  POZADAVEK ------------------------------------------------------
        public bool NewPozadavek { get; set; } = false;

        public void Tisk()
        {
            ClearAlerts();
            if (PozadavekData.PodpisLevel == 0)
                SavePozadavek();
            // string juliFile = @"\\juli-app\Pozadavky";
            string juliFile = @"\\juli-app\Pozadavky";
            
            string mainDirectory = PozadavkyService.GetYearFromFullId(PozadavekData.FullPozadavekID) != "" ?
                PozadavkyService.GetYearFromFullId(PozadavekData.FullPozadavekID) : DateTime.Now.Year.ToString();
            string subDir = "P" + mainDirectory + PozadavekData.ID;

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

            MemoryStream stream;
            int id = 0;

            try
            {
                stream = TiskServices.CreatePdfPozadavkovyListByPozadavekId(PozadavekData.ID);

                string pozId = PozadavkyService.GePozNumber(PozadavekData.FullPozadavekID);
                if (pozId == "") pozId = PozadavekData.ID.ToString();

                id = FilesService.SaveFileGetId(stream, fullPath, "Tiskova_sestava_pozadavek_" + pozId + ".pdf", 0, PozadavekData.ID, 0, "Tisková sestava požadavku.");
            }
            catch (Exception ex)
            {
                AlertText = ex.Message;
            }

            if (String.IsNullOrEmpty(AlertText))
            {
                System.Threading.Thread.Sleep(1000);
                Context.RedirectToRoute("FileDownloadPDF", new { Id = id });
            }
        }

        public void InvesticeChanged()
        {
            switch (InvesticeVyber)
            {
                case 1:
                    PozadavekData.InvesticeNeplanovana = true;
                    PozadavekData.InvesticePlanovana = false;
                    PozadavekData.NakupOstatni = false;
                    break;
                case 2:
                    PozadavekData.InvesticeNeplanovana = false;
                    PozadavekData.InvesticePlanovana = true;
                    PozadavekData.NakupOstatni = false;
                    break;
                case 3:
                    PozadavekData.InvesticeNeplanovana = false;
                    PozadavekData.InvesticePlanovana = false;
                    PozadavekData.NakupOstatni = true;
                    break;
                default:
                    PozadavekData.InvesticeNeplanovana = false;
                    PozadavekData.InvesticePlanovana = false;
                    PozadavekData.NakupOstatni = true;
                    break;
            }
        }

        public void InvesticeItemChanged()
        {
            switch (InvesticeVyber)
            {
                case 1:
                    ItemData.InvesticeNeplanovana = true;
                    ItemData.InvesticePlanovana = false;
                    ItemData.NakupOstatni = false;
                    break;
                case 2:
                    ItemData.InvesticeNeplanovana = false;
                    ItemData.InvesticePlanovana = true;
                    ItemData.NakupOstatni = false;
                    break;
                case 3:
                    ItemData.InvesticeNeplanovana = false;
                    ItemData.InvesticePlanovana = false;
                    ItemData.NakupOstatni = true;
                    break;
                default:
                    ItemData.InvesticeNeplanovana = false;
                    ItemData.InvesticePlanovana = false;
                    ItemData.NakupOstatni = true;
                    break;
            }
        }

        public void ZrusitPodpisyPotvrzeni()
        {
            ClearAlerts();
            if (PozadavekData.Level1Schvaleno || PozadavekData.Level1Odeslano)
                Context.ResourceManager.AddStartupScript("$('div[data-id=confirm]').modal({backdrop: 'static'});");

        }

        public void ZrusitPodpisy()
        {
            PozadavekData.Level1Odeslano = false;
            PozadavekData.Level2Odeslano = false;
            PozadavekData.Level3Odeslano = false;

            PozadavekData.Level1Schvaleno = false;
            PozadavekData.Level2Schvaleno = false;
            PozadavekData.Level3Schvaleno = false;

            PozadavekData.PodpisLevel = 0;
            PozadavekData.Stav = "Koncept";
            SavePozadavek();
            Context.ResourceManager.AddStartupScript("$('div[data-id=confirm]').modal('hide');");
        }

        public void Prepocitat()
        {
            ItemData.CelkovaCena = ItemData.CenaZaJednotku * ItemData.Mnozstvi;
            CelkovaCenaMena = ItemData.CelkovaCena.ToString("n2");
            CelkovaCenaMena += (" " + PozadavekData.Mena);
        }

        //public void Prepocitat()
        //{
        //    PozadavekData.CelkovaCena = PozadavekData.CenaZaJednotku * PozadavekData.Mnozstvi;
        //}

        // save Pozadavek, po odeslani pozadavku zrusi podpisy a odeslani
        public void Save()
        {
            ClearAlerts();
            if (PozadavekData.Level1Schvaleno)
                Context.ResourceManager.AddStartupScript("$('div[data-id=confirm]').modal({backdrop: 'static'});");

            //SavePozadavek();
            // Context.RedirectToRoute("Default"); /* , new { Id = PozadavekData.ObjednavkaID} */
        }

        public void SavePozadavek()
        {
            var items = SeznamItemsGridViewDataSet.Items.ToList();


            // doplneni KST
            if (!string.IsNullOrEmpty(PozadavekData.KST))
            {
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.KST))
                    {
                        var itm = ItemsService.GetItemById(item.ID);
                        itm.KST = PozadavekData.KST;
                        ItemsService.ItemSave(itm);
                    }
                }
            }

            // doplneni nabidky do polozek pokud nemaji
            if (!string.IsNullOrEmpty(PozadavekData.NabidkaCislo))
            {
                foreach (var item in items)
                {                                       
                    var itm = ItemsService.GetItemById(item.ID);
                    if (string.IsNullOrEmpty(itm.NabidkaCislo))
                        itm.NabidkaCislo = PozadavekData.NabidkaCislo;
                    ItemsService.ItemSave(itm);                    
                }
            }

            if (!string.IsNullOrEmpty(PozadavekData.CisloKonta) && PozadavekData.NakupOstatni == true)
            {
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.CisloKonta) && string.IsNullOrEmpty(item.CisloInvestice) && !item.InvesticeNeplanovana)
                    {
                        var itm = ItemsService.GetItemById(item.ID);
                        itm.NakupOstatni = true;
                        itm.CisloKonta = PozadavekData.CisloKonta;
                        ItemsService.ItemSave(itm);
                    }
                }
            }

            if (!string.IsNullOrEmpty(PozadavekData.CisloInvestice) && PozadavekData.InvesticePlanovana == true)
            {
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.CisloKonta) && string.IsNullOrEmpty(item.CisloInvestice) && !item.InvesticeNeplanovana)
                    {
                        var itm = ItemsService.GetItemById(item.ID);
                        itm.InvesticePlanovana = true;
                        itm.CisloInvestice = PozadavekData.CisloInvestice;
                        ItemsService.ItemSave(itm);
                    }
                }
            }

            if (NewPozadavek)
            {
                PozadavekData.Stav = "Koncept";
                PozadavekData.ID = PozadavkyService.PozadavekInsert(PozadavekData);
                //PozadavekData.FullPozadavekID = PozadavekData.ID.ToString();
                NewPozadavek = false;
            }
            else
            {
                PozadavekData.PocetPolozek = ItemsService.GetPocetItemsByPozadavekId(PozadavekData.ID);
                PozadavkyService.PozadavekSave(PozadavekData);
                PozadavkyService.LastPozId = PozadavekData.ID;
            }

         
                foreach (var file in Files)
                {
                    FilesService.SaveFileDescription(file.Description, file.ID);
                }
            

            ConfirmText += "Požadavek uložen...";
        }

        public void Instance1Pripravit()
        {
            //VALIDACE
            ClearAlerts();

            try
            {
                var items = SeznamItemsGridViewDataSet.Items.ToList();
                if (items.Count == 0) throw new ValidationException("items");

                if (string.IsNullOrEmpty(PozadavekData.KST) && !PozadavekData.InvesticePlanovana)
                {
                    foreach (var item in items)
                    {
                        if (string.IsNullOrEmpty(item.KST))
                            throw new ValidationException("KST");
                    }

                    PozadavekData.KST = items.First().KST;
                }

                NabidkaNeuvedenaCheck();
                if (string.IsNullOrEmpty(PozadavekData.NabidkaCislo) && NabidkaNeuvedena == false)
                {
                    foreach (var item in items)
                    {
                        if (string.IsNullOrEmpty(item.NabidkaCislo))
                            throw new ValidationException("nabidka");
                    }
                    PozadavekData.NabidkaCislo = items.First().NabidkaCislo;
                }

                if ((string.IsNullOrEmpty(SelectedDodavatel)) || (PozadavekData.DodavatelID == 0))
                {
                    throw new ValidationException("dodavatel");
                }

                if (!string.IsNullOrEmpty(PozadavekData.KST))
                {
                    foreach (var item in items)
                    {
                        if (string.IsNullOrEmpty(item.KST))
                        {
                            var itm = ItemsService.GetItemById(item.ID);
                            itm.KST = PozadavekData.KST;
                            ItemsService.ItemSave(itm);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(PozadavekData.NabidkaCislo))
                {
                    foreach (var item in items)
                    {
                        if (string.IsNullOrEmpty(item.NabidkaCislo))
                        {
                            var itm = ItemsService.GetItemById(item.ID);
                            itm.NabidkaCislo = PozadavekData.NabidkaCislo;
                            ItemsService.ItemSave(itm);
                        }
                    }
                }

                foreach (var file in Files)
                {
                    if (string.IsNullOrEmpty(file.Description))
                    {
                        file.Description = "Nabídka.";
                        FilesService.SaveFileDescription(file.Description, file.ID);
                    }
                        
                }

                if (!PozadavekData.InvesticeNeplanovana && !PozadavekData.InvesticePlanovana && !PozadavekData.NakupOstatni)
                {
                    var it = ItemsService.GetItemById(items.First().ID);
                    if (it.InvesticeNeplanovana) PozadavekData.InvesticeNeplanovana = true;
                    if (it.InvesticePlanovana)
                    {
                        PozadavekData.InvesticePlanovana = true;
                        PozadavekData.CisloInvestice = it.CisloInvestice;
                    }
                    if (it.NakupOstatni)
                    {
                        PozadavekData.NakupOstatni = true;
                        PozadavekData.CisloKonta = it.CisloKonta;
                    }
                }



                foreach (var item in items)
                {
                    var it = ItemsService.GetItemById(item.ID);
                    if (it.Poptany3Firmy == false & it.DuvodID == 1 & PozadavekData.CelkovaCena > 15000)
                        throw new ValidationException("poptavka");                                       
                }


                OdeslatNaSchvaleni = true;
                SavePozadavek();
            }
            catch (ValidationException exc)
            {
                if (exc.Message == "items")
                    AlertText = "Prosím, pøidejte do požadavku alespoò jednu položku!";

                if (exc.Message == "KST")
                    AlertText = "Není vyplnìno støedisko KST, vyplòte buï hlavní u požadavku nebo u každé položky";

                if (exc.Message == "nabidka")
                    AlertText = "Není vyplnìna nabídka, vyplòte buï hlavní u požadavku nebo u každé položky, nebo zaškrtnìte políèko \"Neuvedeno\"";

                if (exc.Message == "poptavka")
                    AlertText = $"Máte nastaveno nepoptány 3 firmy z dùvodu celkové ceny do 15.000,- Kè, ale celková cena požadavku je {PozadavekData.CelkovaCena}!";


                if (exc.Message == "dodavatel")
                {
                    if (PozadavekData.DodavatelS21ID != 0)
                    {
                        LoadDodavatele(DodavatelSelected);
                        AlertText = "Nebyl nahrán DODAVATEL, nahrán aktuálnì zvolený, prosím o kontrolu v oknì \"Vybraný dodavatel\", " +
                            "popø. jeho úpravu tlaèítkem \"Nový / Edit dodavatele\"";
                    }

                    else
                    {
                        DodavateleInit();
                        AlertText = "Není vyplnìn DODAVATEL, naèítám databázi dodavatelù, po výbìru dodavatele kliknìte na tlaèítko " +
                            "\"Pøiøadit dodavatele k požadavku\" popø. jeho úpravu nebo vytvoøení nového tlaèítkem \"Nový / Edit dodavatele\"";
                    }
                }

                OdeslatNaSchvaleni = false;

                Context.ResourceManager.AddStartupScript(" $('html, body').animate({ scrollTop: $(document).height() }, 'slow');");
            }

        }

        public void OdeslatNaPodpis(int level)
        {
            SavePozadavek();
            //string kst = String.IsNullOrEmpty(PozadavekData.KST) ? SeznamItemsGridViewDataSet.Items[0].KST : PozadavekData.KST;
            string rok = DateTime.Now.Year.ToString();
            int lastNumberOfYear = PozadavkyService.GetLastNumberOfYear(rok);
            //string id = PozadavekData.ID.ToString();
            string newNumber = (lastNumberOfYear + 1).ToString().PadLeft(4, '0');
            if (string.IsNullOrEmpty(PozadavekData.FullPozadavekID))
                PozadavekData.FullPozadavekID = "p" + DateTime.Now.Year.ToString() + "/" + newNumber;
            PozadavekData.PodpisLevel = level;
            PozadavekData.Zamitnuto = false;
            if (level == 1)
            {
                PozadavekData.Level1Odeslano = true;
                PozadavekData.Level1OdeslanoDne = DateTime.Now;
                PozadavekData.Stav = "Odesláno na podpis";
            }

            Vysledek = PozadavkyService.PozadavekSchvalit(PozadavekData); // .ID, level, PozadavekData.Level1SchvalovatelID);            
            OdeslatNaSchvaleni = false;
        }

        public void ChangeMena()
        {
            if (PozadavekData.Mena == "CZK") MenaClass = "form-control";
            if (PozadavekData.Mena != "CZK") MenaClass = "form-control mena-cizy";
            if (DodavatelS21.CURN05 != PozadavekData.Mena) MenaClass = "form-control mena-error";
        }

        public void Neodesilat()
        {
            if (!string.IsNullOrEmpty(PozadavekData.Poznamka))
                PozadavekData.Poznamka += "; OBJEDNÁVKU NEODESÍLAT!";
            else PozadavekData.Poznamka = "OBJEDNÁVKU NEODESÍLAT!";

            Context.ResourceManager.AddStartupScript("$('#BtnNeodesilat').toggleClass('btn-color');");
        }

        // ------------------------  ITEMS ------------------------------------------------------
        public ItemsDTO ItemData { get; set; } = new ItemsDTO();
        public int ItemId { get; set; }
        public float RozuctovatItems { get; set; } = 0;
        public GridViewDataSet<ItemsDTO> SeznamItemsGridViewDataSet { get; set; } = new GridViewDataSet<ItemsDTO>()
        {
            PageSize = 15,
            SortExpression = nameof(ItemsDTO.ID),
            SortDescending = false
        };

        public void EditItem(int? ItemId)
        {
            ClearAlerts();
            if (Editovatelny)
                SavePozadavek();

            //if (NewPozadavek)
            //{
            //    NewPozadavek = false;
            //    ConfirmText = "Pozadavek založen, nyní s ním mùžete pracovat...";
            //    Context.RedirectToRoute("PozadavekEdit", new { Id = PozadavekData.ID });
            //}

            if (ItemId == 0) // nova polozka
            {
                SaveButtonText = "Uložit a pøidat novou položku";
                NewItem = true;
                ItemData = new ItemsDTO();
                ItemData.Zalozil = UserServices.GetActiveUser();
                ItemData.DatumZalozeni = DateTime.Now;
                ItemData.DatumObjednani = null;
                ItemData.TerminDodani = (DateTime.Now.AddDays(14));
                ItemData.KST = PozadavekData.KST;
                ItemData.InvesticeNeplanovana = PozadavekData.InvesticeNeplanovana;
                ItemData.InvesticePlanovana = PozadavekData.InvesticePlanovana;
                ItemData.NakupOstatni = PozadavekData.NakupOstatni;
                ItemData.CisloInvestice = PozadavekData.CisloInvestice;
                ItemData.CisloKonta = PozadavekData.CisloKonta;
                ItemData.NabidkaCislo = PozadavekData.NabidkaCislo;
            }
            else
            {
                ItemData = ItemsService.GetItemById(ItemId.Value);

                if (ItemData.ID == LastItemId)
                    SaveButtonText = "Uložit a pøidat novou položku";
                else SaveButtonText = "Uložit a další";

                ItemData.DodavatelID = PozadavekData.DodavatelID;
                Files = FilesService.GetFilesListByItemID(ItemData.ID);
                NewItem = false;
                DuvodChange();
                Prepocitat();
                // ParrentItemId = PozadavekData.ItemId;
            }

            Context.ResourceManager.AddStartupScript("$('div[data-id=item-detail]').modal({backdrop: 'static'});");
        }

        public void SaveItem()
        {
            ClearAlerts();
            Prepocitat();
            ItemData.PozadavekID = PozadavekData.ID;
            ItemData.DodavatelID = PozadavekData.DodavatelID;

            try
            {
                if (NewItem)
                {
                    ItemData.ID = ItemsService.ItemInsert(ItemData);
                    //NewItem = false;
                }
                else
                {
                    ItemsService.ItemSave(ItemData);
                }

                PozadavekData.CelkovaCena = ItemsService.GetCelkovaCenaByPozadavekId(PozadavekId.Value);
                PozadavekData.PocetPolozek = ItemsService.GetPocetItemsByPozadavekId(PozadavekData.ID);
                SavePozadavek();
            }
            catch (Exception ex)
            {
                if (ItemData.Jednotka == null)
                    AlertText = "Prosím vyplòte jednotku! ";

                if (ex.InnerException == null) AlertText += ex.Message;
                else
                {
                    AlertText += ex.InnerException.Message;
                    if (ex.InnerException.InnerException.Message != null)
                        AlertText += ex.InnerException.InnerException.Message;
                }
            }

            if (AlertText == null)
                if (Rozuctovani)
                {
                    ConfirmText = $"Položka v poøádku uložena, mùžete pokraèovat v rozúètování položek, zbývá {ZbytekRozuctovani} jednotek";
                    if (ZbytekRozuctovani == 0)
                    {
                        ConfirmText = "Rozúètování dokonèeno!";
                        Rozuctovani = false;
                        NewItem = false;
                        Context.ResourceManager.AddStartupScript("$('div[data-id=item-detail]').modal('hide');");
                    }
                        
                    NewItem = true;
                    
                }        
        }

        public void SaveItemAndNew()
        {
            SaveItem();

            if ((ItemData.ID == LastItemId) || NewItem) // jsme na posledni polozce
            {
                string LastKST = ItemData.KST;
                bool LastInvesticeNeplanovana = ItemData.InvesticeNeplanovana;
                bool LastInvesticePlanovana = ItemData.InvesticePlanovana;
                bool LastNakupOstatni = ItemData.NakupOstatni;
                string LastCisloInvestice = ItemData.CisloInvestice;
                string LastCisloKonta = ItemData.CisloKonta;
                string LastJednotka = ItemData.Jednotka;
                DateTime? LastTermin = ItemData.TerminDodani;

                if (AlertText == null)
                {
                    ConfirmText = "Položka v poøádku uložena, mùžete psát další nebo kliknout na tl. \"Ukonèit\"...";
                    SaveButtonText = "Uložit a pøidat novou položku";
                    NewItem = true;
                    ItemData = new ItemsDTO();
                    ItemData.Zalozil = UserServices.GetActiveUser();
                    ItemData.DatumZalozeni = DateTime.Now;
                    // ItemData.DatumObjednani = DateTime.Now;
                    // ItemData.TerminDodani = (DateTime.Now.AddDays(7));
                    ItemData.KST = LastKST;
                    ItemData.InvesticeNeplanovana = LastInvesticeNeplanovana;
                    ItemData.InvesticePlanovana = LastInvesticePlanovana;
                    ItemData.NakupOstatni = LastNakupOstatni;
                    ItemData.CisloInvestice = LastCisloInvestice;
                    ItemData.CisloKonta = LastCisloKonta;
                    ItemData.DodavatelID = PozadavekData.DodavatelID;
                    ItemData.NabidkaCislo = PozadavekData.NabidkaCislo;
                    ItemData.Jednotka = LastJednotka;
                    ItemData.TerminDodani = LastTermin;
                }
            }
            else // jeste neni konec
            {
                var index = GetGViewIndexById(SeznamItemsGridViewDataSet, ItemData.ID); // zjisti kde jsme

                ItemData = ItemsService.GetItemById(SeznamItemsGridViewDataSet.Items[index + 1].ID); // nacti dalsi

                if (ItemData.ID == LastItemId)
                    SaveButtonText = "Uložit a pøidat novou položku";
                else SaveButtonText = "Uložit a další";

                ItemData.DodavatelID = PozadavekData.DodavatelID;
                Files = FilesService.GetFilesListByItemID(ItemData.ID);
                NewItem = false;
                DuvodChange();
                Prepocitat();
            }           
        }

        private int GetGViewIndexById(GridViewDataSet<ItemsDTO> grid, int id)
        {
            foreach (var item in grid.Items)
            {
                if (item.ID == id)
                {
                    return grid.Items.IndexOf(item);
                }
            }

            return -1;
        }

        public void DeleteItem(int id)
        {
            try
            {
                ItemsService.DeleteItem(id);
            }
            catch (Exception ex)
            {
                AlertText = ex.Message;
            }
        }

        public float ZbytekRozuctovani { get; set; } = 0;

        public void Rozuctovat()
        {
            // v RozuctovatItems je kolik nechat, zbytek pùjde do nové položky

            Rozuctovani = true;
            var origMnozstvi = ItemData.Mnozstvi;            
            ItemData.Mnozstvi = RozuctovatItems;
            ZbytekRozuctovani = origMnozstvi - ItemData.Mnozstvi;
            SaveItem();

            ItemData.Mnozstvi = origMnozstvi - RozuctovatItems;
            
            RozuctovatItems = ItemData.Mnozstvi;
        }

        public Reasons[] DuvodyPoptavky { get; set; } =
          {
            new Reasons() { ID = 1, Text = "Nejlepší cena"},
            new Reasons() { ID = 2, Text = "Nejlepší kvalita"},
            new Reasons() { ID = 3, Text = "Nejrychlejší dodávka"},
            new Reasons() { ID = 4, Text = "(*) Speciální Dodavatel"},
            new Reasons() { ID = 5, Text = "(*) Speciální materiál"},
            new Reasons() { ID = 6, Text = "Jiný dùvod: "}
        };

        public Reasons[] DuvodyNepoptani { get; set; } =
        {
            new Reasons() { ID = 1, Text = "Celková výše požadavku (objednávky) do 15.000,- Kè bez DPH"},
            new Reasons() { ID = 2, Text = "Servisní zásah"},
            new Reasons() { ID = 3, Text = "Havárie"},
            new Reasons() { ID = 4, Text = "Porucha"},
            new Reasons() { ID = 5, Text = "(*) Speciální Dodavatel"},
            new Reasons() { ID = 6, Text = "(*) Speciální materiál"},
            new Reasons() { ID = 7, Text = "Jiný dùvod: "}
        };

        public class Reasons
        {
            public Reasons() { }
            public int ID { get; set; }
            public string Text { get; set; }
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
        // ------------------------  FILES ------------------------------------------------------
        public bool FileDescrptChange { get; set; } = false;
        public UploadedFilesCollection UploadedFiles { get; set; } = new UploadedFilesCollection();

        public void DeleteFile(int id)
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

        public void SaveDescription(string popis, int id)
        {
            try
            {
                FilesService.SaveFileDescription(popis, id);
                Vysledek = "Zmìny uloženy...";
            }
            catch (Exception ex)
            {
                AlertText = ex.Message;
            }
        }

        public void FilesUploadedComplete()
        {
            try
            {
                var storage = Context.Configuration.ServiceLocator.GetService<IUploadedFileStorage>();

                foreach (var file in UploadedFiles.Files)
                {
                    if (file.IsAllowed)
                    {
                        var stream = storage.GetFile(file.FileId);


                        string juliFile = @"\\juli-app\Pozadavky";
                        string mainDirectory = DateTime.Now.Year.ToString();
                        string subDir = "P" + DateTime.Now.Year + PozadavekData.ID;

                        string fullPath = Path.Combine(juliFile, mainDirectory, subDir);
                        string filename = Path.GetFileName(file.FileName);

                        filename.Replace('\"', '\'');

                        FilesService.SaveFile(stream, fullPath, filename, 0, PozadavekData.ID);

                        storage.DeleteFile(file.FileId);
                    }
                    else
                    {
                        AlertText = "nahrání tìchto souborù není povoleno!";
                    }
                 
                }

                SavePozadavek();
                UploadedFiles.Clear();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null) AlertText += ex.Message;
                else AlertText += ex.InnerException.Message;
            }
 
        }

        public void PozadavekPodepsat()
        {
            SavePozadavek();
            var signmodel = new SignViewModel();
            signmodel.PozadavekSchvalit(PozadavekData.ID, PozadavekData.PodpisLevel + 1);
            Context.RedirectToRoute("PozadavekEdit", new { Id = PozadavekData.ID });
        }

        //public void ChangeDescription()
        //{
        //    FileDescrptChange = true;
        //}

        //public void AddItem()
        //{
        //    //ClearAlerts();
        //    SaveItem();
        //    NewItem = true;
        //    ItemData = new ItemsDTO();
        //    ItemData.Zalozil = ActiveUser;
        //    ItemData.DatumZalozeni = DateTime.Now;
        //    ItemData.DatumObjednani = DateTime.Now;
        //    ItemData.TerminDodani = (DateTime.Now.AddDays(7));

        //    Context.ResourceManager.AddStartupScript("$('div[data-id=item-detail]').modal('show');");
        //    //Context.RedirectToRoute("ItemEdit", new { Id = 0 });
        //}

        //public void ClearAlerts()
        //{
        //    AlertText = null;
        //    ConfirmText = null;
        //    Vysledek = null;
        //}



        //public void DuvodChange()
        //{
        //    if ((PozadavekData.Poptany3Firmy == true && PozadavekData.DuvodID == 6) 
        //           || (PozadavekData.Poptany3Firmy == false && PozadavekData.DuvodID == 7))
        //    {
        //        JinyDuvod = true;
        //    }
        //    else JinyDuvod = false;
        //}


        // ------------------------  PRERENDER ------------------------------------------------------

        public override Task PreRender()
        {
            //  txtItemPopis.Focus();

            //PozadavkyService.LastPozId = PozadavekId.Value;                 
            if (!Context.IsPostBack)
            {
                KSTlist = ObjednavkyService.GetKSTList();
                InvList = ObjednavkyService.GetInvList();
                if (PozadavekId == 0) // novy pozadavek
                {
                    NewPozadavek = true;

                    PozadavekData.Zalozil = UserServices.GetActiveUser();
                    PozadavekData.Datum = DateTime.Now;
                    PozadavekData.NakupOstatni = true;
                    PozadavekData.Stav = "Koncept";
                    SavePozadavek();

                    Context.RedirectToRoute("PozadavekEdit", new { Id = PozadavekData.ID });

                }
                else
                {
                    try
                    {
                        //Dodavatel = new DodavateleDTO();
                        PozadavekData = PozadavkyService.GetPozadavekById(PozadavekId.Value);

                        //PozadavkyService.LastS21DodId = PozadavekData.DodavatelS21ID;
                        //if (PozadavekData.FullPozadavekID == null) PozadavekData.FullPozadavekID = PozadavekData.ID.ToString();
                        if (PozadavekData.InvesticeNeplanovana) InvesticeVyber = 1;
                        else if (PozadavekData.InvesticePlanovana) InvesticeVyber = 2;
                        else if (PozadavekData.NakupOstatni) InvesticeVyber = 3;

                        vyplnenoKST = (PozadavekData.KST != null) ? true : false;

                        //VlastniPlatba = (PozadavekData.ZpusobPlatbyId == 6);

                        // kdo chce nahravat dodavatele na startu         
                        UsersDTO user = UserServices.GetUsersByUserName(UserServices.GetActiveUser()).FirstOrDefault();
                        // ZMENA if (user != null && user.NacitatDodavatele == true)
                        DodavateleInit();

                        if (PozadavekData.DodavatelID != 0)
                        {
                            DodavatelS21 = DodavatelService.GetDodavatelByIdAsS21(PozadavekData.DodavatelID);
                            PozadavekData.DodavatelS21ID = DodavatelS21.Id;

                            SelectedDodavatel = DodavatelS21.SNAM05 + ", " + DodavatelS21.SUPN05
                                + "\n"
                                + (String.IsNullOrEmpty(DodavatelS21.SAD105) ? "" : DodavatelS21.SAD105 + ", ")
                                + (String.IsNullOrEmpty(DodavatelS21.SAD205) ? "" : DodavatelS21.SAD205 + ", ")
                                + (String.IsNullOrEmpty(DodavatelS21.SAD305) ? "" : DodavatelS21.SAD305 + ", ")
                                + (String.IsNullOrEmpty(DodavatelS21.SAD405) ? "" : DodavatelS21.SAD405 + ", ")
                                + (String.IsNullOrEmpty(DodavatelS21.SAD505) ? "" : DodavatelS21.SAD505 + ", ")
                                + (String.IsNullOrEmpty(DodavatelS21.PSC) ? "" : "\nPSÈ: " + DodavatelS21.PSC)
                                + $"\nmìna: {DodavatelS21.CURN05}"
                               + ""
                               ;
                            SelectedDodavatel += $"\nemail: { DodavatelS21.WURL05}\n";

                            SelectedDodavatel += DodavatelS21.CNTN1A ?? "";

                            Osoba.CNTN1A = DodavatelS21.CNTN1A;
                            Osoba.EMIL1A = DodavatelS21.EMIL1A;
                            Osoba.GTX11A = DodavatelS21.GTX11A;
                        }

                        if (PozadavekData.Mena == "CZK") MenaClass = "form-control";
                        if (PozadavekData.Mena != "CZK") MenaClass = "form-control mena-cizy";
                        if (DodavatelS21.CURN05 != PozadavekData.Mena) MenaClass = "form-control mena-error";

                        if (PozadavekData.Level2Schvaleno && PozadavekData.Level2SchvalovatelID != 0)
                        {
                            PozadavekData.Level2SchvalovatelJmeno =
                                UserServices.GetUserById(PozadavekData.Level2SchvalovatelID).Jmeno;
                        }
                    }
                    catch (Exception ex)
                    {
                        AlertText = "Chyba pøi naèítání požadavku: ";

                        if (ex.InnerException == null) AlertText += ex.Message;
                        else
                        {
                            AlertText += ex.InnerException.Message;
                            if (ex.InnerException.InnerException.Message != null)
                                AlertText += ex.InnerException.InnerException.Message;
                        }
                    }
                    


                }

                Investice = InvesticeService.GetInvesticeList();
                //DodavateleInit();
                Schvalovatele = UserServices.GetUserByLevel(1);
            }

            // podpis level: 0 => novy pozadavek
            // podpis level: 1 => ceka na podpis vedoucim
            // podpis level: 2 => ceka na podpis reditel nebo control.
            // podpis level: 3 => vse podepsano, ceka na vytvoreni obj. a podpis nakupu
            // podpis level: 4 => vse podepsano, ceka na objednani

            // user level: 0 => bezny pracovnik
            // user level: 1 => vedouci
            // user level: 2 => reditel nebo control.
            // user level: 3 => objednavatel
            // user level: 4 => vedouci nakupu

            // NadrizenyUzivatel = (UserServices.GetActiveUserLevels().Contains(PozadavekData.PodpisLevel) || UserServices.GetActiveUserLevels().Contains(PozadavekData.PodpisLevel + 1));


            //NadrizenyUzivatel = ((PozadavekData.PodpisLevel == 1) 
            //    ? (UserServices.GetActiveUserLevels().Contains(1) || UserServices.GetActiveUserLevels().Contains(2))
            //    : PozadavekData.PodpisLevel == 2 ? UserServices.GetActiveUserLevels().Contains(2) : false;

            try
            {
                NadrizenyUzivatel = false;

                switch (PozadavekData.PodpisLevel)
                {
                    case 1:
                        if (UserServices.GetActiveUserLevels().Contains(2)) NadrizenyUzivatel = true;
                        if (UserServices.GetActiveUserLevels().Contains(1) && PozadavekData.Level2Schvaleno == false) NadrizenyUzivatel = true;
                        break;
                    case 2:
                        if (UserServices.GetActiveUserLevels().Contains(2) && PozadavekData.Level2Schvaleno == false) NadrizenyUzivatel = true;
                        break;
                }

                Editovatelny =
                    (UserServices.GetActiveUserLevels().Contains(99)) ||
                    (UploadedFiles.IsBusy == false && PozadavekData.Zamitnuto == false
                     && (PozadavekData.Level1Schvaleno == false || NadrizenyUzivatel == true));


                ZrusitelnyPodpis = (PozadavekData.Level1Odeslano && Editovatelny);

                Podepsatelny = ZrusitelnyPodpis && (UserServices.GetUsersByUserName(UserServices.GetActiveUser()).Select(i => i.ID).Contains(PozadavekData.SchvalovatelID) && PozadavekData.PodpisLevel == 1)
                    || (PozadavekData.PodpisLevel == 2 && UserServices.GetActiveUserLevels().Contains(2));

                //PozadavekData = PozadavkyService.GetPozadavekById(PozadavekId.Value);
                // PozadavekData.DodavatelS21ID = PozadavkyService.LastS21DodId;
                ItemsService.FillGridViewItemsByPozadavekId(SeznamItemsGridViewDataSet, PozadavekId.Value);
                Files = FilesService.GetFilesListByPozadavekID(PozadavekData.ID);
                PozadavekData.CelkovaCena = ItemsService.GetCelkovaCenaByPozadavekId(PozadavekId.Value);

                if (SeznamItemsGridViewDataSet.Items.Count() > 0)
                    LastItemId = SeznamItemsGridViewDataSet.Items.Last().ID;

                PozadavekData.PocetPolozek = ItemsService.GetPocetItemsByPozadavekId(PozadavekData.ID);

                if (PozadavekData.Level1Odeslano)
                {
                    PozadavekData.Level1OdeslanoJmeno =
                    UserServices.GetUserById(PozadavekData.SchvalovatelID).Jmeno;
                }

                if (PozadavekData.Level1Schvaleno && PozadavekData.Level1SchvalovatelID != 0)
                {
                    PozadavekData.Level1SchvalovatelJmeno =
                    UserServices.GetUserById(PozadavekData.Level1SchvalovatelID).Jmeno;
                }

                if (!Editovatelny)
                {
                    AlertText += " POZOR: Zmìny mùže provádìt pouze schvalovatel požadavku!";
                }

                if (PozadavekData.PodpisLevel >= 3)
                {
                    AlertText += " POZOR: Požadavek byl schválen, další zmìny nejsou možné!";
                }

                if (PozadavekData.Zamitnuto)
                {
                    AlertText += " POZOR: Požadavek ZAMÍTNUT, požadavek není možné dále upravovat.!";
                }
            }
            catch (Exception ex)
            {
                AlertText += "Chyba pøi naèítání požadavku: ";

                if (ex.InnerException == null) AlertText += ex.Message;
                else
                {
                    AlertText += ex.InnerException.Message;
                    if (ex.InnerException.InnerException.Message != null)
                        AlertText += ex.InnerException.InnerException.Message;
                }
            }
            
            return base.PreRender();
        }

        //public override Task Load()
        //{
        //    if (AlertText != null || ConfirmText != null || Vysledek != null)
        //    {
        //        Thread.Sleep(2000);
        //        ClearAlerts();
        //    }

        //    return base.Load();
        //}

    }
}

