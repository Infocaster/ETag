using Umbraco.Cms.Core.Routing;

namespace Infocaster.Umbraco.ETag
{
    /// <summary>
    /// When implemented, this type can create an etag string from a published request
    /// </summary>
    public interface IETagFactory
    {
        /// <summary>
        /// When implemented, this method creates an ETag string, based on the published request
        /// </summary>
        /// <param name="request">The Umbraco request related to the current request</param>
        /// <returns>A string that represents the ETag for the current request</returns>
        string Create(IPublishedRequest request);
    }

    internal class ETagFactory : IETagFactory
    {
        public string Create(IPublishedRequest request)
        {
            return request.PublishedContent!.UpdateDate.Ticks.ToString() + request.Template?.UpdateDate.Ticks.ToString();
        }
    }
}
