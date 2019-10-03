using System;
using FSharpPlusCSharp;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace FSharpPlusCSharp.Tests {
    [TestFixture]
    public class ChoiceTests {
        [Test]
        public void Match() {
            var a = Choice.New1Of2<int, string>(1);
            var c = a.Match(i => 0, _ => 1);
            Assert.AreEqual(0, c);
        }

        [Test]
        public void MatchAction() {
            var a = Choice.New1Of2<int, string>(1);
            a.Match(Console.WriteLine, _ => Assert.Fail("is string"));
        }

        [Test]
        public void New() {
            var a = Choice.New1Of2<int, string>(1);
            var b = Choice.New1Of2<int, string>(1);
            Assert.AreEqual(a, b);

            var c = Choice.New2Of2<int, string>("a");
            var d = Choice.New2Of2<int, string>("a");
            Assert.AreEqual(c, d);
        }

        [Test]
        public void Select() {
            var a = Choice.New1Of2<int, string>(5);
            var b = a.Select(i => i + 2);
            b.Match(i => Assert.AreEqual(7, i), _ => Assert.Fail("is string"));
        }

        [Test]
        public void Select2() {
            var a = Choice.New2Of2<int, string>("hello");
            var b = a.Select(i => i + 2);
            b.Match(_ => Assert.Fail("is int"), s => Assert.AreEqual("hello", s));
        }

        private static int ThisThrows(int a) {
            throw new Exception("bad");
        }
    }
}