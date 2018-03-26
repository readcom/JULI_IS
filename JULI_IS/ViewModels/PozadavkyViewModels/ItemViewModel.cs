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
using DotVVM.Framework.Hosting;

namespace ViewModels.PozadavkyViewModels
{
	public class ItemViewModel : AppViewModel
	{
	    public ItemsDTO ItemData { get; set; } = new ItemsDTO();

        public List<DodavateleS21> Dodavatel { get; set; }

        public List<UsersDTO> Schvalovatele { get; set; }

        public List<FilesDTO> Files { get; set; } = new List<FilesDTO>();

        public bool FileDescrptChange { get; set; } = false;

        public int SelectedDodavatelID { get; set; }
	    public string Vysledek { get; set; } = "";

	    public bool JinyDuvod { get; set; } = false;
        public bool DuvodHvezdicka { get; set; } = false;
        public bool OdeslatNaSchvaleni { get; set; } = false;

        public string UploadText { get; set; } = "Nahrát soubory";

        public int ParrentPozadavekId { get; set; }

        public string Mena { get; set; }
        public List<string> MenaList { get; set; } = new List<string> { "CZK", "EUR", "USD", "CHF", "GBP", "JPY" };

        public UploadedFilesCollection UploadedFiles { get; set; } = new UploadedFilesCollection();

        public string UserAlertText { get; set; }

        public int? ItemId
        {
            get { return Convert.ToInt32(Context.Parameters["Id"]); }

        }

	    public bool NewItem { get; set; } = false;

        public void Prepocitat()
        {
            ItemData.CelkovaCena = ItemData.CenaZaJednotku * ItemData.Mnozstvi;
        }

	    public void DeleteFile(int id)
	    {

            //Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('show');");

            FilesService.DeleteFile(id);
	    }

	    public void ChangeDescription()
	    {
            FileDescrptChange = true;
        }

	    public void FilesUploadedComplete()
	    {
	        var storage = Context.Configuration.ServiceLocator.GetService<IUploadedFileStorage>();
            if (NewItem)
            {
                ItemsService.ItemInsert(ItemData);                
                ItemData.ID = ItemsService.LastItemId;
                NewItem = false;
            }

            foreach (var file in UploadedFiles.Files)
            {
                var stream = storage. GetFile(file.FileId);
                

                string juliFile = @"\\juli-app\Pozadavky";
                string mainDirectory = DateTime.Now.Year.ToString();
                string subDir = "P" + DateTime.Now.Year + ItemData.PozadavekID;

                string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

                FilesService.SaveFile(stream, fullPath, file.FileName, ItemData.ID);
            }

	        SaveAll();
            Context.RedirectToRoute("ItemEdit", new { Id = ItemData.ID });
        }

        public void Save()
        {
            SaveAll();
            Context.RedirectToRoute("PozadavekEdit", new {Id = ItemData.PozadavekID});
        }

	    private void SaveAll()
	    {
	        if (NewItem)
	        {
	            ItemsService.ItemInsert(ItemData);
                NewItem = false;
            }
	        else
	        {
	            ItemsService.ItemSave(ItemData);
	        }

	        if (FileDescrptChange)
	        {
	            foreach (var file in Files)
	            {
	                FilesService.SaveFileDescription(file.Description, file.ID);
	            }
	        }
	    }

	    public void DuvodChange()
	    {
	        if ((ItemData.Poptany3Firmy == true && ItemData.DuvodID == 6) 
                || (ItemData.Poptany3Firmy == false && ItemData.DuvodID == 7))
	        {
	            JinyDuvod = true;
	        }
	        else JinyDuvod = false;

            if (ItemData.DuvodID == 5 || ItemData.DuvodID == 6)
                DuvodHvezdicka = true;
            else DuvodHvezdicka = false;
        }

	    public  Reasons[] DuvodyPoptavky { get; set; } =
             {
            new Reasons() { ID = 1, Text = "Nejlepší cena"},
            new Reasons() { ID = 2, Text = "Nejlepší kvalita"},
            new Reasons() { ID = 3, Text = "Nejrychlejší dodávka"},
            new Reasons() { ID = 4, Text = "(*) Speciální Dodavatel"},
            new Reasons() { ID = 5, Text = "(*) Speciální materiál"},
            new Reasons() { ID = 6, Text = "Jiný dùvod: "}
        };

        public  Reasons[] DuvodyNepoptani { get; set; } =
        {
            new Reasons() { ID = 1, Text = "Materiál do 15.000,- Kè"},
            new Reasons() { ID = 2, Text = "Servisní zásah"},
            new Reasons() { ID = 3, Text = "Havárie"},
            new Reasons() { ID = 4, Text = "Porucha"},
            new Reasons() { ID = 5, Text = "(*) Speciální Dodavatel"},
            new Reasons() { ID = 6, Text = "(*) Speciální materiál"},
            new Reasons() { ID = 7, Text = "Jiný dùvod: "}
        };

        public class Reasons
        {
            public int ID { get; set; }
            public string Text { get; set; }
        }

        //public void Instance1Pripravit()
        //{
        //    OdeslatNaSchvaleni = true;

        //}

        //public void Instance1Odeslat()
        //{

        //    Vysledek = MailServices.SendMail("marek.novak@juli.cz", "exchange mail test", "exchange mail test body");

        //}

        public void AddItem()
        {
            SaveAll();
            Context.RedirectToRoute("ItemEdit", new { Id = 0 });
        }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                ItemData.PozadavekID = PozadavkyService.LastPozId;

                if (ItemId == 0) // nova polozka
                {
                    NewItem = true;                    
                    ItemData.Zalozil = UserServices.GetActiveUser();
                    ItemData.DatumZalozeni = DateTime.Now;
                    // ItemData.DatumObjednani = DateTime.Now;
                    ItemData.TerminDodani = (DateTime.Now.AddDays(7));
                }
                else
                {
                    ItemData = ItemsService.GetItemById(ItemId.Value);
                    Files = FilesService.GetFilesListByItemID(ItemData.ID);
                    // ParrentItemId = PozadavekData.ItemId;
                }

                if ((ItemData.Poptany3Firmy == true && ItemData.DuvodID == 6)
                    || (ItemData.Poptany3Firmy == false && ItemData.DuvodID == 7))
                {
                    JinyDuvod = true;
                }
                else JinyDuvod = false;

                if (ItemData.DuvodID == 5 || ItemData.DuvodID == 6)
                    DuvodHvezdicka = true;
                else DuvodHvezdicka = false;

                //  Dodavatel = DodavatelService.GetDodavatelOrderedByName();
                // Schvalovatele = UserServices.GetSchvaGetUserByLevel(1);                
            }
            return base.PreRender();
        }

    }
}

