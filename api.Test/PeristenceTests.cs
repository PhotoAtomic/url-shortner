using api.Configurations;
using api.Persistence;
using api.Persistence.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using SharpTestsEx;

namespace api.Test
{

    public class ExampleDocument : IIdentifiable
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }


    [TestClass]
    public class PeristenceTests
    {

        [TestInitialize]
        public void Initialize()
        {
            PersistenceConfiguration config = new()
            {
                DBName = "Test",
                EndPoint = "https://localhost:8081",
                Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                Througput = 400,
            };
            var options = new OptionsWrapper<PersistenceConfiguration>(config);
            factory = new(options);
        }

        CosmosUnitOfWorkFactory? factory = null;

        [TestMethod]
        public async Task AddingAnItem_Expect_DoesNotThrow()
        {

            using (var uow = await factory!.NewUnitOfWork())
            {
                var repo = await uow.RepositoryOf<ExampleDocument>();


                ExampleDocument doc = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Value= "An example value",
                };
                
                await repo.Add(doc);
            }
        }

        [TestMethod]
        public async Task RetrievedANonExistingItem_Expected_DefaultReturned()
        {

            using (var uow = await factory!.NewUnitOfWork())
            {
                var repo = await uow.RepositoryOf<ExampleDocument>();

                var newId = Guid.NewGuid().ToString();
                var doc = await repo.GetById(newId);
                doc.Should().Be.Null();
            }
        }

        [TestMethod]
        public async Task RetrievedAnExistingItem_Expected_ItemRetrieved()
        {

            using (var uow = await factory!.NewUnitOfWork())
            {
                var repo = await uow.RepositoryOf<ExampleDocument>();

                var newId = Guid.NewGuid().ToString();
                
                ExampleDocument doc = new()
                {
                    Id = newId,
                    Value = "An example value",
                };

                await repo.Add(doc);
                var docReloaded = await repo.GetById(newId);

                docReloaded.Should().Not.Be.Null();
                docReloaded!.Value.Should().Be("An example value");
            }
        }



        [TestMethod]
        public async Task DeletingAnExistingItem_Expected_ItemDeleted()
        {

            using (var uow = await factory!.NewUnitOfWork())
            {
                var repo = await uow.RepositoryOf<ExampleDocument>();

                var newId = Guid.NewGuid().ToString();

                ExampleDocument doc = new()
                {
                    Id = newId,
                    Value = "An example value",
                };

                await repo.Add(doc);

                await repo.RemoveById(newId);

                var docReloaded = await repo.GetById(newId);

                docReloaded.Should().Be.Null();
                
            }
        }



        [TestMethod]
        public async Task ConcurrentUpdateToAnItem_Expected_SecondFailsWithConcurrentException()
        {

            var uow1 = await factory!.NewUnitOfWork();
            var uow2 = await factory!.NewUnitOfWork();


            var newId = Guid.NewGuid().ToString();

            ///////first uow
            var repo1 = await uow1.RepositoryOf<ExampleDocument>();

            

            ExampleDocument doc = new()
            {
                Id = newId,
                Value = "An example value",
            };

            await repo1.Add(doc);
            ///////second uow
            var repo2 = await uow2.RepositoryOf<ExampleDocument>();


            var reloadedDoc = await repo2.GetById(newId);
            reloadedDoc!.Value = "Modified value";

            await repo2.Add(reloadedDoc);


            //////first uow

            doc!.Value = "Second modification";
            try
            {
                await repo1.Add(doc);
                throw new Exception("If code executed untill here, then the expected exception has not be throw");
            }
            catch (ConcurrencyException) { }  // expected exception thrown safely


        }

        [TestMethod]
        public async Task CreatingTwoItemsWithTheSameId_Expected_SecondFailsWithException()
        {

            using (var uow = await factory!.NewUnitOfWork())
            {
                var repo = await uow.RepositoryOf<ExampleDocument>();

                var newId = Guid.NewGuid().ToString();

                ExampleDocument doc1 = new()
                {
                    Id = newId,
                    Value = "An example value",
                };

                await repo.Add(doc1,mustNotExists:true);

                ExampleDocument doc2 = new()
                {
                    Id = newId,
                    Value = "Another value",
                };

                try
                {
                    await repo.Add(doc1, mustNotExists: true);
                    throw new Exception("If code executed untill here, then the expected exception has not be throw");
                }
                catch (ConcurrencyException ex) { } // catch expected exception, 

            }
        }
    }
}