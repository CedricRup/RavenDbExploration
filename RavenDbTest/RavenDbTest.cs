using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Extensions;

namespace RavenDbTest
{
    public class RavenDbTest
    {
        protected IDocumentStore DocumentStore;

        [SetUp]
        public void Setup()
        {
            DocumentStore = new EmbeddableDocumentStore {RunInMemory = true};
            //DocumentStore = new DocumentStore(){Url = "http://localhost:8080/",DefaultDatabase = "Test"};
            DocumentStore.Initialize();
            DocumentStore.DatabaseCommands.EnsureDatabaseExists("Test");
            
            BuilderSetup.SetCreatePersistenceMethod<IList<Dummy>>(Persist);
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(new Dummy { Id  = "Cedric", CreationTime = new DateTime(1981, 8, 17), NumberOfCrash =  42 });
                session.Store(new Dummy { Id =  "Arthur", CreationTime = new DateTime(1979, 2, 5) , NumberOfCrash = 112 });
                session.Store(new Dummy { Id =     "Zoe", CreationTime = new DateTime(2005, 7, 11), NumberOfCrash =   5 });
                session.Store(new Dummy { Id = "Gregory", CreationTime = new DateTime(2010, 6, 14), NumberOfCrash =  22 });
                session.Store(new Dummy { Id =    "Arya", CreationTime = new DateTime(2012, 5, 25), NumberOfCrash =   0 });
                session.SaveChanges();
            }
        }

        private void Persist<T>(IList<T> toStore)
        {
            using (var session = DocumentStore.OpenSession())
            {
                foreach (var entity in toStore)
                {
                    session.Store(entity);    
                }
                session.SaveChanges();
            }
        }

        [TearDown]
        public void TearDown()
        {
            DocumentStore.Dispose();
            DocumentStore = null;
        }
    }
}