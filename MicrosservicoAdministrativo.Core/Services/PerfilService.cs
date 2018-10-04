using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services
{
	public class PerfilService : IPerfilService
	{
		private IRepository<Perfil, string> _perfilRepository;

		public PerfilService(IRepository<Perfil, string> repository)
		{
			_perfilRepository = repository;
		}

		public IEnumerable<Perfil> ListarTodos()
		{
			return _perfilRepository.GetItemsFromCollectionAsync().Result;
		}

		public Perfil ObterPorId(Guid id)
		{
			return _perfilRepository.GetItemFromCollectionAsync(id.ToString()).Result;
		}

		public Perfil Incluir(Perfil novoPerfil)
		{
			return _perfilRepository.AddDocumentIntoCollectionAsync(novoPerfil).Result;
		}

		public Perfil Alterar(Guid id, Perfil perfilASerAlterado)
		{
			return _perfilRepository.UpdateDocumentFromCollection(id.ToString(), perfilASerAlterado).Result;
		}

		public void Excluir(Guid id)
		{
			_perfilRepository.DeleteDocumentFromCollectionAsync(id.ToString());
		}
	}
}