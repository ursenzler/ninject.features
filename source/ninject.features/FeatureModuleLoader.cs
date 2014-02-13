//-------------------------------------------------------------------------------
// <copyright file="FeatureModuleLoader.cs" company="Appccelerate">
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

namespace Ninject.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Modules;

    public class FeatureModuleLoader
    {
        private readonly IKernel kernel;

        public FeatureModuleLoader(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public void Load(params Feature[] features)
        {
            var processor = new Processor(features);
            Result result = processor.GetDistinctModulesAndDependenciesFromFeatures();

            this.LoadExtensionsOntoKernel(result.Extensions);
            this.LoadModulesOntoKernel(result.Modules);
            this.BindDependencies(result.Dependencies);
        }

        private void LoadExtensionsOntoKernel(IEnumerable<INinjectModule> extensions)
        {
            this.kernel.Load(extensions);
        }

        private void LoadModulesOntoKernel(IEnumerable<NinjectModule> ninjectModules)
        {
            this.kernel.Load(ninjectModules);
        }

        private void BindDependencies(IEnumerable<Dependency> dependencies)
        {
            foreach (Dependency dependency in dependencies)
            {
                dependency.Bind(this.kernel);
            }
        }

        private class Processor
        {
            private readonly List<Type> moduleTypes = new List<Type>();
            private readonly List<NinjectModule> modules = new List<NinjectModule>();
            private readonly List<Type> dependencyTypes = new List<Type>();
            private readonly List<Dependency> dependencies = new List<Dependency>();
            private readonly List<INinjectModule> extensions = new List<INinjectModule>();

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

                    this.AddExtensionsFrom(feature);
                    this.AddModulesFrom(feature);
                    this.AddDependenciesFrom(feature);

                    this.EnqueueSubFeaturesFrom(feature);
                }

                return new Result(this.modules, this.dependencies, this.extensions);
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
                foreach (NinjectModule module in feature.Modules)
                {
                    if (!this.moduleTypes.Contains(module.GetType()))
                    {
                        this.moduleTypes.Add(module.GetType());

                        this.modules.Add(module);
                    }
                }
            }

            private void AddDependenciesFrom(Feature feature)
            {
                foreach (Dependency dependency in feature.Dependencies)
                {
                    if (!this.dependencyTypes.Contains(dependency.GetType()))
                    {
                        this.dependencyTypes.Add(dependency.GetType());

                        this.dependencies.Add(dependency);
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

            private bool NotAlreadyKnownExtension(INinjectModule extension)
            {
                return this.extensions.All(knownExtension => knownExtension.GetType() != extension.GetType());
            }
        }

        private class Result
        {
            public Result(IEnumerable<NinjectModule> modules, IEnumerable<Dependency> dependencies, IEnumerable<INinjectModule> extensions)
            {
                this.Extensions = extensions;
                this.Modules = modules;
                this.Dependencies = dependencies;
            }

            public IEnumerable<NinjectModule> Modules { get; private set; }

            public IEnumerable<Dependency> Dependencies { get; private set; }

            public IEnumerable<INinjectModule> Extensions { get; private set; }
        }
    }
}