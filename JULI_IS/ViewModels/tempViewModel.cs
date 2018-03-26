using System;
using System.Globalization;
using System.IO;
using System.Web.UI;
using Pozadavky.Services;
using System.Threading.Tasks;
using Pozadavky.DTO;
using System.Collections.Generic;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using DotVVM.Framework.Hosting;
using Pozadavky.Data;

namespace ViewModels
{
    public class tempViewModel : AppViewModel
    {
        // if you want to write a large string with e.g tabulation and linebreaks, 
        // you shoud use the XTextFormatter class:
        // misto gfx
        // XTextFormatter xt = new XTextFormatter(gfx);

        public List<ItemsDTO> ListItems { get; set; } = new List<ItemsDTO>();
        public List<PozList> ListPozadavekFullId { get; set; } = new List<PozList>();
        public List<string> ListPozadavekFullId2 { get; set; } = new List<string>();
        public List<string> ListKST { get; set; } = new List<string>();
        public List<string> ListDodavatele { get; set; } = new List<string>();
        public List<string> ListDodavateleNumber { get; set; } = new List<string>();
        public List<string> ListZalozil { get; set; } = new List<string>();
        public GridViewDataSet<ItemsDTO> SeznamItemsGv { get; set; } = new GridViewDataSet<ItemsDTO>();



        public string pozId { get; set; }

        public class PozList
        {
            public int id { get; set; }
            public string pozid { get; set; }
        }


        public bool DeleteFile { get; set; } = false;
        public int DeleteFileId { get; set; }
        public string Text2 { get; set; }
        public int DodId { get; set; } = 1;

        public string Text { get; set; } = "TEMP ViewModel!";

        public List<DodavateleS21> Dodavatel { get; set; } = new List<DodavateleS21>();

        public GridViewDataSet<ItemsDTO> SeznamItemsGridViewDataSet { get; set; } = new GridViewDataSet<ItemsDTO>()
        {
            PageSize = 10,
            SortExpression = nameof(ItemsDTO.ID),
            SortDescending = false
        };

        public void java()
        {
            Context.ResourceManager.AddStartupScript("$.MessageBox('A plain MessageBox can replace Javascript's window.alert(), and it looks definitely better...');");

        }

        public void java2()
        {
            Context.ResourceManager.AddStartupScript("$.msgbox('jQuery is a fast and concise JavaScript Library that simplifies HTML document traversing, event handling, animating, and Ajax interactions for rapid web development.', { type: 'info'});");

        }


        public string SelectedCountry { get; set; }

        public List<string> Countries { get; set; } = new List<string> {
            "Czech Republic", "Slovakia", "United States"
        };






        public class CityData
        {

            public string Name { get; set; }

            public int Id { get; set; }

        }

        public List<DodavateleS21> DodavateleS21 { get; set; } = new List<DodavateleS21>();
        public List<DodavateleList> DodavateleS21ListClass { get; set; }
        public List<string> DodavateleS21List { get; set; } = new List<string>();
        public int DodavatelS21ID { get; set; }
        public string DodavatelCombo { get; set; }
        public DodavateleList vyber { get; set; }


        public class DodavateleList
        {
            public string nazev { get; set; }
            public int id { get; set; }
        }

        public void DodavateleInit()
        {
            // 500 jde, 1000 uz je moc
            //vyzkouset to jako list <string>, funguje pro vsechny
            // zkusim jeste jako objekt id, nazev   -> NEJDE
          DodavateleS21 = DodavatelService.GetS21DodavateleOrderedByName().Where(w => w.NazevCislo != null && w.NazevCislo.Length > 3).ToList();
          DodavateleS21List = DodavateleS21.Select(s => s.NazevCislo).ToList();
          DodavateleS21ListClass = DodavateleS21.Select(s => new DodavateleList { nazev = s.NazevCislo, id = s.Id }).ToList();
        }

        public string elapsedTime { get; set; }

        public void Click()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            DodavatelS21ID = 1111;

            //DodavatelS21ID =
            //    DodavateleS21ListClass.Where(w => w.nazev == DodavatelCombo).Select(s => s.id).FirstOrDefault();

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
          
        }

        public string vybranaDtb { get; set; } = "SQLConnection";
        public GridViewDataSet<PozadavekDTO> SeznamGv { get; set; } = new GridViewDataSet<PozadavekDTO>();


        public void Click2()
        {
            string dtb1 = "SQLConnection";
            string dtb2 = "SQLConnectionTest";

            if (vybranaDtb == dtb1) vybranaDtb = dtb2;
            else vybranaDtb = dtb1;

            var db = new PozadavkyContext(vybranaDtb);

            //db.ChangeDatabase(
            //    configConnectionStringName: "SQLConnectionTest"
            //    );

            AlertText = $"Databaze zmenena na {vybranaDtb}";

            var query = (from p in db.Pozadavky
                         from d in db.Dodavatele.Where(dod => dod.Id == p.DodavatelID).DefaultIfEmpty()
                         where p.Smazano == false
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
                             Stav =
                                p.Zamitnuto ? "Zamítnuto" :
                                p.PodpisLevel == 0 ? "Koncept" :
                                p.PodpisLevel == 1 ? "Odesláno na podpis" :
                                p.PodpisLevel == 2 ? "Èeká na øeditel / kontroling" :
                                p.PodpisLevel == 3 ? "Tvorba objednávky" :
                                p.PodpisLevel == 4 ? "Objednáno" : ""
                         });

            // lepsi, bud rucne pres Items, ale pres LoadFromQ se o vse stara sam
            SeznamGv.LoadFromQueryable(query);

           // PozadavkyService.FillGridViewPozadavkyByUser(SeznamGv);

        }

        // --------------------------------------------------------------------




        public void tisk()
        {
            string fullPath = Path.Combine("c:\\", "TEMP");

            MemoryStream stream;
            int id = 0;
            stream = TiskServices.CreatePdfObjednavkovyListByObjIdTest(54);

            try
            {
                id = FilesService.SaveFileGetId(stream, fullPath, "temp.pdf");
            }
            catch (Exception ex)
            {
                Text2 = ex.Message;
            }

            System.Threading.Thread.Sleep(1000);
            Context.RedirectToRoute("FileDownloadPDF", new { Id = id });
        }

        public void mail()
        {
            Text2 = MailServices.SendMail("marek.novak@juli.cz; lukas.grundel@juli.cz",
                         "TESTOVACI MAIL",
                         "Testovaci mail...", null, "objednavky"                         
                         );
        }


        public void copy()
        {
            //ClearAlerts();
            int[] pole = { 563, 564, 567, 568, 570, 573, 574, 576, 578 };

            foreach (var id in pole)
            {
                var pozadavekdata = PozadavkyService.GetPozadavekById(id);

                ItemsService.CopyItemsToObjByPoz(pozadavekdata);
            }

            //ConfirmText = "DONE";
        }


        public void print()
        {
            //Process printjob = new Process();

            //printjob.StartInfo.FileName = @"\\juli-app\Pozadavky\2017\P2017550\Tiskova_sestava_pozadavek_550.pdf";  //path of your file;

            //printjob.StartInfo.Verb = "Print";

            //printjob.StartInfo.CreateNoWindow = true;

            //printjob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            //PrinterSettings setting = new PrinterSettings();

            //setting.DefaultPageSettings.Landscape = true;

            //printjob.Start();
        }

         public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
               // DodavateleInit();
                try
                {


                    ItemsService.FillGridViewItemsByUser(SeznamItemsGv);
                    ListItems = SeznamItemsGv.Items.ToList();
                    ItemsService.GridViewSetSort(SeznamItemsGv);


                    ListItems.Where(w => (w.FullPozadavekID != null)).ToList().ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID ?? "" }));                    
                    ListPozadavekFullId2 = ListPozadavekFullId.OrderByDescending(o => o.id).Select(s => s.pozid).Distinct().ToList();



                }
                catch (Exception e)
                {
                   // AlertText = e.Message;
                }
                
            }

            return base.PreRender();
        }
    }
}
