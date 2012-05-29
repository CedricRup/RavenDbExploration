using System.Collections.Generic;

namespace RavenDbTest
{
    public class CrashWithDummies
    {
        public string ExperimentName { get; set; }
        public IList<Dummy> Dummies { get; set; } 
    }
}