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
    using Planning.Bindings;
    using NullGuard;

    public enum FeatureType
    {
        Feature,
        NeededFeature,
        NeededExtensions,
        Module,
        BindingInterface,
        BindingImpl
    }

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

    public class FeatureInfo
    {
        private readonly List<FeatureInfo> dependenciesInfo;
        const string GenericIdentificator = "`1";
        const string GenericSign = "<T>";

        public FeatureInfo(Type feature, [AllowNull]Type factory, [AllowNull]IEnumerable<Type> dependencies, FeatureType featureType)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Feature = feature;
            this.Factory = factory;
            this.Dependencies = dependencies;
            this.FeatureType = featureType;
            this.dependenciesInfo = new List<FeatureInfo>();
            this.BindingTarget = BindingTarget.Self;
        }

        public Type Feature { get; }

        public Type Factory { get; }

        public IEnumerable<Type> Dependencies { get; }

        public FeatureType FeatureType { get; private set; }

        public string Id { get; }

        public List<FeatureInfo> DependenciesInfo => this.dependenciesInfo;

        public BindingTarget BindingTarget { get; private set; }

        public string AssemblyName
        {
            get
            {
                string assemblyName = this.Feature.ToString();
                if (IsGenericTyp(assemblyName))
                {
                    int pos = assemblyName.IndexOf(GenericIdentificator, StringComparison.Ordinal);
                    return assemblyName.Substring(0, pos) + GenericSign;

                }

                return assemblyName;
            }
        }

        public string Name
        {
            get
            {
                string name = Feature.Name;
                if (IsGenericTyp(name))
                {
                    return name.Replace(GenericIdentificator, GenericSign);
                }

                return name;
            }
        }

        public string UniqueName => this.Name + "." + this.AssemblyName;

        public void AddDependency(FeatureInfo dependency)
        {
            this.dependenciesInfo.Add(dependency);
        }

        public void AddDependency(List<FeatureInfo> dependencyList)
        {
            foreach (FeatureInfo dependency in dependencyList)
            {
                this.dependenciesInfo.Add(dependency);
            }
        }

        public void SetBindingType(BindingTarget target)
        {
            this.BindingTarget = target;
        }

        public void ChangeFeatureType(FeatureType featureType)
        {
            this.FeatureType = featureType;
        }

        private bool IsGenericTyp(string name)
        {
            return name.Contains(GenericIdentificator);
        }
    }
}