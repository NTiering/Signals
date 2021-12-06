# Signals

Siganal is a simple to use pub-sub framework. It is designed to allow you to separate concerns easily 

## Installation

Use Nuget to install the latest version 

```bash
Install-Package Signals
```

## Usage
(in startup.cs)
```csharp
using Signals.Extensions;

public void ConfigureServices(IServiceCollection services)
{
            // add controllers etc
            services.AddSignalProcessor(); // add an instance of ISignalProcessor
            services.AddSignalHandlers(GetType().Assembly); // adds any signal handlers from this assembly
}
```

make a message class to send, derived from the Signal class
```csharp
using Signals;

public class BookPriceSignal : Signal
{
    public BookPriceSignal(int bookId)
    {
        BookId = bookId;
    }

    public int BookId { get; }
    public Book BookDetails { get; set; }

}
```

make a handler to process the signal
```csharp
using Signals.Context;
using Signals.Handlers;

public class BookDetailsHandler : SignalHandler<BookPriceSignal>
    {
        public override int Order => 0; // this denotes the order of handlers
        protected override Task OnSignal(BookPriceSignal signal, ISignalContext context, CancellationToken token)
        {
            var bookDetails = dataRepository.GetBook(signal.BookId);
            signal.BookDetails = bookDetails;            
            return Task.CompletedTask;
        }
    }

```

finally you can send a message send a message 
```csharp
var signal = await signalProcessor.Process(new BookPriceSignal(id));
var bookDetails = signal.BookDetails;
```



## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
