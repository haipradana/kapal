using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Kapal.Entities;

[Table("catch")]
public class CatchEntity : BaseModel
{
    [PrimaryKey("catch_id", false)]
    public int CatchId { get; set; }

    [Column("landing_id")] public int LandingId { get; set; }
    [Column("species")] public string Species { get; set; } = "";
    [Column("weight")] public decimal Weight { get; set; }
}
