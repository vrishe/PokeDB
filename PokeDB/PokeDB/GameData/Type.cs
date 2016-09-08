using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace PokeDB.GameData
{
    [Table("TYPE")]
    public class Type
    {
        [Column("ID"), PrimaryKey]
        public int Id { get; set; }
        [Column("NAME")]
        public string Name { get; private set; }
    }
}
