using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVShows.API.Model
{
    public class ShowPerson
    {
        public int ShowId { get; set; }
        public Show Show { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
