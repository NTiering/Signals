using Signals.Context;
using Signals.Handlers;
using Signals.MvcExample.Signals;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.MvcExample.SignalHandlers
{
    public class BookDiscountHandler : SignalHandler<BookPriceSignal>
    {

        public override int Order => 2;
        protected override Task OnSignal(BookPriceSignal signal, ISignalContext context, CancellationToken token)
        {
            var eligibleForDiscount = signal.BookDetails.Price > 10;
            if (eligibleForDiscount)
            {
                signal.Discount = signal.BookDetails.Price / 10;
                signal.Notes.Add("With 10% !off");
            }
            return Task.CompletedTask;
        }
    }
}
