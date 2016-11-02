using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using PokeDB.GameData;
using PokeDB.Infrastructure;
using PropertyChanged;
using Xamarin.Forms;

namespace PokeDB.PokemonSearch
{
    [ImplementPropertyChanged]
    class PokemonSearchPageModel : FreshBasePageModel
    {
        public IEnumerable<PokemonSearchItemCellViewModel> Pokemon { get; private set; }


        ICommand mSearchCommand;

        [DoNotNotify]
        public ICommand SearchCommand
        {
            get
            {
                return mSearchCommand ?? (mSearchCommand = new RelayCommand<string>(
                    SearchCommandBody, SearchCommandPredicate, this, nameof(Pokemon)));
            }
        }

        void SearchCommandBody(string query)
        {
            Search(query);
        }

        bool SearchCommandPredicate(string query)
        {
            return itemsSource != null && itemsSource.Count() > 0;
        }


        ICommand mSelectCommand;

        [DoNotNotify]
        public ICommand SelectCommand
        {
            get
            {
                return mSelectCommand ?? (mSelectCommand = new Command<PokemonSearchItemCellViewModel>(SelectCommandBody));
            }
        }

        void SelectCommandBody(PokemonSearchItemCellViewModel cell)
        {
            var evolution = gameData.LoadEvolutionFor(cell.Pokemon);

            foreach (var monster in evolution)
            {
                System.Diagnostics.Debug.WriteLine(monster);
            }
        }


        IPlatform platform;
        IRepository gameData;

        public PokemonSearchPageModel(IPlatform platform, IRepository gameData)
        {
            this.platform = platform;
            this.gameData = gameData;
        }


        readonly PokemonMatcher matcher = new PokemonMatcher();

        async void Search(string query)
        {
            Pokemon = await Task.Run(() =>
            {
                var match = matcher.Match(query ?? string.Empty);

                return itemsSource.Where(item => match(item.Pokemon)).ToList();
            });
        }


        IEnumerable<PokemonSearchItemCellViewModel> itemsSource;

        public override async void Init(object initData)
        {
            base.Init(initData);

            itemsSource = await Task.Run(() => AsCells(gameData.LoadPokemon()));

            Search(null);
        }

        IEnumerable<PokemonSearchItemCellViewModel> AsCells(IEnumerable<Pokemon> pokemons)
        {
            return pokemons.Select(
                pokemon => new PokemonSearchItemCellViewModel(platform)
                {
                    Pokemon = pokemon
                });
        }
    }
}
