// <copyright file="ColorSelector.cs" company="Ninject.Features">
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

namespace Ninject.FeatureDumper.DgmlWriter
{
    public class ColorSelector
    {
        public string GetBackgroundColor(FeatureType featureType)
        {
            DgmlColorConfiguration dgmlColor = new DgmlColorConfiguration();
            string color = string.Empty;
            switch (featureType)
            {
                case FeatureType.Feature:
                case FeatureType.NeededFeature:
                    color = dgmlColor.FeatureColor;
                    break;
                case FeatureType.NeededExtensions:
                case FeatureType.Module:
                    color = dgmlColor.ModuleColor;
                    break;
                case FeatureType.BindingInterface:
                    color = dgmlColor.BindingInterfaceColor;
                    break;
                case FeatureType.BindingImpl:
                    color = dgmlColor.BindingImplColor;
                    break;
            }

            return color;
        }

        public string GetLinkColor(FeatureType featureType)
        {
            DgmlColorConfiguration dgmlColor = new DgmlColorConfiguration();
            string color = string.Empty;
            switch (featureType)
            {
                case FeatureType.Feature:
                case FeatureType.NeededFeature:
                    color = dgmlColor.FeatureColor;
                    break;
                case FeatureType.NeededExtensions:
                    color = dgmlColor.NeededExtensionsColor;
                    break;
                case FeatureType.Module:
                    color = dgmlColor.ModuleColor;
                    break;
                case FeatureType.BindingInterface:
                    color = dgmlColor.BindingInterfaceColor;
                    break;
                case FeatureType.BindingImpl:
                    color = dgmlColor.BindingImplColor;
                    break;
            }

            return color;
        }
    }
}