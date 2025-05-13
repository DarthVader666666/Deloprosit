namespace Delopro.Bll.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity?>> GetListAsync(int? id = null);
        Task<TEntity?> GetAsync(int? id);
        Task<TEntity?> FindByAsync(object? parameter);
        Task<TEntity?> CreateAsync(TEntity? item);
        Task<TEntity?> UpdateAsync(TEntity? item);
        Task<TEntity?> DeleteAsync(int? id_1, int? id_2 = null);
        Task DeleteRangeAsync(IEnumerable<TEntity> items);
        Task<bool> ExistsAsync(TEntity? item);
    }
}
