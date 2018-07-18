using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace CountdownApp
{
  public static class TileManager
  {
    private const int MaxScheduledNotifications = 5;

    public static async Task<bool> PinCountdown(Countdown countdown)
    {
      var tileId = countdown.Guid;

      if (SecondaryTile.Exists(tileId))
        return true;

      var tile = new SecondaryTile(tileId)
      {
        DisplayName = countdown.Name,
        Arguments = "Countdown"
      };

      tile.VisualElements.ShowNameOnSquare150x150Logo = true;
      tile.VisualElements.ShowNameOnSquare310x310Logo = true;
      tile.VisualElements.ShowNameOnWide310x150Logo = true;

      tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png");
      tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");
      tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");

      var created = await tile.RequestCreateAsync();
      if (!created)
        return false;

      var now = DateTime.Now;
      var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);

      UpdateNotification(updater, countdown, now);
      UpdateScheduledNotifications(updater, countdown, now);

      return true;
    }

    public static async Task<bool> UnpinCountdown(Countdown countdown)
    {
      var tileId = countdown.Guid;

      if (!SecondaryTile.Exists(tileId))
        return true;

      var tile = new SecondaryTile(tileId);

      var deleted = await tile.RequestDeleteAsync();
      return deleted;
    }

    public static async Task<bool> RefreshCountdown(Countdown countdown)
    {
      var tileId = countdown.Guid;

      var tiles = await SecondaryTile.FindAllAsync();
      var tile = tiles.SingleOrDefault(x => x.TileId == tileId);

      if (tile == null)
        return await PinCountdown(countdown);

      var now = DateTime.Now;
      var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);

      ClearNotifications(updater);
      UpdateNotification(updater, countdown, now);
      UpdateScheduledNotifications(updater, countdown, now);

      return true;
    }

    public static async void UpdateTiles(IList<Countdown> countdowns)
    {
      var groupedCountdowns = countdowns.GroupBy(x => x.Guid).ToList();

      var tiles = await SecondaryTile.FindAllAsync();
      foreach (var tile in tiles)
      {
        var countdown = groupedCountdowns.SingleOrDefault(x => x.Key == tile.TileId);
        if (countdown != null)
        {
          var now = DateTime.Now;
          var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);

          UpdateScheduledNotifications(updater, countdown.Single(), now);
        }
      }
    }

    private static void ClearNotifications(TileUpdater updater)
    {
      var scheduledNotifications = updater.GetScheduledTileNotifications();
      foreach (var notificaton in scheduledNotifications)
      {
        updater.RemoveFromSchedule(notificaton);
      }

      updater.Clear();
    }

    private static void UpdateNotification(TileUpdater updater, Countdown countdown, DateTime now)
    {
      var name = countdown.Name;
      var imageUri = countdown.GetImageFileUri();
      var daysRemaining = countdown.GetDaysRemaining(now);

      var content = CreateTileContent(name, imageUri, daysRemaining);
      var notification = new TileNotification(content);

      updater.Update(notification);
    }

    private static void UpdateScheduledNotifications(TileUpdater updater, Countdown countdown, DateTime now)
    {
      var start = now.Date + countdown.Date.TimeOfDay;
      if (start < now)
        start = start.AddDays(1);

      var scheduledNotifications = updater.GetScheduledTileNotifications();
      if (scheduledNotifications.Count > 0)
      {
        var latest = scheduledNotifications.Max(x => x.DeliveryTime.DateTime);
        if (latest > start)
          start = latest;
      }

      var name = countdown.Name;
      var imageUri = countdown.GetImageFileUri();

      var diff = (start.Date - now.Date).TotalDays;
      if (diff < MaxScheduledNotifications)
      {
        var end = start.AddDays(MaxScheduledNotifications - diff);

        for (var plan = start; plan <= end; plan = plan.AddDays(1))
        {
          var daysRemaining = countdown.GetDaysRemaining(plan);

          var content = CreateTileContent(name, imageUri, daysRemaining);
          var notification = new ScheduledTileNotification(content, plan);

          updater.AddToSchedule(notification);
        }
      }
    }

    private static XmlDocument CreateTileContent(string name, Uri imageUri, int days)
    {
      if (Math.Abs(days) >= 1000)
        days = Math.Sign(days) * 999;

      string number;
      if (days < 0)
      {
        number = "+" + Math.Abs(days);
      }
      else
      {
        number = days.ToString();
      }

      string image;
      string branding;
      if (imageUri != null)
      {
        image = $"<image src='{imageUri}' placement='background' hint-overlay='50' />";
        branding = "none";
      }
      else
      {
        image = null;
        branding = "name";
      }

      string smallTextStyle;
      string mediumTextStyle;
      if (number.Length <= 2)
      {
        smallTextStyle = "title";
        mediumTextStyle = "header";
      }
      else if (number.Length == 3)
      {
        smallTextStyle = "subtitle";
        mediumTextStyle = "header";
      }
      else // > 3
      {
        smallTextStyle = "caption";
        mediumTextStyle = "subheader";
      }

      var xml = $@"
        <tile version='3'>
            <visual branding='{branding}' displayName='{name}'>
                <binding template='TileSmall' hint-textStacking='center'>
                    {image}
                    <text hint-style='{smallTextStyle}' hint-align='center'>{number}</text>
                </binding>

                <binding template='TileMedium' hint-textStacking='center'>
                    {image}
                    <text hint-style='{mediumTextStyle}' hint-align='center'>{number}</text>
                </binding>
 
                <binding template='TileWide' hint-textStacking='center'>
                    {image}
                    <text hint-style='header' hint-align='center'>{number}</text>
                </binding>

                <binding template='TileLarge' hint-textStacking='center'>
                    {image}
                    <text hint-style='header' hint-align='center'>{number}</text>
                </binding>
            </visual>
        </tile>";

      var content = new XmlDocument();
      content.LoadXml(xml);

      return content;
    }
  }
}