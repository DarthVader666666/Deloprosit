namespace Deloprosit.Bll.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity?>> GetListAsync(int? id);
        Task<TEntity?> GetAsync(int? id);
        Task<TEntity?> FindByAsync(object? parameter);
        Task<TEntity?> CreateAsync(TEntity item);
        Task<TEntity?> UpdateAsync(TEntity item);
        Task<TEntity?> DeleteAsync(int? id);
        Task<bool> ExistsAsync(TEntity? item);
    }
}
