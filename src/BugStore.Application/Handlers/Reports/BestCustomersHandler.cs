using BugStore.Application.Interfaces;
using BugStore.Application.Repositories;
using BugStore.Application.Responses.Reports;
using BugStore.Application.UseCases.Reports.BestCustomers;

namespace BugStore.Application.Handlers.Reports;

public class BestCustomersHandler(IReportRepository reports) : IHandler<BestCustomersRequest, GetBestCustomersResponse>
{
    private readonly IReportRepository _reports = reports;

    public async Task<GetBestCustomersResponse> HandleAsync(BestCustomersRequest request)
    {
        var items = await _reports.GetBestCustomersAsync(request);
        return new GetBestCustomersResponse
        {
            Customers = items.ToList()
        };
    }
}
