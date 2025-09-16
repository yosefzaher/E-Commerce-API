using E_Commerce.BLL.Settings;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services;
public class PaymentService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;

    public PaymentService(IOptions<StripeSettings> options)
    {
        _stripeSettings = options.Value;

        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }
    public string CreatePaymentIntent(decimal amount, string currency)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), 
            Currency = currency,
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        var paymentIntent = service.Create(options);

        return paymentIntent.ClientSecret; 
    }
}
