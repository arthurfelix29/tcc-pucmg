using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services
{
	public class TipoEmpresaService : ITipoEmpresaService
	{
		private IRepository<TipoEmpresa, string> _tipoEmpresaRepository;

		public TipoEmpresaService(IRepository<TipoEmpresa, string> repository)
		{
			_tipoEmpresaRepository = repository;
		}

		public IEnumerable<TipoEmpresa> ListarTodos()
		{
			return _tipoEmpresaRepository.GetItemsFromCollectionAsync().Result;
		}

		public TipoEmpresa ObterPorId(Guid id)
		{
			return _tipoEmpresaRepository.GetItemFromCollectionAsync(id.ToString()).Result;
		}

		public TipoEmpresa Incluir(TipoEmpresa novoTipoEmpresa)
		{
			return _tipoEmpresaRepository.AddDocumentIntoCollectionAsync(novoTipoEmpresa).Result;
		}

		public TipoEmpresa Alterar(Guid id, TipoEmpresa tipoEmpresaASerAlterado)
		{
			return _tipoEmpresaRepository.UpdateDocumentFromCollection(id.ToString(), tipoEmpresaASerAlterado).Result;
		}

		public void Excluir(Guid id)
		{
			_tipoEmpresaRepository.DeleteDocumentFromCollectionAsync(id.ToString());
		}
	}
}