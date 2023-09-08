using System.Linq.Expressions;

namespace Shared.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetAsync(long id);
    Task<T?> GetAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[]? includes);
    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[]? includes);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[]? includes);

    Task<IReadOnlyList<T>> GetAllPaginatedAsync(int pageNumber, int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[]? includes);

    IQueryable<T> GetQueryable();

    Task AddAsync(T model);
    Task AddRangeAsync(IReadOnlyList<T> models);
    void Update(T model);
    void UpdateRange(List<T> models);
    void Remove(T model);
    Task RemoveRangeAsync(Expression<Func<T, bool>> predicate);
    void Remove(long id);
    Task RemoveAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAll();
    Task<int> CountWhere(Expression<Func<T, bool>> predicate);

    void Clear();
}