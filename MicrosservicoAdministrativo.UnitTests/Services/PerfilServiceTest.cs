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
	public class PerfilServiceTest
	{
		private Mock<IRepository<Perfil, string>> _repositoryMock;
		private PerfilService _service;

		public PerfilServiceTest()
		{
			_repositoryMock = new Mock<IRepository<Perfil, string>>();

			SetupInitialize();

			_service = new PerfilService(_repositoryMock.Object);
		}

		[Fact]
		public void PerfilServiceTest_ListarTodos_RetornandoPerfisCadastrados()
		{
			var result = _service.ListarTodos();

			var items = result.Should().BeAssignableTo<IEnumerable<Perfil>>().Subject;

			items.Count().Should().Be(3);
			_repositoryMock.Verify(x => x.GetItemsFromCollectionAsync(), Times.Once());
		}

		[Fact]
		public void PerfilServiceTest_ObterPorId_RetornandoPerfilEspecifico()
		{
			var result = _service.ObterPorId(new Guid("059c1079-f007-4980-81d3-b5bfd548f621"));

			var item = result.Should().BeAssignableTo<Perfil>().Subject;

			item.Descricao.Should().Be("Acesso Total");
			_repositoryMock.Verify(x => x.GetItemFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		[Fact]
		public void PerfilServiceTest_Incluir_RetornandoPerfilInserido()
		{
			var result = _service.Incluir(new Perfil
			{
				Id = new Guid("00f6ae9c-bec8-4016-bf33-c5c7fc80de4e"),
				Sigla = "CADUS",
				Descricao = "Cadastro de Usuários"
			});

			var item = result.Should().BeAssignableTo<Perfil>().Subject;

			item.Sigla.Should().Be("CADUS");
			_repositoryMock.Verify(x => x.AddDocumentIntoCollectionAsync(It.IsAny<Perfil>()), Times.Once());
		}

		[Fact]
		public void PerfilServiceTest_Alterar_RetornandoPerfilAtualizado()
		{
			var result = _service.Alterar(new Guid("03c949cf-bbb7-4c4f-bccb-9bdc43805c24"), new Perfil
			{
				Id = new Guid("03c949cf-bbb7-4c4f-bccb-9bdc43805c24"),
				Sigla = "CADEM",
				Descricao = "Cadastro de Empresas"
			});

			var item = result.Should().BeAssignableTo<Perfil>().Subject;

			item.Descricao.Should().Be("Cadastro de Empresas");
			_repositoryMock.Verify(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<Perfil>()), Times.Once());
		}

		[Fact]
		public void PerfilServiceTest_Excluir()
		{
			_service.Excluir(It.IsAny<Guid>());

			_repositoryMock.Verify(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_repositoryMock.Setup(x => x.GetItemsFromCollectionAsync()).Returns(Task.FromResult<IEnumerable<Perfil>>(
				new List<Perfil>
				{
					new Perfil { Id = new Guid("059c1079-f007-4980-81d3-b5bfd548f621"), Sigla = "TOTAL", Descricao = "Acesso Total" },
					new Perfil { Id = new Guid("00f6ae9c-bec8-4016-bf33-c5c7fc80de4e"), Sigla = "CADUS", Descricao = "Cadastro de Usuários" },
					new Perfil { Id = new Guid("03c949cf-bbb7-4c4f-bccb-9bdc43805c24"), Sigla = "CADEM", Descricao = "Cadastro de Empresas" }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.GetItemFromCollectionAsync(It.IsAny<string>())).Returns(Task.FromResult(
				new Perfil
				{
					Id = new Guid("059c1079-f007-4980-81d3-b5bfd548f621"),
					Sigla = "TOTAL",
					Descricao = "Acesso Total"
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.AddDocumentIntoCollectionAsync(It.IsAny<Perfil>())).Returns(Task.FromResult(
				new Perfil
				{
					Id = new Guid("00f6ae9c-bec8-4016-bf33-c5c7fc80de4e"),
					Sigla = "CADUS",
					Descricao = "Cadastro de Usuários"
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<Perfil>())).Returns(Task.FromResult(
				new Perfil
				{
					Id = new Guid("03c949cf-bbb7-4c4f-bccb-9bdc43805c24"),
					Sigla = "CADEM",
					Descricao = "Cadastro de Empresas"
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()))
				.Verifiable();
		}
	}
}