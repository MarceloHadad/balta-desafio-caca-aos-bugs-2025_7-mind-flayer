using BugStore.Application.UseCases.Reports.BestCustomers;

namespace BugStore.Application.Responses.Reports;

public class GetBestCustomersResponse
{
    public List<BestCustomersResponse> Customers { get; set; } = [];
}
