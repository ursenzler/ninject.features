//-------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Appccelerate">
//   Copyright (c) 2008-2013
//
//   Licensed under the Apache License, Version 2.0 (the "License");
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
//-------------------------------------------------------------------------------

namespace Ninject.FeatureDumper
{
    using System;
    using System.Diagnostics;

    using Appccelerate.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var commandLineArguments = ParseCommandLineArguments(args);

                var assemblyLoader = new AssemblyLoader();
                var assemblies = assemblyLoader.LoadAssemblies(commandLineArguments.AssemblyFolder);

                var featureProcessor = new FeatureProcessor();
                Features features = featureProcessor.ProcessAssemblies(assemblies);

                var tgfWriter = new TgfWriter();
                tgfWriter.WriteTgfFile(commandLineArguments.OutputPath, features);

                StartYEd(commandLineArguments.OutputPath);

                Console.WriteLine("done output file is " + commandLineArguments.OutputPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine("failed with exception:");
                Console.WriteLine(exception);
            }
        }

        private static CommandLineArguments ParseCommandLineArguments(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: FeatureDumper <outputFile .tgf> <folder with assemblies>");
                return CommandLineArguments.CreateFailed();
            }

            return CommandLineArguments.CreateSuccessful(
                args[0],
                args[1]);
        }

        private static void StartYEd(AbsoluteFilePath outputPath)
        {
            var yEd = new Process
                      {
                          StartInfo =
                          {
                              FileName = @"C:\Program Files (x86)\yWorks\yEd\yEd.exe",
                              Arguments = outputPath
                          }
                      };

            yEd.Start();
        }
    }
}
