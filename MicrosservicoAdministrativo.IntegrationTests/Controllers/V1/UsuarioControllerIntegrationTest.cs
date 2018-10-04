using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.IntegrationTests.Infra;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace MicrosservicoAdministrativo.IntegrationTests.Controllers.V1
{
	[ExcludeFromCodeCoverage]
	public class UsuarioControllerIntegrationTest
	{
		private readonly TestServer _server;

		public UsuarioControllerIntegrationTest()
		{
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		}

		[Fact]
		public async void UsuarioControllerIntegrationTest_ConsigoPostEGetByIdEDelete()
		{
			var usuario = new Usuario
			{
				Id = Guid.NewGuid(),
				Nome = "Vinicius Sousa Rocha",
				Cpf = "84049124084",
				DataNascimento = new DateTime(1938, 11, 27),
				Sexo = "M",
				Email = "viniciussrocha@gmail.com",
				Enderecos = new List<Endereco> { new Endereco() { Logradouro = "Rua 1", Complemento = "Fundos", Cep = "22222222",
																  Bairro = "Santíssimo", Cidade = "Rio de Janeiro", Estado = "RJ",
																  Pais = "Brasil" },
												 new Endereco() { Logradouro = "Rua 2", Complemento = "Apto 601", Cep = "33333333",
																  Bairro = "Morumbi", Cidade = "São Paulo", Estado = "SP",
																  Pais = "Brasil" } },
				Telefones = new List<Telefone>() { new Telefone() { DDI = "55", DDD = "21", Numero = "99999999" } }
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Usuario", usuario);
				var created = await postResponse.Content.ReadAsJsonAsync<Usuario>();

				var getResponse = await client.GetAsync("/api/v1/Usuario/" + created.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Usuario>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Usuario/" + fetched.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Usuario/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Usuario>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				usuario.Nome.Should().BeEquivalentTo(created.Nome);
				usuario.Nome.Should().BeEquivalentTo(fetched.Nome);

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);

				all.Select(x => x.Id).Should().NotContain(usuario.Id);
			}
		}

		[Fact]
		public async void UsuarioControllerIntegrationTest_ConsigoPostEGetAllEDelete()
		{
			var usuario = new Usuario
			{
				Id = Guid.NewGuid(),
				Nome = "Sophia Cardoso Rocha",
				Cpf = "32166771092",
				DataNascimento = new DateTime(1940, 1, 29),
				Sexo = "F",
				Email = "scardoso@hotmail.com",
				Enderecos = new List<Endereco> { new Endereco() { Logradouro = "Rua Três", Cep = "37505448",
																  Bairro = "Qualquer Um", Cidade = "Itajubá", Estado = "MG",
																  Pais = "Brasil" } },
				Telefones = new List<Telefone>() { new Telefone() { DDI = "55", DDD = "21", Numero = "111111111" } }
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Usuario", usuario);
				var created = await postResponse.Content.ReadAsJsonAsync<Usuario>();

				var getAllResponse = await client.GetAsync("/api/v1/Usuario/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Usuario>>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Usuario/" + created.Id);
				var getAllForDeleteResponse = await client.GetAsync("/api/v1/Usuario/");
				var allForDelete = await getAllForDeleteResponse.Content.ReadAsJsonAsync<List<Usuario>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllForDeleteResponse.IsSuccessStatusCode.Should().BeTrue();

				usuario.Nome.Should().BeEquivalentTo(created.Nome);
				created.Id.Should().NotBe(Guid.Empty);

				all.Should().NotBeEmpty();
				created.Id.Should().Be(all.Single(x => x.Id == created.Id).Id);

				allForDelete.Select(x => x.Id).Should().NotContain(usuario.Id);
			}
		}

		[Fact]
		public async void UsuarioControllerIntegrationTest_NaoConsigoPostSemCampoObrigatorio()
		{
			var usuario = new Usuario
			{
				Id = Guid.NewGuid(),
				Cpf = "32166771092",
				DataNascimento = new DateTime(1940, 1, 29),
				Sexo = "F",
				Email = "scardoso@hotmail.com"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Usuario", usuario);
				postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			}
		}

		[Fact]
		public async void UsuarioControllerIntegrationTest_ConsigoPostEPutEGetByIdEDelete()
		{
			var usuario = new Usuario
			{
				Id = Guid.NewGuid(),
				Nome = "Eduardo Alves Goncalves",
				Cpf = "93625389037",
				DataNascimento = new DateTime(1991, 9, 21),
				Sexo = "M",
				Email = "egoncalves@yahoo.com.br",
				Enderecos = new List<Endereco> { new Endereco() { Logradouro = "Avenida Tancredo Neves", Cep = "78070473",
																  Bairro = "Passa Quatro", Cidade = "Cuiabá", Estado = "MT",
																  Pais = "Brasil" } },
				Telefones = new List<Telefone>() { new Telefone() { DDI = "55", DDD = "34", Numero = "33333333" } }
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Usuario", usuario);
				var created = await postResponse.Content.ReadAsJsonAsync<Usuario>();

				usuario.Id = created.Id;
				usuario.Email = "eduardo.goncalves@outlook.com";

				var putResponse = await client.PutAsJsonAsync("/api/v1/Usuario/" + created.Id, usuario);

				var getResponse = await client.GetAsync("/api/v1/Usuario/" + usuario.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Usuario>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Usuario/" + fetched.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Usuario/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Usuario>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				putResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				created.Email.Should().BeEquivalentTo("egoncalves@yahoo.com.br");
				fetched.Email.Should().BeEquivalentTo("eduardo.goncalves@outlook.com");

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);

				all.Select(x => x.Id).Should().NotContain(usuario.Id);
			}
		}

		[Fact]
		public async void UsuarioControllerIntegrationTest_ConsigoPostEDelete()
		{
			var usuario = new Usuario
			{
				Id = Guid.NewGuid(),
				Nome = "Kauê Silva Melo",
				Cpf = "48703161021",
				DataNascimento = new DateTime(1949, 3, 2),
				Sexo = "M",
				Email = "kaue@hotmail.com",
				Enderecos = new List<Endereco> { new Endereco() { Logradouro = "Rua José Primola, 470", Cep = "13873051",
																  Bairro = "Morumbi", Cidade = "São João da Boa Vista", Estado = "SP",
																  Pais = "Brasil" } },
				Telefones = new List<Telefone>() { new Telefone() { DDI = "55", DDD = "12", Numero = "77777777" } }
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Usuario", usuario);
				var created = await postResponse.Content.ReadAsJsonAsync<Usuario>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Usuario/" + created.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Usuario/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Usuario>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				usuario.Nome.Should().BeEquivalentTo(created.Nome);
				created.Id.Should().NotBe(Guid.Empty);

				all.Select(x => x.Id).Should().NotContain(usuario.Id);
			}
		}
	}
}