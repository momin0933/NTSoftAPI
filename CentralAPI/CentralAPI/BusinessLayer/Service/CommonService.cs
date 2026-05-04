using Microsoft.EntityFrameworkCore;
using CentralAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using static Dapper.SqlMapper;

namespace CentralAPI.BusinessLayer.Service 
{ 
    public class CommonService : ICommonService, IDisposable
    {
        //protected NTSoftDbContext ctx { get; set; }
        private readonly NTSoftDbContextFactory _factory;
        public CommonService(NTSoftDbContextFactory factory)
        {
            _factory = factory;
        }
        public void Dispose()
        {

        }        
        public virtual int Add<T>(T entity, bool save = true) where T : Base
        {
            //using var ctx = _factory.CreateDbContext();
            //ctx.Set<T>().Add(entity);
            //if (save)
            //{
            //    Save();
            //}
            //return entity.Id;
            using var ctx = _factory.CreateDbContext();

            ctx.Set<T>().Add(entity);

            if (save)
            {
                return ctx.SaveChanges();  // ✅ same context
            }

            return entity.Id;
        }
        public virtual int AddRange<T>(List<T> entity, bool save = true) where T : Base
        {
            using var ctx = _factory.CreateDbContext();
            ctx.Set<T>().AddRange(entity);
            if (save)
            {
                Save();
            }
            return entity.FirstOrDefault().Id;
        }
        public virtual IEnumerable<T> GetAll<T>() where T : Base
        {
            using var ctx = _factory.CreateDbContext();
            return ctx.Set<T>().ToList();  // ✅ FIX
        }
        public virtual IEnumerable<T> GetAllAccounts<T>() where T : class
        {
            using var ctx = _factory.CreateDbContext();
            return ctx.Set<T>().AsEnumerable();
        }      

        public virtual IQueryable<T> Query<T>() where T : Base
        {
            using var ctx = _factory.CreateDbContext();
            return ctx.Set<T>().AsQueryable<T>();
        }
        public virtual T Get<T>(int id) where T : Base
        {
            using var ctx = _factory.CreateDbContext();
            return ctx.Set<T>().FirstOrDefault(x => x.Id == id);
        }
        public virtual T GetWithNoTracking<T>(int id) where T : Base
        {
            using var ctx = _factory.CreateDbContext();
            return ctx.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
        }
        public virtual int Update<T>(T entity, bool save = true) where T : Base
        {
            using var ctx = _factory.CreateDbContext();
            ctx.Set<T>().Attach(entity);
            ctx.Entry(entity).State = EntityState.Modified;
            if (save)
            {
                return ctx.SaveChanges();   // ✅ SAME CONTEXT
            }
            return entity.Id;           
        }
        public virtual int UpdateByProperty<T>(T entity,string PropertyName="", bool save = true) where T : Base
        {
            using var ctx = _factory.CreateDbContext();
            ctx.Set<T>().Attach(entity);
            //x.GetType().GetProperty("RequisitionId").GetValue(x) == RrId
            ctx.Entry(entity).Property(x => x.GetType().GetProperty(PropertyName).GetValue(x)).IsModified = true; 
            if (save)
            {
                Save();
            }
            return entity.Id;
        }
        public virtual int Remove<T>(T entity, bool save = true) where T : Base
        {
            using var ctx = _factory.CreateDbContext();

            ctx.Set<T>().Remove(entity);

            if (save)
            {
                return ctx.SaveChanges();
            }

            return 0;
        }

        //public virtual bool RemoveById<T>(int id, bool save = true) where T : Base
        //{
        //    using var ctx = _factory.CreateDbContext();
        //    var entity = Get<T>(id);
        //    Remove<entity>(T entity, bool save = true)
        //    return Remove<entity>(entity, save);
        //}
        public virtual long Save()
        {
            using var ctx = _factory.CreateDbContext();
            ctx.ChangeTracker.AutoDetectChangesEnabled = false;
            return ctx.SaveChanges();   
        }
        public int AddIfNotExists<T>(T entity, Expression<Func<T, bool>> predicate = null) where T : Base
        {
                using var ctx = _factory.CreateDbContext();
                ctx.ChangeTracker.LazyLoadingEnabled = false;
                var _entity = ctx.Set<T>();
                var exists = predicate != null ? _entity.Any(predicate) : _entity.Any();
                //_entity.Add(entity);
                int ReturnId = exists ? 0 : 1;
                return ReturnId;           
        }

        public IQueryable<T> GetByCondition<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            using var ctx = _factory.CreateDbContext();
            return ctx.Set<T>().Where(predicate);
        }
    }  
}