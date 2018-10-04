using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.V2.Controllers
{
	[Produces("application/json")]
	[ApiVersion("2.0")]
	[EnableCors("AllowAll")]
	[Route("api/v{api-version:apiVersion}/[controller]")]
	[ApiController]
	public class PerfilUsuarioController : ControllerBase
	{
		private readonly IDistributedCache _cache;
		private readonly IPerfilUsuarioService _perfilUsuarioService;

		private DistributedCacheEntryOptions _cacheOptions;

		public PerfilUsuarioController([FromServices] IDistributedCache cache, IPerfilUsuarioService service)
		{
			_cache = cache;
			_perfilUsuarioService = service;

			_cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
		}

		/// <summary>
		/// Lista todos os usuários com perfis associados
		/// </summary>
		/// <returns>Lista de usuários com perfis associados requerida.</returns>
		/// <response code="200">Lista de usuários com perfis associados retornada com sucesso.</response>
		/// <response code="404">Lista de usuários com perfis associados não existe.</response>
		[Benchmark, HttpGet]
		[ProducesResponseType(typeof(IEnumerable<PerfilUsuario>), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar()
		{
			IEnumerable<PerfilUsuario> associacoesPerfisUsuarios;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("ListaAssociacoesPerfisUsuarios") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("ListaAssociacoesPerfisUsuarios");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					associacoesPerfisUsuarios = _perfilUsuarioService.ListarTodos();
					_cache.SetString("ListaAssociacoesPerfisUsuarios", JsonConvert.SerializeObject(associacoesPerfisUsuarios), _cacheOptions);
				}
				else
				{
					associacoesPerfisUsuarios = JsonConvert.DeserializeObject<IEnumerable<PerfilUsuario>>(valorEmCache);
				}
			}
			else
			{
				associacoesPerfisUsuarios = _perfilUsuarioService.ListarTodos();
			}

			if (associacoesPerfisUsuarios == null)
				return NotFound();

			return Ok(associacoesPerfisUsuarios);
		}

		/// <summary>
		/// Lista as informações de um perfil específico do usuário através do seu Id
		/// </summary>
		/// <param name="id">Id da associação perfil-usuário</param>
		/// <returns>Perfil associado ao usuário requerido.</returns>
		/// <response code="200">Perfil associado ao usuário retornado com sucesso.</response>
		/// <response code="404">Perfil associado ao usuário não existe.</response>
		[Benchmark, HttpGet("{id}", Name = "InfoPerfilUsuario")]
		[ProducesResponseType(typeof(PerfilUsuario), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar([FromRoute] Guid id)
		{
			PerfilUsuario associacaoPerfilUsuario;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("PerfilUsuario") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("PerfilUsuario");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					associacaoPerfilUsuario = _perfilUsuarioService.ObterPorId(id);
					_cache.SetString("PerfilUsuario", JsonConvert.SerializeObject(associacaoPerfilUsuario), _cacheOptions);
				}
				else
				{
					associacaoPerfilUsuario = JsonConvert.DeserializeObject<PerfilUsuario>(valorEmCache);
				}
			}
			else
			{
				associacaoPerfilUsuario = _perfilUsuarioService.ObterPorId(id);
			}

			if (associacaoPerfilUsuario == null)
				return NotFound();

			return Ok(associacaoPerfilUsuario);
		}

		/// <summary>
		/// Associa um perfil à um usuário na base
		/// </summary>
		/// <param name="novaAssociacao">Perfil a ser associado ao usuário</param>
		/// <returns>Associação de perfil ao usuário criada.</returns>
		/// <response code="201">Perfil associado ao usuário com sucesso.</response>
		[Benchmark, HttpPost]
		[ProducesResponseType(typeof(PerfilUsuario), 201)]
		public IActionResult Inserir([FromBody] PerfilUsuario novaAssociacao)
		{
			var associacao = _perfilUsuarioService.Incluir(novaAssociacao);

			return CreatedAtRoute("InfoPerfilUsuario", new { id = associacao.Id }, associacao);
		}

		/// <summary>
		/// Deassocia um perfil de um usuário na base
		/// </summary>
		/// <param name="id">Id do perfil a ser desassociado do usuário</param>
		/// <returns>Perfil desassociado do usuário.</returns>
		/// <response code="204">Perfil desassociado do usuário com sucesso.</response>
		/// <response code="404">Perfil associado ao usuário não existe.</response>
		[Benchmark, HttpDelete("{id}")]
		[ProducesResponseType(typeof(PerfilUsuario), 204)]
		[ProducesResponseType(404)]
		public IActionResult Excluir([FromRoute] Guid id)
		{
			var associacao = _perfilUsuarioService.ObterPorId(id);

			if (associacao == null)
				return NotFound();

			_perfilUsuarioService.Excluir(id);

			return NoContent();
		}
	}
}