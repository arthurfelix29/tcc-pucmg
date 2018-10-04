using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MicrosservicoAdministrativo.Data.Models
{
	public class Telefone
	{
		[JsonProperty(PropertyName = "ddi")]
		public string DDI { get; set; }

		[Required, JsonProperty(PropertyName = "ddd")]
		public string DDD { get; set; }

		[Required, JsonProperty(PropertyName = "numero")]
		public string Numero { get; set; }
	}
}