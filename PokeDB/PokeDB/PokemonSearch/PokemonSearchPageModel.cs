using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using PokeDB.GameData;
using PokeDB.Infrastructure;
using PropertyChanged;

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
                    SearchCommandBody, SearchCommandPredicate, this, "Pokemon"));
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


        IPlatform platform;
        IRepository gameData;

        public PokemonSearchPageModel(IPlatform platform, IRepository gameData)
        {
            this.platform = platform;
            this.gameData = gameData;
        }


        IEnumerable<PokemonSearchItemCellViewModel> itemsSource;

        async void Search(string query)
        {
            Pokemon = await Task.Run(() => (

                from item in itemsSource
                where SearchFilter(item.Pokemon, query)
                select item

            ).ToList());
        }

        static bool SearchFilter(Pokemon pokemon, params string[] query)
        {
            var token = query?.FirstOrDefault();

            return string.IsNullOrEmpty(token)
                || token.Length >= 3 && pokemon.Types.Any(
                    type => token.StartsWith(type.Name, System.StringComparison.OrdinalIgnoreCase))
                || pokemon.Name.StartsWith(token, System.StringComparison.OrdinalIgnoreCase);
        }


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
