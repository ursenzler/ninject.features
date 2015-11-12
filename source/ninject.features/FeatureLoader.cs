// <copyright file="FeatureLoader.cs" company="Ninject.Features">
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

namespace Ninject.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Modules;

    public class FeatureLoader
    {
        private readonly IKernel kernel;

        public FeatureLoader(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public void Load(params Feature[] features)
        {
            var processor = new Processor(features);
            Result result = processor.GetDistinctModulesAndDependenciesFromFeatures();

            this.LoadExtensionsOntoKernel(result.Extensions);
            this.BindFactories(result.Features);
            this.LoadModulesOntoKernel(result.Modules);
            this.BindDependencies(result.Dependencies);

            this.CheckFactoryBindings(result.Factories);
        }

        private void LoadExtensionsOntoKernel(IEnumerable<INinjectModule> extensions)
        {
            this.kernel.Load(extensions);
        }

        private void BindFactories(IEnumerable<Feature> features)
        {
            foreach (var feature in features.OfType<IFactoryFeature>())
            {
                feature.BindFeatureFactory(this.kernel);
            }
        }

        private void LoadModulesOntoKernel(IEnumerable<INinjectModule> ninjectModules)
        {
            this.kernel.Load(ninjectModules);
        }

        private void BindDependencies(IEnumerable<Tuple<Dependency, Type>> dependencies)
        {
            foreach (Tuple<Dependency, Type> dependency in dependencies)
            {
                dependency.Item1.Bind(this.kernel, dependency.Item2);
            }
        }

        private void CheckFactoryBindings(IEnumerable<Type> factories)
        {
            foreach (var factory in factories)
            {
                var bindings = this.kernel.GetBindings(factory);

                if (!bindings.Any())
                {
                    throw new InvalidOperationException($"Missing binding for feature factory `{factory}`.");
                }
            }
        }

        private class Processor
        {
            private readonly List<INinjectModule> modules = new List<INinjectModule>();
            private readonly List<Tuple<Dependency, Type>> dependencies = new List<Tuple<Dependency, Type>>();
            private readonly List<INinjectModule> extensions = new List<INinjectModule>();
            private readonly List<Type> factories = new List<Type>();
            private readonly List<Feature> features = new List<Feature>();

            private readonly Queue<Feature> featureQueue;

            public Processor(IEnumerable<Feature> features)
            {
                this.featureQueue = new Queue<Feature>(features);
            }

            public Result GetDistinctModulesAndDependenciesFromFeatures()
            {
                while (this.featureQueue.Any())
                {
                    Feature feature = this.featureQueue.Dequeue();

                    this.AddFeature(feature);
                    this.AddExtensionsFrom(feature);
                    this.AddModulesFrom(feature);

                    var factoryBinder = feature as IFactoryFeature;
                    if (factoryBinder != null)
                    {
                        this.AddDependenciesFrom(factoryBinder);
                    }

                    this.EnqueueSubFeaturesFrom(feature);
                }

                return new Result(this.features, this.modules, this.dependencies, this.extensions, this.factories);
            }

            private void AddFeature(Feature feature)
            {
                if (this.NotAlreadyKnownFeature(feature))
                {
                    this.features.Add(feature);

                    var factoryFeature = feature as IFactoryFeature;
                    if (factoryFeature != null)
                    {
                        this.factories.Add(factoryFeature.FactoryType);
                    }
                }
            }

            private void AddExtensionsFrom(Feature feature)
            {
                foreach (INinjectModule extension in feature.NeededExtensions)
                {
                    if (this.NotAlreadyKnownExtension(extension))
                    {
                        this.extensions.Add(extension);
                    }
                }
            }

            private void AddModulesFrom(Feature feature)
            {
                foreach (INinjectModule module in feature.Modules)
                {
                    if (this.NotAlreadyKnownModule(module))
                    {
                        this.modules.Add(module);
                    }
                }
            }

            private void AddDependenciesFrom(IFactoryFeature feature)
            {
                foreach (Dependency dependency in feature.Dependencies)
                {
                    if (this.NotAlreadyKnownDepedency(dependency))
                    {
                        this.dependencies.Add(new Tuple<Dependency, Type>(dependency, feature.FactoryType));
                    }
                }
            }

            private void EnqueueSubFeaturesFrom(Feature feature)
            {
                foreach (Feature neededFeature in feature.NeededFeatures)
                {
                    this.featureQueue.Enqueue(neededFeature);
                }
            }

            private bool NotAlreadyKnownFeature(Feature feature)
            {
                return this.features.All(knownFeature => knownFeature.GetType() != feature.GetType());
            }

            private bool NotAlreadyKnownExtension(INinjectModule extension)
            {
                return this.extensions.All(knownExtension => knownExtension.GetType() != extension.GetType());
            }

            private bool NotAlreadyKnownModule(INinjectModule module)
            {
                return this.modules.All(knownModule => knownModule.GetType() != module.GetType());
            }

            private bool NotAlreadyKnownDepedency(Dependency dependency)
            {
                return this.dependencies.All(knownDependency => knownDependency.GetType() != dependency.GetType());
            }
        }

        private class Result
        {
            public Result(IEnumerable<Feature> features, IEnumerable<INinjectModule> modules, IEnumerable<Tuple<Dependency, Type>> dependencies, IEnumerable<INinjectModule> extensions, IEnumerable<Type> factories)
            {
                this.Features = features;
                this.Extensions = extensions;
                this.Factories = factories;
                this.Modules = modules;
                this.Dependencies = dependencies;
            }

            public IEnumerable<INinjectModule> Modules { get; }

            public IEnumerable<Tuple<Dependency, Type>> Dependencies { get; }

            public IEnumerable<Feature> Features { get; }

            public IEnumerable<INinjectModule> Extensions { get; }

            public IEnumerable<Type> Factories { get; }
        }
    }
}