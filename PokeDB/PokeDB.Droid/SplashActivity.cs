using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace PokeDB.Droid
{
    [Activity(Label = "PokeDB", Icon = "@drawable/icon", Theme = "@style/SplashTheme", MainLauncher = true, NoHistory = true, StateNotNeeded = true)]
    public class SplashActivity : Activity
    {
        public new Application Application
        {
            get
            {
                return (Application)base.Application;
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Application.MainHandler.PostDelayed(() =>
            {
                var startIntent = new Intent(this, typeof(MainActivity));
                {
                    startIntent.AddFlags(ActivityFlags.NoAnimation);
                }
                StartActivity(startIntent);
                Finish();
            }, 1250);
        }

        public override void OnBackPressed()
        {
            /* Nothing to do */
        }
    }
}