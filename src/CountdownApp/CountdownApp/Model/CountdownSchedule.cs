using System;

namespace CountdownApp
{
  public class CountdownSchedule
  {
    public DateTime Started { get; set; }

    public DateTime NextCycle { get; set; }
    public DateTime PreviousCycle { get; set; }

    public bool Expired { get; set; }
  }
}