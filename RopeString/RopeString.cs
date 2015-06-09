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
            Length = value.Length;
        }

        private RopeString(RopeString left, RopeString right)
        {
            _leftChild = left;
            _rightChild = right;
            _isLeaf = false;
            Length = GetLength(left) + GetLength(right);
        }

        public string Value
        {
            get { return ToString(); }
        }

        public int Length { get; private set; }

        private static string GetValue(RopeString rope)
        {
            return rope == null ? "" : rope.Value;
        }

        private static int GetLength(RopeString rope)
        {
            return rope == null ? 0 : rope.Length;
        }

        public override string ToString()
        {
            return _isLeaf ? _value : GetValue(_leftChild) + GetValue(_rightChild);
        }

        public static RopeString operator +(RopeString lhs, RopeString rhs)
        {
            return new RopeString(lhs, rhs);
        }

        public char this[int index]
        {
            get
            {
                if(index<0 || index>=Length)
                    throw new IndexOutOfRangeException("Индекс находился вне границ строки");
                if (!_isLeaf)
                {
                    return index < GetLength(_leftChild) ? _leftChild[index] : _rightChild[index - GetLength(_leftChild)];
                }
                else
                    return _value[index];
            }
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

        [Test]
        [TestCase("")]
        [TestCase("hello")]
        [TestCase("hello world")]
        public void EnumerateTest(string str)
        {
            var rope = new RopeString(str);
            for (int i = 0; i < str.Length; ++i)
                Assert.That(rope[i], Is.EqualTo(str[i]));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("hello", "")]
        [TestCase("", "world")]
        [TestCase("hello ", "world")]
        public void EnumerateConcatedTest(string first, string second)
        {
            var str = first + second;
            var rope = new RopeString(first) + new RopeString(second);
            for (int i = 0; i < str.Length; ++i)
                Assert.That(rope[i], Is.EqualTo(str[i]));
        }
    }
}
