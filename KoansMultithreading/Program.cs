using KoansBase.Engine;
using Microsoft.DotNet.Cli.Utils;
using System;

namespace KoansMultithreading
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var reporter = Reporter.Output;
            var sensei = new Sensei(reporter);
            var path = new LearningPath();
            AppDomain.CurrentDomain.UnhandledException += (obj, arg) => Environment.Exit(-2);

            return path.Walk(sensei);
        }
    }
}