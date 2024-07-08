using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Dtos
{
    public class OrderDto
    {

        [Required]
        public string BasketId { get; set; }
        [Required]
        public int DeliveryMethodId { get; set; }
        [Required]
        public AddressDto ShippingAddress { get; set; }
    }
}
