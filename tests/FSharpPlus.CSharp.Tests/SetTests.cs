using System;
using FSharpPlusCSharp;
using Microsoft.FSharp.Collections;
using NUnit.Framework;

namespace FSharpPlusCSharp.Tests {
    [TestFixture]
    public class SetTests {
        [Test]
        public void Create() {
            var set1 = FSharpSets.Create(1, 2, 3);
            var set2 = new FSharpSet<int>(new[] { 1, 2, 3 });
            Assert.AreEqual(set1, set2);
        }

        [Test]
        public void ToFSharpSet() {
            var set = new[] { 1, 2, 3 }.ToFSharpSet();
            Assert.AreEqual(3, set.Count);
        }
    }
}
