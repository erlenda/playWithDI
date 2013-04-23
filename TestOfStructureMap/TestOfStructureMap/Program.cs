#define DEBUG

using System;
using NUnit.Framework;
using StructureMap;

namespace DependencyInjectionWithAndWithoutStructureMap
{
    [TestFixture]
    class TestManualDependencies
    {
        private IReturnOutput _returnOutput;

        [Test]
        public void TestNorwegianGreeting()
        {
            _returnOutput = new DebugOutputDisplay(new NorwegianGreeter());
            var messageReturned = _returnOutput.PrintGreetingAndReturnIt();
            const string expect = "Hei";
            Assert.AreEqual(messageReturned, expect, "Expected a norwegian greeting ({0}) but instead got {1}. ", expect, messageReturned);
        }

        [Test]
        public void TestFrenchGreeting()
        {
            _returnOutput = new DebugOutputDisplay(new FrenchGreeter());
            var messageReturned = _returnOutput.PrintGreetingAndReturnIt();
            const string expect = "Bonjour";
            Assert.AreEqual(messageReturned, expect, "Expected a french greeting ({0}) but instead got {1}. ", expect, messageReturned);
        }
    }
    
    class Program
    {
        private static void Main(string[] args)
        {
            IContainer container = ConfigureDependencies();
            IAppEngine appEngine = container.GetInstance<IAppEngine>();
            appEngine.Run();
        }

        private static IContainer ConfigureDependencies()
        {
            return new Container(x =>
            {
                x.For<IAppEngine>().Use<AppEngine>();
                x.For<IGreeter>().Use<EnglishGreeter>();
                x.For<IOutputDisplay>().Use<ConsoleOutputDisplay>();
            });
        }
    }

    public class AppEngine : IAppEngine
    {
        private readonly IGreeter greeter;
        private readonly IOutputDisplay outputDisplay;

        public AppEngine(IGreeter greeter, IOutputDisplay outputDisplay)
        {
            this.greeter = greeter;
            this.outputDisplay = outputDisplay;
        }

        public void Run()
        {
            outputDisplay.Show(greeter.GetGreeting());
        }
    }

    public class AppEngineTest : IAppEngine
    {
        private readonly IGreeter _greeter;
        private readonly IOutputDisplay _output;

        public AppEngineTest(IGreeter greeter, IOutputDisplay output)
        {
            this._greeter = greeter;
            this._output = output;
        }

        public void Run()
        {
            _output.Show(_greeter.GetGreeting());
        }
    }

    public interface IAppEngine
    {
        void Run();
    }

    public interface IGreeter
    {
        string GetGreeting();
    }

    public class EnglishGreeter : IGreeter
    {
        public string GetGreeting()
        {
            return "Hello";
        }
    }

    public class FrenchGreeter : IGreeter
    {
        public string GetGreeting()
        {
            return "Bonjour";
        }
    }

    public class NorwegianGreeter : IGreeter
    {
        public string GetGreeting()
        {
            return "Hei";
        }
    }

    public interface IOutputDisplay
    {
        void Show(string message);
    }

    public interface IReturnOutput
    {
        string PrintGreetingAndReturnIt();
    }

    public class ConsoleOutputDisplay : IOutputDisplay
    {
        public void Show(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }

    public class DebugOutputDisplay : IReturnOutput
    {
        public IGreeter Greeter;

        public DebugOutputDisplay(IGreeter greeter)
        {
            this.Greeter = greeter;
        }

        public string PrintGreetingAndReturnIt()
        {
            Console.WriteLine(Greeter.GetGreeting());
            System.Diagnostics.Debug.WriteLine(Greeter.GetGreeting());
            return this.Greeter.GetGreeting();
        }
    }
}
