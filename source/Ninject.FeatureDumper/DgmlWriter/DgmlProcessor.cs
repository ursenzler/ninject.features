// <copyright file="DgmlProcessor.cs" company="Ninject.Features">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Appccelerate.IO;

    public class DgmlProcessor
    {
        private readonly List<FeatureInfo> workedFeatureInfo = new List<FeatureInfo>();
        private readonly List<XElement> nodes = new List<XElement>();
        private readonly List<XElement> links = new List<XElement>();

        private readonly XNamespace nameSpace = "http://schemas.microsoft.com/vs/2009/dgml";

        private readonly XElementCreater elementCreater;

        public DgmlProcessor()
        {
            this.elementCreater = new XElementCreater(this.nameSpace, new ColorSelector());
        }

        public void Write(Features features, AbsoluteFilePath fulldgmlFileName)
        {
            var list = new List<FeatureInfo>(features.AllFeatures);
            foreach (FeatureInfo featureInfo in list.OrderBy(x => x.FeatureType))
            {
                this.HandleNode(featureInfo, string.Empty);
            }

            this.WriteDgml(fulldgmlFileName);
        }

        private void HandleNode(FeatureInfo featureInfo, string sourceElementId)
        {
            FeatureInfo existingFeatureInfo = this.workedFeatureInfo.SingleOrDefault(x => x.UniqueName == featureInfo.UniqueName);
            if (existingFeatureInfo != null)
            {
                if (!string.IsNullOrEmpty(sourceElementId))
                {
                    XElement link = this.elementCreater.GenerateLink(sourceElementId, existingFeatureInfo, featureInfo.FeatureType);
                    this.links.Add(link);

                    this.HandleFeatureDependencies(featureInfo, existingFeatureInfo.Id);
                }
            }
            else
            {
                XElement node = this.elementCreater.GenerateNode(featureInfo);
                this.nodes.Add(node);

                XElement link = this.elementCreater.GenerateLink(sourceElementId, featureInfo, featureInfo.FeatureType);
                this.links.Add(link);

                this.workedFeatureInfo.Add(featureInfo);

                HandleFeatureDependencies(featureInfo, featureInfo.Id);
            }
        }

        private void HandleFeatureDependencies(FeatureInfo featureInfo, string sourceElementId)
        {
            foreach (FeatureInfo dependency in featureInfo.DependenciesInfo)
            {
                HandleNode(dependency, sourceElementId);
            }
        }

        private void WriteDgml(AbsoluteFilePath fulldgmlFileName)
        {
            var documentFinal = new XDocument(
                new XDeclaration("1.0", "utf-8", string.Empty),
                new XElement(
                    this.nameSpace + "DirectedGraph",
                    new XElement(this.nameSpace + "Nodes", this.nodes),
                    new XElement(this.nameSpace + "Links", this.links)));

            documentFinal.Save(fulldgmlFileName);
        }
    }
}
