// <copyright file="TgfWriter.cs" company="Ninject.Features">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Appccelerate.IO;

    public class TgfWriter
    {
        public void WriteTgfFile(
            AbsoluteFilePath outputPath,
            Features features,
            bool includeFactories,
            bool includeDependencies)
        {
            using (var writer = new StreamWriter(outputPath))
            {
                var list = new List<FeatureInfo>(features.AllFeatures);
                foreach (var featureInfo in list)
                {
                    var feature = $"{list.IndexOf(featureInfo)} {featureInfo.Feature.Name}";

                    if (includeFactories && featureInfo.Factory != null)
                    {
                        feature += $" Factory = { featureInfo.Factory?.Name}";
                    }

                    if (includeDependencies && featureInfo.Dependencies.Any())
                    {
                        feature += $" Dependencies = { string.Join(", ", featureInfo.Dependencies.Select(d => d.Name))}";
                    }

                    writer.WriteLine(feature);
                }

                var index = list.Select(i => i.Feature).ToList();
                writer.WriteLine("#");

                foreach (var dependency in features.References)
                {
                    writer.WriteLine(
                        index.IndexOf(dependency.Feature) + " " + index.IndexOf(dependency.NeededFeature));
                }
            }
        }
    }
}