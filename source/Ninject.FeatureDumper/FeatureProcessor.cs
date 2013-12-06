//-------------------------------------------------------------------------------
// <copyright file="FeatureProcessor.cs" company="Appccelerate">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Features;

    public class FeatureProcessor
    {
        private readonly HashSet<Tuple<Type, Type>> chain = new HashSet<Tuple<Type, Type>>();
        private readonly HashSet<Type> allFeatures = new HashSet<Type>();

        public Features ProcessAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                this.ProcessAssembly(assembly);
            }

            return new Features(
                this.allFeatures.ToList(),
                this.chain.Select(item => new Reference(item.Item1, item.Item2)).ToList());
        }

        private void ProcessAssembly(Assembly assembly)
        {
            var features = assembly.GetExportedTypes().Where(type => type.IsSubclassOf(typeof(Feature)));

            foreach (Type feature in features)
            {
                this.ProcessFeature(feature);
            }
        }

        private void ProcessFeature(Type feature)
        {
            Console.WriteLine("found feature " + feature.Name);

            var constructor = feature.GetConstructors().OrderBy(c => c.GetParameters().Length).First();

            object[] arguments = new object[constructor.GetParameters().Length];

            var instance = (Feature)Activator.CreateInstance(feature, arguments);

            this.allFeatures.Add(feature);

            IList<Feature> neededFeatures = instance.NeededFeatures.ToList();
            foreach (Feature neededFeature in neededFeatures)
            {
                Console.WriteLine("needed features:");
                Console.WriteLine("- " + neededFeature.GetType().Name);

                this.chain.Add(new Tuple<Type, Type>(feature, neededFeature.GetType()));

                this.ProcessFeature(neededFeature.GetType());
            }
        }
    }
}