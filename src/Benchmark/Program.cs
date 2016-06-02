namespace Benchmark {
    //using System.IO;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;
    using TestCases;

    internal class Program {
        private static void Main() {
            IConfig config =
                ManualConfig.Create(DefaultConfig.Instance)
                    .With(Job.AllJits)
                    //.With(Job.Core)
                    .With(RPlotExporter.Default);

            //string sourceCode = File.ReadAllText("TestCases/RandomIdGenerator.cs");
            //BenchmarkRunner.RunSource(sourceCode, config);

            BenchmarkRunner.Run<RandomIdGenerator>(config);
        }
    }
}