using Android.Net;

namespace PokeDB.Droid
{
    static class AndroidExtensions
    {
        public static Uri asNative(this System.Uri src)
        {
            return Uri.Parse(src.AbsoluteUri);
        }
    }
}