using System;

namespace MessageSender
{
    public class SenderConfiguration
    {
        public Uri BaseUrl { get; }
        public string ClientId { get; }

        public SenderConfiguration(Uri baseUrl, string clientId)
        {
            BaseUrl = baseUrl;
            ClientId = clientId;
        }
    }
}