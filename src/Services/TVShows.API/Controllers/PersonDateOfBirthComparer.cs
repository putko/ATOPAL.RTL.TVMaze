namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;

    public class PersonDateOfBirthComparer : Comparer<Person>
    {
        public new static PersonDateOfBirthComparer Default { get; } = new PersonDateOfBirthComparer();

        public override int Compare(Person x, Person y)
        {
            // handle nulls

            return Comparer<DateTime>.Default.Compare(x: x.BirthDate.GetValueOrDefault(),
                y: y.BirthDate.GetValueOrDefault());
        }
    }
}