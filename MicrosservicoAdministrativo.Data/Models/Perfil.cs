using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicrosservicoAdministrativo.Data.Models
{
	public class Perfil
	{
		[Required, JsonProperty(PropertyName = "id")]
		public Guid Id { get; set; }

		[Required, JsonProperty(PropertyName = "sigla")]
		public string Sigla { get; set; }

		[Required, JsonProperty(PropertyName = "descricao")]
		public string Descricao { get; set; }
	}
}