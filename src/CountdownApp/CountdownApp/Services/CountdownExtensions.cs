using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace CountdownApp
{
  public static class CountdownExtensions
  {
    public static int GetDaysRemaining(this Countdown countdown, DateTime now)
    {
      var schedule = CountdownCalculator.GetSchedule(countdown, now);
      var progress = CountdownCalculator.GetProgress(schedule, now);

      var daysRemaining = progress.DaysRemaining;
      return daysRemaining;
    }

    public static async Task<bool> CreateImageFile(this Countdown countdown)
    {
      if (countdown.HasImage)
      {
        var filename = countdown.Guid;
        var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteBytesAsync(file, countdown.ImageData);

        return true;
      }

      return false;
    }

    public static async Task<bool> DeleteImageFile(this Countdown countdown)
    {
      var filename = countdown.Guid;
      var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(filename);

      if (file != null)
      {
        await file.DeleteAsync();

        return true;
      }

      return false;
    }

    public static Uri GetImageFileUri(this Countdown countdown)
    {
      var imageUri = countdown.HasImage ? new Uri("ms-appdata:///local/" + countdown.Guid) : null;

      return imageUri;
    }
  }
}