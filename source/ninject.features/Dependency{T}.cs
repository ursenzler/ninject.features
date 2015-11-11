// <copyright file="Dependency{T}.cs" company="Ninject.Features">
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

    using Ninject.Activation;
    using Ninject.Syntax;

    public class Dependency<T> : Dependency
    {
        private readonly Action<IBindingToSyntax<T>> implementation;

        public Dependency(Action<IBindingToSyntax<T>> implementation)
        {
            this.implementation = implementation;
        }

        public override void Bind(IKernel kernel, Type factory)
        {
            var b = kernel.Bind<T>();

            this.implementation(b);

            var userCondition = b.BindingConfiguration.Condition ?? (r => true);

            b.BindingConfiguration.Condition = request => userCondition(request) && IsAnyAncestorTheFactory(request, factory);
        }

        private static bool IsAnyAncestorTheFactory(IRequest request, Type factory)
        {
            var parentContext = request.ParentContext;
            if (parentContext == null)
            {
                return false;
            }

            return
                parentContext.Request.Service == factory ||
                IsAnyAncestorTheFactory(parentContext.Request, factory);
        }
    }
}