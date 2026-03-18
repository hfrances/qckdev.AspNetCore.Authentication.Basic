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
            public Task<bool> ValidateAsync(string username, string password, string authenticationScheme, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(true);
            }
        }

        sealed class OptionsMonitorValidator : IBasicAuthenticationValidator
        {
            private readonly IOptionsMonitor<CredentialsBasedOptions> _optionsMonitor;

            public OptionsMonitorValidator(IOptionsMonitor<CredentialsBasedOptions> optionsMonitor)
            {
                _optionsMonitor = optionsMonitor;
            }

            public Task<bool> ValidateAsync(string username, string password, string authenticationScheme, CancellationToken cancellationToken = default)
            {
                var options = _optionsMonitor.Get(authenticationScheme);
                var isValid = string.Equals(options.Username, username, StringComparison.Ordinal)
                    && string.Equals(options.Password, password, StringComparison.Ordinal);
                return Task.FromResult(isValid);
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
            var validator = provider.GetRequiredService<CredentialsBasedValidator>();

            Assert.IsNotNull(options);
            Assert.AreEqual("Test Realm", options.Realm);
            Assert.AreEqual("testuser", options.Username);
            Assert.AreEqual("testpass", options.Password);
            Assert.IsInstanceOfType(validator, typeof(CredentialsBasedValidator));
            Assert.AreEqual(typeof(CredentialsBasedValidator), options.ValidatorType);
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
            Assert.AreEqual(typeof(CredentialsBasedValidator), options.ValidatorType);
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
            var validator = provider.GetRequiredService<CustomValidator>();

            Assert.IsNotNull(options);
            Assert.AreEqual("Application", options.Realm); // Default value
            Assert.IsInstanceOfType(validator, typeof(CustomValidator));
            Assert.AreEqual(typeof(CustomValidator), options.ValidatorType);
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
            Assert.AreEqual(typeof(CustomValidator), customOptions.ValidatorType);
        }

        [TestMethod]
        public void AddBasicAuthentication_ValidatorIsRegisteredAsScoped()
        {
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<CustomValidator>();

            var provider = services.BuildServiceProvider();
            
            var validator1 = provider.CreateScope().ServiceProvider.GetRequiredService<CustomValidator>();
            var validator2 = provider.CreateScope().ServiceProvider.GetRequiredService<CustomValidator>();

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
            Assert.AreEqual(typeof(CredentialsBasedValidator), options.ValidatorType);
        }

        [TestMethod]
        public void AddBasicAuthentication_NonGenericDefaultOverload_UsesCredentialsValidator()
        {
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication(opts =>
                {
                    opts.Realm = "Default Realm";
                    opts.Username = "user-1";
                    opts.Password = "pwd-1";
                });

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CredentialsBasedOptions>>()
                .Get(BasicAuthenticationDefaults.AuthenticationScheme);

            Assert.AreEqual("Default Realm", options.Realm);
            Assert.AreEqual("user-1", options.Username);
            Assert.AreEqual("pwd-1", options.Password);
            Assert.AreEqual(typeof(CredentialsBasedValidator), options.ValidatorType);
        }

        [TestMethod]
        public void AddBasicAuthentication_NonGenericNamedOverload_ConfiguresNamedScheme()
        {
            const string scheme = "Secondary";
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication(
                    scheme,
                    opts =>
                    {
                        opts.Realm = "Secondary Realm";
                        opts.Username = "user-2";
                        opts.Password = "pwd-2";
                    });

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CredentialsBasedOptions>>().Get(scheme);

            Assert.AreEqual("Secondary Realm", options.Realm);
            Assert.AreEqual("user-2", options.Username);
            Assert.AreEqual("pwd-2", options.Password);
            Assert.AreEqual(typeof(CredentialsBasedValidator), options.ValidatorType);
        }

        [TestMethod]
        public async Task AddBasicAuthentication_DefaultScheme_WithTValidator_CanResolveNamedOptionsViaGet()
        {
            var services = new ServiceCollection();

            services
                .AddAuthentication()
                .AddBasicAuthentication<OptionsMonitorValidator, CredentialsBasedOptions>(opts =>
                {
                    opts.Username = "default-user";
                    opts.Password = "default-pass";
                });

            var provider = services.BuildServiceProvider();
            var validator = provider.GetRequiredService<OptionsMonitorValidator>();
            var configured = provider.GetRequiredService<IOptionsMonitor<CredentialsBasedOptions>>()
                .Get(BasicAuthenticationDefaults.AuthenticationScheme);

            var valid = await validator.ValidateAsync(
                "default-user",
                "default-pass",
                BasicAuthenticationDefaults.AuthenticationScheme);
            var invalid = await validator.ValidateAsync(
                "default-user",
                "wrong-pass",
                BasicAuthenticationDefaults.AuthenticationScheme);

            Assert.AreEqual(typeof(OptionsMonitorValidator), configured.ValidatorType);
            Assert.IsTrue(valid);
            Assert.IsFalse(invalid);
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
