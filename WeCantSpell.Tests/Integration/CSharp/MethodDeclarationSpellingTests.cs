﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class MethodDeclarationSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_methods_data
        {
            get
            {
                yield return new object[] { "STATIC", 125 };
                yield return new object[] { "METHOD", 132 };
                yield return new object[] { "set", 215 };
                yield return new object[] { "Timeout", 218 };
                yield return new object[] { "Internal", 322 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_methods_data))]
        public async Task can_find_mistakes_in_methods(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("MethodNames.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "MethodNames.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task does_not_find_mistakes_for_invocation()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("System", "Console", "Write", "Line"));
            var project = await ReadCodeFileAsProjectAsync("MethodNames.Invocation.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
