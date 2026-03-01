using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Authentication.Basic.Test
{
    [TestClass]
    public class DependencyInjectionTest
    {
        sealed class CustomBasicOptions : BasicAuthenticationOptions
        {
            public string? CustomProperty { get; set; }
        }

        sealed class CustomValidator : IBasicAuthenticationValidator
        {
            public Task<bool> ValidateAsync(string username, string password, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(true);
            }
        }

        [TestMethod]
        public void AddBasicAuthentication_WithDefaultScheme_ConfiguresOptionsAndValidator()
        {
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(opts =>
                {
                    opts.Realm = "Test Realm";
                    opts.Username = "testuser";
                    opts.Password = "testpass";
                });

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CredentialsBasedOptions>>()
                .Get(BasicAuthenticationDefaults.AuthenticationScheme);
            var validator = provider.GetRequiredService<IBasicAuthenticationValidator>();

            Assert.IsNotNull(options);
            Assert.AreEqual("Test Realm", options.Realm);
            Assert.AreEqual("testuser", options.Username);
            Assert.AreEqual("testpass", options.Password);
            Assert.IsInstanceOfType(validator, typeof(CredentialsBasedValidator));
        }

        [TestMethod]
        public void AddBasicAuthentication_WithNamedScheme_ConfiguresNamedOptions()
        {
            const string scheme = "BasicAuth";
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(
                    scheme,
                    opts =>
                    {
                        opts.Realm = "Named Scheme";
                        opts.AllowEmptyCredentials = true;
                    });

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CredentialsBasedOptions>>().Get(scheme);

            Assert.AreEqual("Named Scheme", options.Realm);
            Assert.IsTrue(options.AllowEmptyCredentials);
        }

        [TestMethod]
        public async Task AddBasicAuthentication_WithDisplayName_ConfiguresDisplayName()
        {
            const string scheme = "CustomScheme";
            const string displayName = "My Custom Display";
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(
                    scheme,
                    displayName,
                    opts => opts.Realm = "Test");

            var provider = services.BuildServiceProvider();
            var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
            var scheme_ = await schemeProvider.GetSchemeAsync(scheme);

            Assert.IsNotNull(scheme_);
            Assert.AreEqual(displayName, scheme_.DisplayName);
        }

        [TestMethod]
        public void AddBasicAuthentication_WithSimpleSignature_UsesDefaultOptions()
        {
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CustomValidator>();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<BasicAuthenticationOptions>>()
                .Get(BasicAuthenticationDefaults.AuthenticationScheme);
            var validator = provider.GetRequiredService<IBasicAuthenticationValidator>();

            Assert.IsNotNull(options);
            Assert.AreEqual("Application", options.Realm); // Default value
            Assert.IsInstanceOfType(validator, typeof(CustomValidator));
        }

        [TestMethod]
        public void AddBasicAuthentication_WithCustomOptions_RegistersDerivedOptionsType()
        {
            const string scheme = "Custom";
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CustomValidator, CustomBasicOptions>(
                    scheme,
                    opts =>
                    {
                        opts.Realm = "Custom Realm";
                        opts.CustomProperty = "Custom Value";
                    });

            var provider = services.BuildServiceProvider();
            var customOptions = provider.GetRequiredService<IOptionsMonitor<CustomBasicOptions>>().Get(scheme);

            Assert.AreEqual("Custom Realm", customOptions.Realm);
            Assert.AreEqual("Custom Value", customOptions.CustomProperty);
        }

        [TestMethod]
        public void AddBasicAuthentication_ValidatorIsRegisteredAsScoped()
        {
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CustomValidator>();

            var provider = services.BuildServiceProvider();
            
            var validator1 = provider.CreateScope().ServiceProvider.GetRequiredService<IBasicAuthenticationValidator>();
            var validator2 = provider.CreateScope().ServiceProvider.GetRequiredService<IBasicAuthenticationValidator>();

            Assert.IsNotNull(validator1);
            Assert.IsNotNull(validator2);
            Assert.AreNotSame(validator1, validator2); // Different scopes should have different instances
        }

        [TestMethod]
        public void AddBasicAuthentication_WithCredentialsBasedValidator_CanAccessCredentials()
        {
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(opts =>
                {
                    opts.Username = "admin";
                    opts.Password = "secret123";
                });

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CredentialsBasedOptions>>()
                .Get(BasicAuthenticationDefaults.AuthenticationScheme);

            Assert.AreEqual("admin", options.Username);
            Assert.AreEqual("secret123", options.Password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddBasicAuthentication_WithNullScheme_ThrowsArgumentException()
        {
            var services = new ServiceCollection();
            var builder = services.AddAuthentication();

            builder.AddBasicAuthentication<CustomValidator, BasicAuthenticationOptions>(
                null!,
                null,
                null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddBasicAuthentication_WithEmptyScheme_ThrowsArgumentException()
        {
            var services = new ServiceCollection();
            var builder = services.AddAuthentication();

            builder.AddBasicAuthentication<CustomValidator, BasicAuthenticationOptions>(
                "",
                null,
                null);
        }
    }
}
