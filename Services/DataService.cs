using Newtonsoft.Json;
using RedisSqlDemo.Models;
using StackExchange.Redis;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisSqlDemo.Services
{
    public class DataService
    {
        private readonly string _sqlConnectionString;
        private readonly ConnectionMultiplexer _redis;

        public DataService(IConfiguration configuration)
        {
            _sqlConnectionString = configuration["SqlDatabase"] ?? throw new ArgumentNullException("SQL connection string is null.");
            _redis = ConnectionMultiplexer.Connect(configuration["RedisCache"] ?? throw new ArgumentNullException("Redis connection string is null."));
        }

        public async Task<List<SecurityIncidentLog>> GetDataFromSqlAsync()
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("exec [dbo].[sp_GetThreatIncidents] @UniqueParam", connection);
                command.Parameters.AddWithValue("@UniqueParam", (long)DateTime.Now.Ticks);

                var reader = await command.ExecuteReaderAsync();
                var results = new List<SecurityIncidentLog>();
                while (await reader.ReadAsync())
                {
                    results.Add(new SecurityIncidentLog
                    {
                        IncidentID = reader.GetInt32(reader.GetOrdinal("IncidentID")),
                        DateReported = reader.GetDateTime(reader.GetOrdinal("DateReported")),
                        ReportedBy = reader.IsDBNull(reader.GetOrdinal("ReportedBy")) ? null : reader.GetString(reader.GetOrdinal("ReportedBy")),
                        ThreatID = reader.IsDBNull(reader.GetOrdinal("ThreatID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ThreatID")),
                        Severity = reader.IsDBNull(reader.GetOrdinal("Severity")) ? null : reader.GetString(reader.GetOrdinal("Severity")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Threat = new ThreatIntelligence
                        {
                            ThreatName = reader.IsDBNull(reader.GetOrdinal("ThreatName")) ? null : reader.GetString(reader.GetOrdinal("ThreatName")),
                            ThreatType = reader.IsDBNull(reader.GetOrdinal("ThreatType")) ? null : reader.GetString(reader.GetOrdinal("ThreatType")),
                        }
                    });
                }
                return results;
            }
        }

        public async Task<List<SecurityIncidentLog>> GetDataFromRedisAsync()
        {
            var db = _redis.GetDatabase();
            var cachedData = await db.StringGetAsync("SecurityIncidentLogsWithThreatInfoCacheKey");
            if (!cachedData.IsNull)
            {
                var jsonData = cachedData.ToString();
                return JsonConvert.DeserializeObject<List<SecurityIncidentLog>>(jsonData) ?? new List<SecurityIncidentLog>();
            }

            var data = await GetDataFromSqlAsync();
            await db.StringSetAsync("SecurityIncidentLogsWithThreatInfoCacheKey", JsonConvert.SerializeObject(data));
            return data;
        }
    }
}