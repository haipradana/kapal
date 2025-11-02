namespace Kapal.Services;

/// Interface untuk repository pattern dengan polymorphism

public interface IRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T> InsertAsync(T model);
    Task UpdateAsync(T model);
    Task DeleteAsync(int id);
}
