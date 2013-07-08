using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using THOK.Common.Ef.Interfaces;
using Microsoft.Practices.Unity;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace THOK.Common.Ef.EntityRepository
{
    public abstract class RepositoryBase<T> : IRepository<T>
        where T: class
    {
        [Dependency]
        public IRepositoryContext RepositoryContext { get; set; }

        public DbSet<T> dbSet { get { return RepositoryContext.GetDbSet<T>(); } }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);        
        }

        public void Attach(T entity)
        {
            var entry = RepositoryContext.DbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                entry.State = EntityState.Modified;
            }
        }

        public void Detach(T entity)
        {
            ((IObjectContextAdapter)RepositoryContext.DbContext).ObjectContext.Detach(entity);
        }

        public IQueryable<T> GetQueryable()
        {
            return this.dbSet.AsQueryable<T>();
        }

        public ParallelQuery<T> GetParallelQuery()
        {
            return this.dbSet.AsParallel<T>();
        }

        public ObjectSet<T> GetObjectSet()
        {
            return ((IObjectContextAdapter)RepositoryContext.DbContext)
                .ObjectContext.CreateObjectSet<T>();
        }

        public ObjectQuery<T> GetObjectQuery()
        {
            return ((IObjectContextAdapter)RepositoryContext.DbContext)
                .ObjectContext.CreateObjectSet<T>() as ObjectQuery<T>;
        }

        public ObjectContext GetObjectContext()
        {
            return ((IObjectContextAdapter)RepositoryContext.DbContext)
                .ObjectContext;
        }

        public IList<T> GetAll()
        {
            return this.dbSet.ToList<T>();
        }

        public IList<T> GetAll(Expression<Func<T, bool>> whereCondition)
        {
            return this.dbSet.Where(whereCondition).ToList<T>();
        }

        public T GetSingle()
        {
            return this.dbSet.FirstOrDefault<T>();
        }

        public T GetSingle(Expression<Func<T, bool>> whereCondition)
        {
            return this.dbSet.Where(whereCondition).FirstOrDefault<T>();
        }              

        public long Count()
        {
            return this.dbSet.LongCount<T>();
        }

        public long Count(Expression<Func<T, bool>> whereCondition)
        {
            return this.dbSet.Where(whereCondition).LongCount<T>();
        }

        //动态生成ID
        public string GetNewID(string TableName, string ColumnName)
        {
            string strNew = "";
            System.Collections.Generic.IList<T> LUser = GetAll();
            if (LUser.Count == 0)
            {
                string strSQL = string.Format("select data_length as USER_ID  from  user_tab_cols where table_name='{0}' and column_name='{1}'", TableName, ColumnName);
                decimal DataLength = RepositoryContext.DbContext.Database.SqlQuery<decimal>(strSQL).ToList()[0];
                strNew = "1".PadLeft((int)DataLength, '0');
            }
            else
            {

                string strValue = GetAll().Select(i => i.GetType().GetProperty(ColumnName).GetValue(i, null)).Max().ToString();
                strNew = (int.Parse(strValue) + 1).ToString().PadLeft(strValue.Length, '0');

            }
            return strNew;
        }

        public string GetNewID(string PreName, DateTime dt, string AutoCode)
        {
            var pre = RepositoryContext.DbContext.Database.SqlQuery<PrefixTableCode>(string.Format("select * from sys_table_code where PREFIX_CODE='{0}'", PreName)).FirstOrDefault();
            string strNew = "";
            string strSQL = "";
            string PreCode = PreName + dt.ToString(pre.DATE_FORMAT);
            if (!string.IsNullOrEmpty(AutoCode))
            {
                strSQL = string.Format("select {1} from {0} where {1}='{2}'", pre.TABLE_NAME, pre.FIELD_NAME, AutoCode);
                var tmp = RepositoryContext.DbContext.Database.SqlQuery<string>(strSQL);
                if (tmp.Count() == 0)
                    return AutoCode;
            }
            string SuqueceNo = "";
            for (int i = 0; i < int.Parse(pre.SERIAL_LENGTH); i++)
            {
                SuqueceNo += "[0-9]";
            }
            strSQL = string.Format("select {1} from {0} where regexp_like ({1},'^{2}$')", pre.TABLE_NAME, pre.FIELD_NAME, PreCode + SuqueceNo);
            var tmp2 = RepositoryContext.DbContext.Database.SqlQuery<string>(strSQL);
            if (tmp2.Count() > 0)
            {
                string value = tmp2.Max().ToString();
                strNew = PreCode + (int.Parse(value.Substring(PreCode.Length, int.Parse(pre.SERIAL_LENGTH))) + 1).ToString().PadLeft(int.Parse(pre.SERIAL_LENGTH), '0');
            }
            else
            {
                strNew = PreCode + "1".PadLeft(int.Parse(pre.SERIAL_LENGTH), '0');
            }
            return strNew;
        }


        //todo
        public int SaveChanges()
        {
            return RepositoryContext.SaveChanges();
        }
        //todo
        public void Delete<TSub>(TSub[] tsubs)
        {
            if (tsubs.Any())
            {
                var dbSet = RepositoryContext.DbContext.Set(tsubs.First().GetType());
                foreach (var tsub in tsubs)
                {
                    dbSet.Remove(tsub);
                }
            }
        }            
    }
}
