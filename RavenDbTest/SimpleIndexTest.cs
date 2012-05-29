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

        public class NumberOfDummyWithCrash : AbstractIndexCreationTask<Dummy, TotalCrash>
        {
            public NumberOfDummyWithCrash()
            {
                Map = dummies => from dummy in dummies
                                 select new TotalCrash() {NumberOfCrash = dummy.NumberOfCrash, NumberOfDummy = 1};

                Reduce = results => from result in results
                                    group result by result.NumberOfCrash
                                    into g
                                    select new
                                               {
                                                   NumberOfCrash = g.Key,
                                                   NumberOfDummy = g.Sum(x => x.NumberOfDummy)
                                               };


            }
        }

        [SetUp]
        public void SetupOfTest()
        {
            IndexCreation.CreateIndexes(GetType().Assembly, DocumentStore);
        }

        [Test]
        public void CanRetrieveDataFromIndex()
        {
            Builder<Dummy>.CreateListOfSize(20)
                .All()
                .With(d => d.NumberOfCrash = 5)
                .Random(5)
                .With(d => d.NumberOfCrash = 17)
                .Persist();

            using (var session = DocumentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                var result = session.Query<Dummy, DummyIndexByCrashNumber>()
                    .Statistics(out stats)
                    .Customize(c => c.WaitForNonStaleResults())
                    .Where(d => d.NumberOfCrash == 17)
                    .ToList();
                Assert.That(result.Count, Is.EqualTo(5));
            }
        }



        public class TotalCrash
        {
            public int NumberOfCrash { get; set; }
            public int NumberOfDummy { get; set; }
        }
    }
}
