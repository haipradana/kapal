namespace Kapal.Models;

public class LandingWithVessel
{
    public int LandingId { get; set; }
   public int VesselId { get; set; }
    public string VesselName { get; set; } = "";
    public DateTime LandedAt { get; set; }
    public string Notes { get; set; } = "";
}

public class CatchWithVessel
{
    public int CatchId { get; set; }
  public int LandingId { get; set; }
    public int VesselId { get; set; }
  public string VesselName { get; set; } = "";
    public string Species { get; set; } = "";
    public decimal WeightKg { get; set; }
}
