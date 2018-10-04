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
	public class EmpresaControllerIntegrationTest
	{
		private readonly TestServer _server;

		public EmpresaControllerIntegrationTest()
		{
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		}

		[Fact]
		public async void EmpresaControllerIntegrationTest_ConsigoPostEGetByIdEDelete()
		{
			var empresa = new Empresa
			{
				Id = Guid.NewGuid(),
				RazaoSocial = "Furnas Centrais Elétricas S.A",
				NomeFantasia = "Furnas",
				Cnpj = "67493040000193",
				Ativo = true,
				IdTipoEmpresa = Guid.NewGuid()
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Empresa", empresa);
				var created = await postResponse.Content.ReadAsJsonAsync<Empresa>();

				var getResponse = await client.GetAsync("/api/v1/Empresa/" + created.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Empresa>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Empresa/" + fetched.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Empresa/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Empresa>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				empresa.RazaoSocial.Should().BeEquivalentTo(created.RazaoSocial);
				empresa.RazaoSocial.Should().BeEquivalentTo(fetched.RazaoSocial);

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);

				all.Select(x => x.Id).Should().NotContain(empresa.Id);
			}
		}

		[Fact]
		public async void EmpresaControllerIntegrationTest_ConsigoPostEGetAllEDelete()
		{
			var empresa = new Empresa
			{
				Id = Guid.NewGuid(),
				RazaoSocial = "Petroleo Brasileiro SA",
				NomeFantasia = "Petrobras",
				Cnpj = "62516882000108",
				Ativo = true,
				IdTipoEmpresa = Guid.NewGuid()
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Empresa", empresa);
				var created = await postResponse.Content.ReadAsJsonAsync<Empresa>();

				var getAllResponse = await client.GetAsync("/api/v1/Empresa/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Empresa>>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Empresa/" + created.Id);
				var getAllForDeleteResponse = await client.GetAsync("/api/v1/Empresa/");
				var allForDelete = await getAllForDeleteResponse.Content.ReadAsJsonAsync<List<Empresa>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllForDeleteResponse.IsSuccessStatusCode.Should().BeTrue();

				empresa.RazaoSocial.Should().BeEquivalentTo(created.RazaoSocial);
				created.Id.Should().NotBe(Guid.Empty);

				all.Should().NotBeEmpty();
				created.Id.Should().Be(all.Single(x => x.Id == created.Id).Id);

				allForDelete.Select(x => x.Id).Should().NotContain(empresa.Id);
			}
		}

		[Fact]
		public async void EmpresaControllerIntegrationTest_NaoConsigoPostSemCampoObrigatorio()
		{
			var empresa = new Empresa
			{
				Id = Guid.NewGuid(),
				NomeFantasia = "Vale",
				Cnpj = "81832661000185",
				Ativo = true,
				IdTipoEmpresa = Guid.NewGuid()
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Empresa", empresa);
				postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			}
		}

		[Fact]
		public async void EmpresaControllerIntegrationTest_ConsigoPostEPutEGetByIdEDelete()
		{
			var empresa = new Empresa
			{
				Id = Guid.NewGuid(),
				RazaoSocial = "SBF Comércio de Produtos Esportivos LTDA",
				NomeFantasia = "Centauro",
				Cnpj = "62516882000108",
				Ativo = false,
				IdTipoEmpresa = Guid.NewGuid()
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Empresa", empresa);
				var created = await postResponse.Content.ReadAsJsonAsync<Empresa>();

				empresa.Id = created.Id;
				empresa.NomeFantasia = "Centauro Esportes";

				var putResponse = await client.PutAsJsonAsync("/api/v1/Empresa/" + created.Id, empresa);

				var getResponse = await client.GetAsync("/api/v1/Empresa/" + empresa.Id);
				var fetched = await getResponse.Content.ReadAsJsonAsync<Empresa>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Empresa/" + fetched.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Empresa/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Empresa>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				putResponse.IsSuccessStatusCode.Should().BeTrue();
				getResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				created.NomeFantasia.Should().BeEquivalentTo("Centauro");
				fetched.NomeFantasia.Should().BeEquivalentTo("Centauro Esportes");

				created.Id.Should().NotBe(Guid.Empty);
				created.Id.Should().Be(fetched.Id);

				all.Select(x => x.Id).Should().NotContain(empresa.Id);
			}
		}

		[Fact]
		public async void EmpresaControllerIntegrationTest_ConsigoPostEDelete()
		{
			var empresa = new Empresa
			{
				Id = Guid.NewGuid(),
				RazaoSocial = "Globo Comunicação e Participações S/A",
				NomeFantasia = "Rede Globo",
				Cnpj = "64540840000120",
				Ativo = true,
				IdTipoEmpresa = Guid.NewGuid()
			};

			using (var client = _server.CreateClient().AcceptJson())
			{
				var postResponse = await client.PostAsJsonAsync("/api/v1/Empresa", empresa);
				var created = await postResponse.Content.ReadAsJsonAsync<Empresa>();

				var deleteResponse = await client.DeleteAsync("/api/v1/Empresa/" + created.Id);
				var getAllResponse = await client.GetAsync("/api/v1/Empresa/");
				var all = await getAllResponse.Content.ReadAsJsonAsync<List<Empresa>>();

				postResponse.IsSuccessStatusCode.Should().BeTrue();
				deleteResponse.IsSuccessStatusCode.Should().BeTrue();
				getAllResponse.IsSuccessStatusCode.Should().BeTrue();

				empresa.RazaoSocial.Should().BeEquivalentTo(created.RazaoSocial);
				created.Id.Should().NotBe(Guid.Empty);

				all.Select(x => x.Id).Should().NotContain(empresa.Id);
			}
		}
	}
}