using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;



        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responsCasheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>(); 
            //ask CLR for Creating Object from ResponseCacheService Explicitly

            var casheKey = GenerateCasheKeyFromRequest(context.HttpContext.Request);

            var response = await responsCasheService.GetCachedResponseAsync(casheKey);

            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = result;
                return;
            }

            var excutedActionContext = await next.Invoke(); // will excute the next action filter or the action itself

            if (excutedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responsCasheService.CacheResponseAsync(casheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCasheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path);

            //PageIndex=1
            //PageSize=5
            //Sort=name


            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
