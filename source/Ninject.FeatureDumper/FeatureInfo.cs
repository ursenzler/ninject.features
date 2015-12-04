// <copyright file="FeatureInfo.cs" company="Ninject.Features">
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
    public class FeatureInfo
    {
        private const string GenericIdentificator = "`1";
        private const string GenericSign = "<T>";

        private readonly System.Collections.Generic.List<FeatureInfo> dependenciesInfo;

        public FeatureInfo(System.Type feature, [NullGuard.AllowNull]System.Type factory, [NullGuard.AllowNull] System.Collections.Generic.IEnumerable<System.Type> dependencies, FeatureType featureType)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Feature = feature;
            this.Factory = factory;
            this.Dependencies = dependencies;
            this.FeatureType = featureType;
            this.dependenciesInfo = new System.Collections.Generic.List<FeatureInfo>();
            this.BindingTarget = Planning.Bindings.BindingTarget.Self;
        }

        public System.Type Feature { get; }

        public System.Type Factory { get; }

        public System.Collections.Generic.IEnumerable<System.Type> Dependencies { get; }

        public FeatureType FeatureType { get; private set; }

        public string Id { get; }

        public System.Collections.Generic.List<FeatureInfo> DependenciesInfo => this.dependenciesInfo;

        public Planning.Bindings.BindingTarget BindingTarget { get; private set; }

        public string AssemblyName
        {
            get
            {
                string assemblyName = this.Feature.ToString();
                if (this.IsGenericTyp(assemblyName))
                {
                    int pos = assemblyName.IndexOf(GenericIdentificator, System.StringComparison.Ordinal);
                    return assemblyName.Substring(0, pos) + GenericSign;
                }

                return assemblyName;
            }
        }

        public string Name
        {
            get
            {
                string name = this.Feature.Name;
                if (this.IsGenericTyp(name))
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

        public void AddDependency(System.Collections.Generic.List<FeatureInfo> dependencyList)
        {
            foreach (FeatureInfo dependency in dependencyList)
            {
                this.dependenciesInfo.Add(dependency);
            }
        }

        public void SetBindingType(Planning.Bindings.BindingTarget target)
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