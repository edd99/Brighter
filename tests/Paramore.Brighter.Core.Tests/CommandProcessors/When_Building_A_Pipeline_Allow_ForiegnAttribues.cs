﻿#region Licence
/* The MIT License (MIT)
Copyright © 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System.Linq;
using FluentAssertions;
using Paramore.Brighter.Core.Tests.CommandProcessors.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter.Extensions.DependencyInjection;
using Xunit;

namespace Paramore.Brighter.Core.Tests.CommandProcessors
{
    public class PipelineForiegnAttributesTests
    {
        private readonly PipelineBuilder<MyCommand> _pipelineBuilder;
        private IHandleRequests<MyCommand> _pipeline;

        public PipelineForiegnAttributesTests()
        {
            var registry = new SubscriberRegistry();
            registry.Register<MyCommand, MyObsoleteCommandHandler>();

            var container = new ServiceCollection();
            container.AddTransient<IHandleRequests<MyCommand>, MyObsoleteCommandHandler>();
            container.AddTransient<IHandleRequests<MyCommand>, MyValidationHandler<MyCommand>>();
            container.AddTransient<IHandleRequests<MyCommand>, MyLoggingHandler<MyCommand>>();

            var handlerFactory = new ServiceProviderHandlerFactory(container.BuildServiceProvider());
            
            _pipelineBuilder = new PipelineBuilder<MyCommand>(registry, (IAmAHandlerFactory)handlerFactory);
            PipelineBuilder<MyCommand>.ClearPipelineCache();
        }

        [Fact]
        public void When_Building_A_Pipeline_Allow_ForiegnAttribues()
        {
            _pipeline = _pipelineBuilder.Build(new RequestContext()).First();

            TraceFilters().ToString().Should().Be("MyValidationHandler`1|MyObsoleteCommandHandler|MyLoggingHandler`1|");
        }

        private PipelineTracer TraceFilters()
        {
            var pipelineTracer = new PipelineTracer();
            _pipeline.DescribePath(pipelineTracer);
            return pipelineTracer;
        }
    }
}
