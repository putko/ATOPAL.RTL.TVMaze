namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.ActionResult
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error)
            : base(value: error)
        {
            this.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}