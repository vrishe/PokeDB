using System;
using System.Threading;
using System.Threading.Tasks;
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


        CancellationTokenSource cts;

        Task PerformLaunch(CancellationToken token)
        {
            // TODO: Execute something useful here.
            return Task.Delay(500, token);
        }

        void ProceedFurther()
        {
            var startIntent = new Intent(this, typeof(MainActivity));
            {
                startIntent.AddFlags(ActivityFlags.NoAnimation);
            }
            StartActivity(startIntent);
            Finish();
        }


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            cts?.Cancel();
            cts = new CancellationTokenSource();

            try
            {
                await PerformLaunch(cts.Token);

                ProceedFurther();
            }
            catch (TaskCanceledException)
            {
                /* Nothing to do */
            }
        }

        protected override void OnDestroy()
        {
            cts.Cancel();

            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            /* Nothing to do */
        }
    }
}