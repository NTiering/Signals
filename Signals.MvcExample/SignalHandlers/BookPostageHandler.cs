using Signals.Context;
using Signals.Handlers;
using Signals.MvcExample.Data;
using Signals.MvcExample.Signals;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.MvcExample.SignalHandlers
{
    public class BookPostageHandler : SignalHandler<BookPriceSignal>
    {
        private readonly IDataRepository dataRepository;

        public BookPostageHandler(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }
        public override int Order => 1;
        protected override Task OnSignal(BookPriceSignal signal, ISignalContext context, CancellationToken token)
        {
            var bookDetails = dataRepository.GetBook(signal.BookId);
            var isHeavy = bookDetails.Weight > 3;
            signal.PostageCost = isHeavy ? 12.99m : 5.99m;
            if (isHeavy)
            {
                signal.Notes.Add("Books over 3kg cost 12.99 to post");
            }
            return Task.CompletedTask;
        }
    }
}
