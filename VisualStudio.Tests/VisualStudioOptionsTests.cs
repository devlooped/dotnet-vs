﻿using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace VisualStudio.Tests
{
    public class VisualStudioOptionsTests
    {
        [Theory]
        [InlineData("", default)]
        [InlineData("rel", Channel.Release)]
        [InlineData("release", Channel.Release)]
        [InlineData("Release", Channel.Release)]
        [InlineData("--rel", Channel.Release)]
        [InlineData("--release", Channel.Release)]
        [InlineData("pre", Channel.Preview)]
        [InlineData("preview", Channel.Preview)]
        [InlineData("Preview", Channel.Preview)]
        [InlineData("--pre", Channel.Preview)]
        [InlineData("--preview", Channel.Preview)]
        [InlineData("int", Channel.IntPreview)]
        [InlineData("internal", Channel.IntPreview)]
        [InlineData("--int", Channel.IntPreview)]
        [InlineData("--internal", Channel.IntPreview)]
        [InlineData("main", Channel.Main)]
        [InlineData("--main", Channel.Main)]
        public void when_parsing_channel_argument_then_channel_is_set(string argument, Channel? expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithChannel();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Channel);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("e", Sku.Enterprise)]
        [InlineData("ent", Sku.Enterprise)]
        [InlineData("enterprise", Sku.Enterprise)]
        [InlineData("Ent", Sku.Enterprise)]
        [InlineData("Enterprise", Sku.Enterprise)]
        [InlineData("--sku=e", Sku.Enterprise)]
        [InlineData("--sku=ent", Sku.Enterprise)]
        [InlineData("--sku=enterprise", Sku.Enterprise)]
        [InlineData("--sku=Enterprise", Sku.Enterprise)]
        [InlineData("p", Sku.Professional)]
        [InlineData("pro", Sku.Professional)]
        [InlineData("professional", Sku.Professional)]
        [InlineData("Pro", Sku.Professional)]
        [InlineData("Professional", Sku.Professional)]
        [InlineData("--sku=p", Sku.Professional)]
        [InlineData("--sku=pro", Sku.Professional)]
        [InlineData("--sku=professional", Sku.Professional)]
        [InlineData("--sku=Professional", Sku.Professional)]
        [InlineData("c", Sku.Community)]
        [InlineData("com", Sku.Community)]
        [InlineData("community", Sku.Community)]
        [InlineData("Com", Sku.Community)]
        [InlineData("Community", Sku.Community)]
        [InlineData("--sku=c", Sku.Community)]
        [InlineData("--sku=com", Sku.Community)]
        [InlineData("--sku=community", Sku.Community)]
        [InlineData("--sku=Community", Sku.Community)]
        public void when_parsing_sku_argument_then_sku_is_set(string argument, Sku? expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithSku();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Sku);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("--nick=nick1", "nick1")]
        [InlineData("--nickname=nick2", "nick2")]
        public void when_parsing_nickname_argument_then_nickname_is_set(string argument, string expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithNickname();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Nickname);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("exp", true)]
        [InlineData("experimental", true)]
        [InlineData("--exp", true)]
        [InlineData("--experimental", true)]
        public void when_parsing_experimental_then_experimental_is_set(string argument, bool expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithExperimental();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.IsExperimental);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("--expr= x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("--expression= x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("/expr: x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("/expression: x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        public void when_parsing_expression_then_exppression_is_set(string argument, string expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithExpression();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Expression);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("all", true)]
        [InlineData("All", true)]
        [InlineData("--all", true)]
        public void when_parsing_all_argument_then_all_is_set(string argument, bool expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithSelectAll();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.All);
        }

        static (string[] Arguments, Func<VisualStudioOptions, bool> VerifyResult)[] TestCases =>
            new (string[] Arguments, Func<VisualStudioOptions, bool> VerifyResult)[]
            {
                (new [] { "enterprise" , "preview" }, x => x.Sku == Sku.Enterprise && x.Channel == Channel.Preview),
                (new [] { "main" , "exp" }, x => x.Channel == Channel.Main && x.IsExperimental),
                (new [] { "all", "exp" }, x => x.All && x.IsExperimental),
                (new [] { "ent", "main" }, x => x.Sku == Sku.Enterprise && x.Channel == Channel.Main),
                (new [] { "main", "x => x.InstanceId == '123'" }, x => x.Channel == Channel.Main && x.Expression == "x => x.InstanceId == \"123\""),
                (new [] { "pro" , "release", "--nick=foo" }, x => x.Sku == Sku.Professional && x.Channel == Channel.Release && x.Nickname == "foo"),
            };

        // Hack to use typed func and avoid to make VisualStudioOptions type public
        public static IEnumerable<object[]> TestCasesData =>
            TestCases.Select(x => new object[] { x.Arguments, (Func<object, bool>)(options => x.VerifyResult((VisualStudioOptions)options)) });

        [Theory]
        [MemberData(nameof(TestCasesData))]
        public void when_parsing_arguments_then_arguments_are_set(string[] args, Func<object, bool> verify)
        {
            var options = VisualStudioOptions.Full();

            options.Parse(args);

            Assert.True(verify(options));
        }
    }
}
