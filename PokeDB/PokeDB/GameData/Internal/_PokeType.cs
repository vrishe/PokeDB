using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace PokeDB.GameData.Internal
{
    [Table("POKETYPE")]
    class _PokeType
    {
        [Column("POKEMON_ID"), ForeignKey(typeof(Pokemon))]
        public int PokemonId { get; set; }
        [Column("TYPE_ID"), ForeignKey(typeof(Type))]
        public int TypeId { get; set; }
    }
}
