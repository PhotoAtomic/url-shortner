using api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using api.Persistence;
using api.Configurations;
using Microsoft.Extensions.Options;
using SharpTestsEx;
using api.Domain;

namespace api.Test
{
    [TestClass]
    public class ServiceTests
    {
        [TestMethod]
        public async Task ExecutingFirstGenerationOfShortUrl_Expected_NewUniqueIdGenerated()
        {
            

            var config = new UrlShorteningServiceConfiguration() {
                ChunkSize = 3,
                SequenceName = "TestSequence",
                ServerBaseUrl = "http://cor.to"
            };

            var options = new OptionsWrapper<UrlShorteningServiceConfiguration>(config);


            var repositoryMock = new Mock<IRepository<Sequence>>();
            repositoryMock.Setup(x => x.GetById(config.SequenceName)).ReturnsAsync((Sequence?)null);
            repositoryMock.Setup(x => x.Add(It.IsAny<Sequence>())).Verifiable();

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(x => x.RepositoryOf<Sequence>()).ReturnsAsync(repositoryMock.Object);
            

            var factoryMock = new Mock<IUnitOfWorkFactory>();
            factoryMock.Setup(x => x.NewUnitOfWork()).ReturnsAsync(uowMock.Object);
            var db = factoryMock.Object;

            UrlShorteningService service = new(db, options);

            var next = await service.GetSequenceNextValue();

            next.Should().Be(1);

            repositoryMock.Verify();

            var next2 = await service.GetSequenceNextValue();
            next2.Should().Be(2);

        }


        [TestMethod]
        public async Task ExecutingSubsequentGenerationOfShortUrl_Expected_NewUniqueIdGenerated()
        {


            var config = new UrlShorteningServiceConfiguration()
            {
                ChunkSize = 3,
                SequenceName = "TestSequence",
                ServerBaseUrl = "http://cor.to"
            };

            var options = new OptionsWrapper<UrlShorteningServiceConfiguration>(config);

            var storedSequence = new Sequence()
            {
                Id = config.SequenceName,
                Limit = 3,
            };

            var repositoryMock = new Mock<IRepository<Sequence>>();
            repositoryMock.Setup(x => x.GetById(config.SequenceName)).ReturnsAsync(storedSequence);
            repositoryMock.Setup(x => x.Add(storedSequence)).Verifiable();

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(x => x.RepositoryOf<Sequence>()).ReturnsAsync(repositoryMock.Object);


            var factoryMock = new Mock<IUnitOfWorkFactory>();
            factoryMock.Setup(x => x.NewUnitOfWork()).ReturnsAsync(uowMock.Object);
            var db = factoryMock.Object;

            UrlShorteningService service = new(db, options);

            var next = await service.GetSequenceNextValue();

            next.Should().Be(4);

            repositoryMock.Verify();

        }


        [TestMethod]
        public async Task ConsumingWholeChunk_Expected_NewChunkIsTaken()
        {


            var config = new UrlShorteningServiceConfiguration()
            {
                ChunkSize = 3,
                SequenceName = "TestSequence",
                ServerBaseUrl = "http://cor.to"
            };

            var options = new OptionsWrapper<UrlShorteningServiceConfiguration>(config);

            var storedSequence = new Sequence()
            {
                Id = config.SequenceName,
                Limit = 3,
            };

            var repositoryMock = new Mock<IRepository<Sequence>>();
            repositoryMock.Setup(x => x.GetById(config.SequenceName)).ReturnsAsync(storedSequence);
            repositoryMock.Setup(x => x.Add(storedSequence)).Callback<Sequence>(s=> storedSequence = s);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(x => x.RepositoryOf<Sequence>()).ReturnsAsync(repositoryMock.Object);


            var factoryMock = new Mock<IUnitOfWorkFactory>();
            factoryMock.Setup(x => x.NewUnitOfWork()).ReturnsAsync(uowMock.Object);
            var db = factoryMock.Object;

            UrlShorteningService service = new(db, options);

            var next = await service.GetSequenceNextValue();
            next.Should().Be(4);
            next = await service.GetSequenceNextValue();
            next.Should().Be(5);
            next = await service.GetSequenceNextValue();
            next.Should().Be(6);
            next = await service.GetSequenceNextValue();
            next.Should().Be(7);

            storedSequence.Limit.Should().Be(9);


        }

    }
}
