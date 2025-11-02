// Services/Repositories.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kapal.Entities;
using Kapal.Models;

using Supabase;
using SupabasePostgrest = Supabase.Postgrest;
using SupabaseClient = Supabase.Client;
using static Supabase.Postgrest.Constants;

namespace Kapal.Services
{

    /// VesselRepository dengan inheritance dari BaseRepository

    public class VesselRepository : BaseRepository<VesselEntity, Vessel>
    {
        public VesselRepository(SupabaseClient client) : base(client) { }

        protected override Vessel MapEntityToModel(VesselEntity e) => new()
        {
            VesselId = e.VesselId,
            Name = e.Name,
            RegNumber = e.RegNumber,
            OwnerName = e.OwnerName ?? "",
            Gear = e.Gear ?? ""
        };

        protected override VesselEntity MapModelToEntity(Vessel d) => new()
        {
            VesselId = d.VesselId,
            Name = d.Name,
            RegNumber = d.RegNumber,
            OwnerName = string.IsNullOrWhiteSpace(d.OwnerName) ? null : d.OwnerName,
            Gear = string.IsNullOrWhiteSpace(d.Gear) ? null : d.Gear
        };

        public override async Task<List<Vessel>> GetAllAsync()
        {
            var res = await _client
                .From<VesselEntity>()
                .Order(v => v.VesselId, Ordering.Ascending)
                .Get();

            return res.Models.Select(MapEntityToModel).ToList();
        }

        public override async Task<Vessel> InsertAsync(Vessel vessel)
        {
            var inserted = await _client.From<VesselEntity>().Insert(MapModelToEntity(vessel));
            return MapEntityToModel(inserted.Models.First());
        }

        public override async Task UpdateAsync(Vessel vessel) =>
            await _client.From<VesselEntity>().Update(MapModelToEntity(vessel));

        public override async Task DeleteAsync(int vesselId) =>
     await _client.From<VesselEntity>().Where(v => v.VesselId == vesselId).Delete();
    }

   
    /// LandingRepository dengan inheritance dari BaseRepository
    
    public class LandingRepository : BaseRepository<LandingEntity, Landing>
    {
        public LandingRepository(SupabaseClient client) : base(client) { }

        protected override Landing MapEntityToModel(LandingEntity e) => new()
    {
       LandingId = e.LandingId,
          VesselId = e.VesselId,
            LandedAt = e.LandedAt,
Notes = e.Notes ?? ""
    };

        protected override LandingEntity MapModelToEntity(Landing d) => new()
        {
            LandingId = d.LandingId,
  VesselId = d.VesselId,
        LandedAt = d.LandedAt,
        Notes = string.IsNullOrWhiteSpace(d.Notes) ? null : d.Notes
     };

        public override async Task<List<Landing>> GetAllAsync()
        {
var res = await _client
       .From<LandingEntity>()
      .Order(l => l.LandedAt, Ordering.Descending)
            .Get();

      return res.Models.Select(MapEntityToModel).ToList();
        }

        public async Task<List<Landing>> GetByVesselAsync(int vesselId)
    {
        var res = await _client
     .From<LandingEntity>()
           .Filter("vessel_id", Operator.Equals, vesselId)
                .Order(l => l.LandedAt, Ordering.Descending)
 .Get();

            return res.Models.Select(MapEntityToModel).ToList();
 }

      public override async Task<Landing> InsertAsync(Landing landing)
  {
            var inserted = await _client.From<LandingEntity>().Insert(MapModelToEntity(landing));
            return MapEntityToModel(inserted.Models.First());
        }

        public override async Task UpdateAsync(Landing landing) =>
            await _client.From<LandingEntity>().Update(MapModelToEntity(landing));

   public override async Task DeleteAsync(int landingId) =>
      await _client.From<LandingEntity>().Where(l => l.LandingId == landingId).Delete();
  }

    /// CatchRepository dengan inheritance dari BaseRepository
    
  public class CatchRepository : BaseRepository<CatchEntity, Models.Catch>
    {
      public CatchRepository(SupabaseClient client) : base(client) { }

      protected override Models.Catch MapEntityToModel(CatchEntity e) => new()
        {
       CatchId = e.CatchId,
            LandingId = e.LandingId,
            Species = e.Species,
   WeightKg = e.Weight
        };

        protected override CatchEntity MapModelToEntity(Models.Catch d) => new()
        {
   CatchId = d.CatchId,
LandingId = d.LandingId,
        Species = d.Species,
          Weight = d.WeightKg
        };

        public override async Task<List<Models.Catch>> GetAllAsync()
  {
  var res = await _client
.From<CatchEntity>()
         .Select("*")
         .Get();

       return res.Models.Select(MapEntityToModel).ToList();
        }

        public async Task<List<Models.Catch>> GetByLandingAsync(int landingId)
     {
            var res = await _client
    .From<CatchEntity>()
    .Filter("landing_id", Operator.Equals, landingId)
          .Get();

        return res.Models.Select(MapEntityToModel).ToList();
        }

        public override async Task<Models.Catch> InsertAsync(Models.Catch c)
      {
         var inserted = await _client.From<CatchEntity>().Insert(MapModelToEntity(c));
     return MapEntityToModel(inserted.Models.First());
  }

        public override async Task UpdateAsync(Models.Catch c) =>
   await _client.From<CatchEntity>().Update(MapModelToEntity(c));

 public override async Task DeleteAsync(int catchId) =>
      await _client.From<CatchEntity>().Where(c => c.CatchId == catchId).Delete();
    }
}
