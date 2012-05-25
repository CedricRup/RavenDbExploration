using System;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace RavenDbTest
{
    [TestFixture]
    public class DocumentStoreTests
    {
        [Test]
        public void DocumentStore_Throws_Exception_When_Not_Initialized()
        {
            var documentStore = new EmbeddableDocumentStore();
            Assert.Throws<InvalidOperationException>(
                () => documentStore.OpenSession());
        }
    }
}
