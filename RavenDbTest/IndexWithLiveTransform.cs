using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace RavenDbTest
{
    [TestFixture]
    public class IndexWithLiveTransform : RavenDbTest
    {
        public class CrashByWithDummyByExperimentName : AbstractIndexCreationTask<Crash>
        {
            public CrashByWithDummyByExperimentName()
            {
                Map = crashes => from crash in crashes
                                 select new { crash.ExperimentName };

                TransformResults = (database, crashes) =>
                                   from crash in crashes
                                   let dummies = database.Load<Dummy>(crash.DummiesId)
                                   select new { crash.ExperimentName, Dummies = dummies };

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
            

            var crash = new Crash
                            {
                                DummiesId = new[] { "Cedric", "Zoe" },
                                ExperimentName = "Trust",
                                Id = "1",
                                TimeOfExperiment = DateTime.Now

                            };


            using (var session = DocumentStore.OpenSession())
            {
                session.Store(crash);
                session.SaveChanges();

                RavenQueryStatistics stats;
                var result = session.Query<Crash, CrashByWithDummyByExperimentName>()
                    .Statistics(out stats)
                    .Customize(c => c.WaitForNonStaleResults())
                    .As<CrashWithDummies>()
                    .ToList();
                
                Assert.That(result.First().Dummies.Select(d=>d.NumberOfCrash),Is.EquivalentTo(new[]{42,5}));
            }
        }
    }
}
