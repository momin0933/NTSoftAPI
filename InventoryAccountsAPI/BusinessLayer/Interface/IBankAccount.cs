
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IBankAccount: ICommonMethod<POStblAccount>
    {
        public Task<POStblAccount> GetById(int id);
        Task<bool> ToggleStatus(int id, bool isActive);
    }
}
