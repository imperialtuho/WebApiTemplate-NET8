using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Domain.Common;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Domain.Enums;
using WebApiTemplate.Domain.Exceptions;
using WebApiTemplate.Domain.Extensions;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Infrastructure.Repositories.Providers
{
    /// <summary>
    /// Entity framework generic repository.
    /// </summary>
    /// <typeparam name="C">DbContext.</typeparam>
    /// <typeparam name="T">BaseEntity.</typeparam>
    public abstract class EntityFrameworkGenericRepository<C, T> : IEntityFrameworkGenericRepository<T>
        where T : BaseEntity<string>
        where C : DbContext
    {
        protected C _dbContext;
        protected ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected const int DefaultTenantId = 0;

        private UserSession? _UserSession;
        private int? TenantIdentify => _httpContextAccessor.GetTenantIdentify() ?? DefaultTenantId;
        public int? TenantId => LoginSession?.TenantId ?? TenantIdentify;

        public UserSession? LoginSession
        {
            get => _UserSession ?? _httpContextAccessor?.GetUserSession();
            set
            {
                _UserSession = value;
            }
        }

        protected EntityFrameworkGenericRepository(DbContextOptions<C> options, ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _dbContext = Activator.CreateInstance(typeof(C), options) as C ?? throw new InvalidOperationException("Cannot create DbContext");
            _httpContextAccessor = httpContextAccessor;
        }

        protected internal static DbContextOptions<C> CreateDbContextOptions(ISqlConnectionFactory sqlConnectionFactory, ConnectionStringType connectionStringType)
        {
            sqlConnectionFactory.SetConnectionStringType(connectionStringType);
            (string? connectionString, ConnectionStringType dbType) = sqlConnectionFactory.GetConnectionStringAndDbType();
            var optionsBuilder = new DbContextOptionsBuilder<C>();

            if (!string.IsNullOrEmpty(connectionString))
            {
                switch (dbType)
                {
                    case ConnectionStringType.PostgresqlConnection:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;

                    case ConnectionStringType.SqlServerConnection:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;

                    default:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;
                }
            }

            return optionsBuilder.Options;
        }

        public virtual async Task<bool> AddAndSaveChangesAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            int result = await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return result > 0;
        }

        public virtual async Task<T> AddWithSaveChangesAndReturnModelAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return entity;
        }

        public async Task AddAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                InitializeEntity(entity);
            }

            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<T> GetEntityByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id) ?? throw new NotFoundException($"{nameof(GetEntityByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        public virtual async Task<T> GetEntityWithRelationByIdAsync(object id)
        {
            T? entity = await _dbContext.Set<T>().IncludeAllNavigations(_dbContext).FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));

            return entity ?? throw new NotFoundException($"{nameof(GetEntityByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        public async Task<bool> UpdateAndSaveChangesAsync(T entity)
        {
            UpdateEntity(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            int result = await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return result > 0;
        }

        public async Task<T> UpdateWithSaveChangesAndReturnModelAsync(T entity)
        {
            UpdateEntity(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return entity;
        }

        public async Task<bool> DeleteAndSaveChangesAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteRangeAndSaveChangesAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<T>, IQueryable<T>>? predicate = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

            if (predicate != null)
            {
                query = predicate(query);
            }

            return await PaginatedResponse<T>.CreateAsync(query, pageNumber, pageSize);
        }

        private void InitializeEntity(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.TenantId = TenantId;

            if (string.IsNullOrEmpty(entity.CreatedBy))
            {
                entity.CreatedBy = LoginSession?.Email ?? "Site Administrators";
            }

            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = null;
            entity.ModifiedBy = null;
            entity.IsDeleted = false;
        }

        private void UpdateEntity(T entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = LoginSession?.Email ?? "Site Administrators";
        }
    }
}