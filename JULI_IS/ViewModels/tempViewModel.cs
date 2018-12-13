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




        //  SAMPLE 5 ComboBox


        public class Country
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<string> Cities { get; set; } = new List<string>();
            public bool IsEnabled { get; set; } = true;
            public bool IsChecked { get; set; }
        }

        public class City
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Country { get; set; }
        }


        public Country SelectedCountry { get; set; }
        public string SelectedCity { get; set; }
        public int? SelectedCountryId { get; set; }

        public List<Country> Countries { get; set; } = new List<Country> {
            new Country { Id = 1, Name = "Czech Republic", Cities = new List<string> { "Praha", "Brno" } },
            new Country { Id = 2, Name = "Slovakia", Cities = new List<string> { "Bratislava", "Košice" } },
            new Country { Id = 3, Name = "United States", Cities = new List<string> { "Washington", "New York" } }
        };

        public List<string> Cities { get; set; } = new List<string>();

        public void CountryChanged()
        {
            SelectedCity = null;
            Cities = SelectedCountry?.Cities ?? new List<string>();
        }


        public List<City> Cities2 { get; set; } = new List<City> {
            new City { Id = 1, Name = "Praha", Country = "Czech Republic" },
            new City { Id = 2, Name = "Praha", Country = "Slovakia" },
            new City { Id = 3, Name = "Praha", Country = "Texas" }
        };

        public City SelectedCity2 { get; set; }

        // /SAMPLE 7

        public string VybranyText { get; set; }

        public string SelectedObjId { get; set; }

        public List<string> SeznamObj { get; set; }

        public Country SelectedCountry2 { get; set; }

        [AllowStaticCommand]
        public IEnumerable<Country> LoadCountries(string searchText)
        {
            yield return new Country
            {
                Name = searchText
            };
        }


        [AllowStaticCommand]
        public List<string> LoadObjNumb(string searchText)
        {
            return ObjednavkyService.SearchObjFullId(searchText);
        }


        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                SeznamObj = ObjednavkyService.GetObjFullId();
            }

            return base.PreRender(); ;
        }

    }
}
