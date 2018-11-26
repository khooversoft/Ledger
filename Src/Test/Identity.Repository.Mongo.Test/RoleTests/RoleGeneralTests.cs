using FluentAssertions;
using Khooversoft.MongoDb;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Repository.Mongo.Test.RoleTests
{
    public class RoleGeneralTests
    {
        public RoleGeneralTests()
        {
            Utility.Initialize();
        }

        [Fact]
        public async Task CreateAndGetFailTest()
        {
            await Utility.ClearRoleCollection();

            UserRoleDoc testDoc = Utility.CreateUserRoleDoc(0);

            bool status = await Utility.RoleRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserRoleDoc> docs = await Utility.RoleRepository.Get(Utility.Context, "fake");
            docs.Should().BeNull();

            await DeleteAndVerify(testDoc.RoleId);
        }

        [Fact]
        public async Task CreateAndListTest()
        {
            await Utility.ClearRoleCollection();

            UserRoleDoc testDoc = Utility.CreateUserRoleDoc(0);

            bool status = await Utility.RoleRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            PageResult<HeaderDoc<UserRoleDoc>> docs = await Utility.RoleRepository.List(Utility.Context, new PageRequest(10));
            docs.Should().NotBeNull();
            docs.Items.Single().ETag.Should().NotBeNullOrEmpty();

            Utility.CompareDocument(testDoc, docs.Items.Single().Payload);

            await DeleteAndVerify(testDoc.RoleId);
        }

        [Fact]
        public async Task CreateGetDeleteTest()
        {
            await Utility.ClearRoleCollection();

            UserRoleDoc testDoc = Utility.CreateUserRoleDoc(0);

            bool status = await Utility.RoleRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserRoleDoc> docs = await Utility.RoleRepository.Get(Utility.Context, testDoc.RoleId);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);

            await DeleteAndVerify(testDoc.RoleId);
        }

        [Fact]
        public async Task CreateGetUpdateDeleteTest()
        {
            const string newIssuerText = "new issuer";

            await Utility.ClearRoleCollection();

            UserRoleDoc testDoc = Utility.CreateUserRoleDoc(0);

            bool status = await Utility.RoleRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserRoleDoc> docs = await Utility.RoleRepository.Get(Utility.Context, testDoc.RoleId);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);

            testDoc.Claims.First().Issuer = newIssuerText;
            docs.Payload.Claims.First().Issuer = newIssuerText;

            status = await Utility.RoleRepository.Set(Utility.Context, testDoc, docs.ETag);
            status.Should().BeTrue();

            docs = await Utility.RoleRepository.Get(Utility.Context, testDoc.RoleId);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);
            await DeleteAndVerify(testDoc.RoleId);
        }

        [Fact]
        public async Task CreateGetUpdateDeleteErrorTest()
        {
            const string newIssuerText = "new issuer";

            await Utility.ClearRoleCollection();

            UserRoleDoc testDoc = Utility.CreateUserRoleDoc(0);

            bool status = await Utility.RoleRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserRoleDoc> docs = await Utility.RoleRepository.Get(Utility.Context, testDoc.RoleId);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);

            testDoc.Claims.First().Issuer = newIssuerText;
            docs.Payload.Claims.First().Issuer = newIssuerText;

            Func<Task> func = async () =>
            {
                status = await Utility.RoleRepository.Set(Utility.Context, testDoc, "fake");
            };

            func.Should().Throw<ETagException>();

            await Utility.RoleRepository.Delete(Utility.Context, testDoc.RoleId);
        }

        [Fact]
        public async Task CreateManyGetUpdateDeleteTest()
        {
            const string newIssuerText = "new issuer";
            const int count = 11;
            int index = Utility.Random.Next(0, count-1);

            await Utility.ClearRoleCollection();

            List<UserRoleDoc> testDocuments = (await Enumerable.Range(0, count)
                .Select(x => Utility.CreateUserRoleDoc(x))
                .DoAsync(async x => await Utility.RoleRepository.Set(Utility.Context, x))
                ).ToList();

            UserRoleDoc testDoc = testDocuments[index];

            HeaderDoc<UserRoleDoc> docs = await Utility.RoleRepository.Get(Utility.Context, testDoc.RoleId);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();
            Utility.CompareDocument(testDoc, docs.Payload);

            testDoc.Claims.First().Issuer = newIssuerText;
            docs.Payload.Claims.First().Issuer = newIssuerText;

            bool status = await Utility.RoleRepository.Set(Utility.Context, testDoc, docs.ETag);
            status.Should().BeTrue();

            docs = await Utility.RoleRepository.Get(Utility.Context, testDoc.RoleId);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();
            Utility.CompareDocument(testDoc, docs.Payload);

            foreach(var item in testDocuments)
            {
                await Utility.RoleRepository.Delete(Utility.Context, item.RoleId);
            }

            PageResult<HeaderDoc<UserRoleDoc>> deleteState = await Utility.RoleRepository.List(Utility.Context, new PageRequest(10));
            deleteState.Should().NotBeNull();
            deleteState.Items.Count.Should().Be(0);
        }

        private async Task DeleteAndVerify(string roleId)
        {
            await Utility.RoleRepository.Delete(Utility.Context, roleId);

            PageResult<HeaderDoc<UserRoleDoc>> deleteState = await Utility.RoleRepository.List(Utility.Context, new PageRequest(10));
            deleteState.Should().NotBeNull();
            deleteState.Items.Count.Should().Be(0);
        }
    }
}
