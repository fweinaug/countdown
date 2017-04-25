using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace CountdownApp
{
  public class SelectedImage
  {
    public WriteableBitmap OriginalBitmap { get; set; }
    public WriteableBitmap CroppedBitmap { get; set; }

    public bool IsValid
    {
      get { return OriginalBitmap != null; }
    }

    public bool IsSquare
    {
      get { return OriginalBitmap != null && OriginalBitmap.PixelWidth == OriginalBitmap.PixelHeight; }
    }

    public static async Task<SelectedImage> FromFile(StorageFile file)
    {
      return new SelectedImage
      {
        OriginalBitmap = await LoadFile(file),
        CroppedBitmap = null
      };
    }

    private static async Task<WriteableBitmap> LoadFile(StorageFile file)
    {
      using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
      {
        var decoder = await BitmapDecoder.CreateAsync(fileStream);
        var transform = new BitmapTransform();

        const double maxSize = 1000;

        var width = decoder.OrientedPixelWidth;
        var height = decoder.OrientedPixelHeight;

        if (width > maxSize || height > maxSize)
        {
          var scale = Math.Min(maxSize / width, maxSize / height);

          height = Convert.ToUInt32(height * scale);
          width = Convert.ToUInt32(width * scale);

          transform.ScaledWidth = width;
          transform.ScaledHeight = height;
        }

        var pixelData = await decoder.GetPixelDataAsync(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode,
          transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);
        var sourcePixels = pixelData.DetachPixelData();

        var bmp = new WriteableBitmap((int)width, (int)height);
        using (var bmpStream = bmp.PixelBuffer.AsStream())
        {
          bmpStream.Seek(0, SeekOrigin.Begin);
          bmpStream.Write(sourcePixels, 0, (int)bmpStream.Length);
          return bmp;
        }
      }
    }

    public async Task<byte[]> AsBytes()
    {
      var bitmap = CroppedBitmap ?? OriginalBitmap;
      if (bitmap == null)
        return null;

      using (var stream = new InMemoryRandomAccessStream())
      {
        await bitmap.ToStream(stream, BitmapEncoder.PngEncoderId);

        return await StreamToBytes(stream);
      }
    }

    private async Task<byte[]> StreamToBytes(IRandomAccessStream stream)
    {
      var dr = new DataReader(stream.GetInputStreamAt(0));
      var bytes = new byte[stream.Size];

      await dr.LoadAsync((uint)stream.Size);
      dr.ReadBytes(bytes);

      return bytes;
    }
  }
}