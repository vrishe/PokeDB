using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PokeDB.Views;
using Square.Picasso;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RemoteImage), typeof(PokeDB.Droid.Rendering.View.RemoteImageRenderer))]

namespace PokeDB.Droid.Rendering.View
{
    /// <summary>
    /// RemoteImage view renderer.
    /// 
    /// </summary>
    public class RemoteImageRenderer : ViewRenderer<RemoteImage, ImageView>
    {
        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<RemoteImage> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                this.SetNativeControl(new ImageView(Context));
            }
            if (e.NewElement != null)
            {
                UpdateAspect();
                UpdateBitmap();
            }
        }


        /// <inheritdoc/>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RemoteImage.AspectProperty.PropertyName)
            {
                UpdateAspect();
                UpdateBitmap();

                return;
            }
            if (e.PropertyName == RemoteImage.ImageUrlProperty.PropertyName
                || e.PropertyName == RemoteImage.PlaceholderImageResourceIdProperty.PropertyName)
            {
                UpdateBitmap();

                return;
            }
            base.OnElementPropertyChanged(sender, e);
        }


        private void UpdateAspect()
        {
            using (var scaleType = ToScaleType(Element.Aspect))
            {
                Control.SetScaleType(scaleType);
            }
        }


        private void UpdateBitmap()
        {
            var load = Picasso.With(Control.Context)
                .Load(Element.ImageUrl?.asNative());

            if (!string.IsNullOrEmpty(Element.PlaceholderImageResourceId))
            {
                var placeholderId = Control.Resources
                    .GetIdentifier(Element.PlaceholderImageResourceId, "drawable", Context.PackageName);

                if (placeholderId != 0)
                {
                    load = load.Placeholder(placeholderId);
                }
            }
            load = load.Fit();

            switch (Element.Aspect)
            {
                case Aspect.AspectFill:
                    load = load.CenterCrop();
                    break;
                case Aspect.Fill:
                    load = load.CenterInside();
                    break;
            }
            load.Into(Control);
        }


        /// <summary>
        /// Convert <see cref="T:Xamarin.Forms.Aspect"/> to a corresponding Andorid-specific <see cref="T:Android.Widget.ImageView.ScaleType"/> value.
        /// </summary>
        /// 
        /// <returns>The scale type which corresponds to an incoming aspect type.</returns>
        /// <param name="aspect">An aspect to perform conversion with.</param>
        protected static ImageView.ScaleType ToScaleType(Aspect aspect)
        {
            switch (aspect)
            {
                case Aspect.AspectFill:
                    return ImageView.ScaleType.CenterCrop;
                case Aspect.Fill:
                    return ImageView.ScaleType.FitXy;
                default:
                    return ImageView.ScaleType.FitCenter;
            }
        }


        private volatile bool isDisposed;

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        /// 
        /// <param name="disposing">If set to <c>true</c> then all the managed 
        /// resources should be disposed during the current call.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!isDisposed)
            {
                if (disposing)
                {
                    if (Control != null
                        && Control.Handle != IntPtr.Zero)
                    {
                        Control.Dispose();
                    }
                }
                isDisposed = true;
            }
        }
    }
}