using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DigitalRune.Mathematics.Statistics;
using NUnit.Framework;


namespace DigitalRune.Mathematics.Algebra.Tests
{
  [TestFixture]
  public class MatrixTest
  {
    //           1, 2, 3
    // Matrix =  4, 5, 6
    //           7, 8, 9

    // in column-major layout
    float[] columnMajor = new float[] { 1.0f, 4.0f, 7.0f,
                                        2.0f, 5.0f, 8.0f,
                                        3.0f, 6.0f, 9.0f };

    // in row-major layout
    float[] rowMajor = new float[] { 1.0f, 2.0f, 3.0f,
                                     4.0f, 5.0f, 6.0f,
                                     7.0f, 8.0f, 9.0f };


    [SetUp]
    public void SetUp()
    {
      // Initialize random generator with a specific seed.
      RandomHelper.Random = new Random(123456);
    }


    [Test]
    public void Constants()
    {
      Matrix zero = Matrix.Zero;
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(0.0, zero[i]);

      Matrix one = Matrix.One;
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(1.0f, one[i]);
    }


    [Test]
    public void Constructors()
    {
      Matrix m = new Matrix(1.0f, 2.0f, 3.0f,
                                  4.0f, 5.0f, 6.0f,
                                  7.0f, 8.0f, 9.0f);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new List<float>(columnMajor), MatrixOrder.ColumnMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new List<float>(rowMajor), MatrixOrder.RowMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new float[3, 3] { { 1, 2, 3 }, 
                                          { 4, 5, 6 }, 
                                          { 7, 8, 9 } });
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new float[3][] { new float[3] { 1, 2, 3 }, 
                                         new float[3] { 4, 5, 6 }, 
                                         new float[3] { 7, 8, 9 } });
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);
    }


    [Test]
    [ExpectedException(typeof(NullReferenceException))]
    public void ConstructorException1()
    {
      new Matrix(new float[3][]);
    }


    [Test]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void ConstructorException2()
    {
      float[][] elements = new float[3][];
      elements[0] = new float[3];
      elements[1] = new float[2];
      new Matrix(elements);
    }


    [Test]
    public void Properties()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.AreEqual(1.0f, m.M00);
      Assert.AreEqual(2.0f, m.M01);
      Assert.AreEqual(3.0f, m.M02);
      Assert.AreEqual(4.0f, m.M10);
      Assert.AreEqual(5.0f, m.M11);
      Assert.AreEqual(6.0f, m.M12);
      Assert.AreEqual(7.0f, m.M20);
      Assert.AreEqual(8.0f, m.M21);
      Assert.AreEqual(9.0f, m.M22);

      m = Matrix.Zero;
      m.M00 = 1.0f;
      m.M01 = 2.0f;
      m.M02 = 3.0f;
      m.M10 = 4.0f;
      m.M11 = 5.0f;
      m.M12 = 6.0f;
      m.M20 = 7.0f;
      m.M21 = 8.0f;
      m.M22 = 9.0f;
      Assert.AreEqual(new Matrix(rowMajor, MatrixOrder.RowMajor), m);
    }


    [Test]
    public void Indexer1d()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = Matrix.Zero;
      for (int i = 0; i < 9; i++)
        m[i] = rowMajor[i];
      Assert.AreEqual(new Matrix(rowMajor, MatrixOrder.RowMajor), m);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer1dException()
    {
      Matrix m = new Matrix();
      m[-1] = 0.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer1dException2()
    {
      Matrix m = new Matrix();
      m[9] = 0.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer1dException3()
    {
      Matrix m = new Matrix();
      float x = m[-1];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer1dException4()
    {
      Matrix m = new Matrix();
      float x = m[9];
    }


    [Test]
    public void Indexer2d()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int column = 0; column < 3; column++)
        for (int row = 0; row < 3; row++)
          Assert.AreEqual(columnMajor[column * 3 + row], m[row, column]);
      m = Matrix.Zero;
      for (int column = 0; column < 3; column++)
        for (int row = 0; row < 3; row++)
          m[row, column] = (float) (row * 3 + column + 1);
      Assert.AreEqual(new Matrix(rowMajor, MatrixOrder.RowMajor), m);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException()
    {
      Matrix m = Matrix.Zero;
      m[0, 3] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException2()
    {
      Matrix m = Matrix.Zero;
      m[3, 0] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException3()
    {
      Matrix m = Matrix.Zero;
      m[0, -1] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException4()
    {
      Matrix m = Matrix.Zero;
      m[-1, 0] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException5()
    {
      Matrix m = Matrix.Zero;
      m[1, 3] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException6()
    {
      Matrix m = Matrix.Zero;
      m[2, 3] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException7()
    {
      Matrix m = Matrix.Zero;
      m[3, 1] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException8()
    {
      Matrix m = Matrix.Zero;
      m[3, 2] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException9()
    {
      Matrix m = Matrix.Zero;
      float x = m[0, 3];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException10()
    {
      Matrix m = Matrix.Zero;
      float x = m[3, 0];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException11()
    {
      Matrix m = Matrix.Zero;
      float x = m[0, -1];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException12()
    {
      Matrix m = Matrix.Zero;
      float x = m[-1, 0];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException13()
    {
      Matrix m = Matrix.Zero;
      float x = m[3, 1];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException14()
    {
      Matrix m = Matrix.Zero;
      float x = m[3, 2];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException15()
    {
      Matrix m = Matrix.Zero;
      float x = m[1, 3];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException16()
    {
      Matrix m = Matrix.Zero;
      float x = m[2, 3];
    }


    [Test]
    public void Determinant()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.AreEqual(0.0, m.Determinant);
      Assert.AreEqual(0.0, Matrix.Zero.Determinant);
      Assert.AreEqual(0.0, Matrix.One.Determinant);
      Assert.AreEqual(1.0f, Matrix.Identity.Determinant);
    }


    [Test]
    public void IsNaN()
    {
      const int numberOfRows = 3;
      const int numberOfColumns = 3;
      Assert.IsFalse(new Matrix().IsNaN);

      for (int r = 0; r < numberOfRows; r++)
      {
        for (int c = 0; c < numberOfColumns; c++)
        {
          Matrix m = new Matrix();
          m[r, c] = float.NaN;
          Assert.IsTrue(m.IsNaN);
        }
      }
    }


    [Test]
    public void IsOrthogonal()
    {
      Assert.IsTrue(!Matrix.Zero.IsOrthogonal);
      Assert.IsTrue(Matrix.Identity.IsOrthogonal);
      Assert.IsTrue(Matrix.CreateRotation(new Vector3(1, 2, 3).Normalized, 0.5f).IsOrthogonal);
      Assert.IsTrue(new Matrix(1, 0, 0, 0, 1, 0, 0, 0, -1).IsOrthogonal);
    }


    [Test]
    public void IsRotation()
    {
      Assert.IsTrue(!Matrix.Zero.IsRotation);
      Assert.IsTrue(Matrix.Identity.IsRotation);
      Assert.IsTrue(Matrix.CreateRotation(new Vector3(1, 2, 3).Normalized, 0.5f).IsRotation);
      Assert.IsTrue(!new Matrix(1, 0, 0, 0, 1, 0, 0, 0, -1).IsRotation);
    }


    [Test]
    public void Orthogonalize()
    {
      var m = Matrix.CreateRotationX(0.1f) * Matrix.CreateRotationY(20) * Matrix.CreateRotationZ(1000);

      // Introduce error.
      m.M01 += 0.1f;
      m.M22 += 0.1f;

      Assert.IsFalse(m.IsOrthogonal);
      Assert.IsFalse(m.IsRotation);

      m.Orthogonalize();

      Assert.IsTrue(m.IsOrthogonal);
      Assert.IsTrue(m.IsRotation);

      // Orthogonalizing and orthogonal matrix does not change the matrix.
      var n = m;
      n.Orthogonalize();
      Assert.IsTrue(Matrix.AreNumericallyEqual(m, n));
    }


    [Test]
    public void IsSymmetric()
    {
      Matrix m = new Matrix(new float[3, 3] { { 1, 2, 3 }, 
                                                    { 2, 4, 5 }, 
                                                    { 3, 5, 7 } });
      Assert.AreEqual(true, m.IsSymmetric);

      m = new Matrix(new float[3, 3] { { 1, 2, 3 }, 
                                          { 4, 5, 2 }, 
                                          { 7, 4, 1 } });
      Assert.AreEqual(false, m.IsSymmetric);
    }


    [Test]
    public void Trace()
    {
      Matrix m = new Matrix(new float[3, 3] { { 1, 2, 3 }, 
                                                    { 2, 4, 5 }, 
                                                    { 3, 5, 7 } });
      Assert.AreEqual(12, m.Trace);
    }


    [Test]
    public void Transposed()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix mt = new Matrix(rowMajor, MatrixOrder.ColumnMajor);
      Assert.AreEqual(mt, m.Transposed);
      Assert.AreEqual(Matrix.Identity, Matrix.Identity.Transposed);
    }


    [Test]
    public void Transpose()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.Transpose();
      Matrix mt = new Matrix(rowMajor, MatrixOrder.ColumnMajor);
      Assert.AreEqual(mt, m);
      Matrix i = Matrix.Identity;
      i.Transpose();
      Assert.AreEqual(Matrix.Identity, i);
    }


    [Test]
    public void Inverse()
    {
      Assert.AreEqual(Matrix.Identity, Matrix.Identity.Inverse);

      Matrix m = new Matrix(1, 2, 3,
                                  2, 5, 8,
                                  7, 6, -1);
      Vector3 v = Vector3.One;
      Vector3 w = m * v;
      Assert.IsTrue(Vector3.AreNumericallyEqual(v, m.Inverse * w));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * m.Inverse));
    }


    [Test]
    public void InverseWithNearSingularMatrix()
    {
      Matrix m = new Matrix(0.0001f, 0, 0,
                                  0, 0.0001f, 0,
                                  0, 0, 0.0001f);
      Vector3 v = Vector3.One;
      Vector3 w = m * v;
      Assert.IsTrue(Vector3.AreNumericallyEqual(v, m.Inverse * w));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * m.Inverse));
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void InverseException()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m = m.Inverse;
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void InverseException2()
    {
      Matrix m = Matrix.Zero.Inverse;
    }


    [Test]
    public void Invert()
    {
      Assert.AreEqual(Matrix.Identity, Matrix.Identity.Inverse);

      Matrix m = new Matrix(1, 2, 3,
                                  2, 5, 8,
                                  7, 6, -1);
      Vector3 v = Vector3.One;
      Vector3 w = m * v;
      Matrix im = m;
      im.Invert();
      Assert.IsTrue(Vector3.AreNumericallyEqual(v, im * w));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * im));
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void InvertException()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.Invert();
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void InvertException2()
    {
      Matrix.Zero.Invert();
    }


    [Test]
    public void GetColumn()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Assert.AreEqual(new Vector3(1.0f, 4.0f, 7.0f), m.GetColumn(0));
      Assert.AreEqual(new Vector3(2.0f, 5.0f, 8.0f), m.GetColumn(1));
      Assert.AreEqual(new Vector3(3.0f, 6.0f, 9.0f), m.GetColumn(2));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetColumnException1()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.GetColumn(-1);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetColumnException2()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.GetColumn(3);
    }


    [Test]
    public void SetColumn()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.SetColumn(0, new Vector3(0.1f, 0.2f, 0.3f));
      Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f), m.GetColumn(0));
      Assert.AreEqual(new Vector3(2.0f, 5.0f, 8.0f), m.GetColumn(1));
      Assert.AreEqual(new Vector3(3.0f, 6.0f, 9.0f), m.GetColumn(2));

      m.SetColumn(1, new Vector3(0.4f, 0.5f, 0.6f));
      Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f), m.GetColumn(0));
      Assert.AreEqual(new Vector3(0.4f, 0.5f, 0.6f), m.GetColumn(1));
      Assert.AreEqual(new Vector3(3.0f, 6.0f, 9.0f), m.GetColumn(2));

      m.SetColumn(2, new Vector3(0.7f, 0.8f, 0.9f));
      Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f), m.GetColumn(0));
      Assert.AreEqual(new Vector3(0.4f, 0.5f, 0.6f), m.GetColumn(1));
      Assert.AreEqual(new Vector3(0.7f, 0.8f, 0.9f), m.GetColumn(2));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnException1()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.SetColumn(-1, Vector3.One);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnException2()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.SetColumn(3, Vector3.One);
    }


    [Test]
    public void GetRow()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), m.GetRow(0));
      Assert.AreEqual(new Vector3(4.0f, 5.0f, 6.0f), m.GetRow(1));
      Assert.AreEqual(new Vector3(7.0f, 8.0f, 9.0f), m.GetRow(2));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRowException1()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.GetRow(-1);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRowException2()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.GetRow(3);
    }


    [Test]
    public void SetRow()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.SetRow(0, new Vector3(0.1f, 0.2f, 0.3f));
      Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f), m.GetRow(0));
      Assert.AreEqual(new Vector3(4.0f, 5.0f, 6.0f), m.GetRow(1));
      Assert.AreEqual(new Vector3(7.0f, 8.0f, 9.0f), m.GetRow(2));

      m.SetRow(1, new Vector3(0.4f, 0.5f, 0.6f));
      Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f), m.GetRow(0));
      Assert.AreEqual(new Vector3(0.4f, 0.5f, 0.6f), m.GetRow(1));
      Assert.AreEqual(new Vector3(7.0f, 8.0f, 9.0f), m.GetRow(2));

      m.SetRow(2, new Vector3(0.7f, 0.8f, 0.9f));
      Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f), m.GetRow(0));
      Assert.AreEqual(new Vector3(0.4f, 0.5f, 0.6f), m.GetRow(1));
      Assert.AreEqual(new Vector3(0.7f, 0.8f, 0.9f), m.GetRow(2));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowException1()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.SetRow(-1, Vector3.One);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowException2()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m.SetRow(3, Vector3.One);
    }


    [Test]
    public void AreEqual()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Matrix m0 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m1 += new Matrix(0.000001f);
      Matrix m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m2 += new Matrix(0.00000001f);

      Assert.IsTrue(Matrix.AreNumericallyEqual(m0, m0));
      Assert.IsFalse(Matrix.AreNumericallyEqual(m0, m1));
      Assert.IsTrue(Matrix.AreNumericallyEqual(m0, m2));

      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void AreEqualWithEpsilon()
    {
      float epsilon = 0.001f;
      Matrix m0 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m1 += new Matrix(0.002f);
      Matrix m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m2 += new Matrix(0.0001f);

      Assert.IsTrue(Matrix.AreNumericallyEqual(m0, m0, epsilon));
      Assert.IsFalse(Matrix.AreNumericallyEqual(m0, m1, epsilon));
      Assert.IsTrue(Matrix.AreNumericallyEqual(m0, m2, epsilon));
    }


    [Test]
    public void CreateScale()
    {
      Matrix i = Matrix.CreateScale(1.0f);
      Assert.AreEqual(Matrix.Identity, i);

      Vector3 v = Vector3.One;
      Matrix m = Matrix.CreateScale(2.0f);
      Assert.AreEqual(2 * v, m * v);

      m = Matrix.CreateScale(-1.0f, 1.5f, 2.0f);
      Assert.AreEqual(new Vector3(-1.0f, 1.5f, 2.0f), m * v);

      Vector3 scale = new Vector3(-2.0f, -3.0f, -4.0f);
      m = Matrix.CreateScale(scale);
      v = new Vector3(1.0f, 2.0f, 3.0f);
      Assert.AreEqual(v * scale, m * v);
    }


    [Test]
    public void CreateRotation()
    {
      Matrix m = Matrix.CreateRotation(Vector3.UnitX, 0.0f);
      Assert.AreEqual(Matrix.Identity, m);

      m = Matrix.CreateRotation(Vector3.UnitX, (float) Math.PI / 2);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.UnitZ, m * Vector3.UnitY));

      m = Matrix.CreateRotation(Vector3.UnitY, (float) Math.PI / 2);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.UnitX, m * Vector3.UnitZ));

      m = Matrix.CreateRotation(Vector3.UnitZ, (float) Math.PI / 2);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.UnitY, m * Vector3.UnitX));
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateRotationException()
    {
      Matrix.CreateRotation(Vector3.Zero, 1f);
    }


    [Test]
    public void CreateRotationX()
    {
      float angle = (float) MathHelper.ToRadians(30.0f);
      Matrix m = Matrix.CreateRotationX(angle);
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(0, (float) Math.Cos(angle), (float) Math.Sin(angle)), m * Vector3.UnitY));

      Quaternion q = Quaternion.CreateRotation(Vector3.UnitX, angle);
      Assert.IsTrue(Vector3.AreNumericallyEqual(q.Rotate(Vector3.One), m * Vector3.One));

      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.CreateRotation(Vector3.UnitX, angle), m));
    }


    [Test]
    public void CreateRotationY()
    {
      float angle = (float) MathHelper.ToRadians(30);
      Matrix m = Matrix.CreateRotationY(angle);
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3((float) Math.Sin(angle), 0, (float) Math.Cos(angle)), m * Vector3.UnitZ));

      Quaternion q = Quaternion.CreateRotation(Vector3.UnitY, angle);
      Assert.IsTrue(Vector3.AreNumericallyEqual(q.Rotate(Vector3.One), m * Vector3.One));

      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.CreateRotation(Vector3.UnitY, angle), m));
    }


    [Test]
    public void CreateRotationZ()
    {
      float angle = (float) MathHelper.ToRadians(30);
      Matrix m = Matrix.CreateRotationZ(angle);
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3((float) Math.Cos(angle), (float) Math.Sin(angle), 0), m * Vector3.UnitX));

      Quaternion q = Quaternion.CreateRotation(Vector3.UnitZ, angle);
      Assert.IsTrue(Vector3.AreNumericallyEqual(q.Rotate(Vector3.One), m * Vector3.One));

      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.CreateRotation(Vector3.UnitZ, angle), m));
    }


    [Test]
    public void FromQuaternion()
    {
      float angle = -1.6f;
      Vector3 axis = new Vector3(1.0f, 2.0f, -3.0f);
      Matrix matrix = Matrix.CreateRotation(axis, angle);
      Quaternion q = Quaternion.CreateRotation(axis, angle);
      Matrix matrixFromQuaternion = Matrix.CreateRotation(q);
      Vector3 v = new Vector3(0.3f, -2.4f, 5.6f);
      Vector3 result1 = matrix * v;
      Vector3 result2 = matrixFromQuaternion * v;
      Assert.IsTrue(Vector3.AreNumericallyEqual(result1, result2));
    }


    [Test]
    public void HashCode()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Assert.AreNotEqual(Matrix.Identity.GetHashCode(), m.GetHashCode());
    }


    [Test]
    public void TestEquals()
    {
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Assert.IsTrue(m1.Equals(m1));
      Assert.IsTrue(m1.Equals(m2));
      for (int i = 0; i < 9; i++)
      {
        m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
        m2[i] += 0.1f;
        Assert.IsFalse(m1.Equals(m2));
      }

      Assert.IsFalse(m1.Equals(m1.ToString()));
    }


    [Test]
    public void EqualityOperators()
    {
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Assert.IsTrue(m1 == m2);
      Assert.IsFalse(m1 != m2);
      for (int i = 0; i < 9; i++)
      {
        m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
        m2[i] += 0.1f;
        Assert.IsFalse(m1 == m2);
        Assert.IsTrue(m1 != m2);
      }
    }


    [Test]
    public void TestToString()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Assert.IsFalse(String.IsNullOrEmpty(m.ToString()));
    }


    [Test]
    public void NegationOperator()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(-rowMajor[i], (-m)[i]);
    }


    [Test]
    public void Negation()
    {
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(-rowMajor[i], Matrix.Negate(m)[i]);
    }


    [Test]
    public void AdditionOperator()
    {
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor) * 3;
      Matrix result = m1 + m2;
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i] * 4, result[i]);
    }


    [Test]
    public void Addition()
    {
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = Matrix.One;
      Matrix result = Matrix.Add(m1, m2);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i] + 1.0f, result[i]);
    }


    [Test]
    public void SubtractionOperator()
    {
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = new Matrix(columnMajor, MatrixOrder.ColumnMajor) * 3;
      Matrix result = m1 - m2;
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(-rowMajor[i] * 2, result[i]);
    }


    [Test]
    public void Subtraction()
    {
      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = Matrix.One;
      Matrix result = Matrix.Subtract(m1, m2);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i] - 1.0f, result[i]);
    }


    [Test]
    public void MultiplicationOperator()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m = s * m;
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i] * s, m[i]);

      m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m = m * s;
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i] * s, m[i]);
    }


    [Test]
    public void Multiplication()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m = Matrix.Multiply(s, m);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i] * s, m[i]);
    }


    [Test]
    public void DivisionOperator()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m = m / s;
      for (int i = 0; i < 9; i++)
        Assert.IsTrue(Numeric.AreEqual(rowMajor[i] / s, m[i]));
    }


    [Test]
    public void Division()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      m = Matrix.Divide(m, s);
      for (int i = 0; i < 9; i++)
        Assert.IsTrue(Numeric.AreEqual(rowMajor[i] / s, m[i]));
    }


    [Test]
    public void MultiplyMatrixOperator()
    {
      Matrix m = new Matrix(12, 23, 45,
                                67, 89, 90,
                                43, 65, 87);
      Assert.AreEqual(Matrix.Zero, m * Matrix.Zero);
      Assert.AreEqual(Matrix.Zero, Matrix.Zero * m);
      Assert.AreEqual(m, m * Matrix.Identity);
      Assert.AreEqual(m, Matrix.Identity * m);
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * m.Inverse));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m.Inverse * m));

      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = new Matrix(12, 23, 45,
                                 67, 89, 90,
                                 43, 65, 87);
      Matrix result = m1 * m2;
      for (int column = 0; column < 3; column++)
        for (int row = 0; row < 3; row++)
          Assert.AreEqual(Vector3.Dot(m1.GetRow(row), m2.GetColumn(column)), result[row, column]);
    }


    [Test]
    public void MultiplyMatrix()
    {
      Matrix m = new Matrix(12, 23, 45,
                                67, 89, 90,
                                43, 65, 87);
      Assert.AreEqual(Matrix.Zero, Matrix.Multiply(m, Matrix.Zero));
      Assert.AreEqual(Matrix.Zero, Matrix.Multiply(Matrix.Zero, m));
      Assert.AreEqual(m, Matrix.Multiply(m, Matrix.Identity));
      Assert.AreEqual(m, Matrix.Multiply(Matrix.Identity, m));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, Matrix.Multiply(m, m.Inverse)));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, Matrix.Multiply(m.Inverse, m)));

      Matrix m1 = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      Matrix m2 = new Matrix(12, 23, 45,
                                 67, 89, 90,
                                 43, 65, 87);
      Matrix result = Matrix.Multiply(m1, m2);
      for (int column = 0; column < 3; column++)
        for (int row = 0; row < 3; row++)
          Assert.AreEqual(Vector3.Dot(m1.GetRow(row), m2.GetColumn(column)), result[row, column]);
    }


    [Test]
    public void MultiplyVectorOperator()
    {
      Vector3 v = new Vector3(2.34f, 3.45f, 4.56f);
      Assert.AreEqual(v, Matrix.Identity * v);
      Assert.AreEqual(Vector3.Zero, Matrix.Zero * v);

      Matrix m = new Matrix(12, 23, 45,
                                67, 89, 90,
                                43, 65, 87);
      Assert.IsTrue(Vector3.AreNumericallyEqual(v, m * m.Inverse * v));

      for (int i = 0; i < 3; i++)
        Assert.AreEqual(Vector3.Dot(m.GetRow(i), v), (m * v)[i]);
    }


    [Test]
    public void MultiplyVector()
    {
      Vector3 v = new Vector3(2.34f, 3.45f, 4.56f);
      Assert.AreEqual(v, Matrix.Multiply(Matrix.Identity, v));
      Assert.AreEqual(Vector3.Zero, Matrix.Multiply(Matrix.Zero, v));

      Matrix m = new Matrix(12, 23, 45,
                                67, 89, 90,
                                43, 65, 87);
      Assert.IsTrue(Vector3.AreNumericallyEqual(v, Matrix.Multiply(m * m.Inverse, v)));

      for (int i = 0; i < 3; i++)
        Assert.AreEqual(Vector3.Dot(m.GetRow(i), v), Matrix.Multiply(m, v)[i]);
    }


    [Test]
    public void MultiplyTransposed()
    {
      var m = RandomHelper.Random.NextMatrix(1, 10);
      var v = RandomHelper.Random.NextVector3(1, 10);

      Assert.AreEqual(m.Transposed * v, Matrix.MultiplyTransposed(m, v));
    }


    [Test]
    public void ImplicitMatrix33DCast()
    {
      float m00 = 23.5f; float m01 = 0.0f; float m02 = -11.0f;
      float m10 = 33.5f; float m11 = 1.1f; float m12 = -12.0f;
      float m20 = 43.5f; float m21 = 2.2f; float m22 = -13.0f;
      Matrix33D matrix33D = new Matrix(m00, m01, m02, m10, m11, m12, m20, m21, m22);
      Assert.IsTrue(Numeric.AreEqual(m00, (float)matrix33D[0, 0]));
      Assert.IsTrue(Numeric.AreEqual(m01, (float)matrix33D[0, 1]));
      Assert.IsTrue(Numeric.AreEqual(m02, (float)matrix33D[0, 2]));
      Assert.IsTrue(Numeric.AreEqual(m10, (float)matrix33D[1, 0]));
      Assert.IsTrue(Numeric.AreEqual(m11, (float)matrix33D[1, 1]));
      Assert.IsTrue(Numeric.AreEqual(m12, (float)matrix33D[1, 2]));
      Assert.IsTrue(Numeric.AreEqual(m20, (float)matrix33D[2, 0]));
      Assert.IsTrue(Numeric.AreEqual(m21, (float)matrix33D[2, 1]));
      Assert.IsTrue(Numeric.AreEqual(m22, (float)matrix33D[2, 2]));
    }


    [Test]
    public void ToMatrix33D()
    {
      float m00 = 23.5f; float m01 = 0.0f; float m02 = -11.0f;
      float m10 = 33.5f; float m11 = 1.1f; float m12 = -12.0f;
      float m20 = 43.5f; float m21 = 2.2f; float m22 = -13.0f;
      Matrix33D matrix33D = new Matrix(m00, m01, m02, m10, m11, m12, m20, m21, m22).ToMatrix33D();
      Assert.IsTrue(Numeric.AreEqual(m00, (float)matrix33D[0, 0]));
      Assert.IsTrue(Numeric.AreEqual(m01, (float)matrix33D[0, 1]));
      Assert.IsTrue(Numeric.AreEqual(m02, (float)matrix33D[0, 2]));
      Assert.IsTrue(Numeric.AreEqual(m10, (float)matrix33D[1, 0]));
      Assert.IsTrue(Numeric.AreEqual(m11, (float)matrix33D[1, 1]));
      Assert.IsTrue(Numeric.AreEqual(m12, (float)matrix33D[1, 2]));
      Assert.IsTrue(Numeric.AreEqual(m20, (float)matrix33D[2, 0]));
      Assert.IsTrue(Numeric.AreEqual(m21, (float)matrix33D[2, 1]));
      Assert.IsTrue(Numeric.AreEqual(m22, (float)matrix33D[2, 2]));
    }


    [Test]
    public void SerializationXml()
    {
      Matrix m1 = new Matrix(12, 23, 45,
                                  67, 89, 90,
                                  43, 65, 87.3f);
      Matrix m2;

      string fileName = "SerializationMatrix.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      XmlSerializer serializer = new XmlSerializer(typeof(Matrix));
      StreamWriter writer = new StreamWriter(fileName);
      serializer.Serialize(writer, m1);
      writer.Close();

      serializer = new XmlSerializer(typeof(Matrix));
      FileStream fileStream = new FileStream(fileName, FileMode.Open);
      m2 = (Matrix) serializer.Deserialize(fileStream);
      Assert.AreEqual(m1, m2);
    }


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      Matrix m1 = new Matrix(12, 23, 45, 
                                   56, 67, 89, 
                                   90, 12, 43.3f);
      Matrix m2;

      string fileName = "SerializationMatrix.bin";

      if (File.Exists(fileName))
        File.Delete(fileName);

      FileStream fs = new FileStream(fileName, FileMode.Create);

      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(fs, m1);
      fs.Close();

      fs = new FileStream(fileName, FileMode.Open);
      formatter = new BinaryFormatter();
      m2 = (Matrix) formatter.Deserialize(fs);
      fs.Close();

      Assert.AreEqual(m1, m2);
    }


    [Test]
    public void SerializationXml2()
    {
      Matrix m1 = new Matrix(12, 23, 45,
                                   56, 67, 89,
                                   90, 12, 43.3f);
      Matrix m2;

      string fileName = "SerializationMatrix_DataContractSerializer.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractSerializer(typeof(Matrix));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
      using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8))
        serializer.WriteObject(writer, m1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
        m2 = (Matrix)serializer.ReadObject(reader);

      Assert.AreEqual(m1, m2);
    }


    [Test]
    public void SerializationJson()
    {
      Matrix m1 = new Matrix(12, 23, 45,
                                   56, 67, 89,
                                   90, 12, 43.3f);
      Matrix m2;

      string fileName = "SerializationMatrix.json";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractJsonSerializer(typeof(Matrix));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        serializer.WriteObject(stream, m1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        m2 = (Matrix)serializer.ReadObject(stream);

      Assert.AreEqual(m1, m2);
    }


    [Test]
    public void Absolute()
    {
      Matrix absoluteM = new Matrix(-1, -2, -3, -4, -5, -6, -7, -8, -9);
      absoluteM.Absolute();

      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M10);
      Assert.AreEqual(5, absoluteM.M11);
      Assert.AreEqual(6, absoluteM.M12);
      Assert.AreEqual(7, absoluteM.M20);
      Assert.AreEqual(8, absoluteM.M21);
      Assert.AreEqual(9, absoluteM.M22);

      absoluteM = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);
      absoluteM.Absolute();
      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M10);
      Assert.AreEqual(5, absoluteM.M11);
      Assert.AreEqual(6, absoluteM.M12);
      Assert.AreEqual(7, absoluteM.M20);
      Assert.AreEqual(8, absoluteM.M21);
      Assert.AreEqual(9, absoluteM.M22);
    }


    [Test]
    public void AbsoluteStatic()
    {
      Matrix m = new Matrix(-1, -2, -3, -4, -5, -6, -7, -8, -9);
      Matrix absoluteM = Matrix.Absolute(m);

      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M10);
      Assert.AreEqual(5, absoluteM.M11);
      Assert.AreEqual(6, absoluteM.M12);
      Assert.AreEqual(7, absoluteM.M20);
      Assert.AreEqual(8, absoluteM.M21);
      Assert.AreEqual(9, absoluteM.M22);

      m = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);
      absoluteM = Matrix.Absolute(m);
      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M10);
      Assert.AreEqual(5, absoluteM.M11);
      Assert.AreEqual(6, absoluteM.M12);
      Assert.AreEqual(7, absoluteM.M20);
      Assert.AreEqual(8, absoluteM.M21);
      Assert.AreEqual(9, absoluteM.M22);
    }


    [Test]
    public void ClampToZero()
    {
      Matrix m = new Matrix(0.000001f);
      m.ClampToZero();
      Assert.AreEqual(new Matrix(), m);

      m = new Matrix(0.1f);
      m.ClampToZero();
      Assert.AreEqual(new Matrix(0.1f), m);

      m = new Matrix(0.001f);
      m.ClampToZero(0.01f);
      Assert.AreEqual(new Matrix(), m);

      m = new Matrix(0.1f);
      m.ClampToZero(0.01f);
      Assert.AreEqual(new Matrix(0.1f), m);
    }


    [Test]
    public void ClampToZeroStatic()
    {
      Matrix m = new Matrix(0.000001f);
      Assert.AreEqual(new Matrix(), Matrix.ClampToZero(m));
      Assert.AreEqual(new Matrix(0.000001f), m); // m unchanged?

      m = new Matrix(0.1f);
      Assert.AreEqual(new Matrix(0.1f), Matrix.ClampToZero(m));
      Assert.AreEqual(new Matrix(0.1f), m);

      m = new Matrix(0.001f);
      Assert.AreEqual(new Matrix(), Matrix.ClampToZero(m, 0.01f));
      Assert.AreEqual(new Matrix(0.001f), m);

      m = new Matrix(0.1f);
      Assert.AreEqual(new Matrix(0.1f), Matrix.ClampToZero(m, 0.01f));
      Assert.AreEqual(new Matrix(0.1f), m);
    }


    [Test]
    public void ToArray1D()
    {
      Matrix m = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);
      float[] array = m.ToArray1D(MatrixOrder.RowMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], array[i]);
      array = m.ToArray1D(MatrixOrder.ColumnMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(columnMajor[i], array[i]);
    }


    [Test]
    public void ToList()
    {
      Matrix m = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);
      IList<float> list = m.ToList(MatrixOrder.RowMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(rowMajor[i], list[i]);
      list = m.ToList(MatrixOrder.ColumnMajor);
      for (int i = 0; i < 9; i++)
        Assert.AreEqual(columnMajor[i], list[i]);
    }


    [Test]
    public void ToArray2D()
    {
      Matrix m = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);

      float[,] array = m.ToArray2D();
      for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
          Assert.AreEqual(i * 3 + j + 1, array[i, j]);

      array = (float[,]) m;
      for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
          Assert.AreEqual(i * 3 + j + 1, array[i, j]);
    }


    [Test]
    public void ToArrayJagged()
    {
      Matrix m = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);

      float[][] array = m.ToArrayJagged();
      for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
          Assert.AreEqual(i * 3 + j + 1, array[i][j]);

      array = (float[][]) m;
      for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
          Assert.AreEqual(i * 3 + j + 1, array[i][j]);
    }


    [Test]
    public void ToMatrixF()
    {
      Matrix m33 = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);

      MatrixF m = m33.ToMatrixF();
      for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
          Assert.AreEqual(i * 3 + j + 1, m[i, j]);

      m = m33;
      for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
          Assert.AreEqual(i * 3 + j + 1, m[i, j]);
    }
  }
}
