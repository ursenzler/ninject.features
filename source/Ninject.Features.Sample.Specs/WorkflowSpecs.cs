// <copyright file="WorkflowSpecs.cs" company="Ninject.Features">
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

namespace Ninject.Features.Sample.Specs
{
    using FluentAssertions;
    using Ninject.Features.Sample.TimeStamping;
    using Xbehave;

    public class WorkflowSpecs
    {
        [Scenario]
        public void ProcessDocument(
            WorkflowFeature.IWorkflowFactory factory,
            string output)
        {
            const string Input = "some text to process with ä, Ö and ü to replace.";
            const int MaxLineLength = 10;
            const int ExtraCharactersInLine = 5; // brackets, space + \r\n

            "establish the document workflow feature"._(() =>
                {
                    var timeProvider = new TransientTypeDependency<ITimeProvider, FakeTimeProvider>();

                    var kernel = new StandardKernel();
                    var loader = new FeatureLoader(kernel);

                    loader.Load(new WorkflowFeature(timeProvider));
                    factory = kernel.Get<WorkflowFeature.IWorkflowFactory>();
                });

            "when processing a document"._(() =>
                output = factory.CreateWorkflow().Process(Input, MaxLineLength));

            "it should replace all umlauts"._(() =>
                output
                    .Should().NotContain("ä")
                    .And.NotContain("ö")
                    .And.NotContain("ü"));

            "it should wrap lines"._(() =>
                output.GetLines()
                    .Should().OnlyContain(x => x.Length < MaxLineLength + FakeTimeProvider.FakeTime.Length + ExtraCharactersInLine));

            "it should stamp each line with local time"._(() =>
                output.GetLines()
                    .Should().OnlyContain(x => x.StartsWith($"[{FakeTimeProvider.FakeTime}] ")));
        }

        public class FakeTimeProvider : ITimeProvider
        {
            public const string FakeTime = "20:15";

            public string GetCurrentTime()
            {
                return FakeTime;
            }
        }
    }
}