using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace MicrosservicoAdministrativo.IntegrationTests.Infra
{
	[ExcludeFromCodeCoverage]
	public static class HttpContentExtensions
	{
		public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
		{
			// Só aceita JSON do servidor. Não precisa adicionar a dependência System.Runtime.Serialization.Xml, 
			// que é requerido quando usado o formato padrão.
			return await content.ReadAsAsync<T>(GetJsonFormatters());
		}

		private static IEnumerable<MediaTypeFormatter> GetJsonFormatters()
		{
			yield return new JsonMediaTypeFormatter();
		}
	}
}