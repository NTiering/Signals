using Microsoft.VisualStudio.TestTools.UnitTesting;
using Signals;
using Signals.Context;
using Signals.Handlers;
using Signals.Pipelines;
using Signals.Processor;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Signal.Tests
{
    [TestClass]
    public class SignalProcessorTests
    {
        [TestMethod]
        public async Task CanProcessValues()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessor = new TestSignalHandlerOne();
            var target = new SignalProcessor(new[] { signalProcessor });

            // act 
            await target.Process(signal, new CancellationToken());

            // assert
            Assert.AreEqual(signal, signalProcessor.SignalRecieved);
        }

        [TestMethod]
        public async Task CanProcessValuesWithMulitpleHandlers()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var target = new SignalProcessor(new ISignalHandler[] { signalProcessorOne, signalProcessorTwo });

            // act 
            await target.Process(signal, new CancellationToken());

            // assert
            Assert.AreEqual(signal, signalProcessorOne.SignalRecieved, "signalProcessorOne didnt get a signal");
            Assert.AreEqual(signal, signalProcessorTwo.SignalRecieved, "signalProcessorTwo didnt get a signal");

        }

        [TestMethod]
        public async Task DoesNotSwallowExceptions()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            signalProcessorOne.OnProcess = (s, c) => { throw new Exception(); };
            Exception exception = null;

            var target = new SignalProcessor(new ISignalHandler[] { signalProcessorOne, signalProcessorTwo });

            // act 
            try
            {
                await target.Process(signal, new CancellationToken());
            }
            catch (Exception e)
            {
                exception = e;
            }

            // assert
            Assert.IsNotNull(exception);

        }

        [TestMethod]
        public async Task ExceptionsStopProcesing()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();

            signalProcessorOne.OnProcess = (s, c) => { throw new Exception(); };


            var target = new SignalProcessor(new ISignalHandler[] { signalProcessorOne, signalProcessorTwo });

            // act 
            try
            {
                await target.Process(signal, new CancellationToken());
            }
            catch (Exception e)
            {

            }

            // assert
            Assert.IsNull(signalProcessorTwo.SignalRecieved);

        }

        [TestMethod]
        public async Task ExceptionCallsAbort()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var abortCalled = false;

            signalProcessorOne.OnProcess = (s, c) => { throw new Exception(); };
            signalProcessorOne.OnProcessAbort = (s, c) => { abortCalled = true; };


            var target = new SignalProcessor(new ISignalHandler[] { signalProcessorOne, signalProcessorTwo });

            // act 
            try
            {
                await target.Process(signal, new CancellationToken());
            }
            catch
            {

            }

            // assert
            Assert.IsTrue(abortCalled);

        }

        [TestMethod]
        public async Task PipelineHandlersAreAlwaysCalled()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var testPipelineHandler = new TestPipelineHandler();


            var target = new SignalProcessor(
                new ISignalHandler[] { signalProcessorOne, signalProcessorTwo },
                new IPipelineHandler[] { testPipelineHandler });

            // act 
            await target.Process(signal, new CancellationToken());


            // assert
            Assert.AreEqual(signal, testPipelineHandler.ProcessStartSignal);
            Assert.AreEqual(signal, testPipelineHandler.ProcessEndSignal);
        }

        [TestMethod]
        public async Task ContextIsNotNull()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var signalProcessorThree = new TestSignalHandlerThree();


            var target = new SignalProcessor(
                new ISignalHandler[] { signalProcessorOne, signalProcessorTwo, signalProcessorThree });

            // act 
            await target.Process(signal, new CancellationToken());


            // assert
            Assert.IsNotNull(signalProcessorOne.ContextRecieved);
            Assert.IsNotNull(signalProcessorTwo.ContextRecieved);
            Assert.IsNotNull(signalProcessorThree.ContextRecieved);
        }

        [TestMethod]
        public async Task ContextIsFirstIsSet()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var signalProcessorThree = new TestSignalHandlerThree();


            var target = new SignalProcessor(
                new ISignalHandler[] { signalProcessorOne, signalProcessorTwo, signalProcessorThree });

            // act 
            await target.Process(signal, new CancellationToken());


            // assert
            Assert.IsTrue(signalProcessorOne.IsFirst);
            Assert.IsFalse(signalProcessorTwo.IsFirst);
            Assert.IsFalse(signalProcessorThree.IsFirst);
        }

        [TestMethod]
        public async Task ContextIsLastIsSet()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var signalProcessorThree = new TestSignalHandlerThree();


            var target = new SignalProcessor(
                new ISignalHandler[] { signalProcessorOne, signalProcessorTwo, signalProcessorThree });

            // act 
            await target.Process(signal, new CancellationToken());


            // assert
            Assert.IsFalse(signalProcessorOne.IsLast);
            Assert.IsFalse(signalProcessorTwo.IsLast);
            Assert.IsTrue(signalProcessorThree.IsLast);
        }

        [TestMethod]
        public async Task ContextWorkFlowIsSet()
        {
            // arrange
            var signal = new TestSignal();
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var signalProcessorThree = new TestSignalHandlerThree();


            var target = new SignalProcessor(
                new ISignalHandler[] { signalProcessorOne, signalProcessorTwo, signalProcessorThree });

            // act 
            await target.Process(signal, new CancellationToken());
            var workFlow = signalProcessorThree.ContextRecieved.WorkFlow.ToArray();

            // assert
            Assert.AreEqual(3, workFlow.Length);
            Assert.AreEqual(typeof(TestSignalHandlerOne), workFlow[0]);
            Assert.AreEqual(typeof(TestSignalHandlerTwo), workFlow[1]);
            Assert.AreEqual(typeof(TestSignalHandlerThree), workFlow[2]);
        }

        [TestMethod]
        public void WorkFlowsIsNotNull()
        {
            // arrange
            var target = new SignalProcessor(
                new ISignalHandler[] { new TestSignalHandlerOne(), new TestSignalHandlerTwo(), new TestSignalHandlerThree() });

            // act 
            var workFlows = target.WorkFlows;

            // assert
            Assert.IsNotNull(workFlows);
        }

        [TestMethod]
        public void WorkFlowHasCorrectKey()
        {
            // arrange
            var target = new SignalProcessor(
                new ISignalHandler[] { new TestSignalHandlerOne(), new TestSignalHandlerTwo(), new TestSignalHandlerThree() });

            // act 
            var workFlows = target.WorkFlows;

            // assert
            Assert.AreEqual(typeof(TestSignal), workFlows.First().Key);
        }

        [TestMethod]
        public void WorkFlowHasCorrectValues()
        {
            // arrange
            var target = new SignalProcessor(
                new ISignalHandler[] { new TestSignalHandlerOne(), new TestSignalHandlerTwo(), new TestSignalHandlerThree() });

            // act 
            var workFlowValues = target.WorkFlows[typeof(TestSignal)];

            // assert
            Assert.AreEqual(typeof(TestSignalHandlerOne), workFlowValues[0]);
            Assert.AreEqual(typeof(TestSignalHandlerTwo), workFlowValues[1]);
            Assert.AreEqual(typeof(TestSignalHandlerThree), workFlowValues[2]);
        }

        [TestMethod]
        public async Task SignalsMaySkipHandlersAsync()
        {
            // arrange
            var signal = new TestSignal
            {
                HandlersToSkip = new[] { typeof(TestSignalHandlerTwo) }
            };
            var signalProcessorOne = new TestSignalHandlerOne();
            var signalProcessorTwo = new TestSignalHandlerTwo();
            var signalProcessorThree = new TestSignalHandlerThree();


            var target = new SignalProcessor(
                new ISignalHandler[] { signalProcessorOne, signalProcessorTwo, signalProcessorThree });

            // act 
            await target.Process(signal, new CancellationToken());


            // assert
            Assert.IsNotNull(signalProcessorOne.SignalRecieved);
            Assert.IsNull(signalProcessorTwo.SignalRecieved);
            Assert.IsNotNull(signalProcessorThree.SignalRecieved);


        }
    }




    public class TestSignal : Signals.Signal
    {
    }

    public abstract class TestSignalHandler : SignalHandler<TestSignal>
    {
        public int TestOrder { get; set; } = 1;
        public override int Order => TestOrder;

        public ISignal SignalRecieved { get; set; }
        public ISignalContext ContextRecieved { get; private set; }
        public bool IsFirst { get; private set; }
        public bool IsLast { get; private set; }
        public Action<ISignal, CancellationToken> OnProcess { get; set; } = (s, t) => { };
        public Action<ISignal, CancellationToken> OnProcessAbort { get; set; } = (s, t) => { };

        protected override Task OnSignal(TestSignal signal, ISignalContext context, CancellationToken token)
        {
            SignalRecieved = signal;
            ContextRecieved = context;
            IsFirst = context.IsFirst;
            IsLast = context.IsLast;
            OnProcess(signal, token);
            return Task.CompletedTask;
        }

        protected override Task OnSignalAbort(TestSignal signal, ISignalContext context, CancellationToken token)
        {
            OnProcessAbort(signal, token);
            return Task.CompletedTask;
        }
    }

    public class TestSignalHandlerOne : TestSignalHandler { }
    public class TestSignalHandlerTwo : TestSignalHandler { }
    public class TestSignalHandlerThree : TestSignalHandler { }

    public class TestPipelineHandler : PipelineHandler
    {
        public ISignal ProcessStartSignal { get; private set; }
        public ISignal ProcessEndSignal { get; private set; }

        public override Task ProcessStart(ISignal signal, CancellationToken token)
        {
            ProcessStartSignal = signal;
            return Task.CompletedTask;
        }

        public override Task ProcessEnd(ISignal signal, CancellationToken token)
        {
            ProcessEndSignal = signal;
            return Task.CompletedTask;
        }
    }
}
