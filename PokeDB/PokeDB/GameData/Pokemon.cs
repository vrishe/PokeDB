using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Collections;

namespace PokeDB.GameData
{
    [Table("POKEMON")]
    public sealed class Pokemon
    {
        [Column("ID"), PrimaryKey]
        public int Id { get; set; }
        [Column("NAME")]
        public string Name { get; private set; }


        [Column("ATK")]
        public double Attack { get; private set; }
        [Column("DEF")]
        public double Defense { get; private set; }
        [Column("STA")]
        public double Stamina { get; private set; }


        [ManyToMany(typeof(Internal._PokeType), ReadOnly = true)]
        public Type[] Types { get; set; }
    }
}
