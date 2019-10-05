using System;
using FSharpPlusCSharp;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace FSharpPlusCSharp.Tests {
    [TestFixture]
    public class ChoiceTests {
        [Test]
        public void Match() {
            var a = Choices.Create1Of2<int, string>(1);
            var c = a.Match(i => 0, _ => 1);
            Assert.AreEqual(0, c);
        }

        [Test]
        public void MatchAction() {
            var a = Choices.Create1Of2<int, string>(1);
            a.Match(Console.WriteLine, _ => Assert.Fail("is string"));
        }

        [Test]
        public void Create() {
            var a = Choices.Create1Of2<int, string>(1);
            var b = Choices.Create1Of2<int, string>(1);
            Assert.AreEqual(a, b);

            var c = Choices.Create2Of2<int, string>("a");
            var d = Choices.Create2Of2<int, string>("a");
            Assert.AreEqual(c, d);
        }
    }
}
