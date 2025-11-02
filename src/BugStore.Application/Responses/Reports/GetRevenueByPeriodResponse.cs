using BugStore.Application.UseCases.Reports.RevenueByPeriod;

namespace BugStore.Application.Responses.Reports;

public class GetRevenueByPeriodResponse
{
    public List<RevenueByPeriodResponse> Items { get; set; } = [];
}
