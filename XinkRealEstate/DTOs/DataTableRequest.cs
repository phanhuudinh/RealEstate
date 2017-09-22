using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XinkRealEstate.DTOs
{
    public class DataTableRequest
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        //public object columns { get; set; }
        public List<Order> order { get; set; }
        public Search search { get; set; }
        //public int _ { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public string column { get; set; }
        public string dir { get; set; }
    }
}