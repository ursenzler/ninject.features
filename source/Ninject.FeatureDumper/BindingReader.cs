// <copyright file="BindingReader.cs" company="Ninject.Features">
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
    using Activation;
    using Infrastructure;
    using Modules;
    using Planning.Bindings;

    public class BindingReader
    {
        public List<FeatureInfo> GetBindingInformations(INinjectModule module)
        {
            List<FeatureInfo> bindingsFeatureInfoList = new List<FeatureInfo>();

            IKernel kernel = new StandardKernel();
            module.OnLoad(kernel);

            List<KeyValuePair<Type, ICollection<IBinding>>> typeBindingInfo = this.GetBindings(kernel, module.Name);

            foreach (KeyValuePair<Type, ICollection<IBinding>> typeBinding in typeBindingInfo)
            {
                FeatureInfo bindingInterface = new FeatureInfo(typeBinding.Key, null, null, FeatureType.BindingInterface);

                foreach (IBinding binding in typeBinding.Value)
                {
                    ContextMock contextMock = new ContextMock(kernel);

                    IProvider provider = binding.ProviderCallback(contextMock);

                    FeatureInfo bindingInterfaceImpl = new FeatureInfo(provider.Type, null, null, FeatureType.BindingImpl);
                    bindingInterfaceImpl.SetBindingType(binding.Target);

                    bindingInterface.AddDependency(bindingInterfaceImpl);
                }

                bindingsFeatureInfoList.Add(bindingInterface);
            }

            return bindingsFeatureInfoList;
        }

        private List<KeyValuePair<Type, ICollection<IBinding>>> GetBindings(IKernel kernel, string moduleName)
        {
            string firstPartofModuleName = moduleName.Substring(0, moduleName.IndexOfAny(new[] { '.' }) + 1);

            List<KeyValuePair<Type, ICollection<IBinding>>> returnvalue =
                ((Multimap<Type, IBinding>)typeof(KernelBase).GetField("bindings", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(kernel))
                .Where(k => k.Key.FullName.StartsWith(firstPartofModuleName))
                .ToList();

            return returnvalue;
        }
    }
}