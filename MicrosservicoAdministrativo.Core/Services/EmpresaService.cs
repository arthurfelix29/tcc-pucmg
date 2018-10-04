using MicrosservicoAdministrativo.Core.Services.Interfaces;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services
{
	public class EmpresaService : IEmpresaService
	{
		private IRepository<Empresa, string> _empresaRepository;

		public EmpresaService(IRepository<Empresa, string> repository)
		{
			_empresaRepository = repository;
		}

		public IEnumerable<Empresa> ListarTodos()
		{
			return _empresaRepository.GetItemsFromCollectionAsync().Result;
		}

		public Empresa ObterPorId(Guid id)
		{
			return _empresaRepository.GetItemFromCollectionAsync(id.ToString()).Result;
		}

		public Empresa Incluir(Empresa novaEmpresa)
		{
			return _empresaRepository.AddDocumentIntoCollectionAsync(novaEmpresa).Result;
		}

		public Empresa Alterar(Guid id, Empresa empresaASerAlterada)
		{
			return _empresaRepository.UpdateDocumentFromCollection(id.ToString(), empresaASerAlterada).Result;
		}

		public void Excluir(Guid id)
		{
			_empresaRepository.DeleteDocumentFromCollectionAsync(id.ToString());
		}
	}
}