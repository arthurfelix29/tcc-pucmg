using FluentAssertions;
using MicrosservicoAdministrativo.Core.Services;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MicrosservicoAdministrativo.UnitTests.Services
{
	[ExcludeFromCodeCoverage]
	public class PerfilUsuarioServiceTest
	{
		private Mock<IRepository<PerfilUsuario, string>> _repositoryMock;
		private PerfilUsuarioService _service;

		public PerfilUsuarioServiceTest()
		{
			_repositoryMock = new Mock<IRepository<PerfilUsuario, string>>();

			SetupInitialize();

			_service = new PerfilUsuarioService(_repositoryMock.Object);
		}

		[Fact]
		public void PerfilUsuarioServiceTest_ListarTodos_RetornandoAssociacoesDePerfisAUsuariosCadastradas()
		{
			var result = _service.ListarTodos();

			var items = result.Should().BeAssignableTo<IEnumerable<PerfilUsuario>>().Subject;

			items.Count().Should().Be(3);
			_repositoryMock.Verify(x => x.GetItemsFromCollectionAsync(), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioServiceTest_ObterPorId_RetornandoAssociacaoDePerfilAUsuarioEspecifica()
		{
			var result = _service.ObterPorId(new Guid("685e93ec-c4c7-4ae7-a4e4-25d8f36d2a5a"));

			var item = result.Should().BeAssignableTo<PerfilUsuario>().Subject;

			item.DataInclusao.Should().Be(DateTime.Now.Date);
			_repositoryMock.Verify(x => x.GetItemFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioServiceTest_Incluir_RetornandoAssociacaoDePerfilAUsuarioInserida()
		{
			var result = _service.Incluir(new PerfilUsuario
			{
				Id = new Guid("18c443be-d36c-4da2-99ce-87e3e3bccced"),
				IdPerfil = Guid.NewGuid(),
				IdUsuario = Guid.NewGuid(),
				Ativo = false,
				DataInclusao = DateTime.Now.Date.AddYears(-10)
			});

			var item = result.Should().BeAssignableTo<PerfilUsuario>().Subject;

			item.DataInclusao.Should().Be(DateTime.Now.Date.AddYears(-10));
			_repositoryMock.Verify(x => x.AddDocumentIntoCollectionAsync(It.IsAny<PerfilUsuario>()), Times.Once());
		}

		[Fact]
		public void PerfilUsuarioServiceTest_Excluir()
		{
			_service.Excluir(It.IsAny<Guid>());

			_repositoryMock.Verify(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_repositoryMock.Setup(x => x.GetItemsFromCollectionAsync()).Returns(Task.FromResult<IEnumerable<PerfilUsuario>>(
				new List<PerfilUsuario>
				{
					new PerfilUsuario { Id = new Guid("685e93ec-c4c7-4ae7-a4e4-25d8f36d2a5a"), IdPerfil = Guid.NewGuid(),
										IdUsuario = Guid.NewGuid(), Ativo = true, DataInclusao = DateTime.Now },

					new PerfilUsuario { Id = new Guid("1de22750-bb69-4c7b-9b63-d50299dd4076"), IdPerfil = Guid.NewGuid(),
										IdUsuario = Guid.NewGuid(), Ativo = true, DataInclusao = DateTime.Now.AddDays(-5) },

					new PerfilUsuario { Id = new Guid("18c443be-d36c-4da2-99ce-87e3e3bccced"), IdPerfil = Guid.NewGuid(),
										IdUsuario = Guid.NewGuid(), Ativo = false, DataInclusao = DateTime.Now.AddYears(-10) }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.GetItemFromCollectionAsync(It.IsAny<string>())).Returns(Task.FromResult(
				new PerfilUsuario
				{
					Id = new Guid("685e93ec-c4c7-4ae7-a4e4-25d8f36d2a5a"),
					IdPerfil = Guid.NewGuid(),
					IdUsuario = Guid.NewGuid(),
					Ativo = true,
					DataInclusao = DateTime.Now.Date
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.AddDocumentIntoCollectionAsync(It.IsAny<PerfilUsuario>())).Returns(Task.FromResult(
				new PerfilUsuario
				{
					Id = new Guid("18c443be-d36c-4da2-99ce-87e3e3bccced"),
					IdPerfil = Guid.NewGuid(),
					IdUsuario = Guid.NewGuid(),
					Ativo = false,
					DataInclusao = DateTime.Now.Date.AddYears(-10)
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()))
				.Verifiable();
		}
	}
}