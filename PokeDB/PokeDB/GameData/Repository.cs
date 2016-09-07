using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PokeDB.Infrastructure;
using System.Threading.Tasks;

namespace PokeDB.GameData
{
    public interface IRepository
    {
        void Prepare(string path);


        IList<Pokemon> LoadPokemon();
    }

    public sealed class Repository : IRepository
    {
        readonly IDBConnectionProvider provider;

        public Repository(IDBConnectionProvider provider)
        {
#if DEBUG
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
#endif // DEBUG
            this.provider = provider;
        }


        volatile SQLiteConnection mConnection;
        SQLiteConnection Connection
        {
            get
            {
#if DEBUG
                if (mConnection == null)
                {
                    throw new Exception($"{nameof(IRepository)} is not initialized. Call {nameof(Prepare)} first!");
                }
#endif // DEBUG
                return mConnection;
            }
        }


        public void Prepare(string path)
        {
#if DEBUG
            if (mConnection != null)
            {
                throw new Exception($"{nameof(IRepository)} is initialized already.");
            }
#endif // DEBUG
            mConnection = provider.EstablishDBConnection(path);
        }

        public IList<Pokemon> LoadPokemon()
        {
            return Connection.GetAllWithChildren<Pokemon>();
        }
    }
}
