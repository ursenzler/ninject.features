//-------------------------------------------------------------------------------
// <copyright file="AssemblyLoader.cs" company="Ninject.Features">
//   Copyright (c) 2013-2014
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Appccelerate.IO;

    public class AssemblyLoader
    {
        private readonly List<Assembly> loadedAssemblies = new List<Assembly>();

        public IEnumerable<Assembly> LoadAssemblies(AbsoluteFolderPath assemblyFolder)
        {
            var assemblyResolveHandler = new AssemblyResolveHandler(assemblyFolder);

            Console.WriteLine("loading assemblies from " + assemblyFolder);

            List<string> assemblyPaths =
                Directory.EnumerateFiles(assemblyFolder, "*.exe")
                    .Union(Directory.EnumerateFiles(assemblyFolder, "*.dll")).ToList();

            Console.WriteLine("found assemblies:");
            foreach (var assemblyPath in assemblyPaths)
            {
                Console.WriteLine(assemblyPath);
            }

            IEnumerable<Assembly> assemblies = assemblyPaths.Select(Assembly.LoadFile).ToList();

            foreach (Assembly assembly in assemblies)
            {
                this.AddReferencedAssemblies(assembly);
            }

            return this.loadedAssemblies;
        }

        private void AddReferencedAssemblies(Assembly current)
        {
            if (this.loadedAssemblies.Contains(current))
            {
                return;
            }

            this.loadedAssemblies.Add(current);

            foreach (var assemblyName in current.GetReferencedAssemblies())
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);

                    Console.WriteLine("loaded assembly " + current.FullName);

                    this.AddReferencedAssemblies(assembly);
                }
                catch
                {
                    Console.WriteLine("skipping assembly because it is not found: " + assemblyName.Name);
                }
            }
        }
    }
}