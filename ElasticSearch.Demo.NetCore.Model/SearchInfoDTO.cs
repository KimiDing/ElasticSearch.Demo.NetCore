using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Demo.NetCore.Model
{
    public class SearchInfoDTO
    {
        public List<ListDTO> list { get; set; }
        public long total { get; set; }
    }

    public class ListDTO
    {
        public string title { get; set; }
        public string category { get; set; }
        public string detail { get; set; }
    }
}
