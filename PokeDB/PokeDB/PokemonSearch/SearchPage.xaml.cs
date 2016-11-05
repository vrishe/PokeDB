using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokeDB.PokemonSearch
{
    public partial class SearchPage : ContentPage
    {
        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        public static readonly BindableProperty SearchCommandProperty =
            BindableProperty.Create(nameof(SearchCommand), typeof(ICommand), typeof(SearchPage), default(ICommand),
                BindingMode.Default, null, new BindingPropertyChangedDelegateProxy<SearchPage, ICommand>(OnSearchCommandChanged));

        static void OnSearchCommandChanged(SearchPage page, ICommand commandOld, ICommand commandNew)
        {
            if (commandOld != null)
            {
                commandOld.CanExecuteChanged -= page.OnSearchCommandCanExecuteChanged;
            }
            if (commandNew != null)
            {
                commandNew.CanExecuteChanged += page.OnSearchCommandCanExecuteChanged;
            }
            page.OnSearchCommandCanExecuteChanged(page, EventArgs.Empty);
        }

        void OnSearchCommandCanExecuteChanged(object sender, EventArgs e)
        {
            Search.IsEnabled = SearchCommand?.CanExecute(Search.Text) ?? false;
        }


        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        public static readonly BindableProperty SelectCommandProperty =
            BindableProperty.Create(nameof(SelectCommand), typeof(ICommand), typeof(SearchPage), default(ICommand));


        public SearchPage()
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
    }
}
