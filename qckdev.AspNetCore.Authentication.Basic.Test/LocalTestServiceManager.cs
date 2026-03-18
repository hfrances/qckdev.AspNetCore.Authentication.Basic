using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using qckdev.AspNetCore.Authentication.Basic;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Authentication.Basic.Test
{
    /// <summary>
    /// Simple validator for testing that uses hardcoded credentials.
    /// </summary>
    internal class TestBasicAuthenticationValidator : IBasicAuthenticationValidator
    {
        public Task<bool> ValidateAsync(string username, string password, string authenticationScheme, CancellationToken cancellationToken = default)
        {
            // Hardcoded test credentials
            var isValid = username == "testuser" && password == "testpass";
            return Task.FromResult(isValid);
        }
    }

    internal class SecondaryBasicAuthenticationValidator : IBasicAuthenticationValidator
    {
        public Task<bool> ValidateAsync(string username, string password, string authenticationScheme, CancellationToken cancellationToken = default)
        {
            var isValid = username == "otheruser" && password == "otherpass";
            return Task.FromResult(isValid);
        }
    }

    internal static class LocalTestServiceManager
    {
        private static readonly object SyncLock = new object();
        private static IHost? Host;
        private static bool Initialized;
        private static readonly Uri BaseUri = new Uri($"http://localhost:{GetPortForCurrentProcess()}/");

        public static Uri ServiceUri => BaseUri;

        public static void StartIfNeeded()
        {
            lock (SyncLock)
            {
                if (Initialized)
                {
                    return;
                }

                var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls(BaseUri.ToString());
                        webBuilder.ConfigureServices(services =>
                        {
                            services.AddControllers().AddApplicationPart(typeof(BasicAuthenticationTestController).Assembly);
                            services
                                .AddAuthentication()
                                .AddBasicAuthentication<TestBasicAuthenticationValidator>(
                                    opts =>
                                    {
                                        opts.Realm = "Test Realm";
                                    })
                                .AddBasicAuthentication<SecondaryBasicAuthenticationValidator>(
                                    "BasicSecondary",
                                    opts =>
                                    {
                                        opts.Realm = "Secondary Realm";
                                    });
                            services.AddAuthorization();
                        });
                        webBuilder.Configure(app =>
                        {
                            app.UseRouting();
                            app.UseAuthentication();
                            app.UseAuthorization();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                            });
                        });
                    });

                Host = builder.Build();
                Host.Start();

                if (!WaitForService(10000))
                {
                    Stop();
                    throw new InvalidOperationException($"Test service did not become ready at {BaseUri}");
                }

                Initialized = true;
            }
        }

        public static void Stop()
        {
            lock (SyncLock)
            {
                try
                {
                    Host?.StopAsync().GetAwaiter().GetResult();
                    Host?.Dispose();
                }
                finally
                {
                    Host = null;
                    Initialized = false;
                }
            }
        }

        private static bool WaitForService(int timeoutMs)
        {
            using var client = new HttpClient { BaseAddress = BaseUri };
            var started = DateTime.UtcNow;

            while ((DateTime.UtcNow - started).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    var response = client.GetAsync("basic/public").GetAwaiter().GetResult();
                    var statusCode = (int)response.StatusCode;
                    if (statusCode >= 200 && statusCode < 500)
                    {
                        return true;
                    }
                }
                catch
                {
                }

                Thread.Sleep(150);
            }

            return false;
        }

        private static int GetPortForCurrentProcess()
        {
            var pid = Process.GetCurrentProcess().Id;
            return 26000 + (pid % 10000);
        }
    }
}
