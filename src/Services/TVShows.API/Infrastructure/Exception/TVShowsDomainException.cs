namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.Exception
{
    using System;

    public class TVShowsDomainException : Exception
    {
        public TVShowsDomainException()
        {
        }

        public TVShowsDomainException(string message)
            : base(message: message)
        {
        }

        public TVShowsDomainException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }
    }
}