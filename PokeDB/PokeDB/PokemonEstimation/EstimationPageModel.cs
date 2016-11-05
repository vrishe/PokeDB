using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreshMvvm;
using PokeDB.PokemonSearch;
using PropertyChanged;

namespace PokeDB.PokemonEstimation
{
    [ImplementPropertyChanged]
    class EstimationPageModel : FreshBasePageModel
    {
        public FoundItemCellViewModel ItemFound { get; private set; }

        public override void Init(object initData)
        {
            base.Init(initData);

            ItemFound = initData as FoundItemCellViewModel;
        }
    }
}
