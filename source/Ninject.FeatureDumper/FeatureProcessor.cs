// <copyright file="FeatureProcessor.cs" company="Ninject.Features">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Features;

    public class FeatureProcessor
    {
        private readonly HashSet<Tuple<Type, Type>> chain = new HashSet<Tuple<Type, Type>>();
        private readonly HashSet<FeatureInfo> allFeatures = new HashSet<FeatureInfo>();

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
            Console.WriteLine("looking for features in " + assembly.GetName().Name);

            var features = assembly.GetExportedTypes().Where(type => type.IsSubclassOf(typeof(Feature)));

            foreach (Type feature in features)
            {
                this.ProcessFeature(feature);
            }
        }

        private void ProcessFeature(Type feature)
        {
            Console.WriteLine("found feature " + feature.Name + " (" + feature.FullName + ")");

            if (this.allFeatures.Any(x => x.Feature == feature))
            {
                return;
            }

            if (feature.ContainsGenericParameters)
            {
                Console.WriteLine($"skipping feature {feature.Name} because it contains generic parameters.");

                return;
            }

            var constructor = feature.GetConstructors().OrderBy(c => c.GetParameters().Length).FirstOrDefault();

            Feature instance;
            if (constructor != null)
            {
                object[] arguments = new object[constructor.GetParameters().Length];

                instance = (Feature)Activator.CreateInstance(feature, arguments);
            }
            else
            {
                instance = (Feature)Activator.CreateInstance(feature);
            }

            var factory = this.FindFactory(feature);
            var dependencies = this.FindDependencies(feature);

            this.allFeatures.Add(new FeatureInfo(feature, factory, dependencies));

            IList<Feature> neededFeatures = instance.NeededFeatures.ToList();
            foreach (Feature neededFeature in neededFeatures)
            {
                Console.WriteLine("needed features:");
                Console.WriteLine("- " + neededFeature.GetType().Name + "(" + neededFeature.GetType().FullName + ")");

                this.chain.Add(new Tuple<Type, Type>(feature, neededFeature.GetType()));

                this.ProcessFeature(neededFeature.GetType());
            }
        }

        private IEnumerable<Type> FindDependencies(Type feature)
        {
            var constructor = feature.GetConstructors().OrderBy(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor != null)
            {
                return constructor
                    .GetParameters()
                    .Select(a => GetGenericTypeOf(typeof(Dependency<>), a.ParameterType))
                    .Where(type => type != null).ToList();
            }

            return Enumerable.Empty<Type>();
        }

        private Type FindFactory(Type feature)
        {
            if (IsDirectSubclassOfRawGeneric(typeof(Feature<>), feature))
            {
                return feature.BaseType.GetGenericArguments()[0];
            }

            if (feature.BaseType == null)
            {
                return null;
            }

            return this.FindFactory(feature.BaseType);
        }

        private static Type GetGenericTypeOf(Type baseType, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == cur)
                {
                    return toCheck.GenericTypeArguments[0];
                }

                toCheck = toCheck.BaseType;
            }

            return null;
        }

        private static bool IsSubclassOfRawGeneric(Type baseType, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == cur)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }

        private static bool IsDirectSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            return toCheck.BaseType != null && generic == (toCheck.BaseType.IsGenericType ? toCheck.BaseType.GetGenericTypeDefinition() : toCheck.BaseType);
        }
    }
}