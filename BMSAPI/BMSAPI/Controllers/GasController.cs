using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GasController : ControllerBase
    {
        private readonly IGas _Service;
        private readonly ILogger<GasController> _logger;

        public GasController(IGas gas, ILogger<GasController> logger)
        {
            _Service = gas;
            _logger = logger;
        }

        [Route("GetGasesWithSupplies")]
        [HttpGet]
        public List<RptGas> Get(DateTime fromDt, DateTime toDt)
        {
            return _Service.GetGasWithSupplier(fromDt, toDt);
        }

        [Route("GetSupplier")]
        [HttpGet]
        public List<Supplier> GetSupplier()
        {
            return _Service.GetSuppliers();
        }

        [Route("Add")]
        [HttpPost]
        public int Add([FromBody] Gas entity)
        {
            return _Service.Add(entity);
        }

        [Route("GetGasById")]
        [HttpGet]
        public List<Gas> GetGasById(int id)
        {
            return _Service.GetGasById(id);
        }

        [Route("UpdateGas")]
        [HttpPut]
        public int Update([FromBody] Gas entity)
        {
            return _Service.Update(entity);
        }

        [Route("GetGasUsage")]
        [HttpGet]
        public List<RptGasUsage> GetGasUsage([FromQuery] string FCode)
        {
            return _Service.GetGasUsage(FCode);
        }

        [Route("AddUsedGas")]
        [HttpPost]
        public int AddUsedGas([FromBody] GasUsed entity)
        {
            try
            {
                return _Service.AddUsedGas(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddUsedGas failed");
                return 0;
            }
        }

        [Route("UpdateUsedGas")]
        [HttpPut]
        public int UpdateUsedGas([FromBody] GasUsed entity)
        {
            try
            {
                return _Service.UpdateUsedGas(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUsedGas failed");
                return 0;
            }
        }

        [Route("GetGasStock")]
        [HttpGet]
        public List<RptGasStock> GetGasStock([FromQuery] DateTime fromDate, DateTime toDate)
        {
            return _Service.GetGasStock(fromDate, toDate);
        }

        [Route("AddGasStock")]
        [HttpPost]
        public int AddGasStock([FromBody] GasStock entity)
        {
            try
            {
                return _Service.AddGasStock(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddGasStock failed");
                return 0;
            }
        }

        [Route("UpdateGasStock")]
        [HttpPut]
        public int UpdateGasStock([FromBody] GasStock entity)
        {
            try
            {
                return _Service.UpdateGasStock(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateGasStock failed");
                return 0;
            }
        }

        [Route("GetGasOpeningStock")]
        [HttpGet]
        public GasStockCal GetGasOpeningStock(int buildingId)
        {
            return _Service.GetGasOpeningStock(buildingId);
        }

        [Route("GetAllFlatInfo")]
        [HttpGet]
        public List<RptGasUsage> GetAllFlatInfo()
        {
            return _Service.GetAllFlatInfo();
        }

        [Route("AddGasUsed")]
        [HttpPost]
        public int AddGasUsed([FromBody] List<GasUsed> entities)
        {
            try
            {
                return _Service.AddGasUsed(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddGasUsed failed");
                return 0;
            }
        }

        [Route("GetGasUsageList")]
        [HttpGet]
        public List<RptGasUsage> GetGasUsageList()
        {
            return _Service.GetGasUsageList();
        }

        [Route("AddClosingDate")]
        [HttpPost]
        public int AddClosingDate([FromBody] ClosingDeclaration entity)
        {
            try
            {
                return _Service.AddClosingDate(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddClosingDate failed");
                return 0;
            }
        }

        [Route("GetClosingDate")]
        [HttpGet]
        public List<ClosingDeclaration> GetClosingDate()
        {
            return _Service.GetClosingDate();
        }

        [Route("UpdateClosingDate")]
        [HttpPut]
        public int UpdateClosingDate([FromBody] ClosingDeclaration entity)
        {
            try
            {
                return _Service.UpdateClosingDate(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateClosingDate failed");
                return 0;
            }
        }

        [Route("Delete")]
        [HttpPost]
        public int Remove([FromBody] Gas entity)
        {
            try
            {
                return _Service.Remove(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Remove gas failed");
                return 0;
            }
        }
    }
}