using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Infocaster.Umbraco.ETag
{
    /// <summary>
    /// A composer that includes the required services for ETags
    /// </summary>
    public class ETagComposer : IComposer
    {
        /// <inheritdoc />
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IETagFactory, ETagFactory>();
        }
    }
}
