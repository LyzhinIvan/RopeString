using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RopeString
{
    public class RopeString
    {
        #region Private members

        private bool _isLeaf;
        private string _value;
        private RopeString _leftChild;
        private RopeString _rightChild;
        private int _offset = 0;
        private int _length;

        #endregion

        #region Ctr

        public RopeString() : this("")
        {
        }

        public RopeString(string value)
        {
            _value = value;
            _isLeaf = true;
            _length = value.Length;
        }

        private RopeString(RopeString left, RopeString right)
        {
            _leftChild = left;
            _rightChild = right;
            _isLeaf = false;
            _length = GetLength(left) + GetLength(right);
        }

        #endregion

        #region Public fields

        public string Value
        {
            get { return ToString(); }
        }

        public int Length
        {
            get { return _length; }
        }

        #endregion

        #region Private methods

        private static string GetValue(RopeString rope)
        {
            return rope == null ? "" : rope.Value;
        }


        private static int GetLength(RopeString rope)
        {
            return rope == null ? 0 : rope.Length;
        }

        /// <summary>Разбивает строку на две части </summary>
        private void Split(int leftLength, out RopeString left, out RopeString right)
        {
            if (leftLength == 0)
            {
                left = new RopeString();
                right = this;
            }
            else if (leftLength >= Length)
            {
                left = this;
                right = new RopeString();
            }
            else
            {
                if (_isLeaf)
                {
                    left = new RopeString(this.Value);
                    left._length = leftLength;
                    right = new RopeString(this.Value);
                    right._offset = leftLength;
                    right._length = this.Length - leftLength;
                }
                else
                {
                    if (leftLength <= GetLength(_leftChild))
                    {
                        RopeString leftLeft, leftRight;
                        _leftChild.Split(leftLength, out leftLeft, out leftRight);
                        left = leftLeft;
                        right = leftRight + _rightChild;
                    }
                    else
                    {
                        RopeString rightLeft, rightRight;
                        _rightChild.Split(leftLength - GetLength(_leftChild), out rightLeft, out rightRight);
                        left = _leftChild + rightLeft;
                        right = rightRight;
                    }
                }
            }
        }

        #endregion

        #region Public methods

        public override string ToString()
        {
            return _isLeaf ? _value.Substring(_offset, _length) : GetValue(_leftChild) + GetValue(_rightChild);
        }

        /// <summary>Конкатенация строк</summary>
        public static RopeString operator +(RopeString lhs, RopeString rhs)
        {
            return new RopeString(lhs, rhs);
        }

        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException("Индекс находился вне границ строки");
                if (!_isLeaf)
                {
                    return index < GetLength(_leftChild)
                        ? _leftChild[index]
                        : _rightChild[index - GetLength(_leftChild)];
                }
                else
                    return _value[index];
            }
        }

        public RopeString Substring(int startIndex)
        {
            return Substring(startIndex, Length-startIndex);
        }

        public RopeString Substring(int startIndex, int length)
        {
            RopeString left, mid, right;
            this.Split(startIndex, out left, out right);
            right.Split(length, out mid, out right);
            return mid;
        }

        #endregion

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

        [Test]
        [TestCase("", 0)]
        [TestCase("hello", 5)]
        [TestCase("hello", -3)]
        public void ExceptionTest(string str, int index)
        {
            var rope = new RopeString(str);
            Assert.Throws<IndexOutOfRangeException>(() => { char ch = rope[index]; });
        }

        [Test]
        [TestCase("", 0, 0, Result = "")]
        [TestCase("hello", 0, 3, Result = "hel")]
        [TestCase("hello", 3, 2, Result = "lo")]
        [TestCase("hello", 1, 3, Result = "ell")]
        public string SubstringTest(string str, int startIndex, int length)
        {
            var rope = new RopeString(str);
            return rope.Substring(startIndex, length).Value;
        }
    }
}
