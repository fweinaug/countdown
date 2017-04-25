using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountdownApp
{
  public class CountdownService
  {
    public IList<Countdown> GetCountdowns()
    {
      using (var database = new CountdownContext())
      {
        var countdowns = database.Countdowns.ToList();

        database.SaveChanges();

        return countdowns;
      }
    }

    public async Task<Countdown> SaveCountdown(int countdownId, string name, DateTime dateTime, bool isRecurrent, byte[] imageData,
      bool finishedNotification, bool pinnedToStart)
    {
      if (string.IsNullOrWhiteSpace(name))
        name = "Countdown";

      using (var database = new CountdownContext())
      {
        var countdown = database.Countdowns.SingleOrDefault(x => x.Id == countdownId);
        if (countdown == null)
        {
          countdown = new Countdown
          {
            Created = DateTime.Now,
            Guid = Guid.NewGuid().ToString()
          };

          database.Countdowns.Add(countdown);
        }
        else
        {
          if (imageData == null && countdown.Name == name
              && countdown.Date == dateTime && countdown.IsRecurrent == isRecurrent
              && countdown.FinishedNotification == finishedNotification && countdown.PinnedToStart == pinnedToStart)
            return countdown;
        }

        countdown.Name = name;
        countdown.Date = dateTime;
        countdown.IsRecurrent = isRecurrent;
        countdown.FinishedNotification = finishedNotification;
        countdown.PinnedToStart = pinnedToStart;

        if (imageData != null)
        {
          if (imageData.Length > 0)
          {
            countdown.ImageData = imageData;
            countdown.HasImage = true;

            await countdown.CreateImageFile();
          }
          else if (countdown.HasImage)
          {
            countdown.ImageData = null;
            countdown.HasImage = false;

            await countdown.DeleteImageFile();
          }
        }

        if (countdown.PinnedToStart)
        {
          countdown.PinnedToStart = await TileManager.RefreshCountdown(countdown);
        }
        else
        {
          await TileManager.UnpinCountdown(countdown);
        }

        if (countdown.FinishedNotification)
        {
          countdown.FinishedNotification = NotificationManager.RefreshCountdown(countdown);
        }
        else
        {
          NotificationManager.RemoveCountdown(countdown);
        }

        await database.SaveChangesAsync(); 

        return countdown;
      }
    }

    public async Task DeleteCountdown(int countdownId)
    {
      using (var database = new CountdownContext())
      {
        var countdown = database.Countdowns.Single(x => x.Id == countdownId);

        if (countdown.PinnedToStart)
          await TileManager.UnpinCountdown(countdown);

        if (countdown.HasImage)
          await countdown.DeleteImageFile();

        database.Countdowns.Remove(countdown);

        await database.SaveChangesAsync();
      }
    }

    public async Task<bool> PinCountdown(int countdownId)
    {
      using (var database = new CountdownContext())
      {
        var countdown = database.Countdowns.Single(x => x.Id == countdownId);

        var success = await TileManager.PinCountdown(countdown);
        if (!success)
          return false;

        countdown.PinnedToStart = true;

        await database.SaveChangesAsync();

        return true;
      }
    }

    public async Task<bool> UnpinCountdown(int countdownId)
    {
      using (var database = new CountdownContext())
      {
        var countdown = database.Countdowns.Single(x => x.Id == countdownId);

        var success = await TileManager.UnpinCountdown(countdown);
        if (!success)
          return false;

        countdown.PinnedToStart = false;

        await database.SaveChangesAsync();

        return true;
      }
    }
  }
}