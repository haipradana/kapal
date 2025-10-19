using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Kapal.Entities;

[Table("vessel")]
public class VesselEntity : BaseModel
{
    [PrimaryKey("vessel_id", false)]
    public int VesselId { get; set; }

    [Column("name")] public string Name { get; set; } = "";
    [Column("reg_number")] public string RegNumber { get; set; } = "";
    [Column("owner_name")] public string? OwnerName { get; set; }
    [Column("gear")] public string? Gear { get; set; }
}
