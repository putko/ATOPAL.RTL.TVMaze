namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model
{
    using System.Collections.Generic;

    public interface IConcurrencyRepository
    {
        bool IsConcurrencyValueValid(KeyValuePair<string, long> value);
    }
}