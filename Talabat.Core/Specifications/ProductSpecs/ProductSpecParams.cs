﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Specifications.ProductSpecs
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 10;

        private int pageSize=5; 

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value> MaxPageSize ? MaxPageSize : value; } // if value >10 return 10 else return value
        }

        public int PageIndex { get; set; } = 1; 


        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }


        public string? Sort {  get; set; }

        public int? BrandId { get; set; }
        
        public int? CategoryId { get; set; }

    }
}
