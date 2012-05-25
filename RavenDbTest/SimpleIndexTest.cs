using System.Linq;
using FizzWare.NBuilder;
using NUnit.Framework;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace RavenDbTest
{
    [TestFixture]
    public class SimpleIndexTest : RavenDbTest
    {
        public class DummyIndexByCrashNumber : AbstractIndexCreationTask<Dummy>
        {
            public DummyIndexByCrashNumber()
            {
                Map = dummies => from dummy in dummies
                                 select new {dummy.NumberOfCrash};

            }
        }

        [SetUp]
        public void SetupOfTest()
        {
            IndexCreation.CreateIndexes(GetType().Assembly,DocumentStore);
        }

        [Test]
        public void CanRetrieveDataFromIndex()
        {
            Builder<Dummy>.CreateListOfSize(20)
                .All()
                .With(d=>d.NumberOfCrash = 5)
                .Random(5)
                .With(d => d.NumberOfCrash = 17)
                .Persist();

            using (var session = DocumentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                var result = session.Query<Dummy, DummyIndexByCrashNumber>()
                    .Statistics(out stats)
                    .Customize(c=>c.WaitForNonStaleResults())
                    .Where( d => d.NumberOfCrash == 17)
                    .ToList();
                Assert.That(result.Count,Is.EqualTo(5));
            }
        }
    }


}
