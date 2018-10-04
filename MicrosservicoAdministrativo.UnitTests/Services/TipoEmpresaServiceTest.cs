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
	public class TipoEmpresaServiceTest
	{
		private Mock<IRepository<TipoEmpresa, string>> _repositoryMock;
		private TipoEmpresaService _service;

		public TipoEmpresaServiceTest()
		{
			_repositoryMock = new Mock<IRepository<TipoEmpresa, string>>();

			SetupInitialize();

			_service = new TipoEmpresaService(_repositoryMock.Object);
		}

		[Fact]
		public void TipoEmpresaServiceTest_ListarTodos_RetornandoTiposDeEmpresaCadastrados()
		{
			var result = _service.ListarTodos();

			var items = result.Should().BeAssignableTo<IEnumerable<TipoEmpresa>>().Subject;

			items.Count().Should().Be(3);
			_repositoryMock.Verify(x => x.GetItemsFromCollectionAsync(), Times.Once());
		}

		[Fact]
		public void TipoEmpresaServiceTest_ObterPorId_RetornandoTipoDeEmpresaEspecifico()
		{
			var result = _service.ObterPorId(new Guid("25ea4cc6-fd75-4443-acf4-0145481d8d30"));

			var item = result.Should().BeAssignableTo<TipoEmpresa>().Subject;

			item.Descricao.Should().Be("Parceiro");
			_repositoryMock.Verify(x => x.GetItemFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaServiceTest_Incluir_RetornandoTipoDeEmpresaInserido()
		{
			var result = _service.Incluir(new TipoEmpresa
			{
				Id = new Guid("bba1028c-ac9e-4e2b-a318-b284f5d5812b"),
				Descricao = "Concorrente"
			});

			var item = result.Should().BeAssignableTo<TipoEmpresa>().Subject;

			item.Descricao.Should().Be("Concorrente");
			_repositoryMock.Verify(x => x.AddDocumentIntoCollectionAsync(It.IsAny<TipoEmpresa>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaServiceTest_Alterar_RetornandoTipoDeEmpresaAtualizado()
		{
			var result = _service.Alterar(new Guid("f4133133-112c-41a0-84d5-b3f05fc12af7"), new TipoEmpresa
			{
				Id = new Guid("f4133133-112c-41a0-84d5-b3f05fc12af7"),
				Descricao = "Filial"
			});

			var item = result.Should().BeAssignableTo<TipoEmpresa>().Subject;

			item.Descricao.Should().Be("Filial");
			_repositoryMock.Verify(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<TipoEmpresa>()), Times.Once());
		}

		[Fact]
		public void TipoEmpresaServiceTest_Excluir()
		{
			_service.Excluir(It.IsAny<Guid>());

			_repositoryMock.Verify(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_repositoryMock.Setup(x => x.GetItemsFromCollectionAsync()).Returns(Task.FromResult<IEnumerable<TipoEmpresa>>(
				new List<TipoEmpresa>
				{
					new TipoEmpresa() { Id = new Guid("25ea4cc6-fd75-4443-acf4-0145481d8d30"), Descricao = "Parceiro" },
					new TipoEmpresa() { Id = new Guid("bba1028c-ac9e-4e2b-a318-b284f5d5812b"), Descricao = "Concorrente" },
					new TipoEmpresa() { Id = new Guid("f4133133-112c-41a0-84d5-b3f05fc12af7"), Descricao = "Filial" }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.GetItemFromCollectionAsync(It.IsAny<string>())).Returns(Task.FromResult(
				new TipoEmpresa
				{
					Id = new Guid("25ea4cc6-fd75-4443-acf4-0145481d8d30"),
					Descricao = "Parceiro"
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.AddDocumentIntoCollectionAsync(It.IsAny<TipoEmpresa>())).Returns(Task.FromResult(
				new TipoEmpresa
				{
					Id = new Guid("bba1028c-ac9e-4e2b-a318-b284f5d5812b"),
					Descricao = "Concorrente"
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<TipoEmpresa>())).Returns(Task.FromResult(
				new TipoEmpresa
				{
					Id = new Guid("f4133133-112c-41a0-84d5-b3f05fc12af7"),
					Descricao = "Filial"
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()))
				.Verifiable();
		}
	}
}