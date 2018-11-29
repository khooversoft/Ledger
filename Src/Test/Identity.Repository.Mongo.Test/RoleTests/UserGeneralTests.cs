using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Repository.Mongo.Test.RoleTests
{
    public class UserGeneralTests
    {
        public UserGeneralTests()
        {
            Utility.Initialize();
        }

        [Fact]
        public async Task CreateAndGetFailTest()
        {
            await Utility.ClearUserCollection();

            UserDoc testDoc = Utility.CreateUserDoc(0);

            bool status = await Utility.UserRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserDoc> docs = await Utility.UserRepository.Get(Utility.Context, "fake");
            docs.Should().BeNull();

            await DeleteAndVerify(testDoc.UserName);
        }

        [Fact]
        public async Task CreateAndListTest()
        {
            await Utility.ClearUserCollection();

            UserDoc testDoc = Utility.CreateUserDoc(0);

            bool status = await Utility.UserRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            PageResult<HeaderDoc<UserDoc>> docs = await Utility.UserRepository.List(Utility.Context, new PageRequest(10));
            docs.Should().NotBeNull();
            docs.Items.Single().ETag.Should().NotBeNullOrEmpty();

            Utility.CompareDocument(testDoc, docs.Items.Single().Payload);

            await DeleteAndVerify(testDoc.UserName);
        }

        [Fact]
        public async Task CreateGetDeleteTest()
        {
            await Utility.ClearUserCollection();

            UserDoc testDoc = Utility.CreateUserDoc(0);

            bool status = await Utility.UserRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserDoc> docs = await Utility.UserRepository.Get(Utility.Context, testDoc.UserName);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);

            await DeleteAndVerify(testDoc.UserName);
        }

        [Fact]
        public async Task CreateGetUpdateDeleteTest()
        {
            const string newEmail = "new email";

            await Utility.ClearUserCollection();

            UserDoc testDoc = Utility.CreateUserDoc(0);

            bool status = await Utility.UserRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserDoc> docs = await Utility.UserRepository.Get(Utility.Context, testDoc.UserName);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);

            testDoc.Email = newEmail;
            docs.Payload.Email = newEmail;

            status = await Utility.UserRepository.Set(Utility.Context, testDoc, docs.ETag);
            status.Should().BeTrue();

            docs = await Utility.UserRepository.Get(Utility.Context, testDoc.UserName);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);
            await DeleteAndVerify(testDoc.UserName);
        }

        [Fact]
        public async Task CreateGetUpdateDeleteErrorTest()
        {
            const string newIssuerText = "new issuer";

            await Utility.ClearUserCollection();

            UserDoc testDoc = Utility.CreateUserDoc(0);

            bool status = await Utility.UserRepository.Set(Utility.Context, testDoc);
            status.Should().BeTrue();

            HeaderDoc<UserDoc> docs = await Utility.UserRepository.Get(Utility.Context, testDoc.UserName);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();

            Utility.CompareDocument(testDoc, docs.Payload);

            testDoc.Email = newIssuerText;
            docs.Payload.Email = newIssuerText;

            status = await Utility.UserRepository.Set(Utility.Context, testDoc, "fake");
            status.Should().BeFalse();

            await Utility.UserRepository.Delete(Utility.Context, testDoc.UserName);
        }

        [Fact]
        public async Task CreateManyGetUpdateDeleteTest()
        {
            const string newEmail = "new email";
            const int count = 11;
            int index = Utility.Random.Next(0, count - 1);

            await Utility.ClearUserCollection();

            List<UserDoc> testDocuments = (await Enumerable.Range(0, count)
                .Select(x => Utility.CreateUserDoc(x))
                .DoAsync(async x => await Utility.UserRepository.Set(Utility.Context, x))
                ).ToList();

            UserDoc testDoc = testDocuments[index];

            HeaderDoc<UserDoc> docs = await Utility.UserRepository.Get(Utility.Context, testDoc.UserName);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();
            Utility.CompareDocument(testDoc, docs.Payload);

            testDoc.Email = newEmail;
            docs.Payload.Email = newEmail;

            bool status = await Utility.UserRepository.Set(Utility.Context, testDoc, docs.ETag);
            status.Should().BeTrue();

            docs = await Utility.UserRepository.Get(Utility.Context, testDoc.UserName);
            docs.Should().NotBeNull();
            docs.ETag.Should().NotBeEmpty();
            Utility.CompareDocument(testDoc, docs.Payload);

            foreach (var item in testDocuments)
            {
                await Utility.UserRepository.Delete(Utility.Context, item.UserName);
            }

            PageResult<HeaderDoc<UserDoc>> deleteState = await Utility.UserRepository.List(Utility.Context, new PageRequest(10));
            deleteState.Should().NotBeNull();
            deleteState.Items.Count.Should().Be(0);
        }

        private async Task DeleteAndVerify(string userName)
        {
            await Utility.UserRepository.Delete(Utility.Context, userName);

            PageResult<HeaderDoc<UserDoc>> deleteState = await Utility.UserRepository.List(Utility.Context, new PageRequest(10));
            deleteState.Should().NotBeNull();
            deleteState.Items.Count.Should().Be(0);
        }
    }
}
