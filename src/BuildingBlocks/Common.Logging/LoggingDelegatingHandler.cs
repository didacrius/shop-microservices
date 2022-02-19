using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Logging
{
    public class LoggingDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingDelegatingHandler> _logger;

        public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
            
                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Received a success response from {Url}", response.RequestMessage.RequestUri);
                else
                    _logger.LogWarning("Received a non-success status code {StatusCode} from {Uri}",
                        (int)response.StatusCode, response.RequestMessage.RequestUri);

                return response;
            }
            catch (HttpRequestException e)
                when (e.InnerException is SocketException {SocketErrorCode: SocketError.ConnectionRefused})
            {
                var hostWithPort = request.RequestUri.IsDefaultPort
                    ? request.RequestUri.DnsSafeHost
                    : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";
            
                _logger.LogCritical(e, "Unable to connect to {Host}. Please check the " +
                                       "configuration to ensure the correct URL for the service " +
                                       "has been configured.", hostWithPort);
            }

            return new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                RequestMessage = request
            };
        }
    }
}