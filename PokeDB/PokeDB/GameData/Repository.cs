using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using PokeDB.Infrastructure;

namespace PokeDB.GameData
{
    public interface IRepository
    {
        void Prepare(string path);


        IList<Pokemon> LoadPokemon();

        IList<Pokemon> LoadEvolutionFor(Pokemon pokemon);
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
            mConnection = provider.EstablishDBConnection(path, true);
        }

        public IList<Pokemon> LoadPokemon()
        {
            return Connection.GetAllWithChildren<Pokemon>();
        }

        public IList<Pokemon> LoadEvolutionFor(Pokemon pokemon)
        {
            var result = Connection.Query<Pokemon>("SELECT POKEMON.* FROM EVOLUTION "
                + "JOIN POKEMON ON POKEMON.ID == EVOLUTION_ID WHERE POKEMON_ID IS ?", pokemon.Id);

            foreach (var item in result)
            {
                Connection.GetChildren(item);
            }
            return result;
        }
    }
}
