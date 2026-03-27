using System.Data;
using HRSystem.Core.DTOs.Analytics;
using HRSystem.Core.Interfaces.Services;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace HRSystem.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly HRSystemDbContext _context;

    public AnalyticsService(HRSystemDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        // Execute stored procedure
        var result = new DashboardStatsDto(0,0,0,0,0,0,0,0,0);
        
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetDashboardStats";
            command.CommandType = CommandType.StoredProcedure;
            
            await _context.Database.OpenConnectionAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    result = new DashboardStatsDto(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        reader.GetInt32(5),
                        reader.GetInt32(6),
                        reader.GetDecimal(7),
                        reader.GetDecimal(8)
                    );
                }
            }
        }
        return result;
    }

    public async Task<List<ApplicationsPerMonthDto>> GetApplicationsPerMonthAsync(int months = 6)
    {
        var list = new List<ApplicationsPerMonthDto>();
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetApplicationsPerMonth";
            command.CommandType = CommandType.StoredProcedure;
            
            var param = command.CreateParameter();
            param.ParameterName = "@MonthsBack";
            param.Value = months;
            command.Parameters.Add(param);
            
            await _context.Database.OpenConnectionAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new ApplicationsPerMonthDto(
                        reader.GetString(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3)
                    ));
                }
            }
        }
        return list;
    }

    public async Task<List<PipelineFunnelDto>> GetPipelineFunnelAsync()
    {
        var list = new List<PipelineFunnelDto>();
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetPipelineFunnel";
            command.CommandType = CommandType.StoredProcedure;
            
            await _context.Database.OpenConnectionAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new PipelineFunnelDto(
                        reader.GetString(0),
                        reader.GetInt32(1),
                        reader.GetDecimal(2)
                    ));
                }
            }
        }
        return list;
    }

    public async Task<List<TopJobDto>> GetTopJobsAsync(int count = 5)
    {
        var list = new List<TopJobDto>();
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetTopJobsByApplications";
            command.CommandType = CommandType.StoredProcedure;
            
            var param = command.CreateParameter();
            param.ParameterName = "@TopN";
            param.Value = count;
            command.Parameters.Add(param);
            
            await _context.Database.OpenConnectionAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new TopJobDto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
                    ));
                }
            }
        }
        return list;
    }
}
