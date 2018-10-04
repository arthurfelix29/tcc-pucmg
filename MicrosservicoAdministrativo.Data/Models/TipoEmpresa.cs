using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicrosservicoAdministrativo.Data.Models
{
	public class TipoEmpresa
	{
		[Required, JsonProperty(PropertyName = "id")]
		public Guid Id { get; set; }

		[Required, JsonProperty(PropertyName = "descricao")]
		public string Descricao { get; set; }
	}
}