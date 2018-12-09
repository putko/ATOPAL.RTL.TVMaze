namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.Filters
{
    using System.Net;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.ActionResult;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.Exception;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            this.logger.LogError(eventId: new EventId(id: context.Exception.HResult),
                exception: context.Exception,
                message: context.Exception.Message);

            if (context.Exception.GetType() == typeof(TVShowsDomainException))
            {
                var problemDetails = new ValidationProblemDetails
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                problemDetails.Errors.Add(key: "DomainValidations", value: new[] {context.Exception.Message});

                context.Result = new BadRequestObjectResult(error: problemDetails);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            else
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] {"An error ocurred."}
                };

                if (this.env.IsDevelopment())
                {
                    json.DeveloperMeesage = context.Exception;
                }

                context.Result = new InternalServerErrorObjectResult(error: json);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMeesage { get; set; }
        }
    }
}