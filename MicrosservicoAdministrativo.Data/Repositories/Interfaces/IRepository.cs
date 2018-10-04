using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicrosservicoAdministrativo.Data.Repositories.Interfaces
{
	public interface IRepository<TModel, in TPk>
	{
		Task<IEnumerable<TModel>> GetItemsFromCollectionAsync();
		Task<TModel> GetItemFromCollectionAsync(TPk id);
		Task<TModel> AddDocumentIntoCollectionAsync(TModel item);
		Task<TModel> UpdateDocumentFromCollection(TPk id, TModel item);
		Task DeleteDocumentFromCollectionAsync(TPk id);
	}
}