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
	public class PerfilUsuarioControllerTest
	{
		private Mock<IDistributedCache> _cacheMock;
		private Mock<IPerfilUsuarioService> _serviceMock;
		private PerfilUsuarioController _controller;

		public PerfilUsuarioControllerTest()
		{
			_cacheMock = new Mock<IDistributedCache>();
			_serviceMock = new Mock<IPerfilUsuarioService>();

			SetupInitialize();

			_controller = new PerfilUsuarioController(_cacheMock.Object, _serviceMock.Object);
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Listar_Retorna404()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => null);

			var result = _controller.Listar();

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Listar_PossuindoDados_Retorna200()
		{
			var result = _controller.Listar();

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Listar_PossuindoDados_Retorna200ComTodasAsAssociacoesDePerfisAUsuarios()
		{
			var result = _controller.Listar();

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var items = okResult.Value.Should().BeAssignableTo<IEnumerable<PerfilUsuario>>().Subject;

			items.Count().Should().Be(3);
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Listar_PassandoGuidDesconhecido_Retorna404()
		{
			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			var result = _controller.Listar(It.IsAny<Guid>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Listar_PassandoGuidExistente_Retorna200()
		{
			var result = _controller.Listar(new Guid("685e93ec-c4c7-4ae7-a4e4-25d8f36d2a5a"));

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Listar_PassandoGuidExistente_Retorna200ComAssociacaoDoPerfilAoUsuarioCorreta()
		{
			var guid = new Guid("685e93ec-c4c7-4ae7-a4e4-25d8f36d2a5a");
			var result = _controller.Listar(guid);

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var item = okResult.Value.Should().BeAssignableTo<PerfilUsuario>().Subject;

			item.Id.Should().Be(guid);
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Inserir_PassandoObjetoValido_Retorna201()
		{
			var result = _controller.Inserir(It.IsAny<PerfilUsuario>());

			result.Should().BeOfType<CreatedAtRouteResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<PerfilUsuario>()), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Inserir_PassandoObjetoValido_Retorna201ComAssociacaoDoPerfilAoUsuarioCriada()
		{
			var result = _controller.Inserir(It.IsAny<PerfilUsuario>()) as CreatedAtRouteResult;

			var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
			var item = createdAtRouteResult.Value.Should().BeAssignableTo<PerfilUsuario>().Subject;

			item.DataInclusao.Date.Should().Be(DateTime.Now.AddYears(-10).Date);
			_serviceMock.Verify(x => x.Incluir(It.IsAny<PerfilUsuario>()), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Excluir_PassandoGuidInexistente_Retorna404()
		{
			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			var result = _controller.Excluir(It.IsAny<Guid>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
			_serviceMock.Verify(x => x.Excluir(It.IsAny<Guid>()), Times.Never());
		}

		[Fact]
		public void PerfilUsuarioControllerTest_Excluir_PassandoGuidExistente_Retorna204()
		{
			var result = _controller.Excluir(It.IsAny<Guid>());

			result.Should().BeOfType<NoContentResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
			_serviceMock.Verify(x => x.Excluir(It.IsAny<Guid>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => new List<PerfilUsuario>
			{
				new PerfilUsuario {	Id = new Guid("685e93ec-c4c7-4ae7-a4e4-25d8f36d2a5a"), IdPerfil = Guid.NewGuid(), IdUsuario = Guid.NewGuid(),
									Ativo = true, DataInclusao = DateTime.Now },

				new PerfilUsuario { Id = new Guid("1de22750-bb69-4c7b-9b63-d50299dd4076"), IdPerfil = Guid.NewGuid(), IdUsuario = Guid.NewGuid(),
									Ativo = true, DataInclusao = DateTime.Now.AddDays(-5) },

				new PerfilUsuario { Id = new Guid("18c443be-d36c-4da2-99ce-87e3e3bccced"), IdPerfil = Guid.NewGuid(), IdUsuario = Guid.NewGuid(),
									Ativo = false, DataInclusao = DateTime.Now.AddYears(-10) }
			})
				.Verifiable();

			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => new PerfilUsuario
			{
				Id = new Guid("685e93ec-c4c7-4ae7-a4e4-25d8f36d2a5a"),
				IdPerfil = Guid.NewGuid(),
				IdUsuario = Guid.NewGuid(),
				Ativo = true,
				DataInclusao = DateTime.Now
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Incluir(It.IsAny<PerfilUsuario>())).Returns(() => new PerfilUsuario
			{
				Id = new Guid("18c443be-d36c-4da2-99ce-87e3e3bccced"),
				IdPerfil = Guid.NewGuid(),
				IdUsuario = Guid.NewGuid(),
				Ativo = false,
				DataInclusao = DateTime.Now.AddYears(-10)
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Excluir(It.IsAny<Guid>()))
				.Verifiable();
		}
	}
}