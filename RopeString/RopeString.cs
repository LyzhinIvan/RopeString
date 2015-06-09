using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RopeString
{
    public class RopeString
    {
        private bool _isLeaf;
        private string _value;
        private RopeString _leftChild;
        private RopeString _rightChild;
         
        public RopeString(string value = "")
        {
            _value = value;
            _isLeaf = true;
        }

        private RopeString(RopeString left, RopeString right)
        {
            _leftChild = left;
            _rightChild = right;
            _isLeaf = false;
        }

        public string Value
        {
            get {
                return _isLeaf ? _value : GetValue(_leftChild)+GetValue(_rightChild);
            }
        }

        private static string GetValue(RopeString rope)
        {
            return rope == null ? "" : rope.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static RopeString operator +(RopeString lhs, RopeString rhs)
        {
            return new RopeString(lhs, rhs);
        }
    }

    [TestFixture]
    public class RopeStringTests
    {
        [Test]
        public void EmptyConstructor()
        {
            var emptyRope = new RopeString();
            Assert.That(emptyRope.Value, Is.EqualTo(""));
        }

        [Test]
        [TestCase("")]
        [TestCase("a")]
        [TestCase("hello")]
        [TestCase("Hello World")]
        public void ConstructorTest(string str)
        {
            var rope = new RopeString(str);
            Assert.That(rope.Value, Is.EqualTo(str));
        }

        [Test]
        [TestCase("", "", Result = "")]
        [TestCase("hello", "", Result = "hello")]
        [TestCase("", "world", Result = "world")]
        [TestCase("hello", " world", Result = "hello world")]
        public string ConcatTest(string first, string second)
        {
            return (new RopeString(first) + new RopeString(second)).Value;
        }
    }
}
