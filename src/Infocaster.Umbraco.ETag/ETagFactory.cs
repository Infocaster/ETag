using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

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
        // a long list of prime numbers should be enough to generate an acceptable amount of randomness
        private static readonly long[] _primes = new[]
        {
            609101L,
            956993L,
            695477L,
            298483L,
            984341L,
            159793L,
            368881L,
            837271L,
            689699L,
            576883L
        };

        public string Create(IPublishedRequest request)
        {
            /* Business rule: Always update on following criteria:
             *  - Current page has been updated
             *  - Template of current page has updated
             *  - Any of the ancestors has updated
             *  - Update every hour
             */

            List<long> numbers = new ();
            numbers.Add(request.Template?.UpdateDate.Ticks ?? 0);
            numbers.AddRange(request.PublishedContent!.AncestorsOrSelf().Select(c => c.UpdateDate.Ticks));
            numbers.Add(DateTime.UtcNow.Hour);

            // We need to wrap the number back to long.minvalue when we overflow. Overflow is by design here
            long finalNumber = 0;
            unchecked
            {
                for(int i = 0; i < numbers.Count; i++)
                {
                    finalNumber += numbers[i] * _primes[i % _primes.Length];
                }
            }

            return finalNumber.ToString();
        }
    }
}
