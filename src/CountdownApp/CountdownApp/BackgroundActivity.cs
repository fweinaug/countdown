using System;
using System.Linq;
using Windows.ApplicationModel.Background;

namespace CountdownApp
{
  public class BackgroundActivity
  {
    private readonly CountdownService countdownService = new CountdownService();

    public void Run(IBackgroundTaskInstance taskInstance)
    {
      var deferral = taskInstance.GetDeferral();

      var countdowns = countdownService.GetCountdowns();

      if (countdowns.Count > 0)
      {
        TileManager.UpdateTiles(countdowns);
        NotificationManager.UpdateNotifications(countdowns);
      }

      deferral.Complete();
    }

    public static void Start(IBackgroundTaskInstance taskInstance)
    {
      var task = new BackgroundActivity();
      task.Run(taskInstance);
    }

    public static async void RegisterBackgroundTask()
    {
      var name = nameof(BackgroundActivity);

      var registered = BackgroundTaskRegistration.AllTasks.Values.Any(task => task.Name == name);
      if (registered)
        return;

      var requestTask = await BackgroundExecutionManager.RequestAccessAsync();
      if (requestTask != BackgroundAccessStatus.AlwaysAllowed && requestTask != BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
        return;

      var builder = new BackgroundTaskBuilder
      {
        Name = name
      };

      builder.SetTrigger(new TimeTrigger(720, false));
      builder.Register();
    }
  }
}