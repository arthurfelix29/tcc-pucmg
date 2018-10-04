using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MicrosservicoAdministrativo.Data.Models
{
	public class Usuario
	{
		[Required, JsonProperty(PropertyName = "id")]
		public Guid Id { get; set; }

		[Required, JsonProperty(PropertyName = "nome")]
		public string Nome { get; set; }

		[Required, JsonProperty(PropertyName = "cpf")]
		public string Cpf { get; set; }

		[JsonProperty(PropertyName = "dataNascimento")]
		public DateTime DataNascimento { get; set; }

		[JsonProperty(PropertyName = "sexo")]
		public string Sexo { get; set; }

		[Required, JsonProperty(PropertyName = "email")]
		public string Email { get; set; }

		[Required, JsonProperty(PropertyName = "telefones")]
		public List<Telefone> Telefones { get; set; }

		[Required, JsonProperty(PropertyName = "enderecos")]
		public List<Endereco> Enderecos { get; set; }
	}
}