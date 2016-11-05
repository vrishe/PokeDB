using System;
using System.IO;
using Android.App;
using Android.OS;
using PCLExt.FileStorage;
using PokeDB.Infrastructure;
using PokeDB.Insfrastructure;
using SQLite.Net;
using SQLite.Net.Interop;

namespace PokeDB.Droid
{
    [Application(Icon = "@mipmap/icon", HasCode = true)]
    public class Application : global::Android.App.Application, IPlatform
    {
        /// <summary>
        /// Here we set up all the platform-specific modules, that are safe 
        /// to use, while application is still starting up.
        /// </summary>
        void SetIoC()
        {
            // TODO: setup your DI container here.
        }


        readonly Lazy<App> applicationLazy;

        public static implicit operator App(Application application)
        {
            return application.applicationLazy.Value;
        }


        readonly Lazy<Handler> mainHandlerLazy = new Lazy<Handler>(
            () => new Handler(Looper.MainLooper), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public Handler MainHandler
        {
            get
            {
                return mainHandlerLazy.Value;
            }
        }


        public Application(IntPtr handle, global::Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            applicationLazy = new Lazy<App>(() => new App(this), false);
        }


        public override void OnCreate()
        {
            base.OnCreate();

            SetIoC();
        }


        #region IDBConnectionProvider implementation

        SQLiteConnection IDBConnectionProvider.EstablishDBConnection(string path, bool readOnly)
        {
            return new SQLiteConnection(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), path, 
                readOnly ? SQLiteOpenFlags.ReadOnly : SQLiteOpenFlags.ReadWrite);
        }

        #endregion // IDBConnectionProvider implementation

        #region IPlatform implementation

        IFolder IPlatform.ApplicationDataFolder
        {
            get
            {
                return new FileSystemFolder((GetExternalFilesDir(null) ?? FilesDir).AbsolutePath);
            }
        }

        void IPlatform.WriteStream(Stream src, string dstPath)
        {
            src.WriteTo(dstPath);
        }

        #endregion // IPlatform implementation
    }
}