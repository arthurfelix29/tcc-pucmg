using MicrosservicoAdministrativo.Data.Models;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services.Interfaces
{
	public interface ITipoEmpresaService
	{
		IEnumerable<TipoEmpresa> ListarTodos();
		TipoEmpresa ObterPorId(Guid id);
		TipoEmpresa Incluir(TipoEmpresa novoTipoEmpresa);
		TipoEmpresa Alterar(Guid id, TipoEmpresa tipoEmpresaASerAlterado);
		void Excluir(Guid id);
	}
}