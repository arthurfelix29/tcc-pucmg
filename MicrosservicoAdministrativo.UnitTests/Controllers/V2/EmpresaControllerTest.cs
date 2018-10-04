using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.V2.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace MicrosservicoAdministrativo.UnitTests.Controllers.V2
{
	[ExcludeFromCodeCoverage]
	public class EmpresaControllerTest
	{
		private Mock<IDistributedCache> _cacheMock;
		private Mock<IEmpresaService> _serviceMock;
		private EmpresaController _controller;

		public EmpresaControllerTest()
		{
			_cacheMock = new Mock<IDistributedCache>();
			_serviceMock = new Mock<IEmpresaService>();

			SetupInitialize();

			_controller = new EmpresaController(_cacheMock.Object, _serviceMock.Object);
		}

		[Fact]
		public void EmpresaControllerTest_Listar_Retorna404()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => null);

			var result = _controller.Listar();

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Listar_PossuindoDados_Retorna200()
		{
			var result = _controller.Listar();

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Listar_PossuindoDados_Retorna200ComTodasAsEmpresas()
		{
			var result = _controller.Listar();

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var items = okResult.Value.Should().BeAssignableTo<IEnumerable<Empresa>>().Subject;

			items.Count().Should().Be(3);
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Listar_PassandoGuidDesconhecido_Retorna404()
		{
			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			var result = _controller.Listar(It.IsAny<Guid>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Listar_PassandoGuidExistente_Retorna200()
		{
			var result = _controller.Listar(new Guid("a9f03762-b81a-4702-a39e-80d7e639acff"));

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Listar_PassandoGuidExistente_Retorna200ComEmpresaCorreta()
		{
			var guid = new Guid("a9f03762-b81a-4702-a39e-80d7e639acff");
			var result = _controller.Listar(guid);

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var item = okResult.Value.Should().BeAssignableTo<Empresa>().Subject;

			item.Id.Should().Be(guid);
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Inserir_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("RazaoSocial", "Required");

			var result = _controller.Inserir(It.IsAny<Empresa>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Empresa>()), Times.Never());
		}

		[Fact]
		public void EmpresaControllerTest_Inserir_PassandoObjetoValido_Retorna201()
		{
			var result = _controller.Inserir(It.IsAny<Empresa>());

			result.Should().BeOfType<CreatedAtRouteResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Empresa>()), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Inserir_PassandoObjetoValido_Retorna201ComEmpresaCriada()
		{
			var result = _controller.Inserir(It.IsAny<Empresa>()) as CreatedAtRouteResult;

			var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
			var item = createdAtRouteResult.Value.Should().BeAssignableTo<Empresa>().Subject;

			item.RazaoSocial.Should().Be("Empresa 2 S/A");
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Empresa>()), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Atualizar_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("RazaoSocial", "Required");

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Empresa>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Empresa>()), Times.Never());
		}

		[Fact]
		public void EmpresaControllerTest_Atualizar_PassandoGuidInexistente_Retorna404()
		{
			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Empresa>())).Returns(() => null);

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Empresa>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Empresa>()), Times.Once());
		}

		[Fact]
		public void EmpresaControllerTest_Atualizar_PassandoGuidExistente_Retorna204()
		{
			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Empresa>());

			result.Should().BeOfType<NoContentResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Empresa>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => new List<Empresa>
			{
				new Empresa	{ Id = new Guid("a9f03762-b81a-4702-a39e-80d7e639acff"), RazaoSocial = "Empresa 1 S/A", NomeFantasia = "Empresa A",
							  Cnpj = "65266571000118", Ativo = true,
							  IdTipoEmpresa = new Guid("3d78f7a1-3714-4eac-a943-9747fd1a8938") },

				new Empresa	{ Id = new Guid("97127d82-0ffc-4797-9147-455187959023"), RazaoSocial = "Empresa 2 S/A",	NomeFantasia = "Empresa B",
							  Cnpj = "69156765000111", Ativo = false,
							  IdTipoEmpresa = new Guid("ce3b322d-0ac6-408f-92f7-119869c62e10") },

				new Empresa	{ Id = new Guid("c2bea2a0-7b1c-4917-8c36-beca5327b9e0"), RazaoSocial = "Empresa 4 S/A",	NomeFantasia = "Empresa D",
							  Cnpj = "42027937000109", Ativo = true,
							  IdTipoEmpresa = new Guid("2284ecf0-a037-47c6-b3fc-c699b756bd54") }
			})
				.Verifiable();

			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => new Empresa
			{
				Id = new Guid("a9f03762-b81a-4702-a39e-80d7e639acff"),
				RazaoSocial = "Empresa 1 S/A",
				NomeFantasia = "Empresa A",
				Cnpj = "65266571000118",
				Ativo = true,
				IdTipoEmpresa = new Guid("3d78f7a1-3714-4eac-a943-9747fd1a8938")
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Incluir(It.IsAny<Empresa>())).Returns(() => new Empresa
			{
				Id = new Guid("97127d82-0ffc-4797-9147-455187959023"),
				RazaoSocial = "Empresa 2 S/A",
				NomeFantasia = "Empresa B",
				Cnpj = "69156765000111",
				Ativo = false,
				IdTipoEmpresa = new Guid("ce3b322d-0ac6-408f-92f7-119869c62e10")
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Empresa>())).Returns(() => new Empresa
			{
				Id = new Guid("c2bea2a0-7b1c-4917-8c36-beca5327b9e0"),
				RazaoSocial = "Empresa 4 S/A",
				NomeFantasia = "Empresa D",
				Cnpj = "42027937000109",
				Ativo = true,
				IdTipoEmpresa = new Guid("3d78f7a1-3714-4eac-a943-9747fd1a8938")
			})
				.Verifiable();
		}
	}
}