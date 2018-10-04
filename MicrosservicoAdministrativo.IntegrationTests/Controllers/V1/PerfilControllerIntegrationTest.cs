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
	public class PerfilControllerIntegrationTest
	{
		private readonly TestServer _server;

		public PerfilControllerIntegrationTest()
		{
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPostEGetByIdEDelete()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "VISPER",
				Descricao = "Visualizar Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				var getResponse = await client.GetAsync("/api/v1/Perfil/" + created.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Perfil>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Perfil/" + fetched.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Perfil/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Perfil>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				perfil.Descricao.Should().BeEquivalentTo(created.Descricao);
				perfil.Descricao.Should().BeEquivalentTo(fetched.Descricao);

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);

				all.Select(x => x.Id).Should().NotContain(perfil.Id);
			}
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPostEGetAllEDelete()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "CADPER",
				Descricao = "Cadastrar Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				var getAllResponse = await client.GetAsync("/api/v1/Perfil/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Perfil>>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Perfil/" + created.Id);
				var getAllForDeleteResponse = await client.GetAsync("/api/v1/Perfil/");
				var allForDelete = await getAllForDeleteResponse.Content.ReadAsJsonAsync<List<Perfil>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllForDeleteResponse.IsSuccessStatusCode.Should().BeTrue();

				perfil.Descricao.Should().BeEquivalentTo(created.Descricao);
				created.Id.Should().NotBe(Guid.Empty);

				all.Should().NotBeEmpty();
				created.Id.Should().Be(all.Single(x => x.Id == created.Id).Id);

				allForDelete.Select(x => x.Id).Should().NotContain(perfil.Id);
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
				var postResponse = await client.PostAsJsonAsync("/api/v1/Perfil", perfil);
				postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			}
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPostEPutEGetByIdEDelete()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "EXCPER",
				Descricao = "Excluir Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				perfil.Id = created.Id;
				perfil.Descricao = "Excluir Perfis Associados";

				var putResponse = await client.PutAsJsonAsync("/api/v1/Perfil/" + created.Id, perfil);

				var getResponse = await client.GetAsync("/api/v1/Perfil/" + perfil.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Perfil>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Perfil/" + fetched.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Perfil/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Perfil>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				putResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				created.Descricao.Should().BeEquivalentTo("Excluir Perfis");
				fetched.Descricao.Should().BeEquivalentTo("Excluir Perfis Associados");

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);

				all.Select(x => x.Id).Should().NotContain(perfil.Id);
			}
		}

		[Fact]
		public async void PerfilControllerIntegrationTest_ConsigoPostEDelete()
		{
			var perfil = new Perfil
			{
				Id = Guid.NewGuid(),
				Sigla = "ATUPER",
				Descricao = "Atualizar Perfis"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Perfil", perfil);
				var created = await postResponse.Content.ReadAsJsonAsync<Perfil>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Perfil/" + created.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Perfil/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Perfil>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				perfil.Descricao.Should().BeEquivalentTo(created.Descricao);
				created.Id.Should().NotBe(Guid.Empty);

				all.Select(x => x.Id).Should().NotContain(perfil.Id);
			}
		}
	}
}