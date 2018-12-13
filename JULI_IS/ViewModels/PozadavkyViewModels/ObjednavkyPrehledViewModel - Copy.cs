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
//using Pozadavky.Data;

namespace ViewModels.PozadavkyViewModels
{
	public class ObjednavkyPrehledViewModel2 : AppViewModel
    {
        public GridViewDataSet<ObjednavkaDTO> SeznamObjednavekGv { get; set; } = new GridViewDataSet<ObjednavkaDTO>();

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




        // DELEGAT
        public delegate void DelegatProVyberFiltru(string filtr);
        public DelegatProVyberFiltru VyberFiltr;
      

        //public void ChangeObjStyle()
        //{
        //    if (JenPozadavky)
        //        PozadavkyService.FillGridViewPozadavkyByPodpisLevel(SeznamPozadavkuGv, ActiveUserLevel);
        //    else
        //        ItemsService.FillGridViewItemsNaObjednani(SeznamObjednavekGv);
        //}

	   

        public void DeleteObj(int id)
        {
            try
            {
                ObjednavkyService.DeleteObj(id); ;
                Vysledek = $"Objednávka smazána...";
            }
            catch (Exception e)
            {
                Vysledek = e.Message;
            }

        }

        // ----------------------  FILTRY -----------------------------------------------------
        public List<ObjItemsDTO> ListObjItems { get; set; } = new List<ObjItemsDTO>();
        //public List<ObjList> ListObjFullId { get; set; } = new List<ObjList>();
        public List<string> ListObjFullId { get; set; } = new List<string>();
        public List<string> ListStav { get; set; } = new List<string>();
        public List<string> ListDodavatele { get; set; } = new List<string>();
        public List<string> ListZalozil { get; set; } = new List<string>();
        public List<string> ListHlavniRada { get; set; } = new List<string>();


        public string ColumnFilter { get; set; } = "";
        public string WhereFilter { get; set; } = "";

        public string WhereFilterObj { get; set; } = "";
        public string WhereFilterDodavatel { get; set; } = "";
        public string WhereFilterZalozil { get; set; } = "";
        public string WhereFilterStav { get; set; } = "";
        public string WhereFilterHlavniRada { get; set; } = "";

        public string WhereFilterPolozkaLike { get; set; } = "";

        public DateTime DatumOd { get; set; } = DateTime.Today.AddYears(-1);
        public DateTime DatumDo { get; set; } = DateTime.Today;
        public bool SetDatum { get; set; } = false;

        public string HledanyText { get; set; } = "";

        public class ObjList
        {
            public int id { get; set; }
            public string objid { get; set; }
        }

        [AllowStaticCommand]
        public List<string> LoadObjNumb(string searchText)
        {
            return ObjednavkyService.SearchObjFullId(searchText);
        }


        public void SetFiltr(string filter)
        {
            // ColumnFilter =  jmeno sloupce v databazi
            // WhereFilter = podminka pro ten sloupec z ComboBoxu

            if (!string.IsNullOrEmpty(filter))
            {
                ColumnFilter = filter;
                switch (filter)
                {
                    case "ObjednavkaFullID":
                        //int PomlckaIndex = WhereFilterObj.LastIndexOf('-');
                        //WhereFilterObj = WhereFilterObj.Substring(PomlckaIndex + 1, 4) + "-" + WhereFilterObj.Substring(0, PomlckaIndex);
                        if (ListObjFullId.Count < 1)
                        {
                            //ItemsService.FillGridViewObjednavkyWithItemsFilteredSchvalene(SeznamItemsGv, ColumnFilter, WhereFilter, null, null, ActiveUser, ActiveUserLevel);
                            //ListObjItems = SeznamItemsGv.Items.ToList();
                            //ItemsService.GridViewSetSortByID(SeznamItemsGv);

                            ////  ListObjItems.Where(w => (w.ObjednavkaFullID != null)).ToList().ForEach(p => ListObjFullId.Add(new ObjList { id = p.ID, objid = (p.ObjednavkaFullID.Substring(5) + "-" + p.ObjednavkaFullID.Substring(0,4))}));
                            //ListObjItems.Where(w => (w.ObjednavkaFullID != null)).ToList().ForEach(p => ListObjFullId.Add(new ObjList { id = p.ID, objid = (p.ObjednavkaFullID) }));

                            ListObjFullId = FilterService.GetListObjFullId();

                        }



                        WhereFilter = WhereFilterObj;
                        break;
                    case "FullDodavatelName":
                        WhereFilter = WhereFilterDodavatel;
                        break;
                    case "Zalozil":
                        WhereFilter = WhereFilterZalozil;
                        break;
                    case "Stav":
                        WhereFilter = WhereFilterStav;
                        break;
                    case "Polozka":
                        WhereFilter = WhereFilterPolozkaLike;
                        break;
                    case "HlavniRada":
                        WhereFilter = WhereFilterHlavniRada;
                        break;
                }

                if (filter == "Datum") SetDatum = true;
                else SetDatum = false;
            }
        }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {                                             
                // POUZE POPRVE

                ActiveUserLevel = UserServices.GetActiveUserLevels();
                ActiveUser = UserServices.GetActiveUser();

                ListObjFullId = ObjednavkyService.GetObjFullId(); 

                ListHlavniRada.Add("02 - Modelové zařízení");
                ListHlavniRada.Add("04 - Ostatní");
                ListHlavniRada.Add("10 - Investice");


                ListStav.Add("Objednávka odeslána");
                ListStav.Add("Objednávku neodesílat");
                ListStav.Add("Storno");
                ListStav.Add("Avízo zasláno");
                ListStav.Add("Zboží dodáno");

                ListDodavatele = ObjednavkyService.GetDodavateleList();
                ListZalozil = ObjednavkyService.GetCreatorList();

                ItemsService.GridViewSetSortByID(SeznamItemsGv);
            }


            if (String.IsNullOrEmpty(WhereFilter)) ColumnFilter = "";

            if (SetDatum)
                 ItemsService.FillGridViewObjednavkyWithItemsFilteredSchvalene(SeznamItemsGv, "", "", DatumOd, DatumDo, ActiveUser, ActiveUserLevel);
            else 
                 ItemsService.FillGridViewObjednavkyWithItemsFilteredSchvalene(SeznamItemsGv, ColumnFilter, WhereFilter, null, null, ActiveUser, ActiveUserLevel);

            

            // musi se data natahnout znovu aby fungovalo strankovani

            //   ItemsService.FillGridViewObjednavkyWithItemsFilteredSchvalene(SeznamItemsGv, ColumnFilter, WhereFilter, null, null, ActiveUser, ActiveUserLevel);



            //info = ObjednavkyService.InfoText;
            //PozadavekData.CelkovaCena = ItemsService.GetCelkovaCenaByPozadavekId(PozadavekId.Value);
            NothingFound = SeznamItemsGv.PagingOptions.TotalItemsCount == 0;            
            
            return base.PreRender();
        }


    }
}

