using FluentAssertions;
using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Repository.Mongo.Test
{
    internal static class Utility
    {
        private const string _dbName = "TestIdentityDatabase";
        private static IActorManager _actorManager = new ActorManager();
        private static readonly object _lock = new object();

        public static IWorkContext Context { get; } = WorkContext.Empty;

        public static IIdentityConfiguration IdentityConfiguration { get; } = new IdentityConfiguration(Constants.ConnectionString, _dbName, _actorManager);

        public static IAdministrationRepository AdministrationRepository { get; private set; }

        public static IRoleRepository RoleRepository { get; private set; }

        public static IUserRepository UserRepository { get; private set; }

        public static Random Random { get; } = new Random();

        public static void Initialize()
        {
            InitializeInternal()
                .GetAwaiter()
                .GetResult();
        }

        private static async Task InitializeInternal()
        {
            lock (_lock)
            {
                if (AdministrationRepository != null)
                {
                    return;
                }

                AdministrationRepository = new AdministrationRepository(IdentityConfiguration);
                RoleRepository = new RoleRepository(IdentityConfiguration);
                UserRepository = new UserRepository(IdentityConfiguration);
            }

            await AdministrationRepository.ResetCollections(Context).ConfigureAwait(false);
        }

        public static UserRoleDoc CreateUserRoleDoc(int index)
        {
            return new UserRoleDoc
            {
                RoleId = $"RoleId_{index}",
                Claims = Enumerable.Range(0, 5)
                    .Select(x => new UserClaimDoc
                    {
                        Type = $"Type_{x}",
                        Value = $"Value_{x}",
                        Issuer = $"Issuer_{x}"
                    }).ToList(),
            };
        }

        public static void CompareDocument(UserRoleDoc source, UserRoleDoc compareTo)
        {
            source.Should().NotBeNull();
            compareTo.Should().NotBeNull();

            source.RoleId.Should().Be(compareTo.RoleId);
            source.Claims.Should().NotBeNull();
            compareTo.Claims.Should().NotBeNull();
            source.Claims.Count.Should().Be(compareTo.Claims.Count);

            for (int i = 0; i < source.Claims.Count; i++)
            {
                source.Claims[i].Type.Should().Be(compareTo.Claims[i].Type);
                source.Claims[i].Value.Should().Be(compareTo.Claims[i].Value);
                source.Claims[i].Issuer.Should().Be(compareTo.Claims[i].Issuer);
            }
        }

        public static UserDoc CreateUserDoc(int index)
        {
            return new UserDoc
            {
                UserName = $"UserName_{index}",
                Email = $"Email_{index}",
                NormalizedEmail = $"NormalizedEmail_{index}",
                EmailConfirmed = true,
                LockoutEnabled = false,
                PasswordHash = $"PasswordHash_{index}",
                PhoneNumber = $"PhoneNumber_{index}",
                PhoneNumberConfirmed = true,
                SecurityStamp = $"SecurityStamp_{index}",
                TwoFactorEnabled = false,
                AccessFailedCount = index,
                Tokens = Enumerable.Range(0, 3)
                    .Select(x => new UserTokenDoc
                    {
                        LoginProvider = $"LoginProvider_{x}",
                        Name = $"Name_{x}",
                        Value = $"Value_{x}",
                    }).ToList(),
                UserTokens = Enumerable.Range(0, 4)
                    .Select(x => new UserClaimDoc
                    {
                        Type = $"Type_{x}",
                        Value = $"Value_{x}",
                        Issuer = $"Issuer_{x}"

                    }).ToList(),
                Roles = Enumerable.Range(0, 1).Select(x => $"Role{x}").ToList(),
                Logins = Enumerable.Range(0, 5)
                    .Select(x => new UserLoginDoc
                    {
                        LoginProvider = $"LoginProvider{x}",
                        ProviderKey = $"ProviderKey{x}",
                        ProviderDisplayName = $"ProviderDisplayName{x}"
                    }).ToList(),
            };
        }

        public static void CompareDocument(UserDoc source, UserDoc compareTo)
        {
            source.Should().NotBeNull();
            compareTo.Should().NotBeNull();

            source.UserName.Should().Be(compareTo.UserName);
            source.Email.Should().Be(compareTo.Email);
            source.NormalizedEmail.Should().Be(compareTo.NormalizedEmail);
            source.EmailConfirmed.Should().Be(compareTo.EmailConfirmed);
            source.LockoutEnabled.Should().Be(compareTo.LockoutEnabled);
            source.PasswordHash.Should().Be(compareTo.PasswordHash);
            source.PhoneNumber.Should().Be(compareTo.PhoneNumber);
            source.PhoneNumberConfirmed.Should().Be(compareTo.PhoneNumberConfirmed);
            source.SecurityStamp.Should().Be(compareTo.SecurityStamp);
            source.TwoFactorEnabled.Should().Be(compareTo.TwoFactorEnabled);
            source.AccessFailedCount.Should().Be(compareTo.AccessFailedCount);

            source.Tokens.Should().NotBeNull();
            compareTo.Tokens.Should().NotBeNull();
            source.Tokens.Count.Should().Be(compareTo.Tokens.Count);

            for (int i = 0; i < source.Tokens.Count; i++)
            {
                source.Tokens[i].LoginProvider.Should().Be(compareTo.Tokens[i].LoginProvider);
                source.Tokens[i].Name.Should().Be(compareTo.Tokens[i].Name);
                source.Tokens[i].Value.Should().Be(compareTo.Tokens[i].Value);
            }

            source.UserTokens.Should().NotBeNull();
            compareTo.UserTokens.Should().NotBeNull();
            source.UserTokens.Count.Should().Be(compareTo.UserTokens.Count);

            for (int i = 0; i < source.UserTokens.Count; i++)
            {
                source.UserTokens[i].Type.Should().Be(compareTo.UserTokens[i].Type);
                source.UserTokens[i].Value.Should().Be(compareTo.UserTokens[i].Value);
                source.UserTokens[i].Issuer.Should().Be(compareTo.UserTokens[i].Issuer);
            }

            source.Roles.Should().NotBeNull();
            compareTo.Roles.Should().NotBeNull();
            source.Roles.Count.Should().Be(compareTo.Roles.Count);

            for (int i = 0; i < source.Roles.Count; i++)
            {
                source.Roles[i].Should().Be(compareTo.Roles[i]);
            }

            source.Logins.Should().NotBeNull();
            compareTo.Logins.Should().NotBeNull();
            source.Logins.Count.Should().Be(compareTo.Logins.Count);

            for (int i = 0; i < source.Logins.Count; i++)
            {
                source.Logins[i].LoginProvider.Should().Be(compareTo.Logins[i].LoginProvider);
                source.Logins[i].ProviderKey.Should().Be(compareTo.Logins[i].ProviderKey);
                source.Logins[i].ProviderDisplayName.Should().Be(compareTo.Logins[i].ProviderDisplayName);
            }
        }

        public static async Task ClearRoleCollection()
        {
            PageResult<HeaderDoc<UserRoleDoc>> deleteState = await Utility.RoleRepository.List(Utility.Context, new PageRequest(100));
            deleteState.Should().NotBeNull();

            foreach (var item in deleteState.Items)
            {
                await Utility.RoleRepository.Delete(Utility.Context, item.Payload.RoleId);
            }

            deleteState = await Utility.RoleRepository.List(Utility.Context, new PageRequest(10));
            deleteState.Should().NotBeNull();
            deleteState.Items.Count.Should().Be(0);
        }


        public static async Task ClearUserCollection()
        {
            PageResult<HeaderDoc<UserDoc>> deleteState = await Utility.UserRepository.List(Utility.Context, new PageRequest(100));
            deleteState.Should().NotBeNull();

            foreach (var item in deleteState.Items)
            {
                await Utility.UserRepository.Delete(Utility.Context, item.Payload.NormalizedUserName);
            }

            deleteState = await Utility.UserRepository.List(Utility.Context, new PageRequest(10));
            deleteState.Should().NotBeNull();
            deleteState.Items.Count.Should().Be(0);
        }
    }
}
