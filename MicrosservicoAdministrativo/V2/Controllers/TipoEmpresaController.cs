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
	public class TipoEmpresaController : ControllerBase
	{
		private readonly IDistributedCache _cache;
		private readonly ITipoEmpresaService _tipoEmpresaService;

		private DistributedCacheEntryOptions _cacheOptions;

		public TipoEmpresaController([FromServices] IDistributedCache cache, ITipoEmpresaService service)
		{
			_cache = cache;
			_tipoEmpresaService = service;

			_cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
		}

		/// <summary>
		/// Lista todos os tipos de empresa cadastrados na base
		/// </summary>
		/// <returns>Lista de tipos de empresa requerida.</returns>
		/// <response code="200">Lista dos tipos de empresa retornada com sucesso.</response>
		/// <response code="404">Lista dos tipos de empresa não existe.</response>
		[Benchmark, HttpGet]
		[ProducesResponseType(typeof(IEnumerable<TipoEmpresa>), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar()
		{
			IEnumerable<TipoEmpresa> tiposEmpresa;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("ListaTiposEmpresa") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("ListaTiposEmpresa");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					tiposEmpresa = _tipoEmpresaService.ListarTodos();
					_cache.SetString("ListaTiposEmpresa", JsonConvert.SerializeObject(tiposEmpresa), _cacheOptions);
				}
				else
				{
					tiposEmpresa = JsonConvert.DeserializeObject<IEnumerable<TipoEmpresa>>(valorEmCache);
				}
			}
			else
			{
				tiposEmpresa = _tipoEmpresaService.ListarTodos();
			}

			if (tiposEmpresa == null)
				return NotFound();

			return Ok(tiposEmpresa);
		}

		/// <summary>
		/// Lista as informações de um tipo de empresa através do seu Id
		/// </summary>
		/// <param name="id">Id do tipo de empresa</param>
		/// <returns>Tipo de empresa requerido.</returns>
		/// <response code="200">Tipo de empresa retornado com sucesso.</response>
		/// <response code="404">Tipo de empresa não existe.</response>
		[Benchmark, HttpGet("{id}", Name = "InfoTipoEmpresa")]
		[ProducesResponseType(typeof(TipoEmpresa), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar([FromRoute] Guid id)
		{
			TipoEmpresa tipoEmpresa;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("TipoEmpresa") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("TipoEmpresa");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					tipoEmpresa = _tipoEmpresaService.ObterPorId(id);
					_cache.SetString("TipoEmpresa", JsonConvert.SerializeObject(tipoEmpresa), _cacheOptions);
				}
				else
				{
					tipoEmpresa = JsonConvert.DeserializeObject<TipoEmpresa>(valorEmCache);
				}
			}
			else
			{
				tipoEmpresa = _tipoEmpresaService.ObterPorId(id);
			}

			if (tipoEmpresa == null)
				return NotFound();

			return Ok(tipoEmpresa);
		}

		/// <summary>
		/// Insere um tipo de empresa na base
		/// </summary>
		/// <param name="novoTipoEmpresa">Tipo de empresa a ser inserido</param>
		/// <returns>Tipo de empresa criado.</returns>
		/// <response code="201">Tipo de empresa criado com sucesso.</response>
		/// <response code="400">Tipo de empresa é inválido.</response>
		[Benchmark, HttpPost]
		[ProducesResponseType(typeof(TipoEmpresa), 201)]
		[ProducesResponseType(400)]
		public IActionResult Inserir([FromBody] TipoEmpresa novoTipoEmpresa)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var tipoEmpresa = _tipoEmpresaService.Incluir(novoTipoEmpresa);

			return CreatedAtRoute("InfoTipoEmpresa", new { id = tipoEmpresa.Id }, tipoEmpresa);
		}

		/// <summary>
		/// Atualiza as informações de um tipo de empresa através do seu Id
		/// </summary>
		/// <param name="id">Id do tipo de empresa</param>
		/// <param name="tipoEmpresaASerAlterado">Informações do tipo de empresa a serem atualizadas</param>
		/// <returns>Tipo de empresa atualizado.</returns>
		/// <response code="204">Tipo de empresa atualizado com sucesso.</response>
		/// <response code="404">Tipo de empresa não existe.</response>
		[Benchmark, HttpPut("{id}")]
		[ProducesResponseType(typeof(TipoEmpresa), 204)]
		[ProducesResponseType(404)]
		public IActionResult Atualizar([FromRoute] Guid id, [FromBody] TipoEmpresa tipoEmpresaASerAlterado)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var tipoEmpresa = _tipoEmpresaService.Alterar(id, tipoEmpresaASerAlterado);

			if (tipoEmpresa == null)
				return NotFound();

			return NoContent();
		}
	}
}