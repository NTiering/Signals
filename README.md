# Signals

Signals is a simple to use pub-sub/workflow hybrid framework. 

It is designed to allow you to separate concerns and call workflows in an async manner 

## Installation

Use Nuget to install the latest version 

```bash
Install-Package Signals-pubsub
```
## Usage
### Setting up 
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

public class BookDetailSignal : Signal
{
    public BookDetailSignal(int bookId)
    {
        BookId = bookId;
    }

    public int BookId { get; }
    public Book BookDetails { get; set; }

}
```
### Making a call 
Add some handler classes to process the signal
```csharp
using Signals.Context;
using Signals.Handlers;

public class BookDetailsHandler : SignalHandler<BookDetailSignal>
{
    public override int Order => 0; // this denotes the order of handlers (lowest first)
    
    protected override Task OnSignal(BookPriceSignal signal, ISignalContext context, CancellationToken token)
    {
        var bookDetails = dataRepository.GetBook(signal.BookId);
        signal.BookDetails = bookDetails;            
        return Task.CompletedTask;
    }

    // this is called if this OR any later handlers throw an exception
    protected override Task OnSignalAbort(BookPriceSignal signal, ISignalContext context, CancellationToken token)
    {
        Console.WriteLine($"An error was thrown {context.Exception.Message}");
        return Task.CompletedTask;
    }
}

public class BookDetailsLoggerHandler : SignalHandler<BookDetailSignal>
{
    public override int Order => 1; // this denotes the order of handlers (lowest first)
    protected override Task OnSignal(BookPriceSignal signal, ISignalContext context, CancellationToken token)
    {
        if(signal.BookDetails == null)
        {
            Console.WriteLine($"book details not found {BookDetailSignal.Id}");
        } 
        else
        {
            Console.WriteLine($"book details found {BookDetailSignal.Id}");
    
        }           
        return Task.CompletedTask;
    }
}

// Pipeline handlers recieve EVERY signal sent 
public class LoggingPipelineHandler : PipelineHandler
{
        
    public override Task ProcessStart(ISignal signal, CancellationToken token)
    {
        ProcessStartSignal = signal;
        Console.WriteLine("signal processing started");
        return Task.CompletedTask;
    }

    public override Task ProcessEnd(ISignal signal, CancellationToken token)
    {
        Console.WriteLine("signal processing ended");        
        return Task.CompletedTask;
    }
}

```

You can send a message (from a controller in this case)
```csharp
public class HomeController : Controller
{
        private readonly ISignalProcessor signalProcessor;

        public HomeController(ISignalProcessor signalProcessor)
        {  
            // inject a version of ISignalProcessor , called signalProcessor           
            this.signalProcessor = signalProcessor;
        }

        public async Task<IActionResult> Detail(int id)
        {
            // we can send a signal to get the book details from a workflow
            var signal = await signalProcessor.Process(new BookDetailSignal(id));
            return View(signal.BookDetails);
        }

        public async Task<IActionResult> WorkFlows()
        {
            // we can see what workflows have been set up 
            var workFlow = signalProcessor.WorkFlows;
            return View(workFlow);
        }
}
```



## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
