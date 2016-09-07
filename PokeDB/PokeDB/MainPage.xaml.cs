using System;
using PokeDB.Resources;
using Xamarin.Forms;

namespace PokeDB
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            var count = 0;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                GreetingLabel.Text = string.Format(Strings.GreetingTextFormat, count++);

                return IsVisible;
            });
        }
    }
}
