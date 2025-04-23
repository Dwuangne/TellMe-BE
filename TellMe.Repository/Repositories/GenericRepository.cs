using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TellMe.Repository.DBContexts;

namespace TellMe.Repository.Repositories
{
    public class GenericRepository<T> where T : class
    {
        protected readonly TellMeDBContext _context;
        private readonly DbSet<T> _dbSet;

        private const int DefaultPageSize = 12;

        public GenericRepository(TellMeDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<(IEnumerable<T> Items, int TotalPages, int TotalRecords)> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            List<Expression<Func<T, object>>>? includes = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            // Apply includes
            query = ApplyIncludes(query, includes);

            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Get total records (only if pagination is requested)
            int totalRecords = pageIndex.HasValue && pageSize.HasValue
                ? await query.CountAsync()
                : 0;

            // Apply ordering
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Apply pagination
            int validPageSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : DefaultPageSize;
            int validPageIndex = pageIndex.HasValue && pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
            int totalPages = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / validPageSize) : 0;

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            // Execute query
            var items = await query.ToListAsync();

            return (items, totalPages, totalRecords);
        }

        private IQueryable<T> ApplyIncludes(IQueryable<T> query, List<Expression<Func<T, object>>>? includes)
        {
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}
