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
	public class UsuarioServiceTest
	{
		private Mock<IRepository<Usuario, string>> _repositoryMock;
		private UsuarioService _service;

		public UsuarioServiceTest()
		{
			_repositoryMock = new Mock<IRepository<Usuario, string>>();

			SetupInitialize();

			_service = new UsuarioService(_repositoryMock.Object);
		}

		[Fact]
		public void UsuarioServiceTest_ListarTodos_RetornandoUsuariosCadastrados()
		{
			var result = _service.ListarTodos();

			var items = result.Should().BeAssignableTo<IEnumerable<Usuario>>().Subject;

			items.Count().Should().Be(3);
			_repositoryMock.Verify(x => x.GetItemsFromCollectionAsync(), Times.Once());
		}

		[Fact]
		public void UsuarioServiceTest_ObterPorId_RetornandoUsuarioEspecifico()
		{
			var result = _service.ObterPorId(new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"));

			var item = result.Should().BeAssignableTo<Usuario>().Subject;

			item.Nome.Should().Be("Arthur Silva");
			_repositoryMock.Verify(x => x.GetItemFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		[Fact]
		public void UsuarioServiceTest_Incluir_RetornandoUsuarioInserido()
		{
			var result = _service.Incluir(new Usuario
			{
				Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
				Nome = "João Ferreira",
				Cpf = "51401335470",
				DataNascimento = new DateTime(1958, 2, 25),
				Sexo = "M",
				Email = "joao@hotmail.com",
				Enderecos =
					new List<Endereco>() { new Endereco() { Logradouro = "Rua 3", Complemento = string.Empty, Bairro = "São Bento",
															Cidade = "Belo Horizonte", Estado = "MG" } },
				Telefones =
					new List<Telefone>() { new Telefone() { DDD = "21", Numero = "88888888" },
											new Telefone() { DDD = "31", Numero = "666666666" } }
			});

			var item = result.Should().BeAssignableTo<Usuario>().Subject;

			item.Cpf.Should().Be("51401335470");
			_repositoryMock.Verify(x => x.AddDocumentIntoCollectionAsync(It.IsAny<Usuario>()), Times.Once());
		}

		[Fact]
		public void UsuarioServiceTest_Alterar_RetornandoUsuarioAtualizado()
		{
			var result = _service.Alterar(new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"), new Usuario
			{
				Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
				Nome = "Maria Oliveira",
				Cpf = "69483148219",
				DataNascimento = new DateTime(1994, 10, 5),
				Sexo = "F",
				Email = "moliveira@yahoo.com.br",
				Enderecos =
					new List<Endereco>() { new Endereco() { Logradouro = "Rua 5", Complemento = "Travessa", Bairro = "Venda Velha",
															Cidade = "Vitória", Estado = "ES" } }
			});

			var item = result.Should().BeAssignableTo<Usuario>().Subject;

			item.Email.Should().Be("moliveira@yahoo.com.br");
			_repositoryMock.Verify(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<Usuario>()), Times.Once());
		}

		[Fact]
		public void UsuarioServiceTest_Excluir()
		{
			_service.Excluir(It.IsAny<Guid>());

			_repositoryMock.Verify(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_repositoryMock.Setup(x => x.GetItemsFromCollectionAsync()).Returns(Task.FromResult<IEnumerable<Usuario>>(
				new List<Usuario>
				{
					new Usuario() { Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"), Nome = "Arthur Silva", Cpf = "84793732020",
									DataNascimento = new DateTime(1989, 8, 11), Sexo = "M", Email = "arthur@gmail.com",
									Enderecos =
										new List<Endereco> { new Endereco() { Logradouro = "Rua 1", Complemento = "Fundos", Bairro = "Santa Cruz",
																			  Cidade = "Rio de Janeiro", Estado = "RJ" },
															 new Endereco() { Logradouro = "Rua 2", Complemento = "Apto 601", Bairro = "Morumbi",
																			  Cidade = "São Paulo", Estado = "SP" } },
									Telefones =
										new List<Telefone>() { new Telefone() { DDD = "21", Numero = "99999999" } }
							  },
					new Usuario() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Nome = "João Ferreira", Cpf = "51401335470",
									DataNascimento = new DateTime(1958, 2, 25), Sexo = "M", Email = "joao@hotmail.com",
									Enderecos =
										new List<Endereco>() { new Endereco() { Logradouro = "Rua 3", Complemento = string.Empty, Bairro = "São Bento",
																				Cidade = "Belo Horizonte", Estado = "MG" } },
									Telefones =
										new List<Telefone>() { new Telefone() { DDD = "21", Numero = "88888888" },
															   new Telefone() { DDD = "31", Numero = "666666666" } }
								  },
					new Usuario() { Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"), Nome = "Maria Oliveira", Cpf = "69483148219",
									DataNascimento = new DateTime(1994, 10, 5), Sexo = "F", Email = "moliveira@yahoo.com.br",
									Enderecos =
										new List<Endereco>() { new Endereco() { Logradouro = "Rua 5", Complemento = "Travessa", Bairro = "Venda Velha",
																				Cidade = "Vitória", Estado = "ES" } }
								  }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.GetItemFromCollectionAsync(It.IsAny<string>())).Returns(Task.FromResult(
				new Usuario
				{
					Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"),
					Nome = "Arthur Silva",
					Cpf = "84793732020",
					DataNascimento = new DateTime(1989, 8, 11),
					Sexo = "M",
					Email = "arthur@gmail.com",
					Enderecos =
						new List<Endereco> { new Endereco() { Logradouro = "Rua 1", Complemento = "Fundos", Bairro = "Santa Cruz",
																Cidade = "Rio de Janeiro", Estado = "RJ" },
												new Endereco() { Logradouro = "Rua 2", Complemento = "Apto 601", Bairro = "Morumbi",
																Cidade = "São Paulo", Estado = "SP" } },
					Telefones =
						new List<Telefone>() { new Telefone() { DDD = "21", Numero = "99999999" } }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.AddDocumentIntoCollectionAsync(It.IsAny<Usuario>())).Returns(Task.FromResult(
				new Usuario
				{
					Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
					Nome = "João Ferreira",
					Cpf = "51401335470",
					DataNascimento = new DateTime(1958, 2, 25),
					Sexo = "M",
					Email = "joao@hotmail.com",
					Enderecos =
						new List<Endereco>() { new Endereco() { Logradouro = "Rua 3", Complemento = string.Empty, Bairro = "São Bento",
																Cidade = "Belo Horizonte", Estado = "MG" } },
					Telefones =
						new List<Telefone>() { new Telefone() { DDD = "21", Numero = "88888888" },
												new Telefone() { DDD = "31", Numero = "666666666" } }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.UpdateDocumentFromCollection(It.IsAny<string>(), It.IsAny<Usuario>())).Returns(Task.FromResult(
				new Usuario
				{
					Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
					Nome = "Maria Oliveira",
					Cpf = "69483148219",
					DataNascimento = new DateTime(1994, 10, 5),
					Sexo = "F",
					Email = "moliveira@yahoo.com.br",
					Enderecos =
						new List<Endereco>() { new Endereco() { Logradouro = "Rua 5", Complemento = "Travessa", Bairro = "Venda Velha",
																Cidade = "Vitória", Estado = "ES" } }
				}))
				.Verifiable();

			_repositoryMock.Setup(x => x.DeleteDocumentFromCollectionAsync(It.IsAny<string>()))
				.Verifiable();
		}
	}
}