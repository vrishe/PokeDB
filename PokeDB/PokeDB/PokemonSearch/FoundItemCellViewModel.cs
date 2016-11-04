using System;
using System.Linq;
using PokeDB.GameData;
using PokeDB.Infrastructure;
using PropertyChanged;

namespace PokeDB.PokemonSearch
{
    [ImplementPropertyChanged]
    class FoundItemCellViewModel
    {
        public Pokemon Pokemon { get; set; }

        void OnPokemonChanged()
        {
            ResetCaches();
        }


        [DependsOn(nameof(Pokemon))]
        public string Name { get { return Pokemon?.Name; } }

        [DependsOn(nameof(Pokemon))]
        public Uri Icon { get { return GetIconCached(); } }

        [DependsOn(nameof(Pokemon))]
        public string Type { get { return GetTypeCached(); } }


        Uri mIcon;

        Uri GetIconCached()
        {
            if (mIcon == null && Pokemon != null)
            {
                var builder = new UriBuilder
                {
                    Scheme = "file",
                    Host = platform.ApplicationDataFolder.Path,
                    Path = $"GameData/Images/Pokemon/pogo_icon{Pokemon.Id}.png"
                };
                mIcon = builder.Uri;
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

        public FoundItemCellViewModel(IPlatform platform)
        {
            this.platform = platform;
        }
    }
}
