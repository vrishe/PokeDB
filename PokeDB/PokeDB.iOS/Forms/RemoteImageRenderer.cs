using System;
using System.Threading.Tasks;
using Foundation;
using PokeDB.Forms;
using SDWebImage;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(RemoteImage), typeof(PokeDB.iOS.Forms.RemoteImageRenderer))]

namespace PokeDB.iOS.Forms
{
    /// <summary>
    /// RemoteImage view renderer.
    /// 
    /// </summary>
    public class RemoteImageRenderer : ViewRenderer<RemoteImage, UIImageView>
    {
        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<RemoteImage> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                SetNativeControl(new UIImageView(Frame)
                {
                    ClipsToBounds = true
                });
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
            Control.ContentMode = ToUIViewContentMode(Element.Aspect);
        }


        static readonly UIImage EmptyPlaceholder = new UIImage();

        private async void UpdateBitmap()
        {
            var imageUrl = Element.ImageUrl?.AbsoluteUri;
            var placeholderImage = !string.IsNullOrEmpty(Element.PlaceholderImageResourceId) 
                ? UIImage.FromBundle(Element.PlaceholderImageResourceId) : EmptyPlaceholder;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                Element.IsFailed = await UpdateBitmapAsync(imageUrl, placeholderImage);
            }
            else
            {
                Control.Image = placeholderImage;
                Element.IsFailed = false;
            }
        }

        /// <summary>
        /// Updates the bitmap asynchronously.
        /// 
        /// Here's a place where someone can perform own image fetching approach.
        /// </summary>
        /// <returns>>Should yield with <c>true</c> if image was loaded successfully, <c>false</c> otherwise.</returns>
        /// <param name="imageUrl">An image URL to retrieve image by.</param>
        /// <param name="placeholderImage">A placeholder image.</param>
        protected virtual Task<bool> UpdateBitmapAsync(string imageUrl, UIImage placeholderImage)
        {
            var tcs = new TaskCompletionSource<bool>();

            Element.IsLoading = true;
            Control.SetImage(new NSUrl(imageUrl), placeholderImage,
                SDWebImageOptions.RetryFailed | SDWebImageOptions.AllowInvalidSSLCertificates,
                (image, error, type, url) => {
                    Element.IsLoading = false;

                    tcs.SetResult(error == null);
                });

            return tcs.Task;
        }


        /// <summary>
        /// Convert <see cref="T:Xamarin.Forms.Aspect"/> to a corresponding iOS-specific <see cref="T:UIKit.UIViewContentMode"/> value.
        /// </summary>
        /// 
        /// <returns>The content mode which corresponds to an incoming aspect type.</returns>
        /// <param name="aspect">An aspect to perform conversion with.</param>
        protected static UIViewContentMode ToUIViewContentMode(Aspect aspect)
        {
            switch (aspect)
            {
                case Aspect.AspectFill:
                    return UIViewContentMode.ScaleAspectFill;
                case Aspect.Fill:
                    return UIViewContentMode.ScaleToFill;
                default:
                    return UIViewContentMode.ScaleAspectFit;
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
