using System;
using System.Collections.Generic;
namespace THOK.Common.Ef.Interfaces
{
    public interface IRepository<T>
     where T : class
    {
        void Add(T entity);
        void Attach(T entity);
        long Count();
        long Count(System.Linq.Expressions.Expression<Func<T, bool>> whereCondition);
        System.Data.Entity.DbSet<T> dbSet { get; }
        void Delete(T entity);
        void Delete<TSub>(TSub[] tsubs);
        void Detach(T entity);
        System.Collections.Generic.IList<T> GetAll();
        System.Collections.Generic.IList<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> whereCondition);
        System.Linq.IQueryable<T> GetQueryable();
        System.Linq.ParallelQuery<T> GetParallelQuery();
        System.Data.Objects.ObjectSet<T> GetObjectSet();
        System.Data.Objects.ObjectQuery<T> GetObjectQuery();
        System.Data.Objects.ObjectContext GetObjectContext();
        T GetSingle();
        T GetSingle(System.Linq.Expressions.Expression<Func<T, bool>> whereCondition);
        THOK.Common.Ef.Interfaces.IRepositoryContext RepositoryContext { get; set; }
        int SaveChanges();
        void Update(T entity);
        //动态生成ID
        string GetNewID(string TableName, string ColumnName);

        string GetNewID(string PreName, DateTime dt, string AutoCode);
        //执行存储过程.
         int Exeprocedure(string storename, out string error);
        //执行sql语句
         System.Linq.IQueryable<T> Exesqlstr(string sqlstr);
    }
}
