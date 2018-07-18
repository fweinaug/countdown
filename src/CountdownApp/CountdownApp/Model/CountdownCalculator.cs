using System;

namespace CountdownApp
{
  public static class CountdownCalculator
  {
    public static CountdownSchedule GetSchedule(Countdown countdown, DateTime now)
    {
      var started = countdown.Date;
      var finished = started;
      var previous = started;
      var expired = false;

      int? cycles = null;

      if (started >= now)
      {
        started = countdown.Created;
        previous = started;
      }
      else if (countdown.IsRecurrent)
      {
        var date = started;
        var addedYears = 0;

        while (date < now)
        {
          date = date.AddYears(1);
          ++addedYears;
        }

        finished = date;
        if (addedYears > 1)
          previous = date.AddYears(-1);
        cycles = addedYears;
      }
      else
      {
        expired = finished < now;
      }

      return new CountdownSchedule
      {
        Started = started,
        NextCycle = finished,
        PreviousCycle = previous,
        NumberOfCycles = cycles,
        Expired = expired
      };
    }

    public static CountdownProgress GetProgress(CountdownSchedule schedule, DateTime now)
    {
      var finished = schedule.NextCycle;
      var previous = schedule.PreviousCycle;

      var remainingDays = (finished - now).TotalDays;
      var totalDays = (finished - previous).TotalDays;
      var progress = totalDays > 0d ? (1 - remainingDays / totalDays) : 1;

      var remainingDaysRounded = finished.TimeOfDay < now.TimeOfDay
        ? (int)Math.Ceiling(remainingDays)
        : (int)Math.Floor(remainingDays);

      var totalDaysRounded = finished.TimeOfDay < previous.TimeOfDay
        ? (int)Math.Ceiling(totalDays)
        : (int)Math.Floor(totalDays);

      return new CountdownProgress
      {
        DaysRemaining = remainingDaysRounded,
        DaysTotal = totalDaysRounded,
        ProgressPercentage = progress
      };
    }

    public static Time GetRemainingTime(CountdownSchedule schedule, DateTime now)
    {
      var finished = schedule.NextCycle;
      if (finished < now)
        return CalculateTime(finished, now);

      return CalculateTime(now, finished);
    }

    public static Time GetElapsedTime(CountdownSchedule schedule, DateTime now)
    {
      var finished = schedule.NextCycle;
      if (finished < now)
        return CalculateTime(finished, now);

      var started = schedule.Started;

      return CalculateTime(started, now);
    }

    private static Time CalculateTime(DateTime start, DateTime end)
    {
      var years = end.Year - start.Year;
      if (years > 0 && (end.Month < start.Month
        || end.Month == start.Month && (end.Day < start.Day || end.Day == start.Day && end.TimeOfDay < start.TimeOfDay)))
        --years;

      if (years > 0)
        start = start.AddYears(years);

      var months = 0;
      var start2 = start.AddMonths(1);
      while (start2 < end)
      {
        ++months;

        start2 = start2.AddMonths(1);
      }

      if (months > 0)
        start = start.AddMonths(months);

      var weeks = 0;
      var start3 = start.AddDays(7);
      while (start3 < end)
      {
        ++weeks;

        start3 = start3.AddDays(7);
      }

      if (weeks > 0)
        start = start.AddDays(weeks * 7);

      var ts = end - start;

      return new Time { Years = years, Months = months, Weeks = weeks, Days = ts.Days, Hours = ts.Hours, Minutes = ts.Minutes, Seconds = ts.Seconds };
    }
  }
}