// <copyright file="Features.cs" company="Ninject.Features">
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

    public class Features
    {
        public Features(IReadOnlyCollection<FeatureInfo> allFeatures, IReadOnlyCollection<Reference> references)
        {
            this.AllFeatures = allFeatures;
            this.References = references;
        }

        public IReadOnlyCollection<FeatureInfo> AllFeatures { get; private set; }

        public IReadOnlyCollection<Reference> References { get; private set; }
    }

    public struct FeatureInfo
    {
        public FeatureInfo(Type feature, Type factory, IEnumerable<Type> dependencies)
        {
            this.Feature = feature;
            this.Factory = factory;
            this.Dependencies = dependencies;
        }

        public Type Feature { get; }

        public Type Factory { get; }

        public IEnumerable<Type> Dependencies { get; }
    }
}