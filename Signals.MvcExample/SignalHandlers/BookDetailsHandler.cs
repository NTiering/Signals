using Signals.Context;
using Signals.Handlers;
using Signals.MvcExample.Data;
using Signals.MvcExample.Signals;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.MvcExample.SignalHandlers
{
    public class BookDetailsHandler : SignalHandler<BookPriceSignal>
    {
        private readonly IDataRepository dataRepository;

        public BookDetailsHandler(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }
        public override int Order => 0;
        protected override Task OnSignal(BookPriceSignal signal, ISignalContext context, CancellationToken token)
        {
            var bookDetails = dataRepository.GetBook(signal.BookId);

            signal.BookDetails = bookDetails;
            signal.Notes.Add($"A great read by {bookDetails.Author}");
            return Task.CompletedTask;
        }
    }
}
