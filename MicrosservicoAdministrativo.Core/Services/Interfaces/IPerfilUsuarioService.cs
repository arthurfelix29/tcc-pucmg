using MicrosservicoAdministrativo.Data.Models;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services.Interfaces
{
	public interface IPerfilUsuarioService
	{
		IEnumerable<PerfilUsuario> ListarTodos();
		PerfilUsuario ObterPorId(Guid id);
		PerfilUsuario Incluir(PerfilUsuario novaAssociacao);
		void Excluir(Guid id);
	}
}