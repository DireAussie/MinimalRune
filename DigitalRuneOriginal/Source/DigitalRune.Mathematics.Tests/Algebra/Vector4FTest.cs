using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace MinimalRune.Mathematics.Algebra.Tests
{
  [TestFixture]
  public class Vector4Test
  {
    [Test]
    public void Constructors()
    {
      Vector4 v = new Vector4();
      Assert.AreEqual(0.0f, v.X);
      Assert.AreEqual(0.0f, v.Y);
      Assert.AreEqual(0.0f, v.Z);
      Assert.AreEqual(0.0f, v.W);

      v = new Vector4(2.3f);
      Assert.AreEqual(2.3f, v.X);
      Assert.AreEqual(2.3f, v.Y);
      Assert.AreEqual(2.3f, v.Z);
      Assert.AreEqual(2.3f, v.W);

      v = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);

      v = new Vector4(new[] { 1.0f, 2.0f, 3.0f, 4.0f });
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);

      v = new Vector4(new List<float>(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);

      v = new Vector4(new Vector3(1.0f, 2.0f, 3.0f), 4.0f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);
    }


    [Test]
    public void Properties()
    {
      Vector4 v = new Vector4();
      v.X = 1.0f;
      v.Y = 2.0f;
      v.Z = 3.0f;
      v.W = 4.0f;
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);
      Assert.AreEqual(new Vector4(1.0f, 2.0f, 3.0f, 4.0f), v);
    }


    [Test]
    public void HashCode()
    {
      Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreNotEqual(Vector4.One.GetHashCode(), v.GetHashCode());
    }


    [Test]
    public void EqualityOperators()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 c = new Vector4(-1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 d = new Vector4(1.0f, -2.0f, 3.0f, 4.0f);
      Vector4 e = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
      Vector4 f = new Vector4(1.0f, 2.0f, 3.0f, -4.0f);

      Assert.IsTrue(a == b);
      Assert.IsFalse(a == c);
      Assert.IsFalse(a == d);
      Assert.IsFalse(a == e);
      Assert.IsFalse(a == f);
      Assert.IsFalse(a != b);
      Assert.IsTrue(a != c);
      Assert.IsTrue(a != d);
      Assert.IsTrue(a != e);
      Assert.IsTrue(a != f);
    }


    [Test]
    public void ComparisonOperators()
    {
      Vector4 a = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
      Vector4 b = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
      Vector4 c = new Vector4(1.0f, 0.5f, 0.5f, 0.5f);
      Vector4 d = new Vector4(0.5f, 1.0f, 0.5f, 0.5f);
      Vector4 e = new Vector4(0.5f, 0.5f, 1.0f, 0.5f);
      Vector4 f = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

      Assert.IsTrue(a > b);
      Assert.IsFalse(a > c);
      Assert.IsFalse(a > d);
      Assert.IsFalse(a > e);
      Assert.IsFalse(a > f);

      Assert.IsTrue(b < a);
      Assert.IsFalse(c < a);
      Assert.IsFalse(d < a);
      Assert.IsFalse(e < a);
      Assert.IsFalse(f < a);

      Assert.IsTrue(a >= b);
      Assert.IsTrue(a >= c);
      Assert.IsTrue(a >= d);
      Assert.IsTrue(a >= e);
      Assert.IsTrue(a >= f);

      Assert.IsFalse(b >= a);
      Assert.IsFalse(b >= c);
      Assert.IsFalse(b >= d);
      Assert.IsFalse(b >= e);
      Assert.IsFalse(b >= f);

      Assert.IsTrue(b <= a);
      Assert.IsTrue(c <= a);
      Assert.IsTrue(d <= a);
      Assert.IsTrue(e <= a);
      Assert.IsTrue(f <= a);

      Assert.IsFalse(a <= b);
      Assert.IsFalse(c <= b);
      Assert.IsFalse(d <= b);
      Assert.IsFalse(e <= b);
      Assert.IsFalse(f <= b);
    }


    [Test]
    public void AreEqual()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Vector4 u = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 v = new Vector4(1.000001f, 2.000001f, 3.000001f, 4.000001f);
      Vector4 w = new Vector4(1.00000001f, 2.00000001f, 3.00000001f, 4.00000001f);

      Assert.IsTrue(Vector4.AreNumericallyEqual(u, u));
      Assert.IsFalse(Vector4.AreNumericallyEqual(u, v));
      Assert.IsTrue(Vector4.AreNumericallyEqual(u, w));

      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void AreEqualWithEpsilon()
    {
      float epsilon = 0.001f;
      Vector4 u = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 v = new Vector4(1.002f, 2.002f, 3.002f, 4.002f);
      Vector4 w = new Vector4(1.0001f, 2.0001f, 3.0001f, 4.0001f);

      Assert.IsTrue(Vector4.AreNumericallyEqual(u, u, epsilon));
      Assert.IsFalse(Vector4.AreNumericallyEqual(u, v, epsilon));
      Assert.IsTrue(Vector4.AreNumericallyEqual(u, w, epsilon));
    }


    [Test]
    public void IsNumericallyZero()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Vector4 u = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
      Vector4 v = new Vector4(1e-9f, -1e-9f, 1e-9f, 1e-9f);
      Vector4 w = new Vector4(1e-7f, 1e-7f, -1e-7f, 1e-7f);

      Assert.IsTrue(u.IsNumericallyZero);
      Assert.IsTrue(v.IsNumericallyZero);
      Assert.IsFalse(w.IsNumericallyZero);

      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void TestEquals()
    {
      Vector4 v0 = new Vector4(678.0f, 234.8f, -123.987f, 4.0f);
      Vector4 v1 = new Vector4(678.0f, 234.8f, -123.987f, 4.0f);
      Vector4 v2 = new Vector4(67.0f, 234.8f, -123.987f, 4.0f);
      Vector4 v3 = new Vector4(678.0f, 24.8f, -123.987f, 4.0f);
      Vector4 v4 = new Vector4(678.0f, 234.8f, 123.987f, 4.0f);
      Vector4 v5 = new Vector4(678.0f, 234.8f, 123.987f, 4.1f);
      Assert.IsTrue(v0.Equals(v0));
      Assert.IsTrue(v0.Equals(v1));
      Assert.IsFalse(v0.Equals(v2));
      Assert.IsFalse(v0.Equals(v3));
      Assert.IsFalse(v0.Equals(v4));
      Assert.IsFalse(v0.Equals(v5));
      Assert.IsFalse(v0.Equals(v0.ToString()));
    }


    [Test]
    public void AdditionOperator()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
      Vector4 c = a + b;
      Assert.AreEqual(new Vector4(3.0f, 5.0f, 7.0f, 9.0f), c);
    }


    [Test]
    public void Addition()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
      Vector4 c = Vector4.Add(a, b);
      Assert.AreEqual(new Vector4(3.0f, 5.0f, 7.0f, 9.0f), c);
    }


    [Test]
    public void SubtractionOperator()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(10.0f, -10.0f, 0.5f, 2.5f);
      Vector4 c = a - b;
      Assert.AreEqual(new Vector4(-9.0f, 12.0f, 2.5f, 1.5f), c);
    }


    [Test]
    public void Subtraction()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(10.0f, -10.0f, 0.5f, 2.5f);
      Vector4 c = Vector4.Subtract(a, b);
      Assert.AreEqual(new Vector4(-9.0f, 12.0f, 2.5f, 1.5f), c);
    }


    [Test]
    public void MultiplicationOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 6.0f;
      float w = 0.4f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);

      Vector4 u = v * s;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
      Assert.AreEqual(w * s, u.W);

      u = s * v;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
      Assert.AreEqual(w * s, u.W);
    }


    [Test]
    public void Multiplication()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 6.0f;
      float w = 0.4f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);

      Vector4 u = Vector4.Multiply(s, v);
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
      Assert.AreEqual(w * s, u.W);
    }


    [Test]
    public void MultiplicationOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 * x2, y1 * y2, z1 * z2, w1 * w2), v * w);
    }


    [Test]
    public void MultiplicationVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 * x2, y1 * y2, z1 * z2, w1 * w2), Vector4.Multiply(v, w));
    }


    [Test]
    public void DivisionOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 7.0f;
      float w = 1.9f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);
      Vector4 u = v / s;
      Assert.IsTrue(Numeric.AreEqual(x / s, u.X));
      Assert.IsTrue(Numeric.AreEqual(y / s, u.Y));
      Assert.IsTrue(Numeric.AreEqual(z / s, u.Z));
      Assert.IsTrue(Numeric.AreEqual(w / s, u.W));
    }


    [Test]
    public void Division()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 7.0f;
      float w = 1.9f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);
      Vector4 u = Vector4.Divide(v, s);
      Assert.IsTrue(Numeric.AreEqual(x / s, u.X));
      Assert.IsTrue(Numeric.AreEqual(y / s, u.Y));
      Assert.IsTrue(Numeric.AreEqual(z / s, u.Z));
      Assert.IsTrue(Numeric.AreEqual(w / s, u.W));
    }


    [Test]
    public void DivisionVectorOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 / x2, y1 / y2, z1 / z2, w1 / w2), v / w);
    }


    [Test]
    public void DivisionVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 / x2, y1 / y2, z1 / z2, w1 / w2), Vector4.Divide(v, w));
    }


    [Test]
    public void NegationOperator()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreEqual(new Vector4(-1.0f, -2.0f, -3.0f, -4.0f), -a);
    }


    [Test]
    public void Negation()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreEqual(new Vector4(-1.0f, -2.0f, -3.0f, -4.0f), Vector4.Negate(a));
    }


    [Test]
    public void Constants()
    {
      Assert.AreEqual(0.0f, Vector4.Zero.X);
      Assert.AreEqual(0.0f, Vector4.Zero.Y);
      Assert.AreEqual(0.0f, Vector4.Zero.Z);
      Assert.AreEqual(0.0f, Vector4.Zero.W);

      Assert.AreEqual(1.0f, Vector4.One.X);
      Assert.AreEqual(1.0f, Vector4.One.Y);
      Assert.AreEqual(1.0f, Vector4.One.Z);
      Assert.AreEqual(1.0f, Vector4.One.W);

      Assert.AreEqual(1.0f, Vector4.UnitX.X);
      Assert.AreEqual(0.0f, Vector4.UnitX.Y);
      Assert.AreEqual(0.0f, Vector4.UnitX.Z);
      Assert.AreEqual(0.0f, Vector4.UnitX.W);

      Assert.AreEqual(0.0f, Vector4.UnitY.X);
      Assert.AreEqual(1.0f, Vector4.UnitY.Y);
      Assert.AreEqual(0.0f, Vector4.UnitY.Z);
      Assert.AreEqual(0.0f, Vector4.UnitX.W);

      Assert.AreEqual(0.0f, Vector4.UnitZ.X);
      Assert.AreEqual(0.0f, Vector4.UnitZ.Y);
      Assert.AreEqual(1.0f, Vector4.UnitZ.Z);
      Assert.AreEqual(0.0f, Vector4.UnitX.W);

      Assert.AreEqual(0.0f, Vector4.UnitW.X);
      Assert.AreEqual(0.0f, Vector4.UnitW.Y);
      Assert.AreEqual(0.0f, Vector4.UnitW.Z);
      Assert.AreEqual(1.0f, Vector4.UnitW.W);
    }


    [Test]
    public void IndexerRead()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      Assert.AreEqual(1.0f, v[0]);
      Assert.AreEqual(-10e10f, v[1]);
      Assert.AreEqual(0.0f, v[2]);
      Assert.AreEqual(0.3f, v[3]);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      Assert.AreEqual(1.0f, v[-1]);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException2()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      Assert.AreEqual(1.0f, v[4]);
    }


    [Test]
    public void IndexerWrite()
    {
      Vector4 v = Vector4.Zero;
      v[0] = 1.0f;
      v[1] = -10e10f;
      v[2] = 0.1f;
      v[3] = 0.3f;
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(-10e10f, v.Y);
      Assert.AreEqual(0.1f, v.Z);
      Assert.AreEqual(0.3f, v.W);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      v[-1] = 0.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException2()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      v[4] = 0.0f;
    }


    [Test]
    public void IsNaN()
    {
      const int numberOfRows = 4;
      Assert.IsFalse(new Vector4().IsNaN);

      for (int i = 0; i < numberOfRows; i++)
      {
        Vector4 v = new Vector4();
        v[i] = float.NaN;
        Assert.IsTrue(v.IsNaN);
      }
    }


    [Test]
    public void IsNormalized()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-7f;
      Vector4 arbitraryVector = new Vector4(1.0f, 0.001f, 0.001f, 0.001f);
      Assert.IsFalse(arbitraryVector.IsNumericallyNormalized);

      Vector4 normalizedVector = new Vector4(1.00000001f, 0.00000001f, 0.000000001f, 0.000000001f);
      Assert.IsTrue(normalizedVector.IsNumericallyNormalized);
      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void TestIsZero()
    {
      Vector4 nonZero = new Vector4(0.001f, 0.001f, 0.0f, 0.001f);
      Assert.IsFalse(Vector4.AreNumericallyEqual(nonZero, Vector4.Zero));

      Vector4 zero = new Vector4(0.0000001f, 0.0000001f, 0.0f, 0.0000001f);
      Assert.IsTrue(Vector4.AreNumericallyEqual(zero, Vector4.Zero));
    }


    [Test]
    public void Length()
    {
      Assert.AreEqual(1.0f, Vector4.UnitX.Length);
      Assert.AreEqual(1.0f, Vector4.UnitY.Length);
      Assert.AreEqual(1.0f, Vector4.UnitZ.Length);
      Assert.AreEqual(1.0f, Vector4.UnitW.Length);

      float x = -1.9f;
      float y = 2.1f;
      float z = 10.0f;
      float w = 1.0f;
      float length = (float)Math.Sqrt(x * x + y * y + z * z + w * w);
      Vector4 v = new Vector4(x, y, z, w);
      Assert.AreEqual(length, v.Length);
    }


    [Test]
    public void Length2()
    {
      Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      v.Length = 0.3f;
      Assert.IsTrue(Numeric.AreEqual(0.3f, v.Length));
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void LengthException()
    {
      Vector4 v = Vector4.Zero;
      v.Length = 0.5f;
    }


    [Test]
    public void LengthSquared()
    {
      Assert.AreEqual(1.0f, Vector4.UnitX.LengthSquared());
      Assert.AreEqual(1.0f, Vector4.UnitY.LengthSquared());
      Assert.AreEqual(1.0f, Vector4.UnitZ.LengthSquared());
      Assert.AreEqual(1.0f, Vector4.UnitW.LengthSquared());

      float x = -1.9f;
      float y = 2.1f;
      float z = 10.0f;
      float w = 1.0f;
      float length = x * x + y * y + z * z + w * w;
      Vector4 v = new Vector4(x, y, z, w);
      Assert.AreEqual(length, v.LengthSquared());
    }


    [Test]
    public void Normalized()
    {
      Vector4 v = new Vector4(3.0f, -1.0f, 23.0f, 0.4f);
      Vector4 normalized = v.Normalized;
      Assert.AreEqual(new Vector4(3.0f, -1.0f, 23.0f, 0.4f), v);
      Assert.IsFalse(v.IsNumericallyNormalized);
      Assert.IsTrue(normalized.IsNumericallyNormalized);
    }


    [Test]
    public void Normalize()
    {
      Vector4 v = new Vector4(3.0f, -1.0f, 23.0f, 0.4f);
      v.Normalize();
      Assert.IsTrue(v.IsNumericallyNormalized);
    }


    [Test]
    [ExpectedException(typeof(DivideByZeroException))]
    public void NormalizeException()
    {
      Vector4.Zero.Normalize();
    }


    [Test]
    public void TryNormalize()
    {
      Vector4 v = Vector4.Zero;
      bool normalized = v.TryNormalize();
      Assert.IsFalse(normalized);

      v = new Vector4(1, 2, 3, 4);
      normalized = v.TryNormalize();
      Assert.IsTrue(normalized);
      Assert.AreEqual(new Vector4(1, 2, 3, 4).Normalized, v);

      v = new Vector4(0, -1, 0, 0);
      normalized = v.TryNormalize();
      Assert.IsTrue(normalized);
      Assert.AreEqual(new Vector4(0, -1, 0, 0).Normalized, v);
    }


    [Test]
    public void DotProduct()
    {
      // 0�
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitX, Vector4.UnitX));
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitY, Vector4.UnitY));
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitZ, Vector4.UnitZ));
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitW, Vector4.UnitW));

      // 180�
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitX, -Vector4.UnitX));
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitY, -Vector4.UnitY));
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitZ, -Vector4.UnitZ));
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitW, -Vector4.UnitW));

      // 90�
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitX, Vector4.UnitY));
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitY, Vector4.UnitZ));
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitZ, Vector4.UnitW));
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitW, Vector4.UnitX));

      // 45�
      float angle = (float)Math.Acos(Vector4.Dot(new Vector4(1, 1, 0, 0).Normalized, Vector4.UnitX));
      Assert.IsTrue(Numeric.AreEqual(MathHelper.ToRadians(45), angle));
      angle = (float)Math.Acos(Vector4.Dot(new Vector4(0, 1, 1, 0).Normalized, Vector4.UnitY));
      Assert.IsTrue(Numeric.AreEqual(MathHelper.ToRadians(45), angle));
      angle = (float)Math.Acos(Vector4.Dot(new Vector4(1, 0, 1, 0).Normalized, Vector4.UnitZ));
      Assert.IsTrue(Numeric.AreEqual(MathHelper.ToRadians(45), angle));
      angle = (float)Math.Acos(Vector4.Dot(new Vector4(1, 0, 0, 1).Normalized, Vector4.UnitW));
      Assert.IsTrue(Numeric.AreEqual(MathHelper.ToRadians(45), angle));
    }


    [Test]
    public void XYZ()
    {
      Vector4 v4 = new Vector4(1.0f, 2.0f, 3.0f, 5.0f);
      Vector3 v3 = v4.XYZ;
      Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), v3);

      v4.XYZ = new Vector3(0.1f, 0.2f, 0.3f);
      Assert.AreEqual(0.1f, v4.X);
      Assert.AreEqual(0.2f, v4.Y);
      Assert.AreEqual(0.3f, v4.Z);
    }


    [Test]
    public void HomogeneousDivide()
    {
      Vector4 v4 = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);
      Vector3 v3 = Vector4.HomogeneousDivide(v4);
      Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), v3);

      v4 = new Vector4(1.0f, 2.0f, 3.0f, 10.0f);
      v3 = Vector4.HomogeneousDivide(v4);
      Assert.AreEqual(new Vector3(1.0f / 10.0f, 2.0f / 10.0f, 3.0f / 10.0f), v3);
    }


    [Test]
    public void ImplicitCastToVectorF()
    {
      Vector4 v = new Vector4(1.1f, 2.2f, 3.3f, 4.4f);
      VectorF v2 = v;

      Assert.AreEqual(4, v2.NumberOfElements);
      Assert.AreEqual(1.1f, v2[0]);
      Assert.AreEqual(2.2f, v2[1]);
      Assert.AreEqual(3.3f, v2[2]);
      Assert.AreEqual(4.4f, v2[3]);
    }


    [Test]
    public void ToVectorF()
    {
      Vector4 v = new Vector4(1.1f, 2.2f, 3.3f, 4.4f);
      VectorF v2 = v.ToVectorF();

      Assert.AreEqual(4, v2.NumberOfElements);
      Assert.AreEqual(1.1f, v2[0]);
      Assert.AreEqual(2.2f, v2[1]);
      Assert.AreEqual(3.3f, v2[2]);
      Assert.AreEqual(4.4f, v2[3]);
    }


    [Test]
    public void ExplicitFromXnaCast()
    {
      Vector4 xna = new Vector4(6, 7, 8, 9);
      Vector4 v = (Vector4)xna;

      Assert.AreEqual(xna.X, v.X);
      Assert.AreEqual(xna.Y, v.Y);
      Assert.AreEqual(xna.Z, v.Z);
      Assert.AreEqual(xna.W, v.W);
    }


    [Test]
    public void FromXna()
    {
      Vector4 xna = new Vector4(6, 7, 8, 9);
      Vector4 v = Vector4.FromXna(xna);

      Assert.AreEqual(xna.X, v.X);
      Assert.AreEqual(xna.Y, v.Y);
      Assert.AreEqual(xna.Z, v.Z);
      Assert.AreEqual(xna.W, v.W);
    }


    [Test]
    public void ExplicitToXnaCast()
    {
      Vector4 v = new Vector4(6, 7, 8, 9);
      Vector4 xna = (Vector4)v;

      Assert.AreEqual(xna.X, v.X);
      Assert.AreEqual(xna.Y, v.Y);
      Assert.AreEqual(xna.Z, v.Z);
      Assert.AreEqual(xna.W, v.W);
    }


    [Test]
    public void ToXna()
    {
      Vector4 v = new Vector4(6, 7, 8, 9);
      Vector4 xna = v.ToXna();

      Assert.AreEqual(xna.X, v.X);
      Assert.AreEqual(xna.Y, v.Y);
      Assert.AreEqual(xna.Z, v.Z);
      Assert.AreEqual(xna.W, v.W);
    }


    [Test]
    public void ExplicitArrayCast()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 0.0f;
      float w = 0.3f;
      float[] values = (float[])new Vector4(x, y, z, w);
      Assert.AreEqual(x, values[0]);
      Assert.AreEqual(y, values[1]);
      Assert.AreEqual(z, values[2]);
      Assert.AreEqual(w, values[3]);
      Assert.AreEqual(4, values.Length);
    }


    [Test]
    public void ExplicitArrayCast2()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 0.0f;
      float w = 0.3f;
      float[] values = (new Vector4(x, y, z, w)).ToArray();
      Assert.AreEqual(x, values[0]);
      Assert.AreEqual(y, values[1]);
      Assert.AreEqual(z, values[2]);
      Assert.AreEqual(w, values[3]);
      Assert.AreEqual(4, values.Length);
    }


    [Test]
    public void ExplicitListCast()
    {
      float x = 23.5f;
      float y = 0.0f;
      float z = -11.0f;
      float w = 0.3f;
      List<float> values = (List<float>)new Vector4(x, y, z, w);
      Assert.AreEqual(x, values[0]);
      Assert.AreEqual(y, values[1]);
      Assert.AreEqual(z, values[2]);
      Assert.AreEqual(w, values[3]);
      Assert.AreEqual(4, values.Count);
    }


    [Test]
    public void ExplicitListCast2()
    {
      float x = 23.5f;
      float y = 0.0f;
      float z = -11.0f;
      float w = 0.3f;
      List<float> values = (new Vector4(x, y, z, w)).ToList();
      Assert.AreEqual(x, values[0]);
      Assert.AreEqual(y, values[1]);
      Assert.AreEqual(z, values[2]);
      Assert.AreEqual(w, values[3]);
      Assert.AreEqual(4, values.Count);
    }


    [Test]
    public void ImplicitVector4DCast()
    {
      float x = 23.5f;
      float y = 0.0f;
      float z = -11.0f;
      float w = 0.3f;
      Vector4D vector4D = new Vector4(x, y, z, w);
      Assert.IsTrue(Numeric.AreEqual(x, (float)vector4D[0]));
      Assert.IsTrue(Numeric.AreEqual(y, (float)vector4D[1]));
      Assert.IsTrue(Numeric.AreEqual(z, (float)vector4D[2]));
      Assert.IsTrue(Numeric.AreEqual(w, (float)vector4D[3]));
    }


    [Test]
    public void ToVector4D()
    {
      float x = 23.5f;
      float y = 0.0f;
      float z = -11.0f;
      float w = 0.3f;
      Vector4D vector4D = new Vector4(x, y, z, w).ToVector4D();
      Assert.IsTrue(Numeric.AreEqual(x, (float)vector4D[0]));
      Assert.IsTrue(Numeric.AreEqual(y, (float)vector4D[1]));
      Assert.IsTrue(Numeric.AreEqual(z, (float)vector4D[2]));
      Assert.IsTrue(Numeric.AreEqual(w, (float)vector4D[3]));
    }   


    [Test]
    public void ProjectTo()
    {
      // Project (1, 1, 1) to axes
      Vector4 v = new Vector4(1, 1, 1, 0);
      Vector4 projection = Vector4.ProjectTo(v, Vector4.UnitX);
      Assert.AreEqual(Vector4.UnitX, projection);
      projection = Vector4.ProjectTo(v, Vector4.UnitY);
      Assert.AreEqual(Vector4.UnitY, projection);
      projection = Vector4.ProjectTo(v, Vector4.UnitZ);
      Assert.AreEqual(Vector4.UnitZ, projection);

      // Project axes to (1, 1, 1)
      Vector4 expected = new Vector4(1, 1, 1, 0) / 3.0f;
      projection = Vector4.ProjectTo(Vector4.UnitX, v);
      Assert.AreEqual(expected, projection);
      projection = Vector4.ProjectTo(Vector4.UnitY, v);
      Assert.AreEqual(expected, projection);
      projection = Vector4.ProjectTo(Vector4.UnitZ, v);
      Assert.AreEqual(expected, projection);
    }


    [Test]
    public void ProjectTo2()
    {
      // Project (1, 1, 1) to axes
      Vector4 projection = new Vector4(1, 1, 1, 0);
      projection.ProjectTo(Vector4.UnitX);
      Assert.AreEqual(Vector4.UnitX, projection);
      projection = Vector4.One;
      projection.ProjectTo(Vector4.UnitY);
      Assert.AreEqual(Vector4.UnitY, projection);
      projection = Vector4.One;
      projection.ProjectTo(Vector4.UnitZ);
      Assert.AreEqual(Vector4.UnitZ, projection);

      // Project axes to (1, 1, 1)
      Vector4 expected = new Vector4(1, 1, 1, 0) / 3.0f;
      projection = Vector4.UnitX;
      projection.ProjectTo(new Vector4(1, 1, 1, 0));
      Assert.AreEqual(expected, projection);
      projection = Vector4.UnitY;
      projection.ProjectTo(new Vector4(1, 1, 1, 0));
      Assert.AreEqual(expected, projection);
      projection = Vector4.UnitZ;
      projection.ProjectTo(new Vector4(1, 1, 1, 0));
      Assert.AreEqual(expected, projection);
    }


    [Test]
    public void Clamp1()
    {
      Vector4 clamped = new Vector4(-10, 1, 100, 1000);
      clamped.Clamp(-100, 1000);
      Assert.AreEqual(-10, clamped.X);
      Assert.AreEqual(1, clamped.Y);
      Assert.AreEqual(100, clamped.Z);
      Assert.AreEqual(1000, clamped.W);
    }


    [Test]
    public void Clamp2()
    {
      Vector4 clamped = new Vector4(-10, 1, 100, 1000);
      clamped.Clamp(-1, 0);
      Assert.AreEqual(-1, clamped.X);
      Assert.AreEqual(0, clamped.Y);
      Assert.AreEqual(0, clamped.Z);
      Assert.AreEqual(0, clamped.W);
    }


    [Test]
    public void ClampStatic1()
    {
      Vector4 clamped = new Vector4(-10, 1, 100, 1000);
      clamped = Vector4.Clamp(clamped, -100, 1000);
      Assert.AreEqual(-10, clamped.X);
      Assert.AreEqual(1, clamped.Y);
      Assert.AreEqual(100, clamped.Z);
      Assert.AreEqual(1000, clamped.W);
    }


    [Test]
    public void ClampStatic2()
    {
      Vector4 clamped = new Vector4(-10, 1, 100, 1000);
      clamped = Vector4.Clamp(clamped, -1, 0);
      Assert.AreEqual(-1, clamped.X);
      Assert.AreEqual(0, clamped.Y);
      Assert.AreEqual(0, clamped.Z);
      Assert.AreEqual(0, clamped.W);
    }


    [Test]
    public void ClampToZero1()
    {
      Vector4 v = new Vector4(
        Numeric.EpsilonF / 2, Numeric.EpsilonF / 2, -Numeric.EpsilonF / 2, -Numeric.EpsilonF / 2);
      v.ClampToZero();
      Assert.AreEqual(Vector4.Zero, v);
      v = new Vector4(-Numeric.EpsilonF * 2, Numeric.EpsilonF, Numeric.EpsilonF * 2, Numeric.EpsilonF);
      v.ClampToZero();
      Assert.AreNotEqual(Vector4.Zero, v);
    }


    [Test]
    public void ClampToZero2()
    {
      Vector4 v = new Vector4(0.1f, 0.1f, -0.1f, 0.09f);
      v.ClampToZero(0.11f);
      Assert.AreEqual(Vector4.Zero, v);
      v = new Vector4(0.1f, -0.11f, 0.11f, 0.0f);
      v.ClampToZero(0.1f);
      Assert.AreNotEqual(Vector4.Zero, v);
    }


    [Test]
    public void ClampToZeroStatic1()
    {
      Vector4 v = new Vector4(
        Numeric.EpsilonF / 2, Numeric.EpsilonF / 2, -Numeric.EpsilonF / 2, -Numeric.EpsilonF / 2);
      v = Vector4.ClampToZero(v);
      Assert.AreEqual(Vector4.Zero, v);
      v = new Vector4(-Numeric.EpsilonF * 2, Numeric.EpsilonF, Numeric.EpsilonF * 2, Numeric.EpsilonF);
      v = Vector4.ClampToZero(v);
      Assert.AreNotEqual(Vector4.Zero, v);
    }


    [Test]
    public void ClampToZeroStatic2()
    {
      Vector4 v = new Vector4(0.1f, 0.1f, -0.1f, 0.09f);
      v = Vector4.ClampToZero(v, 0.11f);
      Assert.AreEqual(Vector4.Zero, v);
      v = new Vector4(0.1f, -0.11f, 0.11f, 0.0f);
      v = Vector4.ClampToZero(v, 0.1f);
      Assert.AreNotEqual(Vector4.Zero, v);
    }


    [Test]
    public void Min()
    {
      Vector4 v1 = new Vector4(1.0f, 2.0f, 2.5f, 4.0f);
      Vector4 v2 = new Vector4(-1.0f, 2.0f, 3.0f, -2.0f);
      Vector4 min = Vector4.Min(v1, v2);
      Assert.AreEqual(new Vector4(-1.0f, 2.0f, 2.5f, -2.0f), min);
    }


    [Test]
    public void Max()
    {
      Vector4 v1 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 v2 = new Vector4(-1.0f, 2.1f, 3.0f, 8.0f);
      Vector4 max = Vector4.Max(v1, v2);
      Assert.AreEqual(new Vector4(1.0f, 2.1f, 3.0f, 8.0f), max);
    }


    [Test]
    public void SerializationXml()
    {
      Vector4 v1 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 v2;
      string fileName = "SerializationVector4.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      XmlSerializer serializer = new XmlSerializer(typeof(Vector4));
      StreamWriter writer = new StreamWriter(fileName);
      serializer.Serialize(writer, v1);
      writer.Close();

      serializer = new XmlSerializer(typeof(Vector4));
      FileStream fileStream = new FileStream(fileName, FileMode.Open);
      v2 = (Vector4)serializer.Deserialize(fileStream);
      Assert.AreEqual(v1, v2);
    }


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      Vector4 v1 = new Vector4(0.1f, -0.2f, 2, 40);
      Vector4 v2;
      string fileName = "SerializationVector4.bin";

      if (File.Exists(fileName))
        File.Delete(fileName);

      FileStream fs = new FileStream(fileName, FileMode.Create);

      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(fs, v1);
      fs.Close();

      fs = new FileStream(fileName, FileMode.Open);
      formatter = new BinaryFormatter();
      v2 = (Vector4)formatter.Deserialize(fs);
      fs.Close();

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationXml2()
    {
      Vector4 v1 = new Vector4(0.1f, -0.2f, 2, 40);
      Vector4 v2;

      string fileName = "SerializationVector4_DataContractSerializer.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractSerializer(typeof(Vector4));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
      using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8))
        serializer.WriteObject(writer, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
        v2 = (Vector4)serializer.ReadObject(reader);

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationJson()
    {
      Vector4 v1 = new Vector4(0.1f, -0.2f, 2, 40);
      Vector4 v2;

      string fileName = "SerializationVector4.json";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractJsonSerializer(typeof(Vector4));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        serializer.WriteObject(stream, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        v2 = (Vector4)serializer.ReadObject(stream);

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void Parse()
    {
      Vector4 vector = Vector4.Parse("(0.0123; 9.876; 0.0; -2.3)", CultureInfo.InvariantCulture);
      Assert.AreEqual(0.0123f, vector.X);
      Assert.AreEqual(9.876f, vector.Y);
      Assert.AreEqual(0.0f, vector.Z);
      Assert.AreEqual(-2.3f, vector.W);

      vector = Vector4.Parse("(   0.0123   ;  9;  0.1 ; -2.3 ) ", CultureInfo.InvariantCulture);
      Assert.AreEqual(0.0123f, vector.X);
      Assert.AreEqual(9f, vector.Y);
      Assert.AreEqual(0.1f, vector.Z);
      Assert.AreEqual(-2.3f, vector.W);
    }


    [Test]
    [ExpectedException(typeof(FormatException))]
    public void ParseException()
    {
      Vector4 vector = Vector4.Parse("(0.0123; 9.876)");
    }


    [Test]
    [ExpectedException(typeof(FormatException))]
    public void ParseException2()
    {
      Vector4 vector = Vector4.Parse("xyzw");
    }


    [Test]
    public void ToStringAndParse()
    {
      Vector4 vector = new Vector4(0.0123f, 9.876f, 0.0f, -2.3f);
      string s = vector.ToString();
      Vector4 parsedVector = Vector4.Parse(s);
      Assert.AreEqual(vector, parsedVector);
    }


    [Test]
    public void AbsoluteStatic()
    {
      Vector4 v = new Vector4(-1, -2, -3, -4);
      Vector4 absoluteV = Vector4.Absolute(v);

      Assert.AreEqual(1, absoluteV.X);
      Assert.AreEqual(2, absoluteV.Y);
      Assert.AreEqual(3, absoluteV.Z);
      Assert.AreEqual(4, absoluteV.W);

      v = new Vector4(1, 2, 3, 4);
      absoluteV = Vector4.Absolute(v);
      Assert.AreEqual(1, absoluteV.X);
      Assert.AreEqual(2, absoluteV.Y);
      Assert.AreEqual(3, absoluteV.Z);
      Assert.AreEqual(4, absoluteV.W);
    }


    [Test]
    public void Absolute()
    {
      Vector4 v = new Vector4(-1, -2, -3, -4);
      v.Absolute();

      Assert.AreEqual(1, v.X);
      Assert.AreEqual(2, v.Y);
      Assert.AreEqual(3, v.Z);
      Assert.AreEqual(4, v.W);

      v = new Vector4(1, 2, 3, 4);
      v.Absolute();
      Assert.AreEqual(1, v.X);
      Assert.AreEqual(2, v.Y);
      Assert.AreEqual(3, v.Z);
      Assert.AreEqual(4, v.W);
    }


    [Test]
    public void GetLargestComponent()
    {
      Vector4 v = new Vector4(-1, -2, -3, -4);
      Assert.AreEqual(-1, v.LargestComponent);

      v = new Vector4(10, 20, -30, -40);
      Assert.AreEqual(20, v.LargestComponent);

      v = new Vector4(-1, 20, 30, 20);
      Assert.AreEqual(30, v.LargestComponent);

      v = new Vector4(10, 20, 30, 40);
      Assert.AreEqual(40, v.LargestComponent);
    }


    [Test]
    public void GetIndexOfLargestComponent()
    {
      Vector4 v = new Vector4(-1, -2, -3, -4);
      Assert.AreEqual(0, v.IndexOfLargestComponent);

      v = new Vector4(10, 20, -30, -40);
      Assert.AreEqual(1, v.IndexOfLargestComponent);

      v = new Vector4(-1, 20, 30, 20);
      Assert.AreEqual(2, v.IndexOfLargestComponent);

      v = new Vector4(10, 20, 30, 40);
      Assert.AreEqual(3, v.IndexOfLargestComponent);
    }


    [Test]
    public void GetSmallestComponent()
    {
      Vector4 v = new Vector4(-4, -2, -3, -1);
      Assert.AreEqual(-4, v.SmallestComponent);

      v = new Vector4(10, 0, 3, 4);
      Assert.AreEqual(0, v.SmallestComponent);

      v = new Vector4(-1, 20, -3, 0);
      Assert.AreEqual(-3, v.SmallestComponent);

      v = new Vector4(40, 30, 20, 10);
      Assert.AreEqual(10, v.SmallestComponent);
    }


    [Test]
    public void GetIndexOfSmallestComponent()
    {
      Vector4 v = new Vector4(-4, -2, -3, -1);
      Assert.AreEqual(0, v.IndexOfSmallestComponent);

      v = new Vector4(10, 0, 3, 4);
      Assert.AreEqual(1, v.IndexOfSmallestComponent);

      v = new Vector4(-1, 20, -3, 0);
      Assert.AreEqual(2, v.IndexOfSmallestComponent);

      v = new Vector4(40, 30, 20, 10);
      Assert.AreEqual(3, v.IndexOfSmallestComponent);
    }
  }
}