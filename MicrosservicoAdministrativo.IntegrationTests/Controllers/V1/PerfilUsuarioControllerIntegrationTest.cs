using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.IntegrationTests.Infra;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace MicrosservicoAdministrativo.IntegrationTests.Controllers.V1
{
	[ExcludeFromCodeCoverage]
	public class PerfilUsuarioControllerIntegrationTest
	{
		private readonly TestServer _server;

		public PerfilUsuarioControllerIntegrationTest()
		{
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		}

		[Fact]
		public async void PerfilUsuarioControllerIntegrationTest_ConsigoPostEGetByIdEDelete()
		{
			var perfilUsuario = new PerfilUsuario
			{
				Id = Guid.NewGuid(),
				IdPerfil = Guid.NewGuid(),
				IdUsuario = Guid.NewGuid(),
				Ativo = true,
				DataInclusao = new DateTime(2018, 5, 3)
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/PerfilUsuario", perfilUsuario);
				var created = await postResponse.Content.ReadAsJsonAsync<PerfilUsuario>();

				var getResponse = await client.GetAsync("/api/v1/PerfilUsuario/" + created.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<PerfilUsuario>();

				var deleteResponse = await client.DeleteAsync("/api/v1/PerfilUsuario/" + fetched.Id);
				var getAllResponse = await client.GetAsync("/api/v1/PerfilUsuario/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<PerfilUsuario>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				perfilUsuario.IdPerfil.Should().Be(created.IdPerfil);
				perfilUsuario.IdPerfil.Should().Be(fetched.IdPerfil);

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);

				all.Select(x => x.Id).Should().NotContain(perfilUsuario.Id);
			}
		}

		[Fact]
		public async void PerfilUsuarioControllerIntegrationTest_ConsigoPostEGetAllEDelete()
		{
			var perfilUsuario = new PerfilUsuario
			{
				Id = Guid.NewGuid(),
				IdPerfil = Guid.NewGuid(),
				IdUsuario = Guid.NewGuid(),
				Ativo = false,
				DataInclusao = new DateTime(2017, 8, 14)
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/PerfilUsuario", perfilUsuario);
				var created = await postResponse.Content.ReadAsJsonAsync<PerfilUsuario>();

				var getAllResponse = await client.GetAsync("/api/v1/PerfilUsuario/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<PerfilUsuario>>();

				var deleteResponse = await client.DeleteAsync("/api/v1/PerfilUsuario/" + created.Id);
				var getAllForDeleteResponse = await client.GetAsync("/api/v1/PerfilUsuario/");
				var allForDelete = await getAllForDeleteResponse.Content.ReadAsJsonAsync<List<PerfilUsuario>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllForDeleteResponse.IsSuccessStatusCode.Should().BeTrue();

				perfilUsuario.IdUsuario.Should().Be(created.IdUsuario);
				created.Id.Should().NotBe(Guid.Empty);

				all.Should().NotBeEmpty();
				created.Id.Should().Be(all.Single(x => x.Id == created.Id).Id);

				allForDelete.Select(x => x.Id).Should().NotContain(perfilUsuario.Id);
			}
		}

		[Fact]
		public async void PerfilUsuarioControllerIntegrationTest_ConsigoPostEDelete()
		{
			var perfilUsuario = new PerfilUsuario
			{
				Id = Guid.NewGuid(),
				IdPerfil = Guid.NewGuid(),
				IdUsuario = Guid.NewGuid(),
				Ativo = true,
				DataInclusao = new DateTime(2016, 1, 30)
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/PerfilUsuario", perfilUsuario);
				var created = await postResponse.Content.ReadAsJsonAsync<PerfilUsuario>();

				var deleteResponse = await client.DeleteAsync("/api/v1/PerfilUsuario/" + created.Id);
				var getAllResponse = await client.GetAsync("/api/v1/PerfilUsuario/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<PerfilUsuario>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				perfilUsuario.DataInclusao.Should().Be(created.DataInclusao);
				created.Id.Should().NotBe(Guid.Empty);

				all.Select(x => x.Id).Should().NotContain(perfilUsuario.Id);
			}
		}
	}
}