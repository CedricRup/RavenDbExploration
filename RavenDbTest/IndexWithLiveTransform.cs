using System.Linq;
using FizzWare.NBuilder;
using NUnit.Framework;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace RavenDbTest
{
    [TestFixture]
    public class IndexWithLiveTransform : RavenDbTest
    {
        public class CrashWithDummyByDummyId : AbstractIndexCreationTask<Crash>
        {
            public CrashWithDummyByDummyId()
            {
                Map = crashes => from crash in crashes
                                 select new { crash.DummiesId };

                TransformResults = (database, crashes) =>
                                   from crash in crashes
                                   let dummies = database.Load<Dummy>(crash.DummiesId)
                                   select new {crash.ExperimentName, Dummies = dummies };

            }
        }

        [SetUpAttribute]
        public void SetupOfTest()
        {
            IndexCreation.CreateIndexes(GetType().Assembly, DocumentStore);
        }

        [Test]
        public void CanRetrieveDataFromIndexWithProjection()
        {

            Builder<Crash>.CreateListOfSize(1000)
                .TheFirst(200)
                .With(c => c.DummiesId = new[] {"Zoe", "Cedric"})
                .TheNext(800)
                .With(c => c.DummiesId = new[] {"Arthur", "Cedric"}).Persist();
            

            using (var session = DocumentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                var result = session.Query<Crash, CrashWithDummyByDummyId>()
                    .Statistics(out stats)
                    .Customize(c => c.WaitForNonStaleResults())
                    .Where(c=>c.DummiesId.Any(id=> id=="Zoe"))
                    .As<CrashWithDummies>().ToList();

                Assert.That(stats.TotalResults,Is.EqualTo(200));
                Assert.That(result,Has.Count.EqualTo(128));
                
            }
        }
    }
}
