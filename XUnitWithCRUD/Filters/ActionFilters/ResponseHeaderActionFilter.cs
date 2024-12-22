using Microsoft.AspNetCore.Mvc.Filters;

namespace XUnitWithCRUD.Filters.ActionFilters;

public class ResponseHeaderActionFilter : IAsyncActionFilter
{
private readonly ILogger<ResponseHeaderActionFilter> _logger;
private readonly string _key;
private readonly string _value;


public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value)
{
_logger = logger;
_key = key;
_value = value;
}

public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
{
_logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
        context.HttpContext.Response.Headers[_key] = _value;


await next(); 

_logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

}
}
