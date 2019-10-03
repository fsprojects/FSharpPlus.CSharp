using System;
using NUnit.Framework;
using FSharpPlusCSharp;

namespace FSharpPlusCSharp.Tests {
    [TestFixture]
    public class ArrayTests {
        [Test]
        public void Does_not_conflict_with_System_Array() {
            var a = new[] {1,2,3};
            Array.Clear(a, 0, 0);
        }
    }
}
