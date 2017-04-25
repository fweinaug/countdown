using System;

namespace CountdownApp
{
  public class Countdown
  {
    public int Id { get; set; }

    public string Name { get; set; }
    public DateTime Date { get; set; }
    public bool IsRecurrent { get; set; }

    public bool FinishedNotification { get; set; }
    public bool PinnedToStart { get; set; }

    public bool HasImage { get; set; }
    public byte[] ImageData { get; set; }

    public DateTime Created { get; set; }
    public string Guid { get; set; }
  }
}