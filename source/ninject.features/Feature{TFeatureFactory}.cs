// <copyright file="Feature{TFeatureFactory}.cs" company="Ninject.Features">
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

    using Ninject.Extensions.Factory;

    public abstract class Feature<TFeatureFactory> : Feature, IFactoryFeature
        where TFeatureFactory : class
    {
        protected Feature(params Dependency[] dependencies)
        {
            this.Dependencies = dependencies;
        }

        public Type FactoryType => typeof(TFeatureFactory);

        public IEnumerable<Dependency> Dependencies { get; }

        public virtual void BindFeatureFactory(IKernel kernel)
        {
            kernel.Bind<TFeatureFactory>().ToFactory(() => new TypeMatchingArgumentInheritanceInstanceProvider());
        }
    }
}