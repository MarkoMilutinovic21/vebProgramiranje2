using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Shared.Events;
using Shared.Interfaces;
using System.Fabric;
using System.Net.Mail;

namespace EventDispatcherService
{
    internal sealed class EventDispatcherService : StatefulService, IEventDispatcherService
    {
        public EventDispatcherService(StatefulServiceContext context)
            : base(context) { }

        public async Task PublishAsync(PurchaseEvent purchaseEvent)
        {
            var queue = await StateManager.GetOrAddAsync<IReliableQueue<PurchaseEvent>>("purchaseEvents");

            using var tx = StateManager.CreateTransaction();
            await queue.EnqueueAsync(tx, purchaseEvent);
            await tx.CommitAsync();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var queue = await StateManager.GetOrAddAsync<IReliableQueue<PurchaseEvent>>("purchaseEvents");

            while (!cancellationToken.IsCancellationRequested)
            {
                using var tx = StateManager.CreateTransaction();
                var result = await queue.TryDequeueAsync(tx);

                if (result.HasValue)
                {
                    var purchaseEvent = result.Value;
                    try
                    {
                        SendEmail(purchaseEvent);
                        await tx.CommitAsync();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
        }

        private void SendEmail(PurchaseEvent e)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("your-email@gmail.com", "your-password"),
                EnableSsl = true
            };

            var body = $@"Postovani,

Obavjestavamo Vas da je Vasa kupovina uspjesno realizovana.

Detalji kupovine:
Naziv knjige: {e.BookTitle}
Kolicina: {e.Quantity}
Ukupan iznos: {e.TotalPrice} KM
Datum i vrijeme: {e.Timestamp}

Hvala Vam na ukazanom povjerenju.

Srdacan pozdrav,
BookStore tim";

            var mail = new MailMessage("your-email@gmail.com", e.Email)
            {
                Subject = "Kupovina uspjesno obavljena",
                Body = body
            };

            smtp.Send(mail);
        }
    }
}