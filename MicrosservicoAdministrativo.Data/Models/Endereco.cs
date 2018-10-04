using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MicrosservicoAdministrativo.Data.Models
{
	public class Endereco
	{
		[Required, JsonProperty(PropertyName = "logradouro")]
		public string Logradouro { get; set; }

		[JsonProperty(PropertyName = "complemento")]
		public string Complemento { get; set; }

		[Required, JsonProperty(PropertyName = "cep")]
		public string Cep { get; set; }

		[Required, JsonProperty(PropertyName = "bairro")]
		public string Bairro { get; set; }

		[Required, JsonProperty(PropertyName = "cidade")]
		public string Cidade { get; set; }

		[Required, JsonProperty(PropertyName = "estado")]
		public string Estado { get; set; }

		[Required, JsonProperty(PropertyName = "pais")]
		public string Pais { get; set; }
	}
}