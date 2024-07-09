using Count;
using Grpc.Core;

namespace PwdManager.notif.Services
{

        public class CounterService : Counter.CounterBase
        {
            public override async Task StartCounter(CounterRequest request, IServerStreamWriter<CounterResponse> responseStream, ServerCallContext context)
            {
                var count = request.Start;

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(new CounterResponse
                    {
                        Count = ++count
                    });

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        
        }
}
