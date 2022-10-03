using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Infocaster.Umbraco.ETag
{
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ETagAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        /// <inheritdoc />
        public int Order { get; set; }

        /// <inheritdoc />
        public bool IsReusable => true;

        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new ETagFilter(serviceProvider.GetRequiredService<IUmbracoContextAccessor>(),
                serviceProvider.GetRequiredService<IRuntimeState>(),
                serviceProvider.GetRequiredService<IETagFactory>());
        }
    }

    internal class ETagFilter : IActionFilter
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IRuntimeState _runtimeState;
        private readonly IETagFactory _etagFactory;

        public ETagFilter(IUmbracoContextAccessor umbracoContextAccessor, IRuntimeState runtimeState, IETagFactory etagFactory)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _runtimeState = runtimeState;
            _etagFactory = etagFactory;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // This action filter cannot be run before Umbraco is completely initialised
            // This action filter only works on requests to Umbraco content,
            //    so exclude every request without assigned published content
            if (_runtimeState.Level < RuntimeLevel.Run ||
                !_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) ||
                umbracoContext.PublishedRequest?.PublishedContent is null)
            {
                return;
            }

            var etag = _etagFactory.Create(umbracoContext.PublishedRequest).FormatAsETag();

            // If the expected ETag matches, then the browser still has an up-to-date version, so the request can be cut short
            var httpContext = context.HttpContext;
            if (httpContext.ETagMatches(etag))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.NotModified);
                return;
            }

            // At this point, we know for certain that a new version of this page will be served,
            //    so we can attach an ETag to the response
            httpContext.SetETag(etag);
        }

        // No logic required after action executed
        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}
