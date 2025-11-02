using BugStore.Application.Interfaces;
using BugStore.Application.Responses.Reports;
using BugStore.Application.UseCases.Reports.BestCustomers;
using BugStore.Application.UseCases.Reports.RevenueByPeriod;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Endpoints;

public static class ReportsEndpoints
{
    public static void MapReportsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1/reports")
            .WithTags("Reports");

        group.MapGet("/best-customers", async ([AsParameters] BestCustomersRequest request, [FromServices] IHandler<BestCustomersRequest, GetBestCustomersResponse> handler) =>
        {
            var response = await handler.HandleAsync(request);
            return Results.Ok(response);
        });

        group.MapGet("/revenue-by-period", async ([AsParameters] RevenueByPeriodRequest request, [FromServices] IHandler<RevenueByPeriodRequest, GetRevenueByPeriodResponse> handler) =>
        {
            var response = await handler.HandleAsync(request);
            return Results.Ok(response);
        });
    }
}
