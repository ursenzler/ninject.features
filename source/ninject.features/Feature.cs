//-------------------------------------------------------------------------------
// <copyright file="Feature.cs" company="Ninject.Features">
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

namespace Ninject.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Ninject.Modules;

    public abstract class Feature
    {
        protected Feature(params Dependency[] dependencies)
        {
            this.Dependencies = dependencies;
        }

        public abstract Type FactoryType { get; }

        public virtual IEnumerable<Feature> NeededFeatures => Enumerable.Empty<Feature>();

        public virtual IEnumerable<INinjectModule> NeededExtensions { get; } = Enumerable.Empty<INinjectModule>();

        public virtual IEnumerable<INinjectModule> Modules { get; } = Enumerable.Empty<INinjectModule>();

        public IEnumerable<Dependency> Dependencies { get; }

        protected IEnumerable<INinjectModule> GetAssemblyNinjectModules(Assembly assembly)
        {
            return assembly.GetExportedTypes().Where(t =>
                typeof(INinjectModule).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                !t.IsInterface &&
                t.GetConstructor(Type.EmptyTypes) != null)
                .Select(type => (INinjectModule)Activator.CreateInstance(type));
        }
    }

    public abstract class Feature<TFeatureFactory> : Feature
    {
        protected Feature(params Dependency[] dependencies)
            : base(dependencies)
        {
        }

        public override Type FactoryType => typeof(TFeatureFactory);
    }
}