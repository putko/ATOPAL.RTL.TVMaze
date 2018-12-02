using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.Exception
{
    public class TVShowsDomainException : System.Exception
    {
        public TVShowsDomainException()
        { }

        public TVShowsDomainException(string message)
            : base(message)
        { }

        public TVShowsDomainException(string message, System.Exception innerException)
            : base(message, innerException)
        { }
    }
}
