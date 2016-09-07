using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace PokeDB.GameData
{
    class PokeType
    {
        [Column("POKEMON_ID"), ForeignKey(typeof(Pokemon))]
        public int PokemonId { get; private set; }
        [Column("TYPE_ID"), ForeignKey(typeof(Pokemon))]
        public int TypeId { get; private set; }
    }
}
