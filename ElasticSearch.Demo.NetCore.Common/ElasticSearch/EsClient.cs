using Elasticsearch.Net;
using ElasticSearch.Demo.NetCore.Common.Helper;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Demo.NetCore.Common.ElasticSearch
{
    public class EsClient : IEsClient
    {
        private readonly static string esIndex = Appsettings.app(new string[] { "AppSettings", "Elastic", "Index" });
        private readonly static string esServer = Appsettings.app(new string[] { "AppSettings", "Elastic", "Server" });
        private readonly static string esUser = Appsettings.app(new string[] { "AppSettings", "Elastic", "User" });
        private readonly static string esPassword = Appsettings.app(new string[] { "AppSettings", "Elastic", "Password" });
        private ElasticClient _client;


        public ElasticClient GetClient()
        {
            if (_client != null)
            {
                return _client;
            }

            var pool = new SingleNodeConnectionPool(new Uri(esServer));//连接池
            //DefaultIndex小写
            var settings = new ConnectionSettings(pool)
                .DefaultIndex(esIndex).BasicAuthentication(esUser, esPassword);

            _client = new ElasticClient(settings);
            return _client;
        }


        /// <summary>
        /// 创建es索引（需要借助特性标签）
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public async Task<bool> CreateIndex<T>(string indexName) where T : class
        {
            if (_client == null)
            {
                GetClient();
            }

            var isExits = await _client.Indices.ExistsAsync(indexName);
            if (!isExits.Exists)
            {
                //通过特性标签创建mapping，index分片设置为5
                var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
                .Map<T>(m => m.AutoMap())
                .Settings(s => s.NumberOfShards(5)));

                return createIndexResponse.Acknowledged;
            }

            return false;
        }

        /// <summary>
        /// 单条插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public async Task<bool> InsertDocWithIndex<T>(T entity, string indexName) where T : class
        {
            if (_client == null)
            {
                GetClient();
            }

            var isExits = await _client.Indices.ExistsAsync(indexName);
            if (isExits.Exists)
            {
                //id相同时会更新
                var response = await _client.IndexAsync(entity, s => s.Index(indexName));

                if (response.IsValid) return true;
            }

            return false;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public async Task<bool> BulkDocWithIndex<T>(List<T> entity, string indexName) where T : class
        {
            if (_client == null)
            {
                GetClient();
            }

            var isExits = await _client.Indices.ExistsAsync(indexName);
            if (isExits.Exists)
            {
                //id相同时会更新
                var bulkIndexResponse = await _client.BulkAsync(b => b.Index(indexName).IndexMany(entity));

                if (!bulkIndexResponse.Errors) return true;
            }

            return false;
        }

        /// <summary>
        /// 搜索（简单封装）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        public async Task<ISearchResponse<T>> Search<T>(Func<SearchDescriptor<T>, ISearchRequest> selector = null) where T : class
        {
            if (_client == null)
            {
                GetClient();
            }
            return await _client.SearchAsync(selector);
            //return _client.Search<T>(searchRequest);

            /*var result = _client.Search<T>(s => s.Query(q => q.Match(
            m => m.Field(field)
                .Query(searchInput)
                )));
            var i = 0;*/
        }

        /// <summary>
        /// 删除数据按条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByQuery<T>(Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> selector) where T : class
        {
            if (_client == null)
            {
                GetClient();
            }

            var response = await _client.DeleteByQueryAsync(selector);
            if (!response.IsValid)
            {
                return false;
            }

            return true;
        }
    }
}
