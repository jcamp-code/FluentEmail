using Azure.Core;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.Graph
{
    internal class ClientAuthHandler : TokenCredential
    {
        private readonly IConfidentialClientApplication _clientApp;
        
        public ClientAuthHandler(string appId, string tenantId, string graphSecret)
        {
            var builder = ConfidentialClientApplicationBuilder
              .Create(appId)
              .WithTenantId(tenantId)
              .WithClientSecret(graphSecret);

            _clientApp = builder.Build();
        }

        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var result = await _clientApp
                .AcquireTokenForClient(requestContext.Scopes)
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var result = _clientApp
                .AcquireTokenForClient(requestContext.Scopes)
                .ExecuteAsync(cancellationToken)
                .GetAwaiter()
                .GetResult();

            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
    }
}
