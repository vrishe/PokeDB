using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PCLExt.FileStorage;
using PokeDB.Infrastructure;
using Xamarin.Forms;

namespace PokeDB
{
    public partial class App : Application
    {
        void SetIoC()
        {
            var container = FreshTinyIoC.FreshTinyIoCContainer.Current;

            container.Register(typeof(IPlatform), Platform);
            container.Register(typeof(IDBConnectionProvider), Platform);
            container.Register(typeof(GameData.IRepository), typeof(GameData.Repository))
                .UsingConstructor(() => new GameData.Repository((IDBConnectionProvider)null));

            container.AutoRegister(type => type.Name.EndsWith("Service"));
            container.AutoRegister(type => type.Name.EndsWith("PageModel"));

            container.Register(typeof(App), this);
        }

        void PrepareFirstChance()
        {
            SetIoC();

            MainPage = new MainPage
            {
                BindingContext = this
            };
        }

        async void PrepareLastChance()
        {
            await CheckResourcesAsync();

            var container = FreshTinyIoC.FreshTinyIoCContainer.Current;
            {
                container.Resolve<GameData.IRepository>()
                    .Prepare(PortablePath.Combine(Platform.ApplicationDataFolder.Path, "GameData", "game_data.db"));
            }
            //PokemonSearch.PokemonMatcher.EnableDebug();

            MainPage = new FreshMvvm.FreshNavigationContainer(
                FreshMvvm.FreshPageModelResolver.ResolvePageModel<PokemonSearch.PokemonSearchPageModel>());
        }


        bool prepared;

        void Prepare()
        {
            if (!prepared)
            {
                PrepareFirstChance();
                PrepareLastChance();

                prepared = true;
            }
        }


        async Task CheckResourcesAsync()
        {
            var assetsFolder = await Platform.ApplicationDataFolder.CreateFolderAsync("GameData", CreationCollisionOption.OpenIfExists);
            var indexFile = await assetsFolder.CreateFileAsync(".index", CreationCollisionOption.OpenIfExists);

            var revisionList = new List<string>();
            using (var index = await indexFile.OpenAsync(FileAccess.ReadAndWrite))
            {
                if (index.Length > 0)
                {
                    var consistent = true;
                    using (var indexReader = new System.IO.StreamReader(index))
                    {
                        string line;
                        while ((line = await indexReader.ReadLineAsync()) != null)
                        {
                            var existence = await assetsFolder.CheckExistsAsync(line);

                            if (existence != ExistenceCheckResult.FileExists)
                            {
                                consistent = false;

                                revisionList.Add(line);
                            }
                        }
                    }
                    if (consistent)
                    {
                        return;
                    }
                    index.Seek(0, System.IO.SeekOrigin.End);
                }
                if (revisionList.Count <= 0)
                {
                    revisionList.Add("game_data.db");
                    revisionList.AddRange(Enumerable.Range(1, 151)
                        .Select(ordinal => PortablePath.Combine("Images", "Pokemon", $"pogo_icon{ordinal}.png")));
                }
                var assembly = typeof(App).GetTypeInfo().Assembly;
                var resourcePathBase = PortablePath.Combine(typeof(App).Namespace, assetsFolder.Name);

                foreach (var item in revisionList)
                {
                    await Task.Run(() =>
                    {
                        var resourceId = PortablePath.Combine(resourcePathBase, item)
                            .Replace(PortablePath.DirectorySeparatorChar, '.');

                        using (var resource = assembly.GetManifestResourceStream(resourceId))
                        {
                            Platform.WriteStream(resource, PortablePath.Combine(assetsFolder.Path, item));
                        }
                    });
                }
                using (var indexWriter = new System.IO.StreamWriter(index))
                {
                    foreach (var item in revisionList)
                    {
                        await indexWriter.WriteLineAsync(item);
                    }
                }
            }
        }


        internal IPlatform Platform { get; private set; }

        public App(IPlatform platform)
        {
            InitializeComponent();

            Platform = platform;
        }


        protected override void OnStart()
        {
            Prepare();
        }

        protected override void OnSleep()
        {
            /* Nothing to do */
        }

        protected override void OnResume()
        {
            /* Nothing to do */
        }
    }
}
