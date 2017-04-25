using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CountdownApp
{
  public static class NotificationManager
  {
    private static readonly ResourceLoader resourceLoader = ResourceLoader.GetForViewIndependentUse();

    public static bool RefreshCountdown(Countdown countdown)
    {
      var notificationId = countdown.Id.ToString();
      var notifier = ToastNotificationManager.CreateToastNotifier();

      RemoveNotification(notifier, notificationId);

      return UpdateNotification(notifier, notificationId, countdown);
    }

    public static void RemoveCountdown(Countdown countdown)
    {
      var notificationId = countdown.Id.ToString();
      var notifier = ToastNotificationManager.CreateToastNotifier();

      RemoveNotification(notifier, notificationId);
    }

    public static void UpdateNotifications(IList<Countdown> countdowns)
    {
      var notifier = ToastNotificationManager.CreateToastNotifier();
      var notifications = notifier.GetScheduledToastNotifications();

      foreach (var countdown in countdowns)
      {
        var notificationId = countdown.Id.ToString();
        var countdownNotifications = notifications.Where(x => x.Id == notificationId);

        if (countdown.FinishedNotification)
        {
          if (!countdownNotifications.Any())
          {
            UpdateNotification(notifier, notificationId, countdown);
          }
        }
        else
        {
          foreach (var notification in countdownNotifications)
          {
            notifier.RemoveFromSchedule(notification);
          }
        }
      }
    }

    private static void RemoveNotification(ToastNotifier notifier, string notificationId)
    {
      var notifications = notifier.GetScheduledToastNotifications();
      var countdownNotifications = notifications.Where(x => x.Id == notificationId);

      foreach (var notification in countdownNotifications)
      {
        notifier.RemoveFromSchedule(notification);
      }
    }

    private static bool UpdateNotification(ToastNotifier notifier, string notificationId, Countdown countdown)
    {
      var now = DateTime.Now;

      var schedule = CountdownCalculator.GetSchedule(countdown, now);
      var date = schedule.NextCycle;

      var content = CreateNotificationContent(countdown.Name, countdown.GetImageFileUri(), countdown.Guid);
      var notification = new ScheduledToastNotification(content, new DateTimeOffset(date))
      {
        Id = notificationId
      };

      if (notification.DeliveryTime > DateTime.Now)
      {
        notifier.AddToSchedule(notification);

        return true;
      }

      return false;
    }

    private static XmlDocument CreateNotificationContent(string name, Uri imageUri, string guid)
    {
      string image = null;
      if (imageUri != null)
      {
        image = $"<image placement='appLogoOverride' src='{imageUri}' hint-crop='circle' />";
      }

      var format = resourceLoader.GetString("CountdownNotificationTextFormatString");
      var text = string.Format(format, name);

      var showAction = resourceLoader.GetString("CountdownNotificationShowActionString");
      var dismissAction = resourceLoader.GetString("CountdownNotificationDismissActionString");

      var xml = $@"
        <toast scenario='reminder' launch='{guid}'>
          <visual>
            <binding template='ToastGeneric'>
              <text>{name}</text>
              <text>{text}</text>
              {image}
            </binding>
          </visual>

          <actions>
            <action activationType='foreground' arguments='{guid}' content ='{showAction}' />
            <action activationType='system' arguments='dismiss' content='{dismissAction}' />
          </actions>
        </toast>";

      var content = new XmlDocument();
      content.LoadXml(xml);

      return content;
    }
  }
}