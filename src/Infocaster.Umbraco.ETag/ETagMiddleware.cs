using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Infocaster.Umbraco.ETag;

internal class ETagMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IRuntimeState _runtimeState;
    private readonly IETagFactory _etagFactory;

    public ETagMiddleware(RequestDelegate next, IUmbracoContextAccessor umbracoContextAccessor, IRuntimeState runtimeState, IETagFactory etagFactory)
    {
        _next = next;
        _umbracoContextAccessor = umbracoContextAccessor;
        _runtimeState = runtimeState;
        _etagFactory = etagFactory;
    }

    public Task InvokeAsync(HttpContext context)
    {
        // This middleware cannot be run before Umbraco is completely initialised
        // This middleware only works on requests to Umbraco content,
        //    so exclude every request without assigned published content
        if (_runtimeState.Level < RuntimeLevel.Run ||
            !_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) ||
            umbracoContext.PublishedRequest?.PublishedContent is null)
        {
            return _next(context);
        }

        var etag = _etagFactory.Create(umbracoContext.PublishedRequest).FormatAsETag();

        // If the expected ETag matches, then the browser still has an up-to-date version, so the request can be cut short
        if (context.ETagMatches(etag))
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.NotModified;
            return Task.CompletedTask;
        }

        // At this point, we know for certain that a new version of this page will be served,
        //    so we can attach an ETag to the response
        context.SetETag(etag);
        return _next(context);
    }
}