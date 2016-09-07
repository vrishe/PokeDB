using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PokeDB.PokemonSearch
{
    public partial class PokemonSearchPage : ContentPage
    {
        public PokemonSearchPage()
        {
            InitializeComponent();

            PokemonList.ItemSelected += (s, e) =>
            {
                if (e.SelectedItem != null)
                {
                    ((ListView)s).SelectedItem = null;
                    // TODO: handle item selection here.
                }
            };
        }
    }
}
