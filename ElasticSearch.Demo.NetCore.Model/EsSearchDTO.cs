using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Demo.NetCore.Model
{
    public class EsSearchDTO
    {
        public int from { get; set; }
        public int size { get; set; }
        public string word { get; set; }
    }
}
