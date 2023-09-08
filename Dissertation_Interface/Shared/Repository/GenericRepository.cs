using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shared.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected internal readonly DbContext _context;

    public GenericRepository(DbContext dbContext) => this._context = dbContext;


    public async Task<T?> GetAsync(long id) => await this._context.Set<T>().FindAsync(id).ConfigureAwait(false);

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = this._context.Set<T>().AsNoTracking();
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, property) => current.Include(property));
        }
        if (orderBy != null) query = orderBy(query);
        return await query.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = this._context.Set<T>().AsNoTracking();
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, property) => current.Include(property));
        }
        if (orderBy != null) query = orderBy(query);
        return await query.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync() => await this._context.Set<T>().AsNoTracking().ToListAsync().ConfigureAwait(false);

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = this._context.Set<T>().AsNoTracking();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, property) => current.Include(property));
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ToListAsync().ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> GetAllPaginatedAsync(int pageNumber, int pageSize,
        Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[]? includes)
    {
        if(pageNumber == 0) pageNumber = 1;
        IQueryable<T> query = this._context.Set<T>().AsNoTracking();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, property) => current.Include(property));
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.Skip(pageNumber - 1).Take(pageSize).ToListAsync().ConfigureAwait(false);
    }

    public IQueryable<T> GetQueryable() => this._context.Set<T>().AsNoTracking();

    public async Task AddAsync(T model) => await this._context.Set<T>().AddAsync(model).ConfigureAwait(false);

    public async Task AddRangeAsync(IReadOnlyList<T> models) =>  await this._context.Set<T>().AddRangeAsync(models).ConfigureAwait(false);

    public void Update(T model) => this._context.Update(model);

    public void UpdateRange(List<T> models) => this._context.Set<T>().UpdateRange(models);

    public void Remove(T model)
    {
        if (this._context.Entry(model).State == EntityState.Detached)
        {
            this._context.Set<T>().Attach(model);
        }

        this._context.Set<T>().Remove(model);
    }

    public async Task RemoveRangeAsync(Expression<Func<T, bool>> predicate)
    {
        List<T> models = await this._context.Set<T>().AsNoTracking().Where(predicate).ToListAsync().ConfigureAwait(false);
        foreach (T model in models)
        {
            this._context.Set<T>().Remove(model);
        }
    }

    public void Remove(long id)
    {
        T? model = this._context.Set<T>().Find(id);
        if (this._context.Entry(model).State == EntityState.Detached)
        {
            if (model != null) this._context.Set<T>().Attach(model);
        }

        if (model != null) this._context.Set<T>().Remove(model);
    }

    public async Task RemoveAsync(Expression<Func<T, bool>> predicate)
    {
        T? model = await this._context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate).ConfigureAwait(false);
        if (this._context.Entry(model).State == EntityState.Detached)
        {
            if (model != null) this._context.Set<T>().Attach(model);
        }

        if (model != null) this._context.Set<T>().Remove(model);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await this._context.Set<T>().AsNoTracking().AnyAsync(predicate).ConfigureAwait(false);

    public Task<int> CountAll() => this._context.Set<T>().CountAsync();


    public Task<int> CountWhere(Expression<Func<T, bool>> predicate) => this._context.Set<T>().CountAsync(predicate);

    public void Clear() => this._context.ChangeTracker.Clear();
}