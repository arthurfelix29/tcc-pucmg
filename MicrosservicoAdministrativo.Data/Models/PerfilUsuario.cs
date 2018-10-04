using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicrosservicoAdministrativo.Data.Models
{
	public class PerfilUsuario
	{
		[Required, JsonProperty(PropertyName = "id")]
		public Guid Id { get; set; }

		[Required, JsonProperty(PropertyName = "idPerfil")]
		public Guid IdPerfil { get; set; }

		[Required, JsonProperty(PropertyName = "idUsuario")]
		public Guid IdUsuario { get; set; }

		[JsonProperty(PropertyName = "ativo")]
		public bool Ativo { get; set; }

		[Required, JsonProperty(PropertyName = "dataInclusao")]
		public DateTime DataInclusao { get; set; }
	}
}