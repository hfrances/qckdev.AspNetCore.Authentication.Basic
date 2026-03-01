using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using qckdev.AspNetCore.Authentication.Basic;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Authentication.Basic.Test
{
    [TestClass]
    public class CredentialsBasedValidatorTest
    {
        [TestMethod]
        public async Task ValidateAsync_WithValidCredentials_ReturnsTrue()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "admin",
                Password = "password123"
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("admin", "password123");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_WithInvalidUsername_ReturnsFalse()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "admin",
                Password = "password123"
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("wronguser", "password123");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_WithInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "admin",
                Password = "password123"
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("admin", "wrongpassword");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_CaseSensitiveComparison_ReturnsFalse()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "Admin",
                Password = "Password123"
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("admin", "password123");

            // Assert
            Assert.IsFalse(result, "Comparison should be case-sensitive");
        }

        [TestMethod]
        public async Task ValidateAsync_WithEmptyUsername_ReturnsFalse()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "",
                Password = "password123"
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("user", "password123");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "admin",
                Password = ""
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("admin", "pass");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_WithNullUsername_ReturnsFalse()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = null,
                Password = "password123"
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("user", "password123");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_WithNullPassword_ReturnsFalse()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "admin",
                Password = null
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);

            // Act
            var result = await validator.ValidateAsync("admin", "pass");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullOptionsMonitor_ThrowsArgumentNullException()
        {
            // Act
            new CredentialsBasedValidator(null!);
        }

        [TestMethod]
        public async Task ValidateAsync_WithCancellationToken_CompletesNormally()
        {
            // Arrange
            var options = Options.Create(new CredentialsBasedOptions
            {
                Username = "admin",
                Password = "password123"
            });
            var optionsMonitor = new MockOptionsMonitor(options.Value);
            var validator = new CredentialsBasedValidator(optionsMonitor);
            var cts = new CancellationTokenSource();

            // Act
            var result = await validator.ValidateAsync("admin", "password123", cts.Token);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Mock implementation of IOptionsMonitor for testing
        /// </summary>
        private class MockOptionsMonitor : IOptionsMonitor<CredentialsBasedOptions>
        {
            private readonly CredentialsBasedOptions _options;

            public MockOptionsMonitor(CredentialsBasedOptions options)
            {
                _options = options;
            }

            public CredentialsBasedOptions CurrentValue => _options;

            public CredentialsBasedOptions Get(string? name) => _options;

            public IDisposable? OnChange(Action<CredentialsBasedOptions, string?> listener) => null;
        }
    }
}
