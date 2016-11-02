using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace PokeDB.GameData
{
    [Table("POKEMON")]
    public sealed class Pokemon
    {
        [Column("ID"), PrimaryKey]
        public int Id { get; set; }
        [Column("NAME")]
        public string Name { get; set; }


        [Column("ATK")]
        public double Attack { get; set; }
        [Column("DEF")]
        public double Defense { get; set; }
        [Column("STA")]
        public double Stamina { get; set; }


        [ManyToMany(typeof(Internal._PokeType), ReadOnly = true)]
        public Type[] Types { get; set; }
    }
}
