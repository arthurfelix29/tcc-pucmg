using MicrosservicoAdministrativo.Data.Models;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services.Interfaces
{
	public interface IPerfilService
	{
		IEnumerable<Perfil> ListarTodos();
		Perfil ObterPorId(Guid id);
		Perfil Incluir(Perfil novoPerfil);
		Perfil Alterar(Guid id, Perfil perfilASerAlterado);
		void Excluir(Guid id);
	}
}