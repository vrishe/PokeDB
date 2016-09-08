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


        public int CountSeconds { get; private set; }


        bool appeared;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            appeared = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                GreetingLabel.Text = string.Format(
                    Strings.GreetingTextFormat, CountSeconds++);

                return appeared;
            });
        }

        protected override void OnDisappearing()
        {
            appeared = false;
            base.OnDisappearing();
        }
    }
}
