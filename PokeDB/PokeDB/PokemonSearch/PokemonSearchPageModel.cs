using PokeDB.GameData;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;

namespace PokeDB.PokemonSearch
{
    [ImplementPropertyChanged]
    class PokemonSearchPageModel : FreshBasePageModel
    {
        public IList<Pokemon> Pokemon { get; private set; }


        IRepository gameData;

        public PokemonSearchPageModel(IRepository gameData)
        {
            this.gameData = gameData;
        }


        public override async void Init(object initData)
        {
            base.Init(initData);

            Pokemon = await Task.Run(() => gameData.LoadPokemon());
        }
    }
}
