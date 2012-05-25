using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace RavenDbTest
{
    [TestFixture]
    public class SimpleOps
    {
        private EmbeddableDocumentStore documentStore;

        [SetUp]
        public void Setup()
        {
            documentStore = new EmbeddableDocumentStore {RunInMemory = true};
            documentStore.Initialize();
            using (var session = documentStore.OpenSession())
            {
                session.Store(new Dummy { Id  = "Cedric", CreationTime = new DateTime(1981, 8, 17), NumberOfCrash =  42 });
                session.Store(new Dummy { Id =  "Arthur", CreationTime = new DateTime(1979, 2, 5) , NumberOfCrash = 112 });
                session.Store(new Dummy { Id =     "Zoe", CreationTime = new DateTime(2005, 7, 11), NumberOfCrash =   5 });
                session.Store(new Dummy { Id = "Gregory", CreationTime = new DateTime(2010, 6, 14), NumberOfCrash =  22 });
                session.Store(new Dummy { Id =    "Arya", CreationTime = new DateTime(2012, 5, 25), NumberOfCrash =   0 });
                session.SaveChanges();
            }
        }

        [TearDown]
        public void TearDown()
        {
            documentStore.Dispose();
            documentStore = null;
        }

        [Test]
        public void Can_Store_And_Retrieve_A_Simple_Instance()
        {
            const string id = "Hope";
            using (var session = documentStore.OpenSession())
            {
                
                var dummy = new Dummy
                                {CreationTime = DateTime.Now, Id = id, NumberOfCrash = 17};
                session.Store(dummy);
                session.SaveChanges();
            }

            using (var session = documentStore.OpenSession())
            {
                var dummy = session.Load<Dummy>(id);
                Assert.That(dummy,Is.Not.Null);
            }
        }

        [Test]
        public void Nothing_Is_Stored_If_SaveChanges_Is_Not_Called()
        {
            const string id = "NotStored";
            using (var session = documentStore.OpenSession())
            {

                var dummy = new Dummy { CreationTime = DateTime.Now, Id = id, NumberOfCrash = 17 };
                session.Store(dummy);
            }

            using (var session = documentStore.OpenSession())
            {
                var dummy = session.Load<Dummy>(id);
                Assert.That(dummy, Is.Null);
            }
        }

        [Test]
        public void You_can_change_an_existing_document()
        {
            var id = Guid.NewGuid();
            using (var session = documentStore.OpenSession())
            {

                var dummy = new Dummy { CreationTime = DateTime.Now, Id = "NotStored", NumberOfCrash = 17 };
                session.Store(dummy);
            }

            using (var session = documentStore.OpenSession())
            {
                var dummy = session.Load<Dummy>(id);
                Assert.That(dummy, Is.Null);
            }
        }

        [Test]
        public void Can_change_an_existing_document()
        {
            using (var session = documentStore.OpenSession())
            {
                var arthur = session.Load<Dummy>("Arthur");
                arthur.NumberOfCrash++;
                session.SaveChanges();
            }

            using (var session = documentStore.OpenSession())
            {
                var arthur = session.Load<Dummy>("Arthur");
                Assert.That(arthur.NumberOfCrash,Is.EqualTo(113));
            }
        }

        [Test]
        public void Can_grab_all_document_of_a_type()
        {
            using (var session = documentStore.OpenSession())
            {
                var allDummies = session.Query<Dummy>().ToList();
                Assert.That(allDummies.Count,Is.EqualTo(5));
            }
        }

        [Test]
        public void Can_filter_documents()
        {
            using (var session = documentStore.OpenSession())
            {
                var allDummies = session.Query<Dummy>().Where(d=>d.CreationTime < new DateTime(2005,1,1)).ToList();
                Assert.That(allDummies.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void Can_Order_documents()
        {
            using (var session = documentStore.OpenSession())
            {
                var allDummies = session.Query<Dummy>().OrderBy(d => d.CreationTime).ToList();
                var dates = allDummies.Select(d => d.CreationTime).ToList();
                Assert.That(dates, Is.Ordered);
            }
        }




    }
}
