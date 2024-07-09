using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notification;
using Npgsql;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.TableInfos;
using System.Data;

namespace PwdManager.notif.Services
{
    public class NotificationerService:Notificationer.NotificationerBase
    {
        private readonly ILogger<NotificationerService> _logger;
        private readonly GrpcDbContext _db;
        string? connectionString = "";
      

        public NotificationerService(ILogger<NotificationerService> logger, GrpcDbContext db, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("postgres");
            _db = db;
        }

 
        public override async Task GetVaultNotifications(Empty request, IServerStreamWriter<VaultChange> responseStream, ServerCallContext context )
        {

            List<ApiUserCoffre> adminCoffres= new List<ApiUserCoffre>();
            HttpContext? httpContext = context.GetHttpContext();
            string? azureName = httpContext?.User?.Identity?.Name ?? throw new Exception("");
            ApiUser? apiUser =await _db.Apiusers.FindAsync(azureName );
          

            await using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            //  con.Notification += (o, e) => Console.WriteLine($"Received notification: {e.Payload}");
            con.Notification += async (o, e) =>
            {

                CompleteCoffreLogNotification dataPayload = JsonConvert.DeserializeObject<CompleteCoffreLogNotification>(e.Payload) ?? new CompleteCoffreLogNotification();

                if (dataPayload!=null )
                {
                    string? azureId =(await( _db.Apiusers.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == dataPayload.data.UserId)))?.AzureId;
                    adminCoffres = await _db.ApiUserCoffres.ToListAsync();
                    if (adminCoffres.Any(x=>x.UserId==dataPayload.data.UserId))
                    {

                        await responseStream.WriteAsync
                        (                       
                            new VaultChange
                            {
                                Table = dataPayload?.table,
                                Action = dataPayload?.action,
                                Data=new VaultData
                                {
                                    AzureId = azureId,
                                    //CoffreDescription = description,
                                    //CoffreTitle =  title,
                                    Operation = dataPayload?.data?.Operation,
                                    //DateOperation = dataPayload.data.DateOperation.ToTimestamp(),
                                    Timeofaction = dataPayload?.data?.DateOperation.ToString(),
                                    CoffreName=dataPayload?.data?.CoffreName,
                                }
                            }
                        );
                    }
                }
                //string? title = _db.Coffres.AsNoTracking().FirstOrDefault(a => a.Title == dataPayload.data.CoffreTitle)?.Title;
               // string? description = _db.Coffres.AsNoTracking().FirstOrDefault(a => a.Description == dataPayload.data.CoffreDescription)?.Description;
            };
            await using (var cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "LISTEN lastcoffrelogchange;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
            while (true)
            {
                // Waiting for Event
                con.Wait();
            }
        }
      

        public override async Task GetEntryNotifications(Empty request, IServerStreamWriter<EntryChange> responseStream, ServerCallContext context)
        {

            await using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();//  con.Notification += (o, e) => Console.WriteLine($"Received notification: {e.Payload}");
            con.Notification += async (o, e) =>
            {
                Console.WriteLine($"Received notification: {e.Payload}");
                CompleteEntreeHistoryNotification dataPayload = JsonConvert.DeserializeObject<CompleteEntreeHistoryNotification>(e.Payload)??new CompleteEntreeHistoryNotification();

                string? azureId =(await( _db.Apiusers.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == dataPayload.data.UserId)))?.AzureId;
                await responseStream.WriteAsync
                (
                    new EntryChange
                    {
                        Table = dataPayload?.table,
                        Action = dataPayload?.action,
                        Data = new EntryData
                        {
                            AzureId = azureId,
                            //EntreeId= dataPayload?.data?.EntreeId.ToString(),
                            EntreeHistoryId= dataPayload?.data?.EntreeHistoryId.ToString(),
                            //CoffreTitle = title,
                            Operation = dataPayload?.data?.Operation,
                            Timeofaction = dataPayload?.data?.DateOperation.ToString(),
                            EntreeName=dataPayload?.data?.EntreeName,
                        }
                    }
                );
            };
            await using (var cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "LISTEN lastentreelogchange;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
            while (true)
            {
                // Waiting for Event
                con.Wait();
            }
        }
    }
}
