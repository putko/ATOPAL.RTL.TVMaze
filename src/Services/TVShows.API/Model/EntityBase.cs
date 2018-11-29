using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVShows.API.Model
{
    public abstract class EntityBase
    {
        public int Id { get; set; }

        public EntityBase(int id)
        {
            this.Id = id;
        }
        public EntityBase()
        {
        }
    }
}
