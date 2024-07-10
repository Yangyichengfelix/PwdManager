using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;


namespace PwdManager.spa.Pages
{
    public partial class SafeOperationNotif
    {
        //[Inject] GrpcChannel? Channel1 { get; set; }
        //[Inject] GrpcChannel? Channel2 { get; set; }

        //private CancellationTokenSource? cts;
        //string[] vaultOperations = { };
        //string[] entryOperations = { };
        //List<string> vaultOps= new List<string>();
        //List<string> entryOps= new List<string>();
        //protected override async Task OnParametersSetAsync()
        //{
        //    cts = new CancellationTokenSource();

        //    var client1 = new Notification.Notificationer.NotificationerClient(Channel1);
        //    var client2 = new Notification.Notificationer.NotificationerClient(Channel2);
        //    using var callEntry = client1.GetEntryNotifications(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cts.Token);
        //    using var callVault = client2.GetVaultNotifications(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cts.Token);
        //    try
        //    {
        //        await foreach (var message in callVault.ResponseStream.ReadAllAsync())
        //        {
        //            Console.WriteLine(message.Data);

        //            var text = $"{message.Data.AzureId} {message.Data.Operation} {message.Data.CoffreName} at {message.Data.Timeofaction}";
        //            vaultOps.Add(text);
        //            vaultOperations = vaultOps.ToArray();
        //            StateHasChanged();
        //        }
        //    }
        //    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        //    {
        //        // Ignore exception from cancellation
        //    }

        //}

        //private void StopCount()
        //{
        //    cts?.Cancel();
        //    cts = null;
        //}
    }
}
