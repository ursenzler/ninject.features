//-------------------------------------------------------------------------------
// <copyright file="TgfWriter.cs" company="Ninject.Features">
//   Copyright (c) 2013-2014
//
//   Licensed under the Apache License, Version 2.0 (the "License");
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
//-------------------------------------------------------------------------------

namespace Ninject.FeatureDumper
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Appccelerate.IO;

    public class TgfWriter
    {
        public void WriteTgfFile(AbsoluteFilePath outputPath, Features features)
        {
            using (var writer = new StreamWriter(outputPath))
            {
                var list = new List<Type>(features.AllFeatures);
                foreach (var type in list)
                {
                    writer.WriteLine(list.IndexOf(type) + " " + type.Name);
                }

                writer.WriteLine("#");

                foreach (var dependency in features.References)
                {
                    writer.WriteLine(
                        list.IndexOf(dependency.Feature) + " " + list.IndexOf(dependency.NeededFeature));
                }
            }
        }
    }
}