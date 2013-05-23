using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace RavenDbTest
{
    [TestFixture]
    public class SimpleOps : RavenDbTest
    {
        [Test]
        public void Can_Store_And_Retrieve_A_Simple_Instance()
        {
            const string id = "Hope";
            using (var session = DocumentStore.OpenSession())
            {
                
                var dummy = new Dummy
                                {CreationTime = DateTime.Now, Id = id, NumberOfCrash = 17};
                session.Store(dummy);
                session.SaveChanges();
            }

            using (var session = DocumentStore.OpenSession())
            {
                var dummy = session.Load<Dummy>(id);
                Assert.That(dummy,Is.Not.Null);
            }
        }

        [Test]
        public void Nothing_Is_Stored_If_SaveChanges_Is_Not_Called()
        {
            const string id = "NotStored";
            using (var session = DocumentStore.OpenSession())
            {

                var dummy = new Dummy { CreationTime = DateTime.Now, Id = id, NumberOfCrash = 17 };
                session.Store(dummy);
            }

            using (var session = DocumentStore.OpenSession())
            {
                var dummy = session.Load<Dummy>(id);
                Assert.That(dummy, Is.Null);
            }
        }

        [Test]
        public void You_can_change_an_existing_document()
        {
            var id = Guid.NewGuid();
            using (var session = DocumentStore.OpenSession())
            {

                var dummy = new Dummy { CreationTime = DateTime.Now, Id = "NotStored", NumberOfCrash = 17 };
                session.Store(dummy);
            }

            using (var session = DocumentStore.OpenSession())
            {
                var dummy = session.Load<Dummy>(id);
                Assert.That(dummy, Is.Null);
            }
        }

        [Test]
        public void Can_change_an_existing_document()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var arthur = session.Load<Dummy>("Arthur");
                arthur.NumberOfCrash++;
                session.SaveChanges();
            }

            using (var session = DocumentStore.OpenSession())
            {
                var arthur = session.Load<Dummy>("Arthur");
                Assert.That(arthur.NumberOfCrash,Is.EqualTo(113));
            }
        }

        [Test]
        public void Can_grab_all_document_of_a_type()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var allDummies = session.Query<Dummy>().ToList();
                Assert.That(allDummies.Count,Is.EqualTo(5));
            }
        }

        [Test]
        public void Can_filter_documents()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var allDummies = session.Query<Dummy>().Where(d=>d.CreationTime < new DateTime(2005,1,1)).ToList();
                Assert.That(allDummies.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void Can_Order_documents()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var allDummies = session.Query<Dummy>().Where(d=>d.NumberOfCrash == 3).OrderBy(d => d.CreationTime).ToList();
                var dates = allDummies.Select(d => d.CreationTime).ToList();
                Assert.That(dates, Is.Ordered);
            }
        }




    }
}
