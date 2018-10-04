using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MicrosservicoAdministrativo.IntegrationTests.Infra
{
	[ExcludeFromCodeCoverage]
	public static class HttpClientExtensions
	{
		public static HttpClient AcceptJson(this HttpClient client)
		{
			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}
	}
}