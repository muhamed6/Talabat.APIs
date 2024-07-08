using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Specs
{
    public class OrderSpecifications : BaseSpecifications<Order>
    {


        public OrderSpecifications(string buyerEmail) 
            : base(O => O.BuyerEmail==buyerEmail) 
        {
            // now i need to include navigational property deliverymethod in class order
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);

            AddOrderByDesc(O => O.OrderDate); 

        }

        // to get specific order for specific user  
        public OrderSpecifications(int orderId, string buyerEmail) 
            : base(O => O.Id==orderId && O.BuyerEmail==buyerEmail) 
        {
            // now i need to include navigational property deliverymethod in class order
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);



        }

      
    }
}
