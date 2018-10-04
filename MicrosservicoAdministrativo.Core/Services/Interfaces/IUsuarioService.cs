using MicrosservicoAdministrativo.Data.Models;
using System;
using System.Collections.Generic;

namespace MicrosservicoAdministrativo.Core.Services.Interfaces
{
	public interface IUsuarioService
	{
		IEnumerable<Usuario> ListarTodos();
		Usuario ObterPorId(Guid id);
		Usuario Incluir(Usuario novoUsuario);
		Usuario Alterar(Guid id, Usuario usuarioASerAlterado);
		void Excluir(Guid id);
	}
}