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
	public class UsuarioControllerTest
	{
		private Mock<IDistributedCache> _cacheMock;
		private Mock<IUsuarioService> _serviceMock;
		private UsuarioController _controller;

		public UsuarioControllerTest()
		{
			_cacheMock = new Mock<IDistributedCache>();
			_serviceMock = new Mock<IUsuarioService>();

			SetupInitialize();

			_controller = new UsuarioController(_cacheMock.Object, _serviceMock.Object);
		}

		[Fact]
		public void UsuarioControllerTest_Listar_Retorna404()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => null);

			var result = _controller.Listar();

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Listar_PossuindoDados_Retorna200()
		{
			var result = _controller.Listar();

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Listar_PossuindoDados_Retorna200ComTodosOsUsuarios()
		{
			var result = _controller.Listar();

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var items = okResult.Value.Should().BeAssignableTo<IEnumerable<Usuario>>().Subject;

			items.Count().Should().Be(3);
			_serviceMock.Verify(x => x.ListarTodos(), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Listar_PassandoGuidDesconhecido_Retorna404()
		{
			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => null);

			var result = _controller.Listar(It.IsAny<Guid>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Listar_PassandoGuidExistente_Retorna200()
		{
			var result = _controller.Listar(new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"));

			result.Should().BeOfType<OkObjectResult>();
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Listar_PassandoGuidExistente_Retorna200ComUsuarioCorreto()
		{
			var guid = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
			var result = _controller.Listar(guid);

			var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
			var item = okResult.Value.Should().BeAssignableTo<Usuario>().Subject;

			item.Id.Should().Be(guid);
			_serviceMock.Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Inserir_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("Nome", "Required");

			var result = _controller.Inserir(It.IsAny<Usuario>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Usuario>()), Times.Never());
		}

		[Fact]
		public void UsuarioControllerTest_Inserir_PassandoObjetoValido_Retorna201()
		{
			var result = _controller.Inserir(It.IsAny<Usuario>());

			result.Should().BeOfType<CreatedAtRouteResult>();
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Usuario>()), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Inserir_PassandoObjetoValido_Retorna201ComUsuarioCriado()
		{
			var result = _controller.Inserir(It.IsAny<Usuario>()) as CreatedAtRouteResult;

			var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
			var item = createdAtRouteResult.Value.Should().BeAssignableTo<Usuario>().Subject;

			item.Cpf.Should().Be("51401335470");
			_serviceMock.Verify(x => x.Incluir(It.IsAny<Usuario>()), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Atualizar_PassandoObjetoInvalido_Retorna400()
		{
			_controller.ModelState.AddModelError("Nome", "Required");

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Usuario>());

			result.Should().BeOfType<BadRequestObjectResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Usuario>()), Times.Never());
		}

		[Fact]
		public void UsuarioControllerTest_Atualizar_PassandoGuidInexistente_Retorna404()
		{
			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Usuario>())).Returns(() => null);

			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Usuario>());

			result.Should().BeOfType<NotFoundResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Usuario>()), Times.Once());
		}

		[Fact]
		public void UsuarioControllerTest_Atualizar_PassandoGuidExistente_Retorna204()
		{
			var result = _controller.Atualizar(It.IsAny<Guid>(), It.IsAny<Usuario>());

			result.Should().BeOfType<NoContentResult>();
			_serviceMock.Verify(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Usuario>()), Times.Once());
		}

		private void SetupInitialize()
		{
			_serviceMock.Setup(x => x.ListarTodos()).Returns(() => new List<Usuario>
			{
				new Usuario() {	Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"), Nome = "Arthur Silva", Cpf = "84793732020",
								DataNascimento = new DateTime(1989, 8, 11),	Sexo = "M",	Email = "arthur@gmail.com",
								Enderecos = 
									new List<Endereco> { new Endereco() { Logradouro = "Rua 1",	Complemento = "Fundos", Cep = "22222222",
																		  Bairro = "Santa Cruz", Cidade = "Rio de Janeiro", Estado = "RJ",
																		  Pais = "Brasil" },
														 new Endereco()	{ Logradouro = "Rua 2",	Complemento = "Apto 601", Cep = "33333333",
																		  Bairro = "Morumbi", Cidade = "São Paulo",	Estado = "SP",
																		  Pais = "Brasil" } },
								Telefones = 
									new List<Telefone>() { new Telefone() {	DDI = "55", DDD = "21",	Numero = "99999999" } }
							  },
				new Usuario() {	Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Nome = "João Ferreira", Cpf = "51401335470",
								DataNascimento = new DateTime(1958, 2, 25),	Sexo = "M",	Email = "joao@hotmail.com",
								Enderecos = 
									new List<Endereco>() { new Endereco() {	Logradouro = "Rua 3", Cep = "99999999", Bairro = "São Bento",
																			Cidade = "Belo Horizonte", Estado = "MG", Pais = "Brasil" } },
								Telefones = 
									new List<Telefone>() { new Telefone() { DDI = "55", DDD = "21",	Numero = "88888888"	},
														   new Telefone() { DDI = "55", DDD = "31",	Numero = "666666666" } }
							  },
				new Usuario() {	Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"), Nome = "Maria Oliveira", Cpf = "69483148219",
								DataNascimento = new DateTime(1994, 10, 5),	Sexo = "F",	Email = "moliveira@yahoo.com.br",
								Enderecos = 
									new List<Endereco>() { new Endereco() {	Logradouro = "Rua 5", Cep = "88888888", Complemento = "Travessa",
																			Bairro = "Venda Velha",	Cidade = "Vitória",	Estado = "ES",
																			Pais = "Brasil"} }
							  }
			})
				.Verifiable();

			_serviceMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).Returns(() => new Usuario
			{
				Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"),
				Nome = "Arthur Silva",
				Cpf = "84793732020",
				DataNascimento = new DateTime(1989, 8, 11),
				Sexo = "M",
				Email = "arthur@gmail.com",
				Enderecos =
					new List<Endereco> { new Endereco() { Logradouro = "Rua 1", Complemento = "Fundos", Cep = "22222222",
														  Bairro = "Santa Cruz", Cidade = "Rio de Janeiro", Estado = "RJ",
														  Pais = "Brasil" },
										 new Endereco() { Logradouro = "Rua 2", Complemento = "Apto 601", Cep = "33333333",
														  Bairro = "Morumbi", Cidade = "São Paulo", Estado = "SP",
														  Pais = "Brasil" } },
				Telefones =
					new List<Telefone>() { new Telefone() { DDD = "21", Numero = "99999999" } }
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Incluir(It.IsAny<Usuario>())).Returns(() => new Usuario
			{
				Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
				Nome = "João Ferreira",
				Cpf = "51401335470",
				DataNascimento = new DateTime(1958, 2, 25),
				Sexo = "M",
				Email = "joao@hotmail.com",
				Enderecos =
					new List<Endereco>() { new Endereco() { Logradouro = "Rua 3", Cep = "99999999", Bairro = "São Bento",
															Cidade = "Belo Horizonte", Estado = "MG", Pais = "Brasil" } },
				Telefones =
					new List<Telefone>() { new Telefone() { DDD = "21", Numero = "88888888" },
											new Telefone() { DDD = "31", Numero = "666666666" } }
			})
				.Verifiable();

			_serviceMock.Setup(x => x.Alterar(It.IsAny<Guid>(), It.IsAny<Usuario>())).Returns(() => new Usuario
			{
				Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad"),
				Nome = "Maria Oliveira",
				Cpf = "69483148219",
				DataNascimento = new DateTime(1994, 10, 5),
				Sexo = "F",
				Email = "moliveira@yahoo.com.br",
				Enderecos =
					new List<Endereco>() { new Endereco() { Logradouro = "Rua 5", Cep = "88888888", Complemento = "Travessa",
															Bairro = "Venda Velha", Cidade = "Vitória", Estado = "ES",
															Pais = "Brasil"} }
			})
				.Verifiable();
		}
	}
}