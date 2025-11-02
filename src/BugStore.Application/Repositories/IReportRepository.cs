using BugStore.Application.UseCases.Reports.BestCustomers;
using BugStore.Application.UseCases.Reports.RevenueByPeriod;

namespace BugStore.Application.Repositories;

public interface IReportRepository
{
    Task<IReadOnlyList<BestCustomersResponse>> GetBestCustomersAsync(BestCustomersRequest request);
    Task<IReadOnlyList<RevenueByPeriodResponse>> GetRevenueByPeriodAsync(RevenueByPeriodRequest request);
}
