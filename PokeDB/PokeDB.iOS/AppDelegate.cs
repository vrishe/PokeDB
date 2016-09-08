
using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using PCLExt.FileStorage;
using PokeDB.Infrastructure;
using PokeDB.Insfrastructure;
using SQLite.Net;
using SQLite.Net.Interop;
using UIKit;

namespace PokeDB.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IPlatform
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

        public static implicit operator App(AppDelegate application)
        {
            return application.applicationLazy.Value;
        }


        public AppDelegate()
        {
            applicationLazy = new Lazy<App>(() => new App(this), false);
        }


        public override bool WillFinishLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            SetIoC();

            SDWebImage.SDImageCache.SharedImageCache.AddReadOnlyCachePath(
                Path.Combine(((IPlatform)this).ApplicationDataFolder.Path, "GameData", "Images", "Pokemon"));

            return base.WillFinishLaunching(uiApplication, launchOptions);
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(this);

            return base.FinishedLaunching(app, options);
        }


        #region IDBConnectionProvider implementation

        SQLiteConnection IDBConnectionProvider.EstablishDBConnection(string path, bool readOnly)
        {
            return new SQLiteConnection(new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS(), path,
                readOnly ? SQLiteOpenFlags.ReadOnly : SQLiteOpenFlags.ReadWrite););
        }

        #endregion // IDBConnectionProvider implementation

        #region IPlatform implementation

        IFolder IPlatform.ApplicationDataFolder
        {
            get
            {
                return new FileSystemFolder(NSSearchPath.GetDirectories(
                    NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0]);
            }
        }

        void IPlatform.WriteStream(Stream src, string dstPath)
        {
            src.WriteTo(dstPath);
        }

        #endregion // IPlatform implementation
    }

    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
