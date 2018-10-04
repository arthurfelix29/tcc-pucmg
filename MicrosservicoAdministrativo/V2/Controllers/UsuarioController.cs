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
	public class UsuarioController : ControllerBase
	{
		private readonly IDistributedCache _cache;
		private readonly IUsuarioService _usuarioService;

		private DistributedCacheEntryOptions _cacheOptions;

		public UsuarioController([FromServices] IDistributedCache cache, IUsuarioService service)
		{
			_cache = cache;
			_usuarioService = service;

			_cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
		}

		/// <summary>
		/// Lista todos os usuários cadastrados na base
		/// </summary>
		/// <returns>Lista de usuários requerida.</returns>
		/// <response code="200">Lista de usuários retornada com sucesso.</response>
		/// <response code="404">Lista dos usuários não existe.</response>
		[Benchmark, HttpGet]
		[ProducesResponseType(typeof(IEnumerable<Usuario>), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar()
		{
			IEnumerable<Usuario> usuarios;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("ListaUsuarios") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("ListaUsuarios");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					usuarios = _usuarioService.ListarTodos();
					_cache.SetString("ListaUsuarios", JsonConvert.SerializeObject(usuarios), _cacheOptions);
				}
				else
				{
					usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(valorEmCache);
				}
			}
			else
			{
				usuarios = _usuarioService.ListarTodos();
			}

			if (usuarios == null)
				return NotFound();

			return Ok(usuarios);
		}

		/// <summary>
		/// Lista as informações de um usuário através do seu Id
		/// </summary>
		/// <param name="id">Id do usuário</param>
		/// <returns>Usuário requerido.</returns>
		/// <response code="200">Usuário retornado com sucesso.</response>
		/// <response code="404">Usuário não existe.</response>
		[Benchmark, HttpGet("{id}", Name = "InfoUsuario")]
		[ProducesResponseType(typeof(Usuario), 200)]
		[ProducesResponseType(404)]
		public IActionResult Listar([FromRoute] Guid id)
		{
			Usuario usuario;
			bool cacheEstaConfigurado = new Func<bool>(() => { try { return _cache.GetString("Usuario") != null; } catch { return false; } })();

			if (cacheEstaConfigurado)
			{
				string valorEmCache = _cache.GetString("Usuario");

				if (string.IsNullOrWhiteSpace(valorEmCache))
				{
					usuario = _usuarioService.ObterPorId(id);
					_cache.SetString("Usuario", JsonConvert.SerializeObject(usuario), _cacheOptions);
				}
				else
				{
					usuario = JsonConvert.DeserializeObject<Usuario>(valorEmCache);
				}
			}
			else
			{
				usuario = _usuarioService.ObterPorId(id);
			}

			if (usuario == null)
				return NotFound();

			return Ok(usuario);
		}

		/// <summary>
		/// Insere um usuário na base
		/// </summary>
		/// <param name="novoUsuario">Usuário a ser inserido</param>
		/// <returns>Usuário criado.</returns>
		/// <response code="201">Usuário criado com sucesso.</response>
		/// <response code="400">Usuário é inválido.</response>
		[Benchmark, HttpPost]
		[ProducesResponseType(typeof(Usuario), 201)]
		[ProducesResponseType(400)]
		public IActionResult Inserir([FromBody] Usuario novoUsuario)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var usuario = _usuarioService.Incluir(novoUsuario);

			return CreatedAtRoute("InfoUsuario", new { id = usuario.Id }, usuario);
		}

		/// <summary>
		/// Atualiza as informações de um usuário através do seu Id
		/// </summary>
		/// <param name="id">Id do usuário</param>
		/// <param name="usuarioASerAlterado">Informações do usuário a serem atualizadas</param>
		/// <returns>Usuário atualizado.</returns>
		/// <response code="204">Usuário atualizado com sucesso.</response>
		/// <response code="404">Usuário não existe.</response>
		[Benchmark, HttpPut("{id}")]
		[ProducesResponseType(typeof(Usuario), 204)]
		[ProducesResponseType(404)]
		public IActionResult Atualizar([FromRoute] Guid id, [FromBody] Usuario usuarioASerAlterado)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var usuario = _usuarioService.Alterar(id, usuarioASerAlterado);

			if (usuario == null)
				return NotFound();

			return NoContent();
		}
	}
}