using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using PCLExt.FileStorage;
using PokeDB.Infrastructure;
using PokeDB.Insfrastructure;
using SQLite.Net;

namespace PokeDB.Droid
{
    [Activity(Label = "PokeDB", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication((Application)Application);
        }
    }

    [Application(Icon = "@drawable/icon")]
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

        SQLiteConnection IDBConnectionProvider.EstablishDBConnection(string path)
        {
            return new SQLiteConnection(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), path);
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

