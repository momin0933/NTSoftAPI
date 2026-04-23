using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using NTSoftCentralAPI.BusinessLayer.TenantService;



namespace NTSoftCentralAPI.BusinessLayer.Service
{
    public class DapperService : IDapperService
    {
        //private readonly string connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ITenantProvider _tenantProvider;
        // SQLConnectionString _connectionStringService = new SQLConnectionString();

        public DapperService(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }
        //public DapperService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        //{
        //    //_httpContextAccessor = httpContextAccessor;
        //    //_configuration = configuration;
        //    //var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;

        //    //if (tenant != null && !string.IsNullOrEmpty(tenant.ConnectionString))
        //    //{
        //    //    connectionString = tenant.ConnectionString;
        //    //}
        //    //else
        //    //{
        //    //    connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    //}
        //}       
            

        public virtual IEnumerable<T> GetAllByQuery<T>(string query) where T : class
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {
                SqlConnection1.Open();
                var q = SqlConnection1.Query<T>(query).ToList();
                dynamic collectionWrapper = new
                {
                    OEBuyerFactName = q
                };
                //serializer.MaxJsonLength = Int32.MaxValue;
                string output = JsonConvert.SerializeObject(collectionWrapper, Formatting.Indented);
                //serializer.Serialize(collectionWrapper);
                return q;
            }
        }
        public virtual string GetStringByQuery(string query)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {
                SqlConnection1.Open();
                var q = SqlConnection1.QueryFirstOrDefault<string>(query);
                //dynamic collectionWrapper = new
                //{
                //    OEBuyerFactName = q
                //};
                ////serializer.MaxJsonLength = Int32.MaxValue;
                //string output = JsonConvert.SerializeObject(collectionWrapper, Formatting.Indented);
                ////serializer.Serialize(collectionWrapper);

                return q;
            }
        }

        public IEnumerable<T> GetAllBySP<T>(string procedure, DynamicParameters p) where T : class
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {

                SqlConnection1.Open();
                //JsonSerializer jss = new JsonSerializer();
                //jss.MaxJsonLength = Int32.MaxValue;
                var q = SqlConnection1.Query<T>(procedure, p, commandType: CommandType.StoredProcedure).ToList();
                dynamic collectionWrapper = new
                {
                    OEBuyerFactName = q
                };
                string output = JsonConvert.SerializeObject(collectionWrapper, Formatting.Indented);
                return q;
            }
        }
        public T GetByDynamicSPSingle<T>(string procedure, DynamicParameters p) where T : class
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {

                SqlConnection1.Open();
                var q = SqlConnection1.Query<T>(procedure, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                dynamic collectionWrapper = new
                {
                    OEBuyerFactName = q
                };
                //string output = JsonSerializer.Serialize(collectionWrapper);
                string output = JsonConvert.SerializeObject(collectionWrapper);
                return q;
            }
        }
        public int Post(string query)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {
                SqlConnection1.Open();
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                var q = SqlConnection1.Execute(query);
                dynamic collectionWrapper = new
                {
                    OEBuyerFactName = q
                };
                //serializer.MaxJsonLength = Int32.MaxValue;               
                string output = JsonConvert.SerializeObject(collectionWrapper);
                return q;
            }
        }
        public int PostBySP(string procedure, DynamicParameters p)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {
                SqlConnection1.Open();
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                var q = SqlConnection1.Execute(procedure, p, commandType: CommandType.StoredProcedure);
                //dynamic collectionWrapper = new
                //{
                //    OEBuyerFactName = q
                //};
                //serializer.MaxJsonLength = Int32.MaxValue;               
                //string output = JsonConvert.SerializeObject(collectionWrapper);
                return q;
            }
        }
        public bool PostBulkInsert<T>(IEnumerable<T> items, string tableName)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["YourConnectionString"].ConnectionString;
            var connectionString = _tenantProvider.GetConnectionString();
            bool IsDbSave = false;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        var table = new DataTable();

                        var properties = typeof(T).GetProperties().Where(p => p.CanRead).ToArray();

                        foreach (var prop in properties)
                        {
                            bulkCopy.ColumnMappings.Add(prop.Name, prop.Name);
                            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        }

                        foreach (var item in items)
                        {
                            var row = table.NewRow();
                            foreach (var prop in properties)
                            {
                                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                            }
                            table.Rows.Add(row);
                        }

                        bulkCopy.WriteToServer(table);
                        IsDbSave = true;
                    }
                }
            }
            catch (Exception ex)
            {
                IsDbSave = false;
            }
            return IsDbSave;
        }
        public void GetByMultipleQueryResult(string query, out string ReqQty, out string ActQty)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {
                SqlConnection1.Open();

                var q = SqlConnection1.QueryMultiple(query);
                ReqQty = q.Read<string>().Single();
                ActQty = q.Read<string>().Single();
                //dynamic collectionWrapper = new
                //{
                //    OEBuyerFactName = q
                //};
                //serializer.MaxJsonLength = Int32.MaxValue;
                //string output = serializer.Serialize(collectionWrapper);

                // return q;
            }
        }
        public int UpdateByQuery(string query)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {
                SqlConnection1.Open();
                var q = SqlConnection1.Query(query);
                return 1;
            }
        }

        public int UpdateByquery(string query1)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (SqlConnection SqlConnection1 = new SqlConnection(connectionString))
            {
                SqlConnection1.Open();
                var q = SqlConnection1.Query(query1);
                return 2;
            }
        }

        public async Task ExecuteAsync(string sql, object parameters = null)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using (var connection = new SqlConnection(connectionString)) // Use your actual connection string
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(sql, parameters);
            }
        }

        // use dapper advance service

        public async Task<IEnumerable<dynamic>> CallProcedureAsync(string procedureName, DynamicParameters parameters)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync(
                sql: procedureName,
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<int> ExecuteProcedureNonQueryAsync(string procedureName, DynamicParameters parameters)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var affectedRows = await connection.ExecuteAsync(
                sql: procedureName,
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return affectedRows;
        }
        public T GetSingle<T>(string query, object param)
        {
            var connectionString = _tenantProvider.GetConnectionString();
            using var conn = new SqlConnection(connectionString);
            return conn.QueryFirstOrDefault<T>(query, param);
        }
    }
}