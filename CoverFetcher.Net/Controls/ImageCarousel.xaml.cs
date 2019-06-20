using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoverFetcher.Controls
{
    /// <summary>
    /// Interaction logic for ImageControl.xaml
    /// </summary>
    public partial class ImageCarousel : UserControl
    {
        private int index;
        private List<ImageSource> images;

        public ImageCarousel()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ImageCarousel), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ImageCarousel;
            if (control != null)
                control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }

            // Add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }

            images = new List<ImageSource>(newValue.OfType<ImageSource>());
            if (images.Count > 0)
            {
                index = 0;

                DisplayImage(images[index]);

                if (images.Count > 1)
                {
                    backButton.Visibility = Visibility.Visible;
                    nextButton.Visibility = Visibility.Visible;
                }
            }
            else
                DisplayImage(null);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    images.InsertRange(e.NewStartingIndex, e.NewItems.OfType<ImageSource>());

                    if (images.Count == 1)
                    {
                        index = 0;
                        DisplayImage(images[index]);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    images.Clear();
                    index = 0;
                    DisplayImage(null);

                    break;
            }

            if (images.Count > 1)
            {
                backButton.Visibility = Visibility.Visible;
                nextButton.Visibility = Visibility.Visible;
            }
            else
            {
                backButton.Visibility = Visibility.Hidden;
                nextButton.Visibility = Visibility.Hidden;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (ItemsSource is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged -= OnCollectionChanged;
            }
        }

        private void OnBack(object sender, MouseButtonEventArgs e)
        {
            if (images?.Count > 0)
            {
                index = (--index > -1) ? index : (images.Count - 1);
                DisplayImage(images[index]);
            }
            else
                image.Source = null;
        }

        private void OnNext(object sender, MouseButtonEventArgs e)
        {
            if (images?.Count > 0)
            {
                index = (++index >= images.Count) ? 0 : index;
                DisplayImage(images[index]);
            }
            else
                image.Source = null;
        }

        private void DisplayImage(ImageSource imageSource)
        {
            image.Source = imageSource;
            bool isDark = IsDarkImage(imageSource);

            if(isDark)
            {
                backButton.Foreground = System.Windows.Media.Brushes.White;
                nextButton.Foreground = System.Windows.Media.Brushes.White;
            }
            else
            {
                backButton.Foreground = System.Windows.Media.Brushes.Black;
                nextButton.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private bool IsDarkImage(ImageSource image)
        {
            if (image is BitmapSource)
            {
                BitmapSource bitmapSource = (BitmapSource)image;

                return IsDark(bitmapSource, 40, 0.7);
            }
            else
                return false;
        }

        private Bitmap BitmapFromBitmapSource(BitmapSource bitmapSource)
        {
            // Based on sample code found at: https://blogs.msdn.microsoft.com/llobo/2007/03/08/bitmapsource-bitmap-interop/

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);

            byte[] bits = new byte[height * stride];
            bitmapSource.CopyPixels(bits, stride, 0);

            unsafe
            {
                fixed (byte* pBits = bits)
                {

                    IntPtr ptr = new IntPtr(pBits);

                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, ptr);

                    return bitmap;
                }
            }
        }

        private bool IsDark(Bitmap bitmap, byte tolerance, double darkProcent)
        {
            byte[] bytes = BitmapToByteArray(bitmap);
            int count = 0, all = bitmap.Width * bitmap.Height;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                byte r = bytes[i + 2], g = bytes[i + 1], b = bytes[i];
                byte brightness = (byte)Math.Round((0.299 * r + 0.5876 * g + 0.114 * b));
                if (brightness <= tolerance)
                    count++;
            }
            return (1d * count / all) >= darkProcent;
        }

        private static unsafe byte[] BitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bmd = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                                             System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] bytes = new byte[bmd.Height * bmd.Stride];
            byte* pnt = (byte*)bmd.Scan0;
            Marshal.Copy((IntPtr)pnt, bytes, 0, bmd.Height * bmd.Stride);
            bitmap.UnlockBits(bmd);
            return bytes;
        }

        private bool IsDark(BitmapSource bitmapSource, byte tolerance, double darkProcent)
        {
            byte[] bytes = BitmapSourceToByteArray(bitmapSource);
            int count = 0, all = bitmapSource.PixelWidth * bitmapSource.PixelHeight;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                byte r = bytes[i + 2], g = bytes[i + 1], b = bytes[i];
                byte brightness = (byte)Math.Round((0.299 * r + 0.5876 * g + 0.114 * b));
                if (brightness <= tolerance)
                    count++;
            }
            return (1d * count / all) >= darkProcent;
        }

        private byte[] BitmapSourceToByteArray(BitmapSource bitmapSource)
        {
            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);

            byte[] bits = new byte[height * stride];
            bitmapSource.CopyPixels(bits, stride, 0);

            return bits;
        }
    }
}
