using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RavenDbTest
{
    public class Crash
    {
        public string Id { get; set; }
        public string ExperimentName { get; set; }
        public string[] DummiesId { get; set; }
        public DateTime TimeOfExperiment;
    }
}
