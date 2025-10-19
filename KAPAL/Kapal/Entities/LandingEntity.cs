using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Kapal.Entities;

[Table("landing")]
public class LandingEntity : BaseModel  
{
    [PrimaryKey("landing_id", false)]
    public int LandingId { get; set; }

    [Column("vessel_id")] public int VesselId { get; set; }
    [Column("landed_at")] public DateTime LandedAt { get; set; }
    [Column("notes")] public string? Notes { get; set; }
}
