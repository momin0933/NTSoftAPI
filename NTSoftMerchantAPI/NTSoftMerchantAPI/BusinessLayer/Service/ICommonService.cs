using NTSoftMerchantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;



namespace NTSoftMerchantAPI.BusinessLayer.Service
{
    public interface ICommonService
    {
        Task<int> Add<T>(T entity, bool save = true) where T : Base;
        Task<int> AddRange<T>(List<T> entity, bool save = true) where T : Base;
        int Update<T>(T entity, bool save = true) where T : Base;
        int UpdateByProperty<T>(T entity, string PropertyName = "", bool save = true) where T : Base;
        //bool Remove<T>(T entity, bool save = true) where T : Base;
        int Remove<T>(int id, bool save = true) where T : Base, new();
         //bool RemoveById<T>(int id, bool save = true) where T : Base;
        T Get<T>(int id) where T : Base;
        T GetWithNoTracking<T>(int id) where T : Base;

        IEnumerable<T> GetAll<T>() where T : Base;

        //IEnumerable<T> GetAllAccounts<T>() where T : class;
        IQueryable<T> Query<T>() where T : Base;
        int AddIfNotExists<T>(T entity, Expression<Func<T, bool>> predicate = null) where T : Base;
        long Save();
        IQueryable<T> GetByCondition<T>(Expression<Func<T, bool>> predicate) where T : class;

    }
}
