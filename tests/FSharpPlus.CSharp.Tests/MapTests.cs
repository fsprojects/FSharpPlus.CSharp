using System;
using System.Linq;
using FSharpPlusCSharp;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace FSharpPlusCSharp.Tests {
    [TestFixture]
    public class MapTests {
        [Test]
        public void New() {
            var m1 = FSharpMaps.Create(Tuple.Create(1, "one"), Tuple.Create(2, "two"));
            var m2 = new FSharpMap<int, string>(new[] {
                Tuple.Create(1, "one"), 
                Tuple.Create(2, "two"),
            });
            #pragma warning disable 219
            FSharpMap<int, string> m3 = new FSharpMap<int, string>(Enumerable.Empty<Tuple<int, string>>()) {
                { 1, "one" },
                { 2, "two" },
            };
            #pragma warning restore 219
            Assert.AreEqual(m1, m2);
        }

        [Test]
        public void TryFind() {
            var map = FSharpMaps.Create(Tuple.Create(1, "one"), Tuple.Create(2, "two"));
            var r = map.TryFind(2).Match(x => x, () => "not found");
            Assert.AreEqual("two", r);
        }
    }
}