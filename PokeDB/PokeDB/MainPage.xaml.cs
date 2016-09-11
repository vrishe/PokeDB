using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace PokeDB
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        public int CountSeconds { get; private set; }


        bool appeared;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            appeared = true;

            ReplaceSplash();
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                ReplaceSplash();

                return appeared;
            });
        }

        protected override void OnDisappearing()
        {
            appeared = false;
            base.OnDisappearing();
        }


        int currentSplashIndex = -1;

        readonly Random random = new Random(Environment.TickCount);

        void ReplaceSplash()
        {
            var available = Enumerable.Range(0, 3)
                .Where(i => i != currentSplashIndex)
                .ToArray();

            currentSplashIndex = available[random.Next(available.Length)];

            var splashIndex = currentSplashIndex;
            Device.BeginInvokeOnMainThread(() =>
            {
                var typeInfo = typeof(MainPage).GetTypeInfo();

                ArtImage.Source = ImageSource.FromResource(
                    $"{typeInfo.Namespace}.Resources.Splash.splash_variant_{(char)('a' + splashIndex)}.jpg", 
                    typeInfo.Assembly);
            });
        }
    }
}
