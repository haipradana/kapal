using Kapal.Entities;
using Kapal.Models;
using Supabase;
using SupabaseClient = Supabase.Client;
using SupabasePostgrestClient = Supabase.Postgrest.Client;
using static Supabase.Postgrest.Constants;



namespace Kapal.Services;

public class VesselRepository
{
    private readonly SupabaseClient _client;
    public VesselRepository(SupabaseClient client) => _client = client;

    private static Vessel Map(VesselEntity e) => new()
    {
        VesselId = e.VesselId,
        Name = e.Name,
        RegNumber = e.RegNumber,
        OwnerName = e.OwnerName ?? "",
        Gear = e.Gear ?? ""
    };

    private static VesselEntity Map(Vessel d) => new()
    {
        VesselId = d.VesselId,
        Name = d.Name,
        RegNumber = d.RegNumber,
        OwnerName = string.IsNullOrWhiteSpace(d.OwnerName) ? null : d.OwnerName,
        Gear = string.IsNullOrWhiteSpace(d.Gear) ? null : d.Gear
    };

    public async Task<List<Vessel>> GetAllAsync()
    {
        var res = await _client.From<VesselEntity>().Order(v => v.VesselId, Ordering.Ascending).Get();
        return res.Models.Select(Map).ToList();
    }

    public async Task<Vessel> InsertAsync(Vessel vessel)
    {
        var inserted = await _client.From<VesselEntity>().Insert(Map(vessel));
        return Map(inserted.Models.First());
    }

    public async Task UpdateAsync(Vessel vessel) =>
        await _client.From<VesselEntity>().Update(Map(vessel));

    public async Task DeleteAsync(int vesselId) =>
        await _client.From<VesselEntity>().Where(v => v.VesselId == vesselId).Delete();
}

public class LandingRepository
{
    private readonly SupabaseClient _client;
    public LandingRepository(SupabaseClient client) => _client = client;

    private static Landing Map(LandingEntity e) => new()
    { LandingId = e.LandingId, VesselId = e.VesselId, LandedAt = e.LandedAt, Notes = e.Notes ?? "" };

    private static LandingEntity Map(Landing d) => new()
    { LandingId = d.LandingId, VesselId = d.VesselId, LandedAt = d.LandedAt, Notes = string.IsNullOrWhiteSpace(d.Notes) ? null : d.Notes };

    public async Task<List<Landing>> GetByVesselAsync(int vesselId)
    {
        var res = await _client.From<LandingEntity>()
            .Filter("vessel_id", Operator.Equals, vesselId)
            .Order(l => l.LandedAt, Ordering.Descending)
            .Get();
        return res.Models.Select(Map).ToList();
    }

    public async Task<Landing> InsertAsync(Landing landing)
    {
        var inserted = await _client.From<LandingEntity>().Insert(Map(landing));
        return Map(inserted.Models.First());
    }
}

public class CatchRepository
{
    private readonly SupabaseClient _client;
    public CatchRepository(SupabaseClient client) => _client = client;

    private static Models.Catch Map(CatchEntity e) => new()
    { CatchId = e.CatchId, LandingId = e.LandingId, Species = e.Species, WeightKg = e.Weight };

    private static CatchEntity Map(Models.Catch d) => new()
    { CatchId = d.CatchId, LandingId = d.LandingId, Species = d.Species, Weight = d.WeightKg };

    public async Task<List<Models.Catch>> GetByLandingAsync(int landingId)
    {
        var res = await _client.From<CatchEntity>().Filter("landing_id", Operator.Equals, landingId).Get();
        return res.Models.Select(Map).ToList();
    }

    public async Task<Models.Catch> InsertAsync(Models.Catch c)
    {
        var inserted = await _client.From<CatchEntity>().Insert(Map(c));
        return Map(inserted.Models.First());
    }
}
