using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shared.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected internal readonly DbContext Context;

    public GenericRepository(DbContext dbContext) => this.Context = dbContext;


    public async Task<T?> GetAsync(long id) => await this.Context.Set<T>().FindAsync(id).ConfigureAwait(false);

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = this.Context.Set<T>().AsNoTracking();
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
        IQueryable<T> query = this.Context.Set<T>().AsNoTracking();
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, property) => current.Include(property));
        }
        if (orderBy != null) query = orderBy(query);
        return await query.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync() => await this.Context.Set<T>().AsNoTracking().ToListAsync().ConfigureAwait(false);

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = this.Context.Set<T>().AsNoTracking();
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
        if (pageNumber == 0) pageNumber = 1;
        IQueryable<T> query = this.Context.Set<T>().AsNoTracking();
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

    public IQueryable<T> GetQueryable() => this.Context.Set<T>().AsNoTracking();

    public async Task AddAsync(T model) => await this.Context.Set<T>().AddAsync(model).ConfigureAwait(false);

    public async Task AddRangeAsync(IReadOnlyList<T> models) => await this.Context.Set<T>().AddRangeAsync(models).ConfigureAwait(false);

    public void Update(T model) => this.Context.Update(model);

    public void UpdateRange(List<T> models) => this.Context.Set<T>().UpdateRange(models);

    public void Remove(T model)
    {
        if (this.Context.Entry(model).State == EntityState.Detached)
        {
            this.Context.Set<T>().Attach(model);
        }

        this.Context.Set<T>().Remove(model);
    }

    public Task RemoveRangeAsync(IReadOnlyList<T> models)
    {
        foreach (T model in models)
        {
            this.Context.Set<T>().Remove(model);
        }

        return Task.CompletedTask;
    }

    public void Remove(long id)
    {
        T? model = this.Context.Set<T>().Find(id);
        if (this.Context.Entry(model).State == EntityState.Detached)
        {
            if (model != null) this.Context.Set<T>().Attach(model);
        }

        if (model != null) this.Context.Set<T>().Remove(model);
    }

    public async Task RemoveAsync(Expression<Func<T, bool>> predicate)
    {
        T? model = await this.Context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate).ConfigureAwait(false);
        if (this.Context.Entry(model).State == EntityState.Detached)
        {
            if (model != null) this.Context.Set<T>().Attach(model);
        }

        if (model != null) this.Context.Set<T>().Remove(model);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await this.Context.Set<T>().AsNoTracking().AnyAsync(predicate).ConfigureAwait(false);

    public Task<int> CountAll() => this.Context.Set<T>().CountAsync();


    public Task<int> CountWhere(Expression<Func<T, bool>> predicate) => this.Context.Set<T>().CountAsync(predicate);

    public void Clear() => this.Context.ChangeTracker.Clear();
}