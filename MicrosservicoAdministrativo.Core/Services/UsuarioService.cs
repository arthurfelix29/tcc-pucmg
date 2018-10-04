using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services
{
	public class UsuarioService : IUsuarioService
	{
		private IRepository<Usuario, string> _usuarioRepository;

		public UsuarioService(IRepository<Usuario, string> repository)
		{
			_usuarioRepository = repository;
		}

		public IEnumerable<Usuario> ListarTodos()
		{
			return _usuarioRepository.GetItemsFromCollectionAsync().Result;
		}

		public Usuario ObterPorId(Guid id)
		{
			return _usuarioRepository.GetItemFromCollectionAsync(id.ToString()).Result;
		}

		public Usuario Incluir(Usuario novoUsuario)
		{
			return _usuarioRepository.AddDocumentIntoCollectionAsync(novoUsuario).Result;
		}

		public Usuario Alterar(Guid id, Usuario usuarioASerAlterado)
		{
			return _usuarioRepository.UpdateDocumentFromCollection(id.ToString(), usuarioASerAlterado).Result;
		}

		public void Excluir(Guid id)
		{
			_usuarioRepository.DeleteDocumentFromCollectionAsync(id.ToString());
		}
	}
}