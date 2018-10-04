using MicrosservicoAdministrativo.Data.Models;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services.Interfaces
{
	public interface IEmpresaService
	{
		IEnumerable<Empresa> ListarTodos();
		Empresa ObterPorId(Guid id);
		Empresa Incluir(Empresa novaEmpresa);
		Empresa Alterar(Guid id, Empresa empresaASerAlterada);
		void Excluir(Guid id);
	}
}