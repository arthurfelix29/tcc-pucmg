using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.V1.Controllers
{
	[Produces("application/json")]
	[ApiVersion("1.0", Deprecated = true)]
	[EnableCors("AllowAll")]
	[Route("api/v{api-version:apiVersion}/[controller]")]
	[ApiController]
	public class EmpresaController : ControllerBase
	{
		private readonly IDistributedCache _cache;
		private readonly IEmpresaService _empresaService;

		private DistributedCacheEntryOptions _cacheOptions;

		public EmpresaController([FromServices] IDistributedCache cache, IEmpresaService service)
		{
			_cache = cache;
			_empresaService = service;

			_cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
		}

		/// <summary>
		/// Lista todas as empresas cadastradas na base
		/// </summary>
		/// <returns>Lista de empresas requerida.</returns>
		/// <response code="200">Lista das empresas retornada com sucesso.</response>
		/// <response code="404">Lista das empresas não existe.</response>
		[Benchmark, HttpGet]
		[ProducesResponseType(typeof(IEnumerable<Empresa>), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar()
		{
			IEnumerable<Empresa> empresas;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("ListaEmpresas") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("ListaEmpresas");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					empresas = _empresaService.ListarTodos();
					_cache.SetString("ListaEmpresas", JsonConvert.SerializeObject(empresas), _cacheOptions);
				}
				else
				{
					empresas = JsonConvert.DeserializeObject<IEnumerable<Empresa>>(valorEmCache);
				}
			}
			else
			{
				empresas = _empresaService.ListarTodos();
			}

			if (empresas == null)
				return NotFound();

			return Ok(empresas);
		}

		/// <summary>
		/// Lista as informações de uma empresa através do seu Id
		/// </summary>
		/// <param name="id">Id da empresa</param>
		/// <returns>Empresa requerida.</returns>
		/// <response code="200">Empresa retornada com sucesso.</response>
		/// <response code="404">Empresa não existe.</response>
		[Benchmark, HttpGet("{id}", Name = "InfoEmpresa")]
		[ProducesResponseType(typeof(Empresa), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar([FromRoute] Guid id)
		{
			Empresa empresa;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("Empresa") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("Empresa");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					empresa = _empresaService.ObterPorId(id);
					_cache.SetString("Empresa", JsonConvert.SerializeObject(empresa), _cacheOptions);
				}
				else
				{
					empresa = JsonConvert.DeserializeObject<Empresa>(valorEmCache);
				}
			}
			else
			{
				empresa = _empresaService.ObterPorId(id);
			}

			if (empresa == null)
				return NotFound();

			return Ok(empresa);
		}

		/// <summary>
		/// Insere uma empresa na base
		/// </summary>
		/// <param name="novaEmpresa">Empresa a ser inserida</param>
		/// <returns>Empresa criada.</returns>
		/// <response code="201">Empresa criada com sucesso.</response>
		/// <response code="400">Empresa é inválida.</response>
		[Benchmark, HttpPost]
		[ProducesResponseType(typeof(Empresa), 201)]
		[ProducesResponseType(400)]
		public IActionResult Inserir([FromBody] Empresa novaEmpresa)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var empresa = _empresaService.Incluir(novaEmpresa);

			return CreatedAtRoute("InfoEmpresa", new { id = empresa.Id }, empresa);
		}

		/// <summary>
		/// Atualiza as informações de uma empresa através do seu Id
		/// </summary>
		/// <param name="id">Id da empresa</param>
		/// <param name="empresaASerAlterada">Informações da empresa a serem atualizadas</param>
		/// <returns>Empresa atualizada.</returns>
		/// <response code="204">Empresa atualizada com sucesso.</response>
		/// <response code="404">Empresa não existe.</response>
		[Benchmark, HttpPut("{id}")]
		[ProducesResponseType(typeof(Empresa), 204)]
		[ProducesResponseType(404)]
		public IActionResult Atualizar([FromRoute] Guid id, [FromBody] Empresa empresaASerAlterada)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var empresa = _empresaService.Alterar(id, empresaASerAlterada);

			if (empresa == null)
				return NotFound();

			return NoContent();
		}

		/// <summary>
		/// Exclui uma empresa da base através do seu Id
		/// </summary>
		/// <param name="id">Id da empresa</param>
		/// <returns>Empresa excluída.</returns>
		/// <response code="204">Empresa excluída com sucesso.</response>
		/// <response code="404">Empresa não existe.</response>
		[Benchmark, HttpDelete("{id}")]
		[ProducesResponseType(typeof(Empresa), 204)]
		[ProducesResponseType(404)]
		public IActionResult Excluir([FromRoute] Guid id)
		{
			var empresa = _empresaService.ObterPorId(id);

			if (empresa == null)
				return NotFound();

			_empresaService.Excluir(id);

			return NoContent();
		}
	}
}