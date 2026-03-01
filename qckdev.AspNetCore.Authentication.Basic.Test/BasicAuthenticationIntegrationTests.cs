using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace qckdev.AspNetCore.Authentication.Basic.Test
{
    [TestClass]
    public class BasicAuthenticationIntegrationTests
    {
        [TestMethod]
        public void PublicEndpoint_WithoutCredentials_ReturnsOk()
        {
            using var client = new HttpClient { BaseAddress = LocalTestServiceManager.ServiceUri };

            var response = client.GetAsync("basic/public").GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.AreEqual("public-ok", content);
        }

        [TestMethod]
        public void ProtectedEndpoint_WithoutCredentials_ReturnsUnauthorized()
        {
            using var client = new HttpClient { BaseAddress = LocalTestServiceManager.ServiceUri };

            var response = client.GetAsync("basic/protected").GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.IsTrue(response.Headers.Contains("WWW-Authenticate"));
            var wwwAuthenticate = response.Headers.GetValues("WWW-Authenticate");
            var wwwAuthValue = string.Join(", ", wwwAuthenticate);
            Assert.IsTrue(wwwAuthValue.Contains("Basic"));
            Assert.IsTrue(wwwAuthValue.Contains("Test Realm"));
        }

        [TestMethod]
        public void ProtectedEndpoint_WithValidCredentials_ReturnsOk()
        {
            using var client = new HttpClient { BaseAddress = LocalTestServiceManager.ServiceUri };
            using var request = new HttpRequestMessage(HttpMethod.Get, "basic/protected");

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass"));
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.AreEqual("protected-ok-testuser", content);
        }

        [TestMethod]
        public void ProtectedEndpoint_WithInvalidUsername_ReturnsUnauthorized()
        {
            using var client = new HttpClient { BaseAddress = LocalTestServiceManager.ServiceUri };
            using var request = new HttpRequestMessage(HttpMethod.Get, "basic/protected");

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("wronguser:testpass"));
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void ProtectedEndpoint_WithInvalidPassword_ReturnsUnauthorized()
        {
            using var client = new HttpClient { BaseAddress = LocalTestServiceManager.ServiceUri };
            using var request = new HttpRequestMessage(HttpMethod.Get, "basic/protected");

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:wrongpass"));
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void ProtectedEndpoint_WithMalformedAuthorizationHeader_ReturnsUnauthorized()
        {
            using var client = new HttpClient { BaseAddress = LocalTestServiceManager.ServiceUri };
            using var request = new HttpRequestMessage(HttpMethod.Get, "basic/protected");

            request.Headers.Add("Authorization", "Basic invalid-base64-data!!!");

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void ProtectedEndpoint_WithEmptyPassword_ReturnsUnauthorized()
        {
            using var client = new HttpClient { BaseAddress = LocalTestServiceManager.ServiceUri };
            using var request = new HttpRequestMessage(HttpMethod.Get, "basic/protected");

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:"));
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
