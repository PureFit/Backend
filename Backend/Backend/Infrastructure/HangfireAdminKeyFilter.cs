using Hangfire.Dashboard;

namespace Backend.Infrastructure;

public class HangfireAdminKeyFilter : IDashboardAuthorizationFilter
{
    private readonly string _adminKey;

    public HangfireAdminKeyFilter(string adminKey)
    {
        _adminKey = adminKey;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var key = httpContext.Request.Query["key"].ToString();
        return key == _adminKey;
    }
}
