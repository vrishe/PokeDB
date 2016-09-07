using System;
using Xamarin.Forms;

namespace PokeDB.Views
{
    /// <summary>
    /// Remote image visual element.
    /// 
    /// Provides a cached image viewer for remote images that can be targeted with URL.
    /// 
    /// </summary>
    public class RemoteImage : View
    {
        /// <summary>
        /// Gets or sets the image URL. This is a bindable property.
        /// 
        /// Image download will start if necessary (see <see cref="IsLoading"/> also).
        /// </summary>
        /// <value>The image URL to locate a resource with.</value>
        public Uri ImageUrl
        {
            get
            {
                return (Uri)GetValue(ImageUrlProperty);
            }
            set
            {
                SetValue(ImageUrlProperty, value);
            }
        }

        /// <summary>
        /// The image URL bindable property. See <see cref="RemoteImage.ImageUrl"/>property description for details.
        /// 
        /// </summary>
        public static readonly BindableProperty ImageUrlProperty = BindableProperty
            .Create("ImageUrl", typeof(Uri), typeof(RemoteImage), default(Uri), BindingMode.OneWay, null,
                (bindable, valueOld, valueNew) => ((RemoteImage)bindable).OnImageUrlChanged((Uri)valueOld, (Uri)valueNew));


        /// <summary>
        /// Gets or sets the image placeholder when on loading or the remote image can't be displayed for some reason. This is a bindable property.
        /// 
        /// </summary>
        /// <value>The image resource identifier that must be shown on loading. This value is treated as a platform-specific.</value>
        public string PlaceholderImageResourceId
        {
            get
            {
                return (string)GetValue(PlaceholderImageResourceIdProperty);
            }
            set
            {
                SetValue(PlaceholderImageResourceIdProperty, value);
            }
        }

        /// <summary>
        /// A placeholder image to show up when the loading is in progress or has failed.
        /// 
        /// This value may present a platform specific resource ID as well as the .Net assembly embedded resource ID.
        /// 
        /// </summary>
        public static readonly BindableProperty PlaceholderImageResourceIdProperty = BindableProperty
            .Create("PlaceholderImageResourceId", typeof(string), typeof(RemoteImage), default(string));


        /// <summary>
        /// Gets or sets the aspect. This is a bindable property.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="T:Xamarin.Forms.Aspect"/> representing the scaling mode of the image. Default is <see cref="E:Xamarin.Forms.Aspect.AspectFit"/>.
        /// </value>
        public Aspect Aspect
        {
            get
            {
                return (Aspect)GetValue(AspectProperty);
            }
            set
            {
                SetValue(AspectProperty, value);
            }
        }
        /// <summary>
        /// The aspect bindable property. See <see cref="RemoteImage.Aspect"/>property description for details.
        /// 
        /// </summary>
        public static readonly BindableProperty AspectProperty = BindableProperty
            .Create("Aspect", typeof(Aspect), typeof(RemoteImage), Aspect.AspectFill);

        /// <summary>
        /// Gets the loading status of the image. This is a bindable readonly property.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="T:System.Boolean"/> indicating if the image is loading. Default is false.
        /// </value>
        public bool IsLoading
        {
            get
            {
                return (bool)GetValue(IsLoadingProperty);
            }
            internal set
            {
                SetValue(IsLoadingPropertyKey, value);
            }
        }

        internal static readonly BindablePropertyKey IsLoadingPropertyKey = BindableProperty
            .CreateReadOnly("IsLoading", typeof(bool), typeof(RemoteImage), default(bool));

        /// <summary>
        /// The is-loading bindable property. See <see cref="RemoteImage.IsLoading"/>property description for details.
        /// 
        /// </summary>
        public static readonly BindableProperty IsLoadingProperty = IsLoadingPropertyKey.BindableProperty;


        /// <summary>
        /// Gets the lates load operation status. This is a bindable readonly property.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="T:System.Boolean"/> indicating if the image lates image load has failed. Default is false.
        /// </value>
        public bool IsFailed
        {
            get
            {
                return (bool)GetValue(IsFailedProperty);
            }
            internal set
            {
                SetValue(IsFailedPropertyKey, value);
            }
        }

        internal static readonly BindablePropertyKey IsFailedPropertyKey = BindableProperty
            .CreateReadOnly("IsFailed", typeof(bool), typeof(RemoteImage), default(bool));

        /// <summary>
        /// The is-failed bindable property. See <see cref="RemoteImage.IsFailed"/>property description for details.
        /// 
        /// </summary>
        public static readonly BindableProperty IsFailedProperty = IsFailedPropertyKey.BindableProperty;



        /// <inheritdoc/>
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            SizeRequest sizeRequest = base.OnMeasure(double.PositiveInfinity, double.PositiveInfinity);

            double num1 = sizeRequest.Request.Width / sizeRequest.Request.Height;
            double num2 = widthConstraint / heightConstraint;
            double width1 = sizeRequest.Request.Width;
            double height1 = sizeRequest.Request.Height;

            if (width1 == 0.0 || height1 == 0.0)
                return new SizeRequest(new Size(0.0, 0.0));

            double width2 = width1;
            double height2 = height1;

            if (num2 > num1)
            {
                switch (this.Aspect)
                {
                    case Aspect.AspectFit:
                    case Aspect.AspectFill:
                        height2 = Math.Min(height1, heightConstraint);
                        width2 = width1 * (height2 / height1);
                        break;
                    case Aspect.Fill:
                        width2 = Math.Min(width1, widthConstraint);
                        height2 = height1 * (width2 / width1);
                        break;
                }
            }
            else if (num2 < num1)
            {
                switch (this.Aspect)
                {
                    case Aspect.AspectFit:
                    case Aspect.AspectFill:
                        width2 = Math.Min(width1, widthConstraint);
                        height2 = height1 * (width2 / width1);
                        break;
                    case Aspect.Fill:
                        height2 = Math.Min(height1, heightConstraint);
                        width2 = width1 * (height2 / height1);
                        break;
                }
            }
            else
            {
                width2 = Math.Min(width1, widthConstraint);
                height2 = height1 * (width2 / width1);
            }
            return new SizeRequest(new Size(width2, height2));
        }

        void OnImageUrlChanged(Uri valueOld, Uri valueNew)
        {
            this.InvalidateMeasure();
        }
    }
}
