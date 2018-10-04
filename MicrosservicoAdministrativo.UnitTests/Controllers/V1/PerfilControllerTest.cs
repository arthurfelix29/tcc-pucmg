using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.V1.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace MicrosservicoAdministrativo.UnitTests.Controllers.V1
{
	[ExcludeFromCodeCoverage]
	public class PerfilControllerTest
	{
		private Mock<IDistributedCache> _cacheMock;
		private Mock<IPerfilService> _serviceMock;
		private PerfilController _controller;

		public PerfilControllerTest()
		{
			_cacheMock = new Mock<IDistributedCache>();
			_serviceMock = new Mock<IPerfilService>();

			SetupInitialize();

			_controller = new PerfilController(_cacheMock.Object, _serviceMock.Object);
		}

		[Fact]
		public void PerfilControllerTest_Listar_Retorna404()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => null);

			var result = _controller.Listar();

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Listar_PossuindoDados_Retorna200()
		{
			var result = _controller.Listar();

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Listar_PossuindoDados_Retorna200ComTodosOsPerfis()
		{
			var result = _controller.Listar();

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var items = okResult.Value.Should().BeAssignableTo<IEnumerable<Perfil>>().Subject;

			items.Count().Should().Be(3);
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Listar_PassandoGuidDesconhecido_Retorna404()
		{
			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			var result = _controller.Listar(It.IsAny<Guid>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Listar_PassandoGuidExistente_Retorna200()
		{
			var result = _controller.Listar(new Guid("059c1079-f007-4980-81d3-b5bfd548f621"));

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Listar_PassandoGuidExistente_Retorna200ComPerfilCorreto()
		{
			var guid = new Guid("059c1079-f007-4980-81d3-b5bfd548f621");
			var result = _controller.Listar(guid);

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var item = okResult.Value.Should().BeAssignableTo<Perfil>().Subject;

			item.Id.Should().Be(guid);
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Inserir_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("Sigla", "Required");

			var result = _controller.Inserir(It.IsAny<Perfil>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Perfil>()), Times.Never());
		}

		[Fact]
		public void PerfilControllerTest_Inserir_PassandoObjetoValido_Retorna201()
		{
			var result = _controller.Inserir(It.IsAny<Perfil>());

			result.Should().BeOfType<CreatedAtRouteResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Perfil>()), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Inserir_PassandoObjetoValido_Retorna201ComPerfilCriado()
		{
			var result = _controller.Inserir(It.IsAny<Perfil>()) as CreatedAtRouteResult;

			var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
			var item = createdAtRouteResult.Value.Should().BeAssignableTo<Perfil>().Subject;

			item.Sigla.Should().Be("CADUS");
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Perfil>()), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Atualizar_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("Sigla", "Required");

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Perfil>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Perfil>()), Times.Never());
		}

		[Fact]
		public void PerfilControllerTest_Atualizar_PassandoGuidInexistente_Retorna404()
		{
			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Perfil>())).Returns(() => null);

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Perfil>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Perfil>()), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Atualizar_PassandoGuidExistente_Retorna204()
		{
			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Perfil>());

			result.Should().BeOfType<NoContentResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Perfil>()), Times.Once());
		}

		[Fact]
		public void PerfilControllerTest_Excluir_PassandoGuidInexistente_Retorna404()
		{
			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			var result = _controller.Excluir(It.IsAny<Guid>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
			_serviceMock.Verify(x => x.Excluir(It.IsAny<Guid>()), Times.Never());
		}

		[Fact]
		public void PerfilControllerTest_Excluir_PassandoGuidExistente_Retorna204()
		{
			var result = _controller.Excluir(It.IsAny<Guid>());

			result.Should().BeOfType<NoContentResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
			_serviceMock.Verify(x => x.Excluir(It.IsAny<Guid>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => new List<Perfil>
			{
				new Perfil { Id = new Guid("059c1079-f007-4980-81d3-b5bfd548f621"), Sigla = "TOTAL", Descricao = "Acesso Total" },
				new Perfil { Id = new Guid("00f6ae9c-bec8-4016-bf33-c5c7fc80de4e"), Sigla = "CADUS", Descricao = "Cadastro de Usuários" },
				new Perfil { Id = new Guid("03c949cf-bbb7-4c4f-bccb-9bdc43805c24"), Sigla = "CADEM", Descricao = "Cadastro de Empresas" }
			})
				.Verifiable();

			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => new Perfil
			{
				Id = new Guid("059c1079-f007-4980-81d3-b5bfd548f621"),
				Sigla = "TOTAL",
				Descricao = "Acesso Total"
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Incluir(It.IsAny<Perfil>())).Returns(() => new Perfil
			{
				Id = new Guid("00f6ae9c-bec8-4016-bf33-c5c7fc80de4e"),
				Sigla = "CADUS",
				Descricao = "Cadastro de Usuários"
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Perfil>())).Returns(() => new Perfil
			{
				Id = new Guid("03c949cf-bbb7-4c4f-bccb-9bdc43805c24"),
				Sigla = "CADEM",
				Descricao = "Cadastro de Empresas"
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Excluir(It.IsAny<Guid>()))
				.Verifiable();
		}
	}
}