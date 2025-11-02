using System.Collections.Generic;
using System.Threading.Tasks;
using SupabaseClient = Supabase.Client;

namespace Kapal.Services
{

    /// Base class untuk semua repository dengan inheritance

    public abstract class BaseRepository<TEntity, TModel> : IRepository<TModel>
        where TEntity : Supabase.Postgrest.Models.BaseModel, new()
    {
        protected readonly SupabaseClient _client;

        protected BaseRepository(SupabaseClient client)
        {
            _client = client;
        }

        // Abstract methods yang harus diimplementasikan oleh derived class
        public abstract Task<List<TModel>> GetAllAsync();
        public abstract Task<TModel> InsertAsync(TModel model);
        public abstract Task UpdateAsync(TModel model);
        public abstract Task DeleteAsync(int id);

        // Helper method untuk mapping (bisa digunakan oleh semua derived class)
        protected abstract TModel MapEntityToModel(TEntity entity);
        protected abstract TEntity MapModelToEntity(TModel model);
    }
}
