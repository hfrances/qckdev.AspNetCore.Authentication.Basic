using Microsoft.AspNetCore.Authentication;
using qckdev.AspNetCore.Authentication.Basic;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure basic authentication.
    /// </summary>
    public static class QAspNetCoreAuthenticationBasic
    {
        /// <summary>
        /// Adds basic authentication using static credentials and the default scheme.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="CredentialsBasedOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            Action<CredentialsBasedOptions>? configureOptions = null)
        {
            return AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(
                builder,
                BasicAuthenticationDefaults.AuthenticationScheme,
                null,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication using static credentials and the specified scheme.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="CredentialsBasedOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<CredentialsBasedOptions>? configureOptions = null)
        {
            return AddBasicAuthentication<CredentialsBasedValidator, CredentialsBasedOptions>(
                builder,
                authenticationScheme,
                null,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication using the default scheme and the specified validator.
        /// </summary>
        /// <typeparam name="TValidator">The type of the basic authentication validator.</typeparam>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="BasicAuthenticationOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication<TValidator>(
            this AuthenticationBuilder builder,
            Action<BasicAuthenticationOptions>? configureOptions = null)
            where TValidator : class, IBasicAuthenticationValidator
        {
            return AddBasicAuthentication<TValidator, BasicAuthenticationOptions>(
                builder,
                BasicAuthenticationDefaults.AuthenticationScheme,
                null,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication using the specified scheme and validator.
        /// </summary>
        /// <typeparam name="TValidator">The type of the basic authentication validator.</typeparam>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="BasicAuthenticationOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication<TValidator>(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<BasicAuthenticationOptions>? configureOptions = null)
            where TValidator : class, IBasicAuthenticationValidator
        {
            return AddBasicAuthentication<TValidator, BasicAuthenticationOptions>(
                builder,
                authenticationScheme,
                null,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication using the specified scheme, display name, and validator.
        /// </summary>
        /// <typeparam name="TValidator">The type of the basic authentication validator.</typeparam>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="BasicAuthenticationOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication<TValidator>(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string? displayName,
            Action<BasicAuthenticationOptions>? configureOptions = null)
            where TValidator : class, IBasicAuthenticationValidator
        {
            return AddBasicAuthentication<TValidator, BasicAuthenticationOptions>(
                builder,
                authenticationScheme,
                displayName,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication using the default scheme with custom options type and validator.
        /// </summary>
        /// <typeparam name="TValidator">The type of the basic authentication validator.</typeparam>
        /// <typeparam name="TOptions">The type of the authentication options.</typeparam>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate that allows configuring <typeparamref name="TOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication<TValidator, TOptions>(
            this AuthenticationBuilder builder,
            Action<TOptions>? configureOptions = null)
            where TValidator : class, IBasicAuthenticationValidator
            where TOptions : BasicAuthenticationOptions, new()
        {
            return AddBasicAuthentication<TValidator, TOptions>(
                builder,
                BasicAuthenticationDefaults.AuthenticationScheme,
                null,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication using the specified scheme with custom options type and validator.
        /// </summary>
        /// <typeparam name="TValidator">The type of the basic authentication validator.</typeparam>
        /// <typeparam name="TOptions">The type of the authentication options.</typeparam>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate that allows configuring <typeparamref name="TOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication<TValidator, TOptions>(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<TOptions>? configureOptions = null)
            where TValidator : class, IBasicAuthenticationValidator
            where TOptions : BasicAuthenticationOptions, new()
        {
            return AddBasicAuthentication<TValidator, TOptions>(
                builder,
                authenticationScheme,
                null,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication using the specified scheme, display name, custom options type, and validator.
        /// </summary>
        /// <typeparam name="TValidator">The type of the basic authentication validator.</typeparam>
        /// <typeparam name="TOptions">The type of the authentication options.</typeparam>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate that allows configuring <typeparamref name="TOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddBasicAuthentication<TValidator, TOptions>(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string? displayName,
            Action<TOptions>? configureOptions = null)
            where TValidator : class, IBasicAuthenticationValidator
            where TOptions : BasicAuthenticationOptions, new()
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrWhiteSpace(authenticationScheme))
            {
                throw new ArgumentException("Authentication scheme cannot be null or empty", nameof(authenticationScheme));
            }

            builder.Services.AddScoped<TValidator>();

            return builder.AddScheme<TOptions, BasicAuthenticationHandler<TOptions>>(
                authenticationScheme,
                displayName,
                options =>
                {
                    configureOptions?.Invoke(options);
                    options.ValidatorType = typeof(TValidator);
                });
        }
    }
}
