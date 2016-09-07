using SQLite.Net;

namespace PokeDB.Infrastructure
{
    public interface IDBConnectionProvider
    {
        SQLiteConnection EstablishDBConnection(string path);
    }
}
