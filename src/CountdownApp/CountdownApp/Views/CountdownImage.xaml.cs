using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace CountdownApp
{
  public sealed partial class CountdownImage : UserControl
  {
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
      "ImageSource", typeof(byte[]), typeof(CountdownImage), new PropertyMetadata(null, OnImageSourceChanged));

    public byte[] ImageSource
    {
      get { return (byte[])GetValue(ImageSourceProperty); }
      set { SetValue(ImageSourceProperty, value); }
    }

    public CountdownImage()
    {
      InitializeComponent();
    }

    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var countdownImage = (CountdownImage)d;
      var imageData = e.NewValue as byte[];

      countdownImage.ChangeImage(imageData);
    }

    private async void ChangeImage(byte[] byteArray)
    {
      if (byteArray != null && byteArray.Length > 0)
      {
        var imageSource = await GetImageSource(byteArray, (int)Width * 2, (int)Height * 2);

        ImageShape.Fill = new ImageBrush { ImageSource = imageSource };
      }
      else
      {
        ImageShape.ClearValue(Shape.FillProperty);
      }
    }

    private static async Task<ImageSource> GetImageSource(byte[] byteArray, int width, int height)
    {
      using (var stream = new InMemoryRandomAccessStream())
      {
        await stream.WriteAsync(byteArray.AsBuffer());

        var image = new BitmapImage
        {
          DecodePixelWidth = width,
          DecodePixelHeight = height
        };

        stream.Seek(0);
        image.SetSource(stream);
        return image;
      }
    }
  }
}