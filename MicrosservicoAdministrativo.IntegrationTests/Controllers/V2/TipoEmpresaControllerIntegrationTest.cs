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
	public class TipoEmpresaControllerIntegrationTest
	{
		private readonly TestServer _server;

		public TipoEmpresaControllerIntegrationTest()
		{
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		}

		[Fact]
		public async void TipoEmpresaControllerIntegrationTest_ConsigoPostEGetById()
		{
			var tipoEmpresa = new TipoEmpresa
			{
				Id = Guid.NewGuid(),
				Descricao = "Parceiro"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/TipoEmpresa", tipoEmpresa);
				var created = await postResponse.Content.ReadAsJsonAsync<TipoEmpresa>();

				var getResponse = await client.GetAsync("/api/v2/TipoEmpresa/" + created.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<TipoEmpresa>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();

				tipoEmpresa.Descricao.Should().BeEquivalentTo(created.Descricao);
				tipoEmpresa.Descricao.Should().BeEquivalentTo(fetched.Descricao);

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);
			}
		}

		[Fact]
		public async void TipoEmpresaControllerIntegrationTest_ConsigoPostEGetAll()
		{
			var tipoEmpresa = new TipoEmpresa
			{
				Id = Guid.NewGuid(),
				Descricao = "Filial"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/TipoEmpresa", tipoEmpresa);
				var created = await postResponse.Content.ReadAsJsonAsync<TipoEmpresa>();

				var getAllResponse = await client.GetAsync("/api/v2/TipoEmpresa/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<TipoEmpresa>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				tipoEmpresa.Descricao.Should().BeEquivalentTo(created.Descricao);
				created.Id.Should().NotBe(Guid.Empty);

				all.Should().NotBeEmpty();
				created.Id.Should().Be(all.Single(x => x.Id == created.Id).Id);
			}
		}

		[Fact]
		public async void TipoEmpresaControllerIntegrationTest_NaoConsigoPostSemCampoObrigatorio()
		{
			var tipoEmpresa = new TipoEmpresa
			{
				Id = Guid.NewGuid()
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/TipoEmpresa", tipoEmpresa);
				postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			}
		}

		[Fact]
		public async void TipoEmpresaControllerIntegrationTest_ConsigoPostEPutEGetById()
		{
			var tipoEmpresa = new TipoEmpresa
			{
				Id = Guid.NewGuid(),
				Descricao = "Concorrente"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/TipoEmpresa", tipoEmpresa);
				var created = await postResponse.Content.ReadAsJsonAsync<TipoEmpresa>();

				tipoEmpresa.Id = created.Id;
				tipoEmpresa.Descricao = "Concorrência";

				var putResponse = await client.PutAsJsonAsync("/api/v2/TipoEmpresa/" + created.Id, tipoEmpresa);

				var getResponse = await client.GetAsync("/api/v2/TipoEmpresa/" + tipoEmpresa.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<TipoEmpresa>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				putResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();

				created.Descricao.Should().BeEquivalentTo("Concorrente");
				fetched.Descricao.Should().BeEquivalentTo("Concorrência");

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);
			}
		}

		[Fact]
		public async void TipoEmpresaControllerIntegrationTest_ConsigoPost()
		{
			var tipoEmpresa = new TipoEmpresa
			{
				Id = Guid.NewGuid(),
				Descricao = "Concorrente"
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v2/TipoEmpresa", tipoEmpresa);
				var created = await postResponse.Content.ReadAsJsonAsync<TipoEmpresa>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();

				tipoEmpresa.Descricao.Should().BeEquivalentTo(created.Descricao);
				created.Id.Should().NotBe(Guid.Empty);
			}
		}
	}
}