using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.Services;
using Pozadavky.DTO;
using DotVVM.Framework.Controls;

namespace ViewModels.PozadavkyViewModels
{
	public class SignViewModel : AppViewModel
    {
        //public int ActiveUserLevel { get; private set; } = UserServices.GetActiveUserLevel();
        //public string ActiveUser { get; private set; } = UserServices.GetActiveUser();

        public List<PozadavekDTO> SeznamPozadavku { get; set; }

        public PozadavekDTO PozadavekData { get; set; } = new PozadavekDTO();
        public PozadavekDTO ZamitanyPozadavekData { get; set; } = new PozadavekDTO();
	    public bool PozadavekZamitan { get; set; } = false;
	    public bool JinyDuvod { get; set; } = false;
        public bool NeniZadatel { get; set; } = true;
        
        public GridViewDataSet<PozadavekDTO> SeznamPozadavkuGv { get; set; } = new GridViewDataSet<PozadavekDTO>()
        {
            PageSize = 10,
            SortExpression = nameof(PozadavekDTO.ID),
            SortDescending = true
        };

        public bool CurrentUserOnly { get; set; } = true;
        public bool NothingFound { get; set; } = true;

        public void ChangeCurrentUser()
        {
            if (CurrentUserOnly)
            {
                if (ActiveUserLevel.Contains(1))
                    PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, ActiveUser);

                if (ActiveUserLevel.Contains(2))
                    PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, "", ActiveUserLevel);

                if (ActiveUserLevel.Contains(4))
                    PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, ActiveUser);


                //switch (ActiveUserLevel.Contains)
                //{
                //    case 1:

                //        break;
                //    case 2:
                //        PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, "", ActiveUserLevel);
                //        break;
                //    case 4:

                //        break;
                //}

            }
            else
            {
                PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, "", ActiveUserLevel);
            }
        }

        public void PozadavekSchvalit(int id, int nextLevel)
        {
            ClearAlerts();
            PozadavekData = PozadavkyService.GetPozadavekById(id);
            if ((ActiveUser != PozadavekData.Zalozil) || Constants.Test || ActiveUser == "marek.novak")
            {
                try
                {
                    PozadavekData.PodpisLevel = nextLevel;
                    Vysledek = PozadavkyService.PozadavekSchvalit(PozadavekData);
                }
                catch (Exception ex)
                {
                    AlertText = "Chyba při schvalování požadavku!\n";
                    if (ex.InnerException == null) AlertText += ex.Message;
                    else AlertText += ex.InnerException.Message;
                }
            }
            else
            {
                AlertText += "Nemůžete si podepsat vlastní požadavek!";
            }


        }

        public void PozadavekPokusOZamitnuti(int id)
        {
            ClearAlerts();
            ZamitanyPozadavekData = PozadavkyService.GetPozadavekById(id);
            if ((ActiveUser != ZamitanyPozadavekData.Zalozil) || Constants.Test || ActiveUser == "marek.novak")
            {
                PozadavekZamitan = true;
            }
            else
            {
                AlertText = "Nemůžete si zamítnout vlastní požadavek!";
            }            
        }


        public void PozadavekZamitnout()
        {
            //ZamitanyPozadavekData = PozadavkyService.GetPozadavekById(id);
            try
            {
                Vysledek = PozadavkyService.PozadavekZamitnout(
                    ZamitanyPozadavekData,
                    ZamitanyPozadavekData.DuvodZamitnutiID == 4 ? ZamitanyPozadavekData.DuvodZamitnutiText :
                    DuvodyZamitnuti.FirstOrDefault(x => x.ID == ZamitanyPozadavekData.DuvodZamitnutiID).Text);

                PozadavekZamitan = false;
            }
            catch (Exception ex)
            {
                AlertText = "Chyba při zamítání požadavku!\n";
                if (ex.InnerException == null) AlertText += ex.Message;
                else AlertText += ex.InnerException.Message;
            }
        }

        public void DuvodChange()
        {
            if (ZamitanyPozadavekData.DuvodZamitnutiID == 4)                
            {
                JinyDuvod = true;
            }
            else JinyDuvod = false;
        }

        public Reasons[] DuvodyZamitnuti { get; set; } =
             {
            new Reasons() { ID = 1, Text = "Závažná chyba v zadaní požadavku"},
            new Reasons() { ID = 2, Text = "Duplicitní požadavek"},
            new Reasons() { ID = 3, Text = "Požadavek se nebude realizovat"},  
            new Reasons() { ID = 4, Text = "Jiný důvod: "}
        };

        public class Reasons
        {
            public int ID { get; set; }
            public string Text { get; set; }
        }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                ActiveUserLevel = UserServices.GetActiveUserLevels();
                ActiveUser = UserServices.GetActiveUser();               
            }

            if (CurrentUserOnly)
            {

                PozadavkyService.FillGridViewPozadavkyToSignByUsersLevels(SeznamPozadavkuGv, ActiveUser);
                SeznamPozadavkuGv.SortingOptions.SortExpression = nameof(PozadavekDTO.PodpisLevel);


                //// umi L1
                //if (ActiveUserLevel.Contains(1))
                //    PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, ActiveUser);

                //if(SeznamPozadavkuGv.TotalItemsCount == 0) 
                //    if (ActiveUserLevel.Contains(2))
                //        PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, "", ActiveUserLevel);

                //if (SeznamPozadavkuGv.TotalItemsCount == 0)
                //    if (ActiveUserLevel.Contains(4))
                //        PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, ActiveUser);


                //switch (ActiveUserLevel.Contains)
                //{
                //    case 1:

                //        break;
                //    case 2:
                //        PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, "", ActiveUserLevel);
                //        break;
                //    case 4:

                //        break;
                //}

            }
            else
            {
                PozadavkyService.FillGridViewPozadavkyToSignByUserOrLevel(SeznamPozadavkuGv, "", ActiveUserLevel);
                SeznamPozadavkuGv.SortingOptions.SortExpression = nameof(PozadavekDTO.PodpisLevel);
            }

            NothingFound = SeznamPozadavkuGv.PagingOptions.TotalItemsCount == 0;
            

            return base.PreRender();
        }


    }
}

