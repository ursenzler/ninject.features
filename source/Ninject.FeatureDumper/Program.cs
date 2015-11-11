// <copyright file="Program.cs" company="Ninject.Features">
//   Copyright (c)  2013-2015
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>

namespace Ninject.FeatureDumper
{
    using System;
    using System.Diagnostics;

    using Appccelerate.CommandLineParser;
    using Appccelerate.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                AbsoluteFilePath outputPath = null;
                AbsoluteFolderPath assemblyFolder = null;
                AbsoluteFilePath yedPath = @"C:\Program Files (x86)\yWorks\yEd\yEd.exe";

                CommandLineConfiguration commandLineConfiguration = CommandLineParserConfigurator
                    .Create()
                        .WithPositional(path => outputPath = path)
                            .Required()
                            .DescribedBy("output.tgf", "The absolute output file path. Must have extension .tgf")
                        .WithPositional(folder => assemblyFolder = folder)
                            .Required()
                            .DescribedBy("binary folder", "The folder containing the binaries to analyze. Must contain all direct and indirect dependencies, too.")
                        .WithNamed("-yed", path => yedPath = path)
                            .DescribedBy("yEd path", @"Path specifing location of yEd. If not specified, the default installation path is used (C:\Program Files (x86)\yWorks\yEd\yEd.exe).")
                    .BuildConfiguration();

                var parser = new CommandLineParser(commandLineConfiguration);
                ParseResult parseResult = parser.Parse(args);

                if (!parseResult.Succeeded)
                {
                    WriteUsageToConsole(commandLineConfiguration, parseResult);

                    return;
                }

                var assemblyLoader = new AssemblyLoader();
                var assemblies = assemblyLoader.LoadAssemblies(assemblyFolder);

                var featureProcessor = new FeatureProcessor();
                Features features = featureProcessor.ProcessAssemblies(assemblies);

                var tgfWriter = new TgfWriter();
                tgfWriter.WriteTgfFile(outputPath, features);

                StartYEd(yedPath, outputPath);

                Console.WriteLine("done output file is " + outputPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine("failed with exception:");
                Console.WriteLine(exception);
            }
        }

        private static void WriteUsageToConsole(CommandLineConfiguration commandLineConfiguration, ParseResult parseResult)
        {
            Usage usage = new UsageComposer(commandLineConfiguration).Compose();

            Console.WriteLine(parseResult.Message);
            Console.WriteLine("usage:" + usage.Arguments);
            Console.WriteLine("options");
            Console.WriteLine(usage.Options.IndentBy(4));
            Console.WriteLine();
        }

        private static void StartYEd(AbsoluteFilePath yedPath, AbsoluteFilePath outputPath)
        {
            var yEd = new Process
                      {
                          StartInfo =
                          {
                              FileName = yedPath,
                              Arguments = outputPath
                          }
                      };

            yEd.Start();
        }
    }
}
