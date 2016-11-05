using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PokeDB.PokemonEstimation
{
    public partial class EstimationPage : ContentPage
    {
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly BindableProperty HeaderProperty =
            BindableProperty.Create(nameof(Header), typeof(object), typeof(EstimationPage), default(object), 
                BindingMode.OneWay, null, new BindingPropertyChangedDelegateProxy<EstimationPage, object>(OnHeaderChanged));

        static void OnHeaderChanged(EstimationPage page, object valueOld, object valueNew)
        {
            page.HeaderPanel.BindingContext = valueNew;
        }


        public EstimationPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            HeaderPanel.BindingContext = Header;
        }
    }
}
