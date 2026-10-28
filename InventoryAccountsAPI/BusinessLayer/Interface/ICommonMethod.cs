
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSoftAPI.Service.BusinessLayer.Interface
{
    public interface ICommonMethod<T> where T : class
    {
        public List<T> GetAll();
        public int Add(T entity);
        public int Update(T entity);
        public int Remove(T entity);

        
    }
}
