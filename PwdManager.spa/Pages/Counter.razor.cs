using Count;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;

namespace PwdManager.spa.Pages
{
    public partial class Counter
    {
        [Inject] GrpcChannel Channel {get;set;}
        private int currentCount = 0;
        private CancellationTokenSource? cts;

        private async Task IncrementCount()
        {
            cts = new CancellationTokenSource();

            var client = new Count.Counter.CounterClient(Channel);
            using var call = client.StartCounter(new CounterRequest() { Start = currentCount }, cancellationToken: cts.Token);

            try
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    currentCount = message.Count;
                    StateHasChanged();
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                // Ignore exception from cancellation
            }
        }

        private void StopCount()
        {
            cts?.Cancel();
            cts = null;
        }
    }
}
