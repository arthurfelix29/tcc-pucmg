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
	public class TipoEmpresaControllerTest
	{
		private Mock<IDistributedCache> _cacheMock;
		private Mock<ITipoEmpresaService> _serviceMock;
		private TipoEmpresaController _controller;

		public TipoEmpresaControllerTest()
		{
			_cacheMock = new Mock<IDistributedCache>();
			_serviceMock = new Mock<ITipoEmpresaService>();

			SetupInitialize();

			_controller = new TipoEmpresaController(_cacheMock.Object, _serviceMock.Object);
		}

		[Fact]
		public void TipoEmpresaControllerTest_Listar_Retorna404()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => null);

			var result = _controller.Listar();

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Listar_PossuindoDados_Retorna200()
		{
			var result = _controller.Listar();

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Listar_PossuindoDados_Retorna200ComTodosOsTiposDeEmpresa()
		{
			var result = _controller.Listar();

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var items = okResult.Value.Should().BeAssignableTo<IEnumerable<TipoEmpresa>>().Subject;

			items.Count().Should().Be(3);
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Listar_PassandoGuidDesconhecido_Retorna404()
		{
			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			var result = _controller.Listar(It.IsAny<Guid>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Listar_PassandoGuidExistente_Retorna200()
		{
			var result = _controller.Listar(new Guid("25ea4cc6-fd75-4443-acf4-0145481d8d30"));

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Listar_PassandoGuidExistente_Retorna200ComTipoEmpresaCorreto()
		{
			var guid = new Guid("25ea4cc6-fd75-4443-acf4-0145481d8d30");
			var result = _controller.Listar(guid);

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var item = okResult.Value.Should().BeAssignableTo<TipoEmpresa>().Subject;

			item.Id.Should().Be(guid);
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Inserir_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("Descricao", "Required");

			var result = _controller.Inserir(It.IsAny<TipoEmpresa>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<TipoEmpresa>()), Times.Never());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Inserir_PassandoObjetoValido_Retorna201()
		{
			var result = _controller.Inserir(It.IsAny<TipoEmpresa>());

			result.Should().BeOfType<CreatedAtRouteResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<TipoEmpresa>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Inserir_PassandoObjetoValido_Retorna201ComTipoEmpresaCriado()
		{
			var result = _controller.Inserir(It.IsAny<TipoEmpresa>()) as CreatedAtRouteResult;

			var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
			var item = createdAtRouteResult.Value.Should().BeAssignableTo<TipoEmpresa>().Subject;

			item.Descricao.Should().Be("Concorrente");
			_serviceMock.Verify(x => x.Incluir(It.IsAny<TipoEmpresa>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Atualizar_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("Descricao", "Required");

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>()), Times.Never());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Atualizar_PassandoGuidInexistente_Retorna404()
		{
			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>())).Returns(() => null);

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaControllerTest_Atualizar_PassandoGuidExistente_Retorna204()
		{
			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>());

			result.Should().BeOfType<NoContentResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => new List<TipoEmpresa>
			{
				new TipoEmpresa() {	Id = new Guid("25ea4cc6-fd75-4443-acf4-0145481d8d30"), Descricao = "Parceiro" },
				new TipoEmpresa() { Id = new Guid("bba1028c-ac9e-4e2b-a318-b284f5d5812b"), Descricao = "Concorrente" },
				new TipoEmpresa() { Id = new Guid("f4133133-112c-41a0-84d5-b3f05fc12af7"), Descricao = "Filial"	}
			})
				.Verifiable();

			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => new TipoEmpresa
			{
				Id = new Guid("25ea4cc6-fd75-4443-acf4-0145481d8d30"),
				Descricao = "Parceiro"
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Incluir(It.IsAny<TipoEmpresa>())).Returns(() => new TipoEmpresa
			{
				Id = new Guid("bba1028c-ac9e-4e2b-a318-b284f5d5812b"),
				Descricao = "Concorrente"
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<TipoEmpresa>())).Returns(() => new TipoEmpresa
			{
				Id = new Guid("f4133133-112c-41a0-84d5-b3f05fc12af7"),
				Descricao = "Filial"
			})
				.Verifiable();
		}
	}
}