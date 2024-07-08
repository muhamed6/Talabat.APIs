using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Product_Aggregate;

namespace Talabat.Core.Specifications.ProductSpecs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        // This Constructor Will be Used for Creating an Object, That Will be Used to Get All Products
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams specParams)
            :base(P=>
            (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search)) &&
            (!specParams.BrandId.HasValue    || P.BrandId== specParams.BrandId.Value) &&
            (!specParams.CategoryId.HasValue || P.CategoryId== specParams.CategoryId.Value)

            )
        {
            Includes.Add(P=>P.Brand);
            Includes.Add(P=>P.Category);

            if(!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort) 
                {
                    case "priceAsc":
                        //OrderBy=P=>P.Price;
                        AddOrderBy(P=>P.Price);
                        break;
                        
                    case "priceDesc":
                        //OrderByDesc=P=>P.Price;
                        AddOrderByDesc(P => P.Price);
                        break;

                        default:
                        AddOrderBy(p=>p.Name); 
                        break;

                }
            }
            else 
                AddOrderBy(p => p.Name);



            ApplyPagination((specParams.PageIndex - 1 ) *specParams.PageSize, specParams.PageSize);

        }  
        // This Constructor Will be Used for Creating an Object, That Will be Used to Get a Specific Product With Id
        public ProductWithBrandAndCategorySpecifications(int id)
            :base(P=>P.Id==id)
        {
            Includes.Add(P=>P.Brand);
            Includes.Add(P=>P.Category);
        }


    }
}
