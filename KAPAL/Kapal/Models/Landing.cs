namespace Kapal.Models;

public class Landing
{
    private int _landingId;
    private int _vesselId;
    private DateTime _landedAt;
    private string _notes = "";

    public int LandingId { get => _landingId; set => _landingId = value; }
    public int VesselId { get => _vesselId; set => _vesselId = value; }
    public DateTime LandedAt { get => _landedAt; set => _landedAt = value; }
    public string Notes { get => _notes; set => _notes = value; }
}
