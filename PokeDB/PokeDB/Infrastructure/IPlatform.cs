using System.IO;
using PCLExt.FileStorage;

namespace PokeDB.Infrastructure
{
    public interface IPlatform : IDBConnectionProvider
    {
        IFolder ApplicationDataFolder { get; }


        void WriteStream(Stream src, string dstPath);
    }
}
