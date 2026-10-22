using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace BMSAPI.BusinessLayer.Manager
{
    public class ProcedureManager : IProcedure
    {
        // শুধু "ProcName" বা "schema.ProcName" — অন্য database বা server-এর নাম দেওয়া যাবে না
        private static readonly Regex ProcNameRule =
            new(@"^[A-Za-z_][A-Za-z0-9_]{0,127}(\.[A-Za-z_][A-Za-z0-9_]{0,127})?$");

        private readonly IDapperService _dapper;
        private readonly HashSet<string> _allowedProcedures;

        public ProcedureManager(IDapperService dapper, IConfiguration configuration)
        {
            _dapper = dapper;

            // appsettings.json-এ "AllowedProcedures" দিলে শুধু সেগুলোই চলবে
            var allowed = configuration.GetSection("AllowedProcedures").Get<string[]>() ?? Array.Empty<string>();
            _allowedProcedures = new HashSet<string>(allowed, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<IEnumerable<dynamic>> CallProcedureAsync1(ProcedureExecuteRequest request)
        {
            var name = ValidateProcedure(request);
            return await _dapper.CallProcedureAsync(name, BuildParameters(request.Parameters));
        }

        public async Task<int> ExecuteProcedureNonQueryAsync(ProcedureExecuteRequest request)
        {
            var name = ValidateProcedure(request);
            return await _dapper.ExecuteProcedureNonQueryAsync(name, BuildParameters(request.Parameters));
        }

        // ---------------- helpers ----------------

        private string ValidateProcedure(ProcedureExecuteRequest request)
        {
            var name = request?.ProcedureName?.Trim();

            if (string.IsNullOrEmpty(name) || !ProcNameRule.IsMatch(name))
                throw new ArgumentException("Invalid procedure name.");

            if (_allowedProcedures.Count > 0 && !_allowedProcedures.Contains(name))
                throw new UnauthorizedAccessException($"Procedure '{name}' is not allowed.");

            // শুধু এই tenant database-এ আপনার নিজের বানানো procedure চলবে।
            // SQL Server-এর system procedure (sp_configure, xp_cmdshell ইত্যাদি) এখানে পাওয়া যায় না, তাই আটকে যাবে।
            var exists = _dapper.GetSingle<int>(
                "SELECT COUNT(1) FROM sys.procedures WHERE object_id = OBJECT_ID(@name) AND is_ms_shipped = 0",
                new { name });

            if (exists == 0)
                throw new ArgumentException($"Procedure '{name}' was not found.");

            return name;
        }

        private static DynamicParameters BuildParameters(Dictionary<string, object>? source)
        {
            var parameters = new DynamicParameters();
            if (source == null) return parameters;

            foreach (var (key, value) in source)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                // frontend "@Id" বা "Id" যেভাবেই পাঠাক, "@Id" হবে
                parameters.Add("@" + key.TrimStart('@'), ToDbValue(value));
            }

            return parameters;
        }

        // Newtonsoft nested object/array-কে JToken হিসেবে দেয়, যা Dapper চেনে না
        private static object? ToDbValue(object? value) => value switch
        {
            null => null,
            JValue jv => jv.Value,
            JToken jt => jt.ToString(Newtonsoft.Json.Formatting.None),
            _ => value
        };
    }
}
