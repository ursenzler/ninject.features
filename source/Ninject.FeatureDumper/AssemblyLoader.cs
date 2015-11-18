// <copyright file="AssemblyLoader.cs" company="Ninject.Features">
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Ninject.FeatureDumper.AssemblyResolveHandler", Justification = "Handler is used only during method.")]
        public IEnumerable<Assembly> LoadAssemblies(AbsoluteFolderPath assemblyFolder)
        {
            var assemblyResolveHandler = new AssemblyResolveHandler(assemblyFolder);

            Console.WriteLine("loading assemblies from " + assemblyFolder);

            List<string> assemblyPaths =
                Directory.EnumerateFiles(assemblyFolder, "*.exe")
                    .Union(Directory.EnumerateFiles(assemblyFolder, "*.dll"))
                    .Where(path => !path.EndsWith("Ninject.Features.dll", StringComparison.OrdinalIgnoreCase))
                    .ToList();

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "we don't care why")]
        private void AddReferencedAssemblies(Assembly current)
        {
            if (this.loadedAssemblies.Contains(current, new AssemblyComparer()))
            {
                return;
            }

            this.loadedAssemblies.Add(current);
            Console.WriteLine("loaded referenced assembly " + current.FullName);

            foreach (var assemblyName in current.GetReferencedAssemblies())
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    this.AddReferencedAssemblies(assembly);
                }
                catch
                {
                    Console.WriteLine("skipping referenced assembly `{0}` of assembly `{1}` because it is not found.", assemblyName.Name, current.FullName);
                }
            }
        }

        private class AssemblyComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.FullName == y.FullName;
            }

            public int GetHashCode(Assembly obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }
    }
}