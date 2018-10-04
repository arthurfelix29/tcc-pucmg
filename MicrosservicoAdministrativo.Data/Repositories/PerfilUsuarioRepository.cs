using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MicrosservicoAdministrativo.Data.Helpers;
using MicrosservicoAdministrativo.Data.Models;
using MicrosservicoAdministrativo.Data.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace MicrosservicoAdministrativo.Data.Repositories
{
	[ExcludeFromCodeCoverage]
	public class PerfilUsuarioRepository : IRepository<PerfilUsuario, string>
	{
		private static readonly string Endpoint = ConfigHelper.GetEndpoint();
		private static readonly string Key = ConfigHelper.GetKey();
		private static readonly string DatabaseId = ConfigHelper.GetDatabaseId();
		private static readonly string CollectionId = "PerfilUsuarioCollection";
		private static DocumentClient docClient;

		public PerfilUsuarioRepository()
		{
			docClient = new DocumentClient(new Uri(Endpoint), Key);
			CreateDatabaseIfNotExistsAsync().Wait();
			CreateCollectionIfNotExistsAsync().Wait();
		}

		public async Task<IEnumerable<PerfilUsuario>> GetItemsFromCollectionAsync()
		{
			var documents = docClient.CreateDocumentQuery<PerfilUsuario>(
				  UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
				  new FeedOptions { MaxItemCount = -1 })
				  .AsDocumentQuery();

			List<PerfilUsuario> perfisAssociados = new List<PerfilUsuario>();

			while (documents.HasMoreResults)
				perfisAssociados.AddRange(await documents.ExecuteNextAsync<PerfilUsuario>());

			return perfisAssociados;
		}

		public async Task<PerfilUsuario> GetItemFromCollectionAsync(string id)
		{
			try
			{
				Document doc = await docClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));

				return JsonConvert.DeserializeObject<PerfilUsuario>(doc.ToString());
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					return null;
				else
					throw;
			}
		}

		public async Task<PerfilUsuario> AddDocumentIntoCollectionAsync(PerfilUsuario item)
		{
			try
			{
				var document = await docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
				var res = document.Resource;

				return JsonConvert.DeserializeObject<PerfilUsuario>(res.ToString());
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<PerfilUsuario> UpdateDocumentFromCollection(string id, PerfilUsuario item)
		{
			try
			{
				var document = await docClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
				var data = document.Resource.ToString();

				return JsonConvert.DeserializeObject<PerfilUsuario>(data);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task DeleteDocumentFromCollectionAsync(string id)
		{
			await docClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
		}

		private static async Task CreateDatabaseIfNotExistsAsync()
		{
			try
			{
				await docClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					await docClient.CreateDatabaseAsync(new Database { Id = DatabaseId });
				else
					throw;
			}
		}

		private static async Task CreateCollectionIfNotExistsAsync()
		{
			try
			{
				await docClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
				{
					await docClient.CreateDocumentCollectionAsync(
						UriFactory.CreateDatabaseUri(DatabaseId),
						new DocumentCollection { Id = CollectionId },
						new RequestOptions { OfferThroughput = 400 });
				}
				else
				{
					throw;
				}
			}
		}
	}
}