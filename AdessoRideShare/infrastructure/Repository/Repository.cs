using AdessoRideShare.model.context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AdessoRideShare.infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AdessoRideShareContext DbContext;
        protected readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AdessoRideShareContext context)
        {
            if (context == null)
            {
                context = new AdessoRideShareContext();
            }

            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void AddRange(IEnumerable<T> list)
        {
            _dbSet.AddRange(list);
        }

        public virtual void Attach(T entity)
        {
            _dbSet.Attach(entity);
        }

        public virtual void AttachRange(IEnumerable<T> list)
        {
            _dbSet.AttachRange(list);
        }

        public virtual void ChangeState(T entity, EntityState state)
        {
            _context.Entry(entity).State = state;
        }

        public virtual EntityState GetState(T entity)
        {
            return _context.Entry<T>(entity).State;
        }

        public virtual void Delete(T entity)
        {
            var dbEntity = _context.Entry(entity);

            if (dbEntity.State != EntityState.Deleted)
            {
                dbEntity.State = EntityState.Deleted;
            }
            else
            {
                _dbSet.Attach(entity);
                _dbSet.Remove(entity);
            }
        }

        public virtual void Delete(int id)
        {
            var entity = FindById(id);

            if (entity == null)
            {
                return;
            }
            else
            {
                if (entity.GetType().GetProperty("IsDeleted") != null)
                {
                    T _entity = entity;
                    _entity.GetType().GetProperty("IsDeleted").SetValue(_entity, true);
                    Update(_entity);
                }
                else
                {
                    Delete(entity);
                }
            }
        }

        public virtual void DeleteRange(IEnumerable<T> list)
        {
            _dbSet.RemoveRange(list);
        }

        public virtual void DeleteRange(IEnumerable<int> list)
        {
            foreach (var id in list)
            {
                var entity = FindById(id);

                if (entity == null)
                {
                    return;
                }
                else
                {
                    if (entity.GetType().GetProperty("IsDeleted") != null)
                    {
                        T _entity = entity;
                        _entity.GetType().GetProperty("IsDeleted").SetValue(_entity, true);
                        Update(_entity);
                    }
                    else
                    {
                        Delete(entity);
                    }
                }
            }
        }

        public virtual int ExecQuery(string query, bool addExec = false, params object[] parameters)
        {
            var sb = new StringBuilder();
            sb.Append(addExec ? "exec " + query : query);

            return _context.Database.ExecuteSqlRaw(sb.ToString(), parameters);
        }

        public virtual async Task<int> ExecQueryAsync(string query, bool addExec = false, params object[] parameters)
        {
            var sb = new StringBuilder();
            sb.Append(addExec ? "exec " + query : query);

            return await _context.Database.ExecuteSqlRawAsync(query, parameters);
        }

        public virtual T FindById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual async Task<T> FindByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual DataTable GetDataTable(string query)
        {
            return ConvertToDataTable(_dbSet.FromSqlRaw(query).ToList());
        }

        public virtual async Task<int> GetCount(Expression<Func<T, bool>> expression = null)
        {
            if (expression == null)
            {
                return await _dbSet.CountAsync();
            }
            else
            {
                return await _dbSet.Where(expression).CountAsync();
            }
        }

        public virtual async Task<DataTable> GetDataTableAsync(string query)
        {
            return ConvertToDataTable(await _dbSet.FromSqlRaw(query).ToListAsync());
        }

        public virtual T GetSingle(Expression<Func<T, bool>> expression)
        {
            return _dbSet.SingleOrDefault(expression);
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.SingleOrDefaultAsync(expression);
        }

        public virtual IList<T> GetSqlQueryResult(string query, params object[] parameters)
        {
            return _dbSet.FromSqlRaw<T>(query, parameters).ToList();
        }

        public virtual void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> expression = null, bool showDeletedRows = false)
        {
            if ((expression != null && showDeletedRows) || expression != null && !showDeletedRows && !HasFlag("<IsDeleted>"))
            {
                return _dbSet.Where(expression);
            }
            else if (expression != null && !showDeletedRows && HasFlag("<IsDeleted>"))
            {
                return _dbSet.Where(GenerateExpression(expression, "IsDeleted"));
            }
            else if (expression == null && showDeletedRows)
            {
                return _dbSet.AsQueryable();
            }
            else if (expression == null && !showDeletedRows && HasFlag("<IsDeleted>"))
            {
                return _dbSet.Where(GenerateExpression("IsDeleted"));
            }
            else
            {
                return _dbSet.AsQueryable();
            }
        }

        public virtual void QuickUpdate(T original, T updated)
        {
            _dbSet.Attach(original);
            _context.Entry(original).CurrentValues.SetValues(updated);
            _context.Entry(original).State = EntityState.Modified;
        }

        public virtual IList<T> SqlQuery(string query)
        {
            return _dbSet.FromSqlRaw(query).ToList();
        }

        public virtual async Task<IList<T>> SqlQueryAsync(string query)
        {
            return await _dbSet.FromSqlRaw(query).ToListAsync();
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Checking if dbset has IsDeleted field
        /// </summary>
        /// <param name="field">field name, example "<IsDeleted>"</param>
        /// <returns>boolean</returns>
        public bool HasFlag(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return false;
            }

            var hasFlag = false;
            var genericTypeArguments = _dbSet.GetType().GenericTypeArguments;

            if (genericTypeArguments.Any())
            {
                var fields = ((System.Reflection.TypeInfo)(_dbSet.GetType().GenericTypeArguments.FirstOrDefault())).DeclaredFields;

                hasFlag = fields.Any(x => x.Name.Contains(field));
            }

            return hasFlag;
        }

        public Expression<Func<T, bool>> GenerateExpression(string fieldName)
        {
            var argument = Expression.Parameter(typeof(T));
            var left = Expression.Property(argument, fieldName);
            var right = Expression.Constant(false);

            return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new[] { argument });
        }

        public Expression<Func<T, bool>> GenerateExpression(string fieldName, object value)
        {
            var argument = Expression.Parameter(typeof(T));
            var left = Expression.Property(argument, fieldName);
            var right = Expression.Constant(value);

            return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new[] { argument });
        }

        public Expression<Func<T, bool>> GenerateExpression(Expression<Func<T, bool>> expression, string fieldName)
        {
            var parameters = expression.Parameters;
            var checkNotDeleted = Expression.Equal(Expression.PropertyOrField(parameters[0], fieldName), Expression.Constant(false));
            var originalBody = expression.Body;
            var fullExpr = Expression.And(originalBody, checkNotDeleted);
            var lambda = Expression.Lambda<Func<T, bool>>(fullExpr, parameters);

            return lambda;
        }

        public Expression<Func<T, object>> GenerateSearchExpression(string fieldName)
        {
            var argument = Expression.Parameter(typeof(T));
            var left = Expression.Property(argument, fieldName);
            var right = Expression.Constant(false);

            return Expression.Lambda<Func<T, object>>(Expression.Equal(left, right), new[] { argument });
        }

        public Expression<Func<T, bool>> GenerateExpression(Expression<Func<T, bool>> expression, string fieldName, object value)
        {
            var parameters = expression.Parameters;
            var column = Expression.PropertyOrField(parameters[0], fieldName);
            var checkNotDeleted = Expression.Equal(column, Expression.Convert(Expression.Constant(value), column.Type));
            var originalBody = expression.Body;
            var fullExpr = Expression.And(originalBody, checkNotDeleted);
            var lambda = Expression.Lambda<Func<T, bool>>(fullExpr, parameters);

            return lambda;
        }

        public Expression<Func<T, bool>> GenerateExpressionLessThan(Expression<Func<T, bool>> expression, string fieldName, object value)
        {
            var parameters = expression.Parameters;
            var column = Expression.PropertyOrField(parameters[0], fieldName);
            var checkNotDeleted = Expression.LessThanOrEqual(column, Expression.Convert(Expression.Constant(value), column.Type));
            var originalBody = expression.Body;
            var fullExpr = Expression.And(originalBody, checkNotDeleted);
            var lambda = Expression.Lambda<Func<T, bool>>(fullExpr, parameters);

            return lambda;
        }

        public Expression<Func<T, bool>> CombineExpression(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameters = expression1.Parameters;
            var originalBody = expression1.Body;
            var fullExpr = Expression.And(originalBody, expression2.Body);
            var lambda = Expression.Lambda<Func<T, bool>>(fullExpr, parameters);

            return lambda;
        }
        public virtual DataTable ConvertToDataTable(IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                var row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
