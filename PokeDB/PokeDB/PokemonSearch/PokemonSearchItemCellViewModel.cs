﻿using System.Linq;
using PokeDB.GameData;
using PokeDB.Infrastructure;
using PropertyChanged;

namespace PokeDB.PokemonSearch
{
    [ImplementPropertyChanged]
    class PokemonSearchItemCellViewModel
    {
        public Pokemon Pokemon { get; set; }

        void OnPokemonChanged()
        {
            ResetCaches();
        }


        [DependsOn(nameof(Pokemon))]
        public string Name { get { return Pokemon?.Name; } }

        [DependsOn(nameof(Pokemon))]
        public string Icon { get { return GetIconCached(); } }

        [DependsOn(nameof(Pokemon))]
        public string Type { get { return GetTypeCached(); } }


        string mIcon;

        string GetIconCached()
        {
            if (mIcon == null && Pokemon != null)
            {
                mIcon = $"file:///{platform.ApplicationDataFolder.Path}/GameData/Images/Pokemon/pogo_icon{Pokemon.Id}.png";
            }
            return mIcon;
        }


        string mType;

        string GetTypeCached()
        {
            if (mType == null && Pokemon != null)
            {
                mType = string.Join("/", Pokemon.Types.Select(type => type.Name));
            }
            return mType;
        }


        void ResetCaches()
        {
            mIcon = null;
            mType = null;
        }


        readonly IPlatform platform;

        public PokemonSearchItemCellViewModel(IPlatform platform)
        {
            this.platform = platform;
        }
    }
}
