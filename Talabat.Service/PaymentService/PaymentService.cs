﻿using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities.Basket;
using Product=Talabat.Core.Entities.Product_Aggregate.Product;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration,
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork
            )
        {
            _configuration = configuration;
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket is null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue) 
            {
               var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);

                shippingPrice = deliveryMethod.Cost;
                basket.ShippingPrice = shippingPrice;
            }

            if(basket.Items?.Count > 0) 
            {
              var productRepo = _unitOfWork.Repository<Product>();
              foreach (var item in basket.Items) 
                {
                    var product = await productRepo.GetByIdAsync(item.Id);
                    if(item.Price !=product.Price)
                        item.Price = product.Price; 
                }
            }

            PaymentIntent paymentIntent;
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            if(string.IsNullOrEmpty(basket.PaymentIntentId)) 
            {
                // Create New Payment Intent

                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }

                };

               paymentIntent=await paymentIntentService.CreateAsync(options); // Integration with stripe 
           
               basket.PaymentIntentId = paymentIntent.Id;
               basket.ClientSecret = paymentIntent.ClientSecret;
            }

            else 
            {
                // Update Existing Payment Intent

                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long)shippingPrice * 100
                };

                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);


            }
            await _basketRepo.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order?> UpdateOrderStatus(string paymentIntentId, bool isPaid)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await orderRepo.GetByIdWithSpecAsync(spec);

            if (order is null) return null;

            if (isPaid)
                order.Status = OrderStatus.PaymentReceived;
            else order.Status = OrderStatus.PaymentFailed;

            orderRepo.Update(order);

            await _unitOfWork.CompleteAsync();

            return order;
        }
    }
}
