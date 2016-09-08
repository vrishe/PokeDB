using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokeDB.PokemonSearch
{
    public partial class PokemonSearchPage : ContentPage
    {
        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        public static readonly BindableProperty SearchCommandProperty =
            BindableProperty.Create("SearchCommand", typeof(ICommand), typeof(PokemonSearchPage), default(ICommand),
                BindingMode.Default, null, (bindable, valueOld, valueNew) =>
                {
                    var page = (PokemonSearchPage)bindable;
                    var command = (ICommand)valueOld;
                    
                    if (command != null)
                    {
                        command.CanExecuteChanged -= page.OnSearchCommandCanExecuteChanged;
                    }
                    command = (ICommand)valueNew;

                    if (command != null)
                    {
                        command.CanExecuteChanged += page.OnSearchCommandCanExecuteChanged;
                    }
                    page.OnSearchCommandCanExecuteChanged(bindable, EventArgs.Empty);
                });


        public PokemonSearchPage()
        {
            InitializeComponent();

            var searchTextObservable = Observable.FromEvent<EventHandler<TextChangedEventArgs>, TextChangedEventArgs>(
                handler =>
                {
                    EventHandler<TextChangedEventArgs> proxy = (s, e) => handler(e);

                    return proxy;
                },
                proxy => Search.TextChanged += proxy,
                proxy => Search.TextChanged -= proxy)
                    .Select(e => e.NewTextValue);

            var searchButtonObservable = Observable.FromEvent<EventHandler, EventArgs>(
                handler =>
                {
                    EventHandler proxy = (s, e) => handler(e);

                    return proxy;
                },
                proxy => Search.SearchButtonPressed += proxy,
                proxy => Search.SearchButtonPressed -= proxy)
                    .Select(e => Search.Text);

            Observable.Merge(searchTextObservable, searchButtonObservable)
                .Throttle(TimeSpan.FromMilliseconds(545.45))
                .Subscribe(Observer.Create<string>(query => SearchCommand?.Execute(query)));

            PokemonList.ItemSelected += (s, e) =>
            {
                if (e.SelectedItem != null)
                {
                    ((ListView)s).SelectedItem = null;
                    // TODO: handle item selection here.
                }
            };
        }

        void OnSearchCommandCanExecuteChanged(object sender, EventArgs e)
        {
            Search.IsEnabled = SearchCommand?.CanExecute(Search.Text) ?? false;
        }
    }
}
