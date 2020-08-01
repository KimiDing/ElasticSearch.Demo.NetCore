using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Demo.NetCore.Common.ElasticSearch
{
    public interface IEsClient
    {
        ElasticClient GetClient();
        Task<bool> CreateIndex<T>(string indexName) where T : class;
        Task<bool> InsertDocWithIndex<T>(T entity, string indexName) where T : class;
        Task<bool> BulkDocWithIndex<T>(List<T> entity, string indexName) where T : class;
        Task<ISearchResponse<T>> Search<T>(Func<SearchDescriptor<T>, ISearchRequest> selector = null) where T : class;
        Task<bool> DeleteByQuery<T>(Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> selector) where T : class;
    }
}
