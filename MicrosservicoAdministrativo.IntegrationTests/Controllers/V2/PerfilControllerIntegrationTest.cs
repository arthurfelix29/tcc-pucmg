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

namespace MicrosservicoAdministrativo.IntegrationTests.Controllers.V2
{
	[ExcludeFromCodeCoverage]
	public class PerfilControllerIntegrationTest
	{
		private readonly TestServer _server;

		public PerfilControllerIntegrationTest()
		{
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPostEGetById()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "VISPER",
				Descricao = "Visualizar Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				var getResponse = await client.GetAsync("/api/v2/Perfil/" + created.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Perfil>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();

				perfil.Descricao.Should().BeEquivalentTo(created.Descricao);
				perfil.Descricao.Should().BeEquivalentTo(fetched.Descricao);

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);
			}
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPostEGetAll()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "CADPER",
				Descricao = "Cadastrar Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				var getAllResponse = await client.GetAsync("/api/v2/Perfil/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Perfil>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				perfil.Descricao.Should().BeEquivalentTo(created.Descricao);
				created.Id.Should().NotBe(Guid.Empty);

				all.Should().NotBeEmpty();
				created.Id.Should().Be(all.Single(x => x.Id == created.Id).Id);
			}
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_NaoConsigoPostSemCampoObrigatorio()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Descricao = "Excluir Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/Perfil", perfil);
				postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			}
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPostEPutEGetById()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "EXCPER",
				Descricao = "Excluir Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				perfil.Id = created.Id;
				perfil.Descricao = "Excluir Perfis Associados";

				var putResponse = await client.PutAsJsonAsync("/api/v2/Perfil/" + created.Id, perfil);

				var getResponse = await client.GetAsync("/api/v2/Perfil/" + perfil.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Perfil>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				putResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();

				created.Descricao.Should().BeEquivalentTo("Excluir Perfis");
				fetched.Descricao.Should().BeEquivalentTo("Excluir Perfis Associados");

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);
			}
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPost()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "ATUPER",
				Descricao = "Atualizar Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();

				perfil.Descricao.Should().BeEquivalentTo(created.Descricao);
				created.Id.Should().NotBe(Guid.Empty);
			}
		}
	}
}