using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Demo.NetCore.Model
{
    [ElasticsearchType(IdProperty = nameof(id))]
    public class ElasticDemo
    {
        [Ignore]
        public long id { get; set; }

        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_smart")]
        public string title { get; set; }

        [Keyword]
        public string category { get; set; }

        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_smart")]
        public string detail { get; set; }

        public DateTime createtime { get; set; }

        //复杂类型测试
        [Object]
        public List<TestListElastic> TestObject { get; set; }
    }

    public class TestListElastic
    {
        [Keyword]
        public string wordcategory { get; set; }

        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_smart")]
        public string word { get; set; }

        public DateTime createtime { get; set; }
    }
}
