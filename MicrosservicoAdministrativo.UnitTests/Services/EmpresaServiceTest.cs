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
	public class EmpresaServiceTest
	{
		private Mock<IRepository<Empresa, string>> _repositoryMock;
		private EmpresaService _service;

		public EmpresaServiceTest()
		{
			_repositoryMock = new Mock<IRepository<Empresa, string>>();

			SetupInitialize();

			_service = new EmpresaService(_repositoryMock.Object);
		}

		[Fact]
		public void EmpresaServiceTest_ListarTodos_RetornandoEmpresasCadastradas()
		{
			var result = _service.ListarTodos();

			var items = result.Should().BeAssignableTo<IEnumerable<Empresa>>().Subject;

			items.Count().Should().Be(3);
			_repositoryMock.Verify(x => x.GetItemsFromCollectionAsync(), Times.Once());
		}

		[Fact]
		public void EmpresaServiceTest_ObterPorId_RetornandoEmpresaEspecifica()
		{
			var result = _service.ObterPorId(new Guid("a9f03762-b81a-4702-a39e-80d7e639acff"));

			var item = result.Should().BeAssignableTo<Empresa>().Subject;

			item.Cnpj.Should().Be("65266571000118");
			_repositoryMock.Verify(x => x.GetItemFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		[Fact]
		public void EmpresaServiceTest_Incluir_RetornandoEmpresaInserida()
		{
			var result = _service.Incluir(new Empresa
			{
				Id = new Guid("97127d82-0ffc-4797-9147-455187959023"),
				RazaoSocial = "Empresa 2 S/A",
				NomeFantasia = "Empresa B",
				Cnpj = "69156765000111",
				Ativo = false,
				IdTipoEmpresa = Guid.NewGuid()
			});

			var item = result.Should().BeAssignableTo<Empresa>().Subject;

			item.RazaoSocial.Should().Be("Empresa 2 S/A");
			_repositoryMock.Verify(x => x.AddDocumentIntoCollectionAsync(It.IsAny<Empresa>()), Times.Once());
		}

		[Fact]
		public void EmpresaServiceTest_Alterar_RetornandoEmpresaAtualizada()
		{
			var result = _service.Alterar(new Guid("c2bea2a0-7b1c-4917-8c36-beca5327b9e0"), new Empresa
			{
				Id = new Guid("c2bea2a0-7b1c-4917-8c36-beca5327b9e0"),
				RazaoSocial = "Empresa 4 S/A",
				NomeFantasia = "Empresa D",
				Cnpj = "42027937000109",
				Ativo = true,
				IdTipoEmpresa = Guid.NewGuid()
			});

			var item = result.Should().BeAssignableTo<Empresa>().Subject;

			item.RazaoSocial.Should().Be("Empresa 4 S/A");
			_repositoryMock.Verify(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<Empresa>()), Times.Once());
		}

		[Fact]
		public void EmpresaServiceTest_Excluir()
		{
			_service.Excluir(It.IsAny<Guid>());

			_repositoryMock.Verify(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_repositoryMock.Setup(x => x.GetItemsFromCollectionAsync()).Returns(Task.FromResult<IEnumerable<Empresa>>(
				new List<Empresa>
				{
					new Empresa { Id = new Guid("a9f03762-b81a-4702-a39e-80d7e639acff"), RazaoSocial = "Empresa 1 S/A", NomeFantasia = "Empresa A",
								  Cnpj = "65266571000118", Ativo = true,
								  IdTipoEmpresa = Guid.NewGuid() },

					new Empresa { Id = new Guid("97127d82-0ffc-4797-9147-455187959023"), RazaoSocial = "Empresa 2 S/A", NomeFantasia = "Empresa B",
								  Cnpj = "69156765000111", Ativo = false,
								  IdTipoEmpresa = Guid.NewGuid() },

					new Empresa { Id = new Guid("c2bea2a0-7b1c-4917-8c36-beca5327b9e0"), RazaoSocial = "Empresa 4 S/A", NomeFantasia = "Empresa D",
								  Cnpj = "42027937000109", Ativo = true,
								  IdTipoEmpresa = Guid.NewGuid() }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.GetItemFromCollectionAsync(It.IsAny<string>())).Returns(Task.FromResult(
				new Empresa
				{
					Id = new Guid("a9f03762-b81a-4702-a39e-80d7e639acff"),
					RazaoSocial = "Empresa 1 S/A",
					NomeFantasia = "Empresa A",
					Cnpj = "65266571000118",
					Ativo = true,
					IdTipoEmpresa = Guid.NewGuid()
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.AddDocumentIntoCollectionAsync(It.IsAny<Empresa>())).Returns(Task.FromResult(
				new Empresa
				{
					Id = new Guid("97127d82-0ffc-4797-9147-455187959023"),
					RazaoSocial = "Empresa 2 S/A",
					NomeFantasia = "Empresa B",
					Cnpj = "69156765000111",
					Ativo = false,
					IdTipoEmpresa = Guid.NewGuid()
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<Empresa>())).Returns(Task.FromResult(
				new Empresa
				{
					Id = new Guid("c2bea2a0-7b1c-4917-8c36-beca5327b9e0"),
					RazaoSocial = "Empresa 4 S/A",
					NomeFantasia = "Empresa D",
					Cnpj = "42027937000109",
					Ativo = true,
					IdTipoEmpresa = Guid.NewGuid()
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()))
				.Verifiable();
		}
	}
}