using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdessoRideShare.infrastructure.Repository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task AddAsync(T entity);
        void AddRange(IEnumerable<T> list);
        void Attach(T entity);
        void AttachRange(IEnumerable<T> list);
        void ChangeState(T entity, EntityState state);
        DataTable ConvertToDataTable(IList<T> list);
        void Delete(T entity);
        void Delete(int id);
        void DeleteRange(IEnumerable<T> list);
        void DeleteRange(IEnumerable<int> list);
        int ExecQuery(string query, bool addExec = false, params object[] parameters);
        Task<int> ExecQueryAsync(string query, bool addExec = false, params object[] parameters);
        Expression<Func<T, object>> GenerateSearchExpression(string fieldName);
        Task<int> GetCount(Expression<Func<T, bool>> expression = null);
        DataTable GetDataTable(string query);
        Task<DataTable> GetDataTableAsync(string query);
        T GetSingle(Expression<Func<T, bool>> expression);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> expression);
        IList<T> GetSqlQueryResult(string query, params object[] parameters);
        T FindById(int id);
        Task<T> FindByIdAsync(int id);
        void Insert(T entity);
        IQueryable<T> Query(Expression<Func<T, bool>> expression = null, bool showDeletedRows = false);
        void QuickUpdate(T original, T updated);
        IList<T> SqlQuery(string query);
        Task<IList<T>> SqlQueryAsync(string query);
        void Update(T entity);
        EntityState GetState(T entity);
    }
}
