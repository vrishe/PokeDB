using PokeDB.GameData;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;
using PokeDB.Infrastructure;
using System.Windows.Input;

namespace PokeDB.PokemonSearch
{
    [ImplementPropertyChanged]
    class PokemonSearchPageModel : FreshBasePageModel
    {
        public IEnumerable<Pokemon> Pokemon { get; private set; }


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

            System.Diagnostics.Debug.WriteLine($"{nameof(PokemonSearchPageModel)}: Searching for '{query}' ...");
        }

        bool SearchCommandPredicate(string query)
        {
            return pokemonSource != null && pokemonSource.Count > 0;
        }


        IRepository gameData;

        public PokemonSearchPageModel(IRepository gameData)
        {
            this.gameData = gameData;
        }


        IList<Pokemon> pokemonSource;

        async void Search(string query)
        {
            Pokemon = await Task.Run(() => (

                from pokemon in pokemonSource
                where SearchFilter(pokemon, query)
                select pokemon

            ).ToList());
        }

        static bool SearchFilter(Pokemon pokemon, params string[] query)
        {
            var token = query?.FirstOrDefault();

            return string.IsNullOrEmpty(token) 
                || pokemon.Name.StartsWith(token);
        }


        public override async void Init(object initData)
        {
            base.Init(initData);

            pokemonSource = await Task.Run(() => gameData.LoadPokemon());

            Search(null);
        }
    }
}
