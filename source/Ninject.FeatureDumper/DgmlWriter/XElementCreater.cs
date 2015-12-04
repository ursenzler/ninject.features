// <copyright file="XElementCreater.cs" company="Ninject.Features">
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
    using System.Xml.Linq;

    public class XElementCreater
    {
        private readonly ColorSelector colorSelector;
        private readonly XNamespace nameSpace;

        public XElementCreater(XNamespace xNamespace, ColorSelector colorSelector)
        {
            this.nameSpace = xNamespace;
            this.colorSelector = colorSelector;
        }

        public XElement GenerateNode(FeatureInfo featureInfo)
        {
            string color = this.colorSelector.GetBackgroundColor(featureInfo.FeatureType);
            var node = new XElement(
                this.nameSpace + "Node",
                new XAttribute("Id", featureInfo.Id),
                new XAttribute("Label", featureInfo.Name),
                new XAttribute("Background", color),
                new XAttribute("Category", featureInfo.AssemblyName));

            return node;
        }

        public XElement GenerateLink(string sourceElementId, FeatureInfo targetElement, FeatureType featureType)
        {
            string color = this.colorSelector.GetLinkColor(featureType);

            XAttribute label =
                string.IsNullOrEmpty(targetElement.BindingTarget.ToString())
                ? new XAttribute("Label", featureType)
                : new XAttribute("Label", "Binding " + targetElement.BindingTarget);

            var link = new XElement(
                this.nameSpace + "Link",
                new XAttribute("Source", sourceElementId),
                new XAttribute("Target", targetElement.Id),
                new XAttribute("Stroke", color),
                new XAttribute("StrokeDashArray", "3"),
                label);

            return link;
        }
    }
}
