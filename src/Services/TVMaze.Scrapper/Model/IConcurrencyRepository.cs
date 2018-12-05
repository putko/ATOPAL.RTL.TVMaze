using System.Collections.Generic;
using System.Threading.Tasks;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model
{
    public interface IConcurrencyRepository
    {
        bool IsConcurrencyValueValid(KeyValuePair<string, long> value);
    }
}
