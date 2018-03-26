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

namespace ViewModels.PozadavkyViewModels
{
	public class ObjednavkyTvorbaViewModel : AppViewModel
    {
        //public int ActiveUserLevel { get; private set; } = UserServices.GetActiveUserLevel();

        public List<PozadavekDTO> SeznamPozadavku { get; set; } = new List<PozadavekDTO>();

        public List<ObjednavkaDTO> SeznamObjednavek { get; set; } = new List<ObjednavkaDTO>();

        public int SelectedObjId { get; set; }

        public List<ObjItemsDTO> SeznamCheckedItems { get; set; } = new List<ObjItemsDTO>();


        public PozadavekDTO PozadavekData { get; set; } = new PozadavekDTO();


        public bool AddToObj { get; set; } = false;

        public string SelectedFiltr { get; set; }

        // public GridViewDataSet<ItemsDTO> SeznamItemsGv { get; set; } = new GridViewDataSet<ItemsDTO>();

        public GridViewDataSet<ObjItemsDTO> SeznamItemsGv { get; set; } = new GridViewDataSet<ObjItemsDTO>();


        public GridViewDataSet<PozadavekDTO> SeznamPozadavkuGv { get; set; } = new GridViewDataSet<PozadavekDTO>()
        {
            PageSize = 30,
            SortExpression = nameof(PozadavekDTO.ID),
            SortDescending = true
        };

        public bool NothingFound { get; set; } = false;

        public bool JenPozadavky { get; set; } = true;

        public void PridatDoObj()
        {
            try
            {
                ObjednavkyService.AddToObj(SelectedObjId, SeznamCheckedItems);
                Vysledek = $"Položky přidány do objednávky č. {SelectedObjId}";
                SeznamCheckedItems.Clear();
            }
            catch (Exception e)
            {
                Vysledek = e.Message;
            }
        }

        public void ChangeObjStyle()
        {
            if (JenPozadavky)
                PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, "", ActiveUserLevel);
            else { }
            // TODO: dodelat prepinani
                //SeznamItemsGv = GridViewDataSet.Create(gridViewDataSetLoadDelegate: ItemsService.FillGridViewItemsNaObjednani, pageSize: 10);
  
        }

	    public void CreateObj()
	    {
            ClearAlerts();
            try
            {
                int NewId = ObjednavkyService.VytvoritObj(SeznamCheckedItems);
                SeznamCheckedItems.Clear();
                ConfirmText = $"Objednávka vytvořena, číslo objednávky: {NewId}";
            }
            catch (Exception e)
            {
                AlertText = e.Message;
            }

        }

        public void DeleteItem(int id)
        {
            try
            {
                ObjednavkyService.DeleteItemFromObj(id);
            }
            catch (Exception e)
            {
                AlertText = e.Message;
            }

           
        }


        // ----------------------  FILTRY -----------------------------------------------------
        public List<ObjItemsDTO> ListObjItems { get; set; } = new List<ObjItemsDTO>();
        public List<string> ListPozadavekFullId { get; set; } = new List<string>();
        public List<string> ListKST { get; set; } = new List<string>();
        public List<string> ListDodavatele { get; set; } = new List<string>();
        public List<string> ListZalozil { get; set; } = new List<string>();    

        public string ColumnFilter { get; set; } = "";
        public string WhereFilter { get; set; } = "";

        public string WhereFilterPozadavek { get; set; } = "";
        public string WhereFilterDodavatel { get; set; } = "";
        public string WhereFilterZalozil { get; set; } = "";
        public string WhereFilterKST { get; set; } = "";

        public string WhereFilterPolozkaLike { get; set; } = "";

        public DateTime DatumOd { get; set; } = DateTime.Today.AddYears(-1);
        public DateTime DatumDo { get; set; } = DateTime.Today;
        public bool SetDatum { get; set; } = false;
            
        public void SetFiltr(string filter)
        {
            ColumnFilter = filter;

            switch (filter)
            {
                case "FullPozadavekID": WhereFilter = WhereFilterPozadavek;
                    break;
                case "FullDodavatelName":
                    WhereFilter = WhereFilterDodavatel;
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
        }

        // pro v. 1.1
        //public override Task Init()
        //{
        //    SeznamItemsGv = GridViewDataSet.Create(gridViewDataSetLoadDelegate: ItemsService.FillGridViewItemsNaObjednani, pageSize: 10);
        //    SeznamItemsGv.SortingOptions.SortExpression = nameof(PozadavekDTO.ID);
        //    SeznamItemsGv.SortingOptions.SortDescending = true;
        //    return base.Init();
        //}



        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                ActiveUserLevel = UserServices.GetActiveUserLevels();
                ActiveUser = UserServices.GetActiveUser();
            }


                ItemsService.FillGridViewItemsFiltered(SeznamItemsGv, ColumnFilter, WhereFilter);                
                ListObjItems = SeznamItemsGv.Items.ToList();
                ItemsService.GridViewSetSortByID(SeznamItemsGv);


            ListPozadavekFullId.Clear();
                ListObjItems.ForEach(p => ListPozadavekFullId.Add(p.FullPozadavekID));
                ListPozadavekFullId = ListPozadavekFullId.Distinct().OrderBy(o => o).ToList();

            ListDodavatele.Clear();
                ListObjItems.ForEach(p => ListDodavatele.Add(p.FullDodavatelName));
                ListDodavatele = ListDodavatele.Distinct().OrderBy(o => o).ToList();

            ListZalozil.Clear();
                ListObjItems.ForEach(p => ListZalozil.Add(p.Zalozil));
                ListZalozil = ListZalozil.Distinct().OrderBy(o => o).ToList();

            ListKST.Clear();
                ListObjItems.ForEach(p => ListKST.Add(p.Stredisko));
                ListKST = ListKST.Distinct().OrderBy(o => o).ToList();

            

            if (SetDatum)
                ItemsService.FillGridViewItemsFiltered(SeznamItemsGv, "", "", DatumOd, DatumDo);
            else
                ItemsService.FillGridViewItemsFiltered(SeznamItemsGv, ColumnFilter, WhereFilter);

            SeznamObjednavek = ObjednavkyService.GetListObjednavek();
            //SeznamItemsGv = GridViewDataSet.Create(gridViewDataSetLoadDelegate: ItemsService.FillGridViewItemsNaObjednani, pageSize: 10);
            NothingFound = SeznamItemsGv.PagingOptions.TotalItemsCount == 0;




            return base.PreRender();
        }


    }
}

