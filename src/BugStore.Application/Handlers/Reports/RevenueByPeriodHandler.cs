using BugStore.Application.Interfaces;
using BugStore.Application.Repositories;
using BugStore.Application.Responses.Reports;
using BugStore.Application.UseCases.Reports.RevenueByPeriod;

namespace BugStore.Application.Handlers.Reports;

public class RevenueByPeriodHandler(IReportRepository reports) : IHandler<RevenueByPeriodRequest, GetRevenueByPeriodResponse>
{
    private readonly IReportRepository _reports = reports;

    public async Task<GetRevenueByPeriodResponse> HandleAsync(RevenueByPeriodRequest request)
    {
        var items = await _reports.GetRevenueByPeriodAsync(request);
        return new GetRevenueByPeriodResponse
        {
            Items = items.ToList()
        };
    }
}
