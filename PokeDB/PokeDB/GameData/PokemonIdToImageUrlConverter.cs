using System;
using System.Globalization;
using PokeDB.Infrastructure;
using Xamarin.Forms;

namespace PokeDB.GameData
{
    class PokemonIdToImageUrlConverter : IValueConverter
    {
        readonly IPlatform platform = FreshTinyIoC.FreshTinyIoCContainer.Current.Resolve<IPlatform>();

        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            var target = $"file:///{platform.ApplicationDataFolder.Path}/GameData/Images/Pokemon/pogo_icon{(int)System.Convert.ChangeType(value, typeof(int))}.png";

            if (targetType == typeof(Uri))
            {
                return new Uri(target, UriKind.Absolute);
            }
#if DEBUG
            if (targetType != typeof(string))
            {
                throw new InvalidCastException($"Can't convert PokemonID to {targetType}!");
            }
#endif // DEBUG
            return target;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
