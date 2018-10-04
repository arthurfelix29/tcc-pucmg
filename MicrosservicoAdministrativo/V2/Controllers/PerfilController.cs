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
	public class PerfilController : ControllerBase
	{
		private readonly IDistributedCache _cache;
		private readonly IPerfilService _perfilService;

		private DistributedCacheEntryOptions _cacheOptions;

		public PerfilController([FromServices] IDistributedCache cache, IPerfilService service)
		{
			_cache = cache;
			_perfilService = service;

			_cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
		}

		/// <summary>
		/// Lista todos os perfis cadastrados na base
		/// </summary>
		/// <returns>Lista de perfis requerida.</returns>
		/// <response code="200">Lista de perfis retornada com sucesso.</response>
		/// <response code="404">Lista de perfis não existe.</response>
		[Benchmark, HttpGet]
		[ProducesResponseType(typeof(IEnumerable<Perfil>), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar()
		{
			IEnumerable<Perfil> perfis;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("ListaPerfis") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("ListaPerfis");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					perfis = _perfilService.ListarTodos();
					_cache.SetString("ListaPerfis", JsonConvert.SerializeObject(perfis), _cacheOptions);
				}
				else
				{
					perfis = JsonConvert.DeserializeObject<IEnumerable<Perfil>>(valorEmCache);
				}
			}
			else
			{
				perfis = _perfilService.ListarTodos();
			}

			if (perfis == null)
				return NotFound();

			return Ok(perfis);
		}

		/// <summary>
		/// Lista as informações de um perfil através do seu Id
		/// </summary>
		/// <param name="id">Id do perfil</param>
		/// <returns>Perfil requerido.</returns>
		/// <response code="200">Perfil retornado com sucesso.</response>
		/// <response code="404">Perfil não existe.</response>
		[Benchmark, HttpGet("{id}", Name = "InfoPerfil")]
		[ProducesResponseType(typeof(Perfil), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar([FromRoute] Guid id)
		{
			Perfil perfil;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("Perfil") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("Perfil");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					perfil = _perfilService.ObterPorId(id);
					_cache.SetString("Perfil", JsonConvert.SerializeObject(perfil), _cacheOptions);
				}
				else
				{
					perfil = JsonConvert.DeserializeObject<Perfil>(valorEmCache);
				}
			}
			else
			{
				perfil = _perfilService.ObterPorId(id);
			}

			if (perfil == null)
				return NotFound();

			return Ok(perfil);
		}

		/// <summary>
		/// Insere um perfil na base
		/// </summary>
		/// <param name="novoPerfil">Perfil a ser inserido</param>
		/// <returns>Perfil criado.</returns>
		/// <response code="201">Perfil criado com sucesso.</response>
		/// <response code="400">Perfil é inválido.</response>
		[Benchmark, HttpPost]
		[ProducesResponseType(typeof(Perfil), 201)]
		[ProducesResponseType(400)]
		public IActionResult Inserir([FromBody] Perfil novoPerfil)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var perfil = _perfilService.Incluir(novoPerfil);

			return CreatedAtRoute("InfoPerfil", new { id = perfil.Id }, perfil);
		}

		/// <summary>
		/// Atualiza as informações de um perfil através do seu Id
		/// </summary>
		/// <param name="id">Id do perfil</param>
		/// <param name="perfilASerAlterado">Informações do perfil a serem atualizadas</param>
		/// <returns>Perfil atualizado.</returns>
		/// <response code="204">Perfil atualizado com sucesso.</response>
		/// <response code="404">Perfil não existe.</response>
		[Benchmark, HttpPut("{id}")]
		[ProducesResponseType(typeof(Perfil), 204)]
		[ProducesResponseType(404)]
		public IActionResult Atualizar([FromRoute] Guid id, [FromBody] Perfil perfilASerAlterado)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var perfil = _perfilService.Alterar(id, perfilASerAlterado);

			if (perfil == null)
				return NotFound();

			return NoContent();
		}
	}
}