using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services
{
	public class PerfilUsuarioService : IPerfilUsuarioService
	{
		private IRepository<PerfilUsuario, string> _perfilUsuarioRepository;

		public PerfilUsuarioService(IRepository<PerfilUsuario, string> repository)
		{
			_perfilUsuarioRepository = repository;
		}

		public IEnumerable<PerfilUsuario> ListarTodos()
		{
			return _perfilUsuarioRepository.GetItemsFromCollectionAsync().Result;
		}

		public PerfilUsuario ObterPorId(Guid id)
		{
			return _perfilUsuarioRepository.GetItemFromCollectionAsync(id.ToString()).Result;
		}

		public PerfilUsuario Incluir(PerfilUsuario novaAssociacao)
		{
			return _perfilUsuarioRepository.AddDocumentIntoCollectionAsync(novaAssociacao).Result;
		}

		public void Excluir(Guid id)
		{
			_perfilUsuarioRepository.DeleteDocumentFromCollectionAsync(id.ToString());
		}
	}
}