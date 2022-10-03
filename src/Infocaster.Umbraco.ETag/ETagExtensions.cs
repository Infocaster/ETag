using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Infocaster.Umbraco.ETag
{
    /// <summary>
    /// Extensions on the pipeline for integration of Umbraco ETag
    /// </summary>
    public static class ETagExtensions
    {
        /// <summary>
        /// Include ETag headers on all requests for Umbraco pages
        /// </summary>
        /// <param name="builder">The Umbraco pipeline builder</param>
        /// <returns>The Umbraco pipeline builder after the middlewares have been registered</returns>
        public static IUmbracoApplicationBuilderContext UseETag(
            this IUmbracoApplicationBuilderContext builder)
        {
            builder.AppBuilder.UseMiddleware<ETagMiddleware>();
            return builder;
        }

        internal static string FormatAsETag(this string input)
        {
            return $"\"{input}\"";
        }

        internal static bool ETagMatches(this HttpContext context, string etag)
        {
            var existingHeader = context.Request.GetTypedHeaders();
            return existingHeader.IfNoneMatch.Any(i => i.Tag == etag);
        }

        internal static void SetETag(this HttpContext context, string etag)
        {
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    var responseHeaders = context.Response.GetTypedHeaders();
                    responseHeaders.ETag = new EntityTagHeaderValue(etag, true);
                }

                return Task.CompletedTask;
            });
        }
    }

}
