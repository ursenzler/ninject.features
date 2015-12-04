// <copyright file="ContextMock.cs" company="Ninject.Features">
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
    using Activation;
    using Parameters;
    using Planning;
    using Planning.Bindings;


    public class ContextMock : IContext
    {
        public ContextMock(IKernel kernel)
        {
            this.Kernel = kernel;
        }

        public IProvider GetProvider()
        {
            throw new NotImplementedException();
        }

        public object GetScope()
        {
            throw new NotImplementedException();
        }

        public object Resolve()
        {
            throw new NotImplementedException();
        }

        public IKernel Kernel { get; private set; }

        public IRequest Request
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBinding Binding
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IPlan Plan
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<IParameter> Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Type[] GenericArguments
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HasInferredGenericArguments
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
