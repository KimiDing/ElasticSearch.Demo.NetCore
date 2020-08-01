using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Demo.NetCore.Common.ElasticSearch;
using ElasticSearch.Demo.NetCore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Demo.NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticDemoController : ControllerBase
    {
        public IEsClient esClient { get; }

        public ElasticDemoController(IEsClient esClient)
        {
            this.esClient = esClient;
        }

        [HttpPost]
        [Route("EsSearch")]
        public async Task<MessageResult<SearchInfoDTO>> EsSearch(EsSearchDTO esSearchDTO)
        {
            //esClient.CreateIndex<ChinaCodex>("myindex");
            /*var esResult = esClient.Search(new SearchRequest<ChinaCodex> 
            { 
                Query = new MatchQuery { Field = "detail", Query = esSearchDTO.word },
                From = esSearchDTO.from,
                Size = esSearchDTO.size
            });*/
            var esResult = await esClient.Search<ElasticDemo>(s => s
            .Query(q => q.Match(m => m.Field(f => f.detail).Query(esSearchDTO.word)))
            .From(esSearchDTO.from)
            .Size(esSearchDTO.size));

            var result = new SearchInfoDTO();
            if (esResult.IsValid)
            {
                result.list = new List<ListDTO>();
                result.total = esResult.Total;
                foreach (var d in esResult.Documents)
                {

                    result.list.Add(new ListDTO
                    {
                        title = d.title,
                        category = d.category,
                        detail = d.detail
                    });
                }

                return new MessageResult<SearchInfoDTO> { result = result };
            }
            else
            {
                return new MessageResult<SearchInfoDTO> { success = false, code = esResult.ServerError.Status, msg = esResult.ServerError.Error.Reason };
            }

        }

        [HttpPost]
        [Route("EsCreateIndexMapping")]
        public async Task<bool> EsCreateIndexMapping(string indexName)
        {
            return await esClient.CreateIndex<ElasticDemo>(indexName);
        }

        [HttpPost]
        [Route("EsInsert")]
        public async Task<bool> EsInsert(string indexName)
        {
            var t = new ElasticDemo { id = 1, category = "000", detail = "test", title = "test", createtime = DateTime.Now, TestObject = new List<TestListElastic> { new TestListElastic { createtime = DateTime.Now, word = "test", wordcategory = "test" } } };
            var l = new List<ElasticDemo>();
            for (int i = 10; i < 110; i++)
            {
                //t.id = i;
                l.Add(new ElasticDemo { id = i, category = "111", detail = "test", title = "test", createtime = DateTime.Now, TestObject = new List<TestListElastic> { new TestListElastic { createtime = DateTime.Now, word = "test", wordcategory = "test" } } });
            }

            return await esClient.BulkDocWithIndex(l, indexName);
        }

        [HttpPost]
        [Route("EsDel")]
        public async Task<bool> EsDel(string indexName)
        {

            //目前全部删除，谨慎操作
            return await esClient.DeleteByQuery<ElasticDemo>(c => c.Index(indexName).MatchAll());
        }
    }
}
