using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MicrosservicoAdministrativo.Data.Models
{
	public class Empresa
	{
		[Required, JsonProperty(PropertyName = "id")]
		public Guid Id { get; set; }

		[Required, JsonProperty(PropertyName = "razaoSocial")]
		public string RazaoSocial { get; set; }

		[Required, JsonProperty(PropertyName = "nomeFantasia")]
		public string NomeFantasia { get; set; }

		[Required, JsonProperty(PropertyName = "cnpj")]
		public string Cnpj { get; set; }

		[JsonProperty(PropertyName = "ativo")]
		public bool Ativo { get; set; }

		[Required, JsonProperty(PropertyName = "idTipoEmpresa")]
		public Guid IdTipoEmpresa { get; set; }
	}
}