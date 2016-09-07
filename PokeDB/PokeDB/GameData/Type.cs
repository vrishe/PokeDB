using SQLite.Net.Attributes;

namespace PokeDB.GameData
{
    [Table("TYPE")]
    public class Type
    {
        [Column("ID"), PrimaryKey]
        int Id { get; }
        [Column("NAME")]
        int Tag { get; }
    }
}
