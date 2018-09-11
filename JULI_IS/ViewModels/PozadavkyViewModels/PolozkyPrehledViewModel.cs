using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.Services;
using Pozadavky.DTO;
using DotVVM.Framework.Controls;
using Pozadavky.Data;
using System.IO;

namespace ViewModels.PozadavkyViewModels
{
	public class PolozkyPrehledViewModel : AppViewModel
    {
        //public int ActiveUserLevel { get; private set; } = UserServices.GetActiveUserLevel();

        public List<PozadavekDTO> SeznamPozadavku { get; set; } = new List<PozadavekDTO>();


        public List<ItemsDTO> SeznamCheckedItems { get; set; } = new List<ItemsDTO>();
           
        public bool CurrentUserOnly { get; set; } = false;

        public bool IQDodavateleSearch { get; set; } = true;

        public GridViewDataSet<ItemsDTO> SeznamItemsGv { get; set; } = new GridViewDataSet<ItemsDTO>();
        //{
        //    PageSize = 20,
        //    SortExpression = nameof(ItemsDTO.DatumZalozeni),
        //    SortDescending = true
        //};



        public void ChangeCurrentUser()
        {
            if (CurrentUserOnly)
            {
                //SeznamItemsGv.PagingOptions.PageSize = SeznamItemsGv.PagingOptions.TotalItemsCount;
                ItemsService.FillGridViewItemsByUser(SeznamItemsGv, UserServices.GetActiveUser());
                ListItems = SeznamItemsGv.Items.ToList();
                ItemsService.GridViewSetSort(SeznamItemsGv);

                //ListItems.ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID.Substring(6) + "-" + p.FullPozadavekID.Substring(1, 4) }));
                ListPozadavekFullId.Clear();
                ListItems.ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID}));
                ListPozadavekFullId2 = ListPozadavekFullId.OrderByDescending(o => o.id).Select(s => s.pozid).Distinct().ToList();
                ListPozadavekFullId2.RemoveAll(item => item == null);

                ListDodavatele.Clear();
                ListItems.ForEach(p => ListDodavatele.Add(p.FullDodavatelName));
                ListDodavatele = ListDodavatele.Distinct().ToList();
                ListDodavatele.RemoveAll(item => item == null);

                //ListItems.ForEach(p => ListDodavateleNumber.Add(p.FullDodavatelNumber));
                //ListDodavateleNumber = ListDodavateleNumber.Distinct().OrderBy(o => o).ToList();

                ListZalozil.Clear();
                ListItems.ForEach(p => ListZalozil.Add(p.Zalozil));
                ListZalozil = ListZalozil.Distinct().ToList();
                ListZalozil.RemoveAll(item => item == null);

                ListKST.Clear();
                ListItems.ForEach(p => ListKST.Add(p.Stredisko));
                ListKST = ListKST.Distinct().ToList();
                ListKST.RemoveAll(item => item == null);
            }
            else
            {
                //SeznamItemsGv.PagingOptions.PageSize = SeznamItemsGv.PagingOptions.TotalItemsCount;
                ItemsService.FillGridViewItemsByUser(SeznamItemsGv);
                ListItems = SeznamItemsGv.Items.ToList();
                ItemsService.GridViewSetSort(SeznamItemsGv);


                //ListItems.ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID.Substring(6) + "-" + p.FullPozadavekID.Substring(1, 4) }));
                ListPozadavekFullId.Clear();
                ListItems.ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID }));
                ListPozadavekFullId2 = ListPozadavekFullId.OrderByDescending(o => o.id).Select(s => s.pozid).Distinct().ToList();
                ListPozadavekFullId2.RemoveAll(item => item == null);

                ListDodavatele.Clear();
                ListItems.ForEach(p => ListDodavatele.Add(p.FullDodavatelName));
                ListDodavatele = ListDodavatele.Distinct().ToList();
                ListDodavatele.RemoveAll(item => item == null);



                //ListItems.ForEach(p => ListDodavateleNumber.Add(p.FullDodavatelNumber));
                //ListDodavateleNumber = ListDodavateleNumber.Distinct().OrderBy(o => o).ToList();

                ListZalozil.Clear();
                ListItems.ForEach(p => ListZalozil.Add(p.Zalozil));
                ListZalozil = ListZalozil.Distinct().ToList();
                ListZalozil.RemoveAll(item => item == null);

                ListKST.Clear();
                ListItems.ForEach(p => ListKST.Add(p.Stredisko));
                ListKST = ListKST.Distinct().ToList();
                ListKST.RemoveAll(item => item == null);
            }  
        }
        
        public bool NothingFound { get; set; } = false;

        public bool JenPozadavky { get; set; } = true;

        public void CopyPozadavek(int id)
        {
            ClearAlerts();
            try
            {
                PozadavekDTO PozadavekData;
                List<ItemsDTO> ItemsList;
                List<FilesDTO> FileList;

                ConfirmText = "Vytvářím nový požadavek...";

                PozadavekData = PozadavkyService.GetPozadavekById(id);
                ItemsList = ItemsService.GetItemsByPozadavekId(id);
                FileList = FilesService.GetFilesListByPozadavekID(id);

                PozadavekData.Datum = DateTime.Now;
                PozadavekData.FullPozadavekID = "koncept";
                PozadavekData.Zalozil = ActiveUser;
                PozadavekData.Smazano = false;
                PozadavekData.SchvalovatelID = 0;
                PozadavekData.Level1Odeslano = false;
                PozadavekData.Level2Odeslano = false;
                PozadavekData.Level3Odeslano = false;
                PozadavekData.Level1Schvaleno = false;
                PozadavekData.Level2Schvaleno = false;
                PozadavekData.Level3Schvaleno = false;

                PozadavekData.Objednano = false;
                PozadavekData.PodpisLevel = 0;
                PozadavekData.Zamitnuto = false;


                int NewID = PozadavkyService.PozadavekInsert(PozadavekData);


                ConfirmText = "Kopíruju položky do nového požadavku...";

                foreach (var item in ItemsList)
                {
                    item.PozadavekID = NewID;
                    item.DatumZalozeni = DateTime.Now;
                    item.ObjednavkaID = 0;
                    item.ObjednavkaFullID = "není";
                    ItemsService.ItemInsert(item);
                }


                ConfirmText = "Kopíruju přiložené soubory do nového požadavku...";

                string juliFile = @"\\juli-app\Pozadavky";
                string mainDirectory = DateTime.Now.Year.ToString();
                string subDir = "P" + DateTime.Now.Year + NewID;

                string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                foreach (var item in FileList)
                {


                    string fileout = Path.Combine(fullPath, item.FileName);


                    using (var stream = new FileStream(item.FullPath, FileMode.Open, FileAccess.Read))
                    {
                        using (var fsout = new FileStream(fileout, FileMode.Create, FileAccess.Write))
                        { stream.CopyTo(fsout); }
                    }

                    using (var db = new PozadavkyContext())
                    {
                        var file = new Pozadavky.Data.Files()
                        {
                            Description = item.Description,
                            FileName = item.FileName,
                            FullPath = fileout,
                            PozadavekID = NewID
                        };

                        db.Files.Add(file);
                        db.SaveChanges();
                    }

                }
                ConfirmText = $"Hotovo, požadavek zkopírován, nové ID: {NewID}";
            }
            catch (Exception ex)
            {

                AlertText = $"Chyba: {ex.Message} \n";
                AlertText += (ex.InnerException != null)
                     ? ex.InnerException.Message
                     : "";
            }

            Context.ResourceManager.AddStartupScript(" $('html, body').animate({ scrollTop: $(document).height() }, 'slow');");
        }


        // ----------------------  FILTRY -----------------------------------------------------
        public List<ItemsDTO> ListItems { get; set; } = new List<ItemsDTO>();
        public List<string> ListPozadavekFullId2 { get; set; } = new List<string>();
        public List<string> ListKST { get; set; } = new List<string>();
        public List<string> ListDodavatele { get; set; } = new List<string>();
        public List<string> ListDodavateleNumber { get; set; } = new List<string>();
        public List<string> ListZalozil { get; set; } = new List<string>();

        public List<string> SeznamSloupcuI { get; set; } = new List<string>();
        public List<string> SeznamSloupcuP { get; set; } = new List<string>();
        public string VybranySloupec { get; set; } = "";

        public List<PozList> ListPozadavekFullId { get; set; } = new List<PozList>();

        public class PozList
        {
            public int id { get; set; }
            public string pozid { get; set; }
        }

        public string ColumnFilter { get; set; } = "";
        public string WhereFilter { get; set; } = "";

        public string WhereFilterPozadavek { get; set; } = "";
        public string WhereFilterDodavatel { get; set; } = "";
        public string WhereFilterDodavatelNumber { get; set; } = "";        
        public string WhereFilterZalozil { get; set; } = "";
        public string WhereFilterKST { get; set; } = "";

        public string WhereFilterPolozkaLike { get; set; } = "";

        public DateTime DatumOd { get; set; } = DateTime.Today.AddYears(-1);
        public DateTime DatumDo { get; set; } = DateTime.Today;
        public bool SetDatum { get; set; } = false;
        public bool qSQL { get; set; } = false;

        public void SetFiltr(string filter)
        {
       
            ColumnFilter = filter;
            switch (filter)
            {
                case "FullPozadavekID":
                //int PomlckaIndex = WhereFilterPozadavek.LastIndexOf('-');
                //WhereFilterPozadavek = "p" + WhereFilterPozadavek.Substring(PomlckaIndex + 1, 4) + "-" + WhereFilterPozadavek.Substring(0, PomlckaIndex);
                    WhereFilter = WhereFilterPozadavek;
                    break;
                case "FullDodavatelName":
                    WhereFilter = WhereFilterDodavatel;
                    break;
                case "DodavatelNumber":
                    WhereFilter = WhereFilterDodavatelNumber;
                    break;
                case "Zalozil":
                    WhereFilter = WhereFilterZalozil;
                    break;
                case "KST":
                    WhereFilter = WhereFilterKST;
                    break;
                case "Polozka":
                    WhereFilter = WhereFilterPolozkaLike;
                    break;
            }

            if (filter == "Datum") SetDatum = true;
            else SetDatum = false;

            qSQL = false;
            
        }

       public void SetSQL()
        {
            ClearAlerts();
            qSQL = true;
            MailServices.SendMail("marek.novak@juli.cz", "Pozadavky - vlastni SQL",
                $"uzivatel: {UserServices.GetActiveUser()}<br>" +
                $"dotaz: {WhereFilter}"
                );
        }

        public List<string> GetSeznamSloupcu(string sloupce)
        {
            var list = new List<string>();

            if (sloupce == "I")
            {
                Items items = new Items();
                list = items.GetType().GetProperties().Select(s => "i." + s.Name).ToList();
            }
            else
            {
                Pozadavky.Data.Pozadavky poz = new Pozadavky.Data.Pozadavky();
                list = poz.GetType().GetProperties().Select(s => "p." + s.Name).ToList();
            }

            return list;
        }

        public void AddSloupec()
        {
            WhereFilter += VybranySloupec;
        }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                ActiveUserLevel = UserServices.GetActiveUserLevels();
                ActiveUser = UserServices.GetActiveUser();

                ItemsService.FillGridViewItemsByUser(SeznamItemsGv);
                ListItems = SeznamItemsGv.Items.ToList();           
                ItemsService.GridViewSetSort(SeznamItemsGv);
                SeznamItemsGv.RequestRefreshAsync();
                
                //ListItems.Where(w => (w.FullPozadavekID != null)).ToList().ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID ?? ""}));

                //ListItems.ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID }));
                //ListPozadavekFullId2 = ListPozadavekFullId.OrderByDescending(o => o.id).Select(s => s.pozid).Distinct().ToList();

                ListItems.Where(w => (w.FullPozadavekID != null)).ToList().ForEach(p => ListPozadavekFullId.Add(new PozList { id = p.PozadavekID, pozid = p.FullPozadavekID ?? "" }));
                ListPozadavekFullId2 = ListPozadavekFullId.OrderByDescending(o => o.id).Select(s => s.pozid).Distinct().ToList();

                ListItems.Where(w => (w.FullDodavatelName != null && w.FullDodavatelName.Length > 3)).ToList().ForEach(p => ListDodavatele.Add(p.FullDodavatelName));
                ListDodavatele = ListDodavatele.Distinct().OrderBy(o => o).ToList();
                //ListDodavatele.RemoveAll(item => item == null);

                //ListItems.ForEach(p => ListDodavateleNumber.Add(p.FullDodavatelNumber));
                //ListDodavateleNumber = ListDodavateleNumber.Distinct().OrderBy(o => o).ToList();
                //ListDodavateleNumber.RemoveAll(item => item == null);

                ListItems.ForEach(p => ListZalozil.Add(p.Zalozil ?? ""));
                ListZalozil = ListZalozil.Distinct().OrderBy(o => o).ToList();
                ListZalozil.RemoveAll(item => item == null);

                ListItems.ForEach(p => ListKST.Add(p.Stredisko));
                ListKST = ListKST.Distinct().OrderBy(o => o).ToList();
                ListKST.RemoveAll(item => item == null);

                SeznamSloupcuI = GetSeznamSloupcu("I");
                SeznamSloupcuP = GetSeznamSloupcu("P");
            }
            else
            {                
                // NE, nenatahne se strankovani a zobrazi to vsechny zaznamy
            }

            if (String.IsNullOrEmpty(WhereFilter)) ColumnFilter = "";

            if (CurrentUserOnly)
            {
                if (SetDatum)
                    ItemsService.FillGridViewItemsByUser(SeznamItemsGv, UserServices.GetActiveUser(), "", "", DatumOd, DatumDo);
                else
                    ItemsService.FillGridViewItemsByUser(SeznamItemsGv, UserServices.GetActiveUser(), ColumnFilter, WhereFilter, null, null, IQDodavateleSearch);
            }
            else
            {
                if (SetDatum)
                    ItemsService.FillGridViewItemsByUser(SeznamItemsGv, "", "", "", DatumOd, DatumDo);
                else
                    ItemsService.FillGridViewItemsByUser(SeznamItemsGv, "", ColumnFilter, WhereFilter, null, null, IQDodavateleSearch);
            }

            if (qSQL)
            {
                try
                {
                    ItemsService.FillGridViewItemsBySQL(SeznamItemsGv, WhereFilter);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "sqlinjection")
                    {
                        AlertText += "SQL INJECTION?!?!";
                    }
                    AlertText += "Chyba v SQL dotazu:\n";
                    if (ex.InnerException == null) AlertText += ex.Message;
                    else AlertText = ex.InnerException.Message;
                    Context.ResourceManager.AddStartupScript(" $('html, body').animate({ scrollTop: 0 }, 'slow');");
                }

            }
                

            NothingFound = SeznamItemsGv.PagingOptions.TotalItemsCount == 0;
      
            
            return base.PreRender();
        }


    }
}

