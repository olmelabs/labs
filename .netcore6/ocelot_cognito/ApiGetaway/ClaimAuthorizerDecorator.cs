using System.Security.Claims;
using Ocelot.Authorization;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Responses;

//https://github.com/ThreeMammals/Ocelot/issues/679

namespace ApiGateway
{
    public class ClaimAuthorizerDecorator : IClaimsAuthorizer
    {
        private readonly IClaimsAuthorizer authoriser;

        public ClaimAuthorizerDecorator(IClaimsAuthorizer authoriser)
        {
            this.authoriser = authoriser;
        }

        public Response<bool> Authorize(ClaimsPrincipal claimsPrincipal,
            Dictionary<string, string> routeClaimsRequirement,
            List<PlaceholderNameAndValue> urlPathPlaceholderNameAndValues)
        {
            var newRouteClaimsRequirement = new Dictionary<string, string>();
            foreach (var kvp in routeClaimsRequirement)
            {
                if (kvp.Key.Contains("__"))
                {
                    var key = kvp.Key.Replace("__", ":");
                    newRouteClaimsRequirement.Add(key, kvp.Value);
                }
                else
                {
                    newRouteClaimsRequirement.Add(kvp.Key, kvp.Value);
                }
            }

            return authoriser.Authorize(claimsPrincipal, newRouteClaimsRequirement, urlPathPlaceholderNameAndValues);
        }
    }
}
