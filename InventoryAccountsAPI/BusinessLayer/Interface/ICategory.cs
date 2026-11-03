
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface ICategory: ICommonMethod<POStblCategory>
    {
        public Task<POStblCategory> GetById(int id);
    }
}
