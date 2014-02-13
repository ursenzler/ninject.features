//-------------------------------------------------------------------------------
// <copyright file="AssemblyResolveHandler.cs" company="Ninject.Features">
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
    using System.IO;
    using System.Reflection;

    using Appccelerate.IO;

    internal class AssemblyResolveHandler
    {
        private readonly AbsoluteFolderPath assemblyFolder;

        public AssemblyResolveHandler(AbsoluteFolderPath assemblyFolder)
        {
            this.assemblyFolder = assemblyFolder;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(this.HandleUnresolvedAssembly);
        }

        private Assembly HandleUnresolvedAssembly(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.InvariantCulture));
            string assemblyFile = Path.Combine(this.assemblyFolder, assemblyName + ".dll");
            var assembly = Assembly.LoadFrom(assemblyFile);

            return assembly;
        }
    }
}