using System;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using FSharpPlusCSharp;

namespace FSharpPlusCSharp.Tests {
    [TestFixture]
    public class ResultTests {
        [Test]
        public void Match() {
            var a = Results.Ok<int, string>(1);
            var c = a.Match(i => 0, _ => 1);
            Assert.AreEqual(0, c);
        }

        [Test]
        public void MatchAction() {
            var a = Results.Ok<int, string>(1);
            a.Match(Console.WriteLine, _ => Assert.Fail("is string"));
        }

        [Test]
        public void New() {
            var a = Results.Ok<int, string>(1);
            var b = FSharpResult<int, string>.NewOk(1);
            Assert.AreEqual(a, b);

            var c = Results.Error<int, string>("a");
            var d = FSharpResult<int, string>.NewError("a");
            Assert.AreEqual(c, d);
        }

        [Test]
        public void Select() {
            var a = Results.Ok<int, string>(5);
            var b = a.Select(i => i + 2);
            b.Match(i => Assert.AreEqual(7, i), _ => Assert.Fail("is string"));
        }

        [Test]
        public void Select2() {
            var a = Results.Error<int, string>("hello");
            var b = a.Select(i => i + 2);
            b.Match(_ => Assert.Fail("is int"), s => Assert.AreEqual("hello", s));
        }

        [Test]
        public void Cast_OK() {
            object a = 40;
            Results.Cast<int>(a)
                .Match(i => Assert.AreEqual(40, i),
                       e => Assert.Fail(e.Message));
        }

        [Test]
        public void Cast_Exception() {
            object a = "hello";
            Results.Cast<int>(a)
                .Match(i => Assert.Fail("should not have succeeded with value {0}", i),
                       e => {});
        }

        [Test]
        public void ChoiceToOption() {
            object a = 40;
            const string b = "60";
            var r = from i in Options.ParseInt(b)
                    from j in Results.Cast<int>(a).ToOption()
                    select i + j;
            Assert.AreEqual(100.Some(), r);

        }

        [Test]
        public void OptionToChoice() {
            object a = 40;
            const string b = "60";
            var r = from i in Options.ParseInt(b).ToResult(new Exception())
                    from j in Results.Cast<int>(a)
                    select i + j;
            r.Match(i => Assert.AreEqual(100, i),
                    e => Assert.Fail(e.Message));
        }

        [Test]
        public void SelectSecond_OK() {
            object a = 40;
            const string b = "60";
            var r = from i in Options.ParseInt(b).ToResult("Invalid value b")
                    from j in Results.Cast<int>(a).SelectError(_ => "Invalid value a")
                    select i + j;
            r.Match(i => Assert.AreEqual(100, i),
                    Assert.Fail);
        }

        [Test]
        public void SelectSecond_Error() {
            object a = 40;
            const string b = "xx";
            var r = from i in Options.ParseInt(b).ToResult("Invalid value b")
                    from j in Results.Cast<int>(a).SelectError(_ => "Invalid value a")
                    select i + j;
            r.Match(i => Assert.Fail("should not have succeeded with value {0}", i),
                    e => Assert.AreEqual("Invalid value b", e));
        }
    }
}
