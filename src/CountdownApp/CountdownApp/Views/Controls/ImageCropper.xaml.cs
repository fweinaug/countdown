using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace CountdownApp
{
  public sealed partial class ImageCropper : UserControl
  {
    public static readonly DependencyProperty SourceImageProperty = DependencyProperty.Register(
      "SourceImage", typeof(WriteableBitmap), typeof(ImageCropper), new PropertyMetadata(null, OnSourceImageChanged));

    public static readonly DependencyProperty CroppedImageProperty = DependencyProperty.Register(
      "CroppedImage", typeof(WriteableBitmap), typeof(ImageCropper), null);

    private readonly SelectedRegion selectedRegion = new SelectedRegion { MinSelectRegionSize = 60 };

    public WriteableBitmap SourceImage
    {
      get { return (WriteableBitmap)GetValue(SourceImageProperty); }
      set { SetValue(SourceImageProperty, value); }
    }

    public WriteableBitmap CroppedImage
    {
      get { return (WriteableBitmap)GetValue(CroppedImageProperty); }
      set { SetValue(CroppedImageProperty, value); }
    }

    public SelectedRegion SelectedRegion
    {
      get { return selectedRegion; }
    }

    public ImageCropper()
    {
      InitializeComponent();
    }

    private static void OnSourceImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var imageCropper = (ImageCropper)d;
      var bitmap = e.NewValue as WriteableBitmap;

      imageCropper.Image.Source = bitmap;
    }

    private void SelectRegionManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
      this.selectedRegion.UpdateSelectedRect(e.Delta.Scale, e.Delta.Translation.X, e.Delta.Translation.Y);

      e.Handled = true;
    }

    private void SelectRegionManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
      UpdateCroppedImage();
    }

    private void SourceImageSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (e.NewSize.IsEmpty || double.IsNaN(e.NewSize.Height) || e.NewSize.Height <= 0)
      {
        ImageCanvas.Visibility = Visibility.Collapsed;

        this.selectedRegion.OuterRect = Rect.Empty;
        this.selectedRegion.ResetCorner(0, 0, 0, 0);
      }
      else
      {
        ImageCanvas.Visibility = Visibility.Visible;
        ImageCanvas.Height = e.NewSize.Height;
        ImageCanvas.Width = e.NewSize.Width;

        this.selectedRegion.OuterRect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

        var width = Math.Min(e.NewSize.Width, e.NewSize.Height);
        this.selectedRegion.ResetCorner(0, 0, width, width);

        UpdateCroppedImage();
      }
    }

    private void UpdateCroppedImage()
    {
      var bitmap = (WriteableBitmap)Image.Source;

      var sourceImageWidthScale = ImageCanvas.Width / bitmap.PixelWidth;
      var sourceImageHeightScale = ImageCanvas.Height / bitmap.PixelHeight;

      var previewImageSize = new Size(
          this.selectedRegion.SelectedRect.Width / sourceImageWidthScale,
          this.selectedRegion.SelectedRect.Height / sourceImageHeightScale);

      CroppedImage = bitmap.Crop((int)(this.selectedRegion.SelectedRect.X / sourceImageWidthScale),
        (int)(this.selectedRegion.SelectedRect.Y / sourceImageHeightScale),
        (int)previewImageSize.Width, (int)previewImageSize.Height);
    }
  }
}