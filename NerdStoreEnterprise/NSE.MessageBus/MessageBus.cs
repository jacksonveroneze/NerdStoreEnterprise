using System;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using NSE.Core.Integration;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;

namespace NSE.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private IBus _bus;
        private IAdvancedBus _advancedBus;

        private readonly string _connectionString;

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;

            TryConnect();
        }

        public bool IsConnected => _bus?.IsConnected ?? false;

        public IAdvancedBus AdvancedBus => _bus?.Advanced;

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            TryConnect();

            _bus.Publish(message);
        }

        public async Task PublishAsync<T>(T message) where T : IntegrationEvent
        {
            TryConnect();

            await _bus.PublishAsync(message);
        }

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
        {
            TryConnect();

            _bus.Subscribe(subscriptionId, onMessage);
        }

        public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
        {
            TryConnect();

            _bus.SubscribeAsync(subscriptionId, onMessage);
        }

        public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();

            return _bus.Request<TRequest, TResponse>(request);
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent where TResponse : ResponseMessage
        {
            TryConnect();

            return await _bus.RequestAsync<TRequest, TResponse>(request);
        }

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent where TResponse : ResponseMessage
        {
            TryConnect();

            return _bus.Respond(responder);
        }

        public IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent where TResponse : ResponseMessage
        {
            TryConnect();

            return _bus.RespondAsync(responder);
        }

        private void TryConnect()
        {
            if (IsConnected) return;

            RetryPolicy policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (outcome, timespan, retryCount, context) =>
                    {
                        Console.WriteLine($"Tentando pela {retryCount} vez!");
                    });

            policy.Execute(() =>
            {
                _bus = RabbitHutch.CreateBus(_connectionString);
                _advancedBus = _bus.Advanced;
                _advancedBus.Disconnected += OnDisconnect;
            });
        }

        private void OnDisconnect(object s, EventArgs e)
        {
            RetryPolicy policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .RetryForever();

            policy.Execute(TryConnect);
        }

        public void Dispose()
            => _bus.Dispose();
    }
}



//services.AddHttpClient<ICatalogoService, CatalogoService>()
//.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
////.AddTransientHttpErrorPolicy(
////p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
//.AddPolicyHandler(PollyExtensions.EsperarTentar())
//.AddTransientHttpErrorPolicy(
//p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

//services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//services.AddScoped<IUser, AspNetUser>();

//#region Refit

////services.AddHttpClient("Refit",
////        options =>
////        {
////            options.BaseAddress = new Uri(configuration.GetSection("CatalogoUrl").Value);
////        })
////    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
////    .AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);

//#endregion
//}
//}

//public class PollyExtensions
//{
//    public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
//    {
//        var retry = HttpPolicyExtensions
//            .HandleTransientHttpError()
//            .WaitAndRetryAsync(new[]
//            {
//                TimeSpan.FromSeconds(1),
//                TimeSpan.FromSeconds(5),
//                TimeSpan.FromSeconds(10),
//            }, (outcome, timespan, retryCount, context) =>
//            {
//                Console.ForegroundColor = ConsoleColor.Blue;
//                Console.WriteLine($"Tentando pela {retryCount} vez!");
//                Console.ForegroundColor = ConsoleColor.White;
//            });

//        return retry;
//    }
//}
//}
