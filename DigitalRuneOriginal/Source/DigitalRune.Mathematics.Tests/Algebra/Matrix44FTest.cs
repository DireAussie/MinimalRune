using System;
using System.Collections.Generic;
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
  public class MatrixTest
  {
    //           1,  2,  3,  4
    // Matrix =  5,  6,  7,  8
    //           9, 10, 11, 12,
    //          13, 14, 15, 16 

    // in column-major layout
    private readonly float[] columnMajor = new[] { 1.0f, 5.0f, 9.0f, 13.0f,
                                                   2.0f, 6.0f, 10.0f, 14.0f,
                                                   3.0f, 7.0f, 11.0f, 15.0f,
                                                   4.0f, 8.0f, 12.0f, 16.0f };

    // in row-major layout
    private readonly float[] rowMajor = new[] { 1.0f, 2.0f, 3.0f, 4.0f, 
                                                5.0f, 6.0f, 7.0f, 8.0f, 
                                                9.0f, 10.0f, 11.0f, 12.0f,
                                                13.0f, 14.0f, 15.0f, 16.0f };

    [Test]
    public void Constants()
    {
      Matrix zero = Matrix.Zero;
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(0.0, zero[i]);

      Matrix one = Matrix.One;
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(1.0f, one[i]);
    }


    [Test]
    public void Constructors()
    {
      Matrix m = new Matrix(1.0f, 2.0f, 3.0f, 4.0f,
                                  5.0f, 6.0f, 7.0f, 8.0f,
                                  9.0f, 10.0f, 11.0f, 12.0f,
                                  13.0f, 14.0f, 15.0f, 16.0f);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(columnMajor, MatrixOrder.ColumnMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new List<float>(columnMajor), MatrixOrder.ColumnMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new List<float>(rowMajor), MatrixOrder.RowMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new float[4, 4] { { 1, 2, 3, 4 }, 
                                          { 5, 6, 7, 8 }, 
                                          { 9, 10, 11, 12 }, 
                                          { 13, 14, 15, 16}});
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new float[4][] { new float[4] { 1, 2, 3, 4 }, 
                                         new float[4] { 5, 6, 7, 8 }, 
                                         new float[4] { 9, 10, 11, 12 },
                                         new float[4] { 13, 14, 15, 16}});
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = new Matrix(new Matrix(1, 2, 3,
                                      4, 5, 6,
                                      7, 8, 9),
                        new Vector3(10, 11, 12));
      Assert.AreEqual(new Matrix(1, 2, 3, 10,
                                    4, 5, 6, 11,
                                    7, 8, 9, 12,
                                    0, 0, 0, 1), m);
    }


    [Test]
    [ExpectedException(typeof(NullReferenceException))]
    public void ConstructorException1()
    {
      new Matrix(new float[4][]);
    }


    [Test]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void ConstructorException2()
    {
      float[][] elements = new float[4][];
      elements[0] = new float[4];
      elements[1] = new float[3];
      new Matrix(elements);
    }


    [Test]
    public void Properties()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.AreEqual(1.0f, m.M00);
      Assert.AreEqual(2.0f, m.M01);
      Assert.AreEqual(3.0f, m.M02);
      Assert.AreEqual(4.0f, m.M03);
      Assert.AreEqual(5.0f, m.M10);
      Assert.AreEqual(6.0f, m.M11);
      Assert.AreEqual(7.0f, m.M12);
      Assert.AreEqual(8.0f, m.M13);
      Assert.AreEqual(9.0f, m.M20);
      Assert.AreEqual(10.0f, m.M21);
      Assert.AreEqual(11.0f, m.M22);
      Assert.AreEqual(12.0f, m.M23);
      Assert.AreEqual(13.0f, m.M30);
      Assert.AreEqual(14.0f, m.M31);
      Assert.AreEqual(15.0f, m.M32);
      Assert.AreEqual(16.0f, m.M33);

      m = Matrix.Zero;
      m.M00 = 1.0f;
      m.M01 = 2.0f;
      m.M02 = 3.0f;
      m.M03 = 4.0f;
      m.M10 = 5.0f;
      m.M11 = 6.0f;
      m.M12 = 7.0f;
      m.M13 = 8.0f;
      m.M20 = 9.0f;
      m.M21 = 10.0f;
      m.M22 = 11.0f;
      m.M23 = 12.0f;
      m.M30 = 13.0f;
      m.M31 = 14.0f;
      m.M32 = 15.0f;
      m.M33 = 16.0f;
      Assert.AreEqual(new Matrix(rowMajor, MatrixOrder.RowMajor), m);
    }


    [Test]
    public void Indexer1d()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], m[i]);

      m = Matrix.Zero;
      for (int i = 0; i < 16; i++)
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
      m[16] = 0.0f;
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
      float x = m[16];
    }


    [Test]
    public void Indexer2d()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int column = 0; column < 4; column++)
        for (int row = 0; row < 4; row++)
          Assert.AreEqual(columnMajor[column * 4 + row], m[row, column]);
      m = Matrix.Zero;
      for (int column = 0; column < 4; column++)
        for (int row = 0; row < 4; row++)
          m[row, column] = (float)(row * 4 + column + 1);
      Assert.AreEqual(new Matrix(rowMajor, MatrixOrder.RowMajor), m);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException()
    {
      Matrix m = Matrix.Zero;
      m[0, 4] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException2()
    {
      Matrix m = Matrix.Zero;
      m[4, 0] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException3()
    {
      Matrix m = Matrix.Zero;
      m[3, -1] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException4()
    {
      Matrix m = Matrix.Zero;
      m[-1, 3] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException5()
    {
      Matrix m = Matrix.Zero;
      m[1, 4] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException6()
    {
      Matrix m = Matrix.Zero;
      m[2, 4] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException7()
    {
      Matrix m = Matrix.Zero;
      m[4, 1] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException8()
    {
      Matrix m = Matrix.Zero;
      m[4, 2] = 1.0f;
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException9()
    {
      Matrix m = Matrix.Zero;
      float x = m[0, 4];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException10()
    {
      Matrix m = Matrix.Zero;
      float x = m[4, 0];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException11()
    {
      Matrix m = Matrix.Zero;
      float x = m[3, -1];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException12()
    {
      Matrix m = Matrix.Zero;
      float x = m[-1, 3];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException13()
    {
      Matrix m = Matrix.Zero;
      float x = m[4, 1];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException14()
    {
      Matrix m = Matrix.Zero;
      float x = m[4, 2];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException15()
    {
      Matrix m = Matrix.Zero;
      float x = m[1, 4];
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Indexer2dException16()
    {
      Matrix m = Matrix.Zero;
      float x = m[2, 4];
    }


    [Test]
    public void GetMinor()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix minor = m.Minor;
      Matrix expected = new Matrix(1.0f, 2.0f, 3.0f,
                                       5.0f, 6.0f, 7.0f,
                                       9.0f, 10.0f, 11.0f);
      Assert.AreEqual(expected, minor);
    }


    [Test]
    public void SetMinor()
    {
      Matrix m = Matrix.Zero;
      Matrix minor = new Matrix(1.0f, 2.0f, 3.0f,
                                    5.0f, 6.0f, 7.0f,
                                    9.0f, 10.0f, 11.0f);
      m.Minor = minor;
      Assert.AreEqual(minor, m.Minor);
    }


    [Test]
    public void Rotation()
    {
      float angle = 0.3f;
      Vector3 axis = new Vector3(1.0f, 2.0f, 3.0f);
      Matrix m = Matrix.CreateRotation(axis, angle);
      Assert.IsTrue(Matrix.AreNumericallyEqual(Quaternion.CreateFromRotationMatrix(axis, angle).ToRotationMatrix44(), m));
    }


    [Test]
    public void Translation()
    {
      Vector3 translation = new Vector3(1.0f, 2.0f, 3.0f);
      Matrix m = Matrix.CreateTranslation(translation);
      Assert.AreEqual(translation, m.GetColumn(3).XYZ);
    }


    [Test]
    public void Translation2()
    {
      Vector3 translation = new Vector3(1.0f, 2.0f, 3.0f);
      Matrix m = Matrix.CreateTranslation(1.0f, 2.0f, 3.0f);
      Assert.AreEqual(translation, m.GetColumn(3).XYZ);
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
      m = Matrix.Transpose(m);
      Matrix mt = new Matrix(rowMajor, MatrixOrder.ColumnMajor);
      Assert.AreEqual(mt, m);
      Matrix i = Matrix.Identity;
      i = Matrix.Transpose(i);
      Assert.AreEqual(Matrix.Identity, i);
    }


    [Test]
    public void Inverse()
    {
      Assert.AreEqual(Matrix.Identity, Matrix.Identity.Inverse);

      Matrix m = new Matrix(1, 2, 3, 4,
                                2, 5, 8, 3,
                                7, 6, -1, 1,
                                4, 9, 7, 7);
      Vector4 v = Vector4.One;
      Vector4 w = m * v;
      Assert.IsTrue(Vector4.AreNumericallyEqual(v, m.Inverse * w));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * m.Inverse));
    }


    [Test]
    public void InverseWithNearSingularMatrix()
    {
      Matrix m = new Matrix(0.0001f, 0, 0, 0,
                                  0, 0.0001f, 0, 0,
                                  0, 0, 0.0001f, 0,
                                  0, 0, 0, 0.0001f);
      Vector4 v = Vector4.One;
      Vector4 w = m * v;
      Assert.IsTrue(Vector4.AreNumericallyEqual(v, m.Inverse * w));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * m.Inverse));
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void InverseException()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
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

      Matrix m = new Matrix(1, 2, 3, 4,
                                2, 5, 8, 3,
                                7, 6, -1, 1,
                                4, 9, 7, 7);
      Vector4 v = Vector4.One;
      Vector4 w = m * v;
      Matrix im = m;
      im.Invert();
      Assert.IsTrue(Vector4.AreNumericallyEqual(v, im * w));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * im));

      m = new Matrix(0.4f, 34, 0.33f, 4,
                                2, 5, -8, 3,
                                7, 0, -1, 1,
                                4, 9, -7, -45);
      v = Vector4.One;
      w = m * v;
      im = m;
      im.Invert();
      Assert.IsTrue(Vector4.AreNumericallyEqual(v, im * w));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * im));
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void InvertException()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.Invert();
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void InvertException2()
    {
      Matrix.Zero.Invert();
    }


    [Test]
    public void Determinant()
    {
      Matrix m = new Matrix(1, 2, 3, 4,
                                5, 6, 7, 8,
                                9, 10, 11, 12,
                                13, 14, 15, 16);
      Assert.AreEqual(0, m.Determinant());

      m = new Matrix(1, 2, 3, 4,
                       -3, 4, 5, 6,
                       2, -5, 7, 4,
                       10, 2, -3, 9);
      Assert.AreEqual(1142, m.Determinant());
    }


    [Test]
    public void IsNaN()
    {
      const int numberOfRows = 4;
      const int numberOfColumns = 4;
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
    public void IsSymmetric()
    {
      Matrix m = new Matrix(new float[4, 4] { { 1, 2, 3, 4 }, 
                                                    { 2, 4, 5, 6 }, 
                                                    { 3, 5, 7, 8 }, 
                                                    { 4, 6, 8, 9 } });
      Assert.AreEqual(true, m.IsSymmetric);

      m = new Matrix(new float[4, 4] { { 4, 3, 2, 1 }, 
                                          { 6, 5, 4, 2 }, 
                                          { 8, 7, 5, 3 }, 
                                          { 9, 8, 6, 4 } });
      Assert.AreEqual(false, m.IsSymmetric);
    }


    [Test]
    public void Trace()
    {
      Matrix m = new Matrix(new float[4, 4] { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } });
      Assert.AreEqual(34, m.Trace);
    }



    [Test]
    public void GetColumn()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.AreEqual(new Vector4(1.0f, 5.0f, 9.0f, 13.0f), m.GetColumn(0));
      Assert.AreEqual(new Vector4(2.0f, 6.0f, 10.0f, 14.0f), m.GetColumn(1));
      Assert.AreEqual(new Vector4(3.0f, 7.0f, 11.0f, 15.0f), m.GetColumn(2));
      Assert.AreEqual(new Vector4(4.0f, 8.0f, 12.0f, 16.0f), m.GetColumn(3));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetColumnException1()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.GetColumn(-1);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetColumnException2()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.GetColumn(4);
    }


    [Test]
    public void SetColumn()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.SetColumn(0, new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetColumn(0));
      Assert.AreEqual(new Vector4(2.0f, 6.0f, 10.0f, 14.0f), m.GetColumn(1));
      Assert.AreEqual(new Vector4(3.0f, 7.0f, 11.0f, 15.0f), m.GetColumn(2));
      Assert.AreEqual(new Vector4(4.0f, 8.0f, 12.0f, 16.0f), m.GetColumn(3));

      m.SetColumn(1, new Vector4(0.4f, 0.5f, 0.6f, 0.7f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetColumn(0));
      Assert.AreEqual(new Vector4(0.4f, 0.5f, 0.6f, 0.7f), m.GetColumn(1));
      Assert.AreEqual(new Vector4(3.0f, 7.0f, 11.0f, 15.0f), m.GetColumn(2));
      Assert.AreEqual(new Vector4(4.0f, 8.0f, 12.0f, 16.0f), m.GetColumn(3));

      m.SetColumn(2, new Vector4(0.7f, 0.8f, 0.9f, 1.0f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetColumn(0));
      Assert.AreEqual(new Vector4(0.4f, 0.5f, 0.6f, 0.7f), m.GetColumn(1));
      Assert.AreEqual(new Vector4(0.7f, 0.8f, 0.9f, 1.0f), m.GetColumn(2));
      Assert.AreEqual(new Vector4(4.0f, 8.0f, 12.0f, 16.0f), m.GetColumn(3));

      m.SetColumn(3, new Vector4(1.1f, 1.8f, 1.9f, 1.2f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetColumn(0));
      Assert.AreEqual(new Vector4(0.4f, 0.5f, 0.6f, 0.7f), m.GetColumn(1));
      Assert.AreEqual(new Vector4(0.7f, 0.8f, 0.9f, 1.0f), m.GetColumn(2));
      Assert.AreEqual(new Vector4(1.1f, 1.8f, 1.9f, 1.2f), m.GetColumn(3));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnException1()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.SetColumn(-1, Vector4.One);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetColumnException2()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.SetColumn(4, Vector4.One);
    }


    [Test]
    public void GetRow()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.AreEqual(new Vector4(1.0f, 2.0f, 3.0f, 4.0f), m.GetRow(0));
      Assert.AreEqual(new Vector4(5.0f, 6.0f, 7.0f, 8.0f), m.GetRow(1));
      Assert.AreEqual(new Vector4(9.0f, 10.0f, 11.0f, 12.0f), m.GetRow(2));
      Assert.AreEqual(new Vector4(13.0f, 14.0f, 15.0f, 16.0f), m.GetRow(3));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRowException1()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.GetRow(-1);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRowException2()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.GetRow(4);
    }


    [Test]
    public void SetRow()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.SetRow(0, new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetRow(0));
      Assert.AreEqual(new Vector4(5.0f, 6.0f, 7.0f, 8.0f), m.GetRow(1));
      Assert.AreEqual(new Vector4(9.0f, 10.0f, 11.0f, 12.0f), m.GetRow(2));
      Assert.AreEqual(new Vector4(13.0f, 14.0f, 15.0f, 16.0f), m.GetRow(3));

      m.SetRow(1, new Vector4(0.4f, 0.5f, 0.6f, 0.7f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetRow(0));
      Assert.AreEqual(new Vector4(0.4f, 0.5f, 0.6f, 0.7f), m.GetRow(1));
      Assert.AreEqual(new Vector4(9.0f, 10.0f, 11.0f, 12.0f), m.GetRow(2));
      Assert.AreEqual(new Vector4(13.0f, 14.0f, 15.0f, 16.0f), m.GetRow(3));

      m.SetRow(2, new Vector4(0.7f, 0.8f, 0.9f, 1.0f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetRow(0));
      Assert.AreEqual(new Vector4(0.4f, 0.5f, 0.6f, 0.7f), m.GetRow(1));
      Assert.AreEqual(new Vector4(0.7f, 0.8f, 0.9f, 1.0f), m.GetRow(2));
      Assert.AreEqual(new Vector4(13.0f, 14.0f, 15.0f, 16.0f), m.GetRow(3));

      m.SetRow(3, new Vector4(1.7f, 1.8f, 1.9f, 1.3f));
      Assert.AreEqual(new Vector4(0.1f, 0.2f, 0.3f, 0.4f), m.GetRow(0));
      Assert.AreEqual(new Vector4(0.4f, 0.5f, 0.6f, 0.7f), m.GetRow(1));
      Assert.AreEqual(new Vector4(0.7f, 0.8f, 0.9f, 1.0f), m.GetRow(2));
      Assert.AreEqual(new Vector4(1.7f, 1.8f, 1.9f, 1.3f), m.GetRow(3));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowException1()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.SetRow(-1, Vector4.One);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetRowException2()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m.SetRow(4, Vector4.One);
    }


    [Test]
    public void AreEqual()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Matrix m0 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m1 += new Matrix(0.000001f);
      Matrix m2 = new Matrix(rowMajor, MatrixOrder.RowMajor);
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
      Matrix m0 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m1 += new Matrix(0.002f);
      Matrix m2 = new Matrix(rowMajor, MatrixOrder.RowMajor);
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

      Vector4 v = Vector4.One;
      Matrix m = Matrix.CreateScale(2.0f);
      Vector4 scaled = m * v;
      Assert.AreEqual(2 * v.X, scaled.X);
      Assert.AreEqual(2 * v.Y, scaled.Y);
      Assert.AreEqual(2 * v.Z, scaled.Z);
      Assert.AreEqual(1.0f, scaled.W);


      m = Matrix.CreateScale(-1.0f, 1.5f, 2.0f);
      scaled = m * v;
      Assert.AreEqual(-1.0f * v.X, scaled.X);
      Assert.AreEqual(1.5 * v.Y, scaled.Y);
      Assert.AreEqual(2.0f * v.Z, scaled.Z);
      Assert.AreEqual(1.0f, scaled.W);

      Vector3 scale = new Vector3(-2.0f, -3.0f, -4.0f);
      m = Matrix.CreateScale(scale);
      v = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);
      scaled = m * v;
      Assert.AreEqual(-2.0f * v.X, scaled.X);
      Assert.AreEqual(-3.0f * v.Y, scaled.Y);
      Assert.AreEqual(-4.0f * v.Z, scaled.Z);
      Assert.AreEqual(1.0f, scaled.W);
    }


    [Test]
    public void CreateRotation()
    {
      Matrix m = Matrix.CreateRotation(Vector3.UnitX, 0.0f);
      Assert.AreEqual(Matrix.Identity, m);

      m = Matrix.CreateRotation(Vector3.UnitX, (float)Math.PI / 2);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.UnitZ, m.TransformPosition(Vector3.UnitY)));

      m = Matrix.CreateRotation(Vector3.UnitY, (float)Math.PI / 2);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.UnitX, m.TransformPosition(Vector3.UnitZ)));

      m = Matrix.CreateRotation(Vector3.UnitZ, (float)Math.PI / 2);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.UnitY, m.TransformPosition(Vector3.UnitX)));
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
      float angle = (float)MathHelper.ToRadians(30);
      Matrix m = Matrix.CreateRotationX(angle);
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.CreateRotation(Vector3.UnitX, angle), m));
    }


    [Test]
    public void CreateRotationY()
    {
      float angle = (float)MathHelper.ToRadians(30);
      Matrix m = Matrix.CreateRotationY(angle);
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.CreateRotation(Vector3.UnitY, angle), m));
    }


    [Test]
    public void CreateRotationZ()
    {
      float angle = MathHelper.ToRadians(30.0f);
      Matrix m = Matrix.CreateRotationZ(angle);
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.CreateRotation(Vector3.UnitZ, angle), m));
    }


    [Test]
    public void FromQuaternion()
    {
      float angle = -1.6f;
      Vector3 axis = new Vector3(1.0f, 2.0f, -3.0f);
      Matrix matrix = Matrix.CreateRotation(axis, angle);
      Quaternion q = Quaternion.CreateFromRotationMatrix(axis, angle);
      Matrix matrixFromQuaternion = Matrix.CreateRotation(q);
      Vector4 v = new Vector4(0.3f, -2.4f, 5.6f, 1.0f);
      Vector4 result1 = matrix * v;
      Vector4 result2 = matrixFromQuaternion * v;
      Assert.IsTrue(Vector4.AreNumericallyEqual(result1, result2));
    }


    [Test]
    public void HashCode()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.AreNotEqual(Matrix.Identity.GetHashCode(), m.GetHashCode());
    }


    [Test]
    public void TestEquals()
    {
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.IsTrue(m1.Equals(m1));
      Assert.IsTrue(m1.Equals(m2));
      for (int i = 0; i < 16; i++)
      {
        m2 = new Matrix(rowMajor, MatrixOrder.RowMajor);
        m2[i] += 0.1f;
        Assert.IsFalse(m1.Equals(m2));
      }

      Assert.IsFalse(m1.Equals(m1.ToString()));
    }


    [Test]
    public void EqualityOperators()
    {
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.IsTrue(m1 == m2);
      Assert.IsFalse(m1 != m2);
      for (int i = 0; i < 16; i++)
      {
        m2 = new Matrix(rowMajor, MatrixOrder.RowMajor);
        m2[i] += 0.1f;
        Assert.IsFalse(m1 == m2);
        Assert.IsTrue(m1 != m2);
      }
    }


    [Test]
    public void TestToString()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Assert.IsFalse(String.IsNullOrEmpty(m.ToString()));
    }


    [Test]
    public void NegationOperator()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(-rowMajor[i], (-m)[i]);
    }


    [Test]
    public void Negation()
    {
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(-rowMajor[i], Matrix.Negate(m)[i]);
    }


    [Test]
    public void AdditionOperator()
    {
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = new Matrix(rowMajor, MatrixOrder.RowMajor) * 3;
      Matrix result = m1 + m2;
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i] * 4, result[i]);
    }


    [Test]
    public void Addition()
    {
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = Matrix.One;
      Matrix result = Matrix.Add(m1, m2);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i] + 1.0f, result[i]);
    }


    [Test]
    public void SubtractionOperator()
    {
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = new Matrix(rowMajor, MatrixOrder.RowMajor) * 3;
      Matrix result = m1 - m2;
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(-rowMajor[i] * 2, result[i]);
    }


    [Test]
    public void Subtraction()
    {
      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = Matrix.One;
      Matrix result = Matrix.Subtract(m1, m2);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i] - 1.0f, result[i]);
    }


    [Test]
    public void MultiplicationOperator()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m = s * m;
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i] * s, m[i]);

      m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m = m * s;
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i] * s, m[i]);
    }


    [Test]
    public void Multiplication()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m = Matrix.Multiply(s, m);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i] * s, m[i]);
    }


    [Test]
    public void DivisionOperator()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m = m / s;
      for (int i = 0; i < 16; i++)
        Assert.IsTrue(Numeric.AreEqual(rowMajor[i] / s, m[i]));
    }


    [Test]
    public void Division()
    {
      float s = 0.1234f;
      Matrix m = new Matrix(rowMajor, MatrixOrder.RowMajor);
      m = Matrix.Divide(m, s);
      for (int i = 0; i < 16; i++)
        Assert.IsTrue(Numeric.AreEqual(rowMajor[i] / s, m[i]));
    }


    [Test]
    public void MultiplyMatrixOperator()
    {
      Matrix m = new Matrix(12, 23, 45, 56,
                                  67, 89, 90, 12,
                                  43, 65, 87, 43,
                                  34, -12, 84, 44);
      Assert.AreEqual(Matrix.Zero, m * Matrix.Zero);
      Assert.AreEqual(Matrix.Zero, Matrix.Zero * m);
      Assert.AreEqual(m, m * Matrix.Identity);
      Assert.AreEqual(m, Matrix.Identity * m);
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m * m.Inverse));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, m.Inverse * m));

      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = new Matrix(12, 23, 45, 56,
                                   67, 89, 90, 12,
                                   43, 65, 87, 43,
                                   34, -12, 84, 44);
      Matrix result = m1 * m2;
      for (int column = 0; column < 4; column++)
        for (int row = 0; row < 4; row++)
          Assert.AreEqual(Vector4.Dot(m1.GetRow(row), m2.GetColumn(column)), result[row, column]);
    }


    [Test]
    public void MultiplyMatrix()
    {
      Matrix m = new Matrix(12, 23, 45, 56,
                                  67, 89, 90, 12,
                                  43, 65, 87, 43,
                                  34, -12, 84, 44);
      Assert.AreEqual(Matrix.Zero, Matrix.Multiply(m, Matrix.Zero));
      Assert.AreEqual(Matrix.Zero, Matrix.Multiply(Matrix.Zero, m));
      Assert.AreEqual(m, Matrix.Multiply(m, Matrix.Identity));
      Assert.AreEqual(m, Matrix.Multiply(Matrix.Identity, m));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, Matrix.Multiply(m, m.Inverse)));
      Assert.IsTrue(Matrix.AreNumericallyEqual(Matrix.Identity, Matrix.Multiply(m.Inverse, m)));

      Matrix m1 = new Matrix(rowMajor, MatrixOrder.RowMajor);
      Matrix m2 = new Matrix(12, 23, 45, 56,
                                   67, 89, 90, 12,
                                   43, 65, 87, 43,
                                   34, -12, 84, 44);
      Matrix result = Matrix.Multiply(m1, m2);
      for (int column = 0; column < 4; column++)
        for (int row = 0; row < 4; row++)
          Assert.AreEqual(Vector4.Dot(m1.GetRow(row), m2.GetColumn(column)), result[row, column]);
    }


    [Test]
    public void MultiplyVectorOperator()
    {
      Vector4 v = new Vector4(2.34f, 3.45f, 4.56f, 23.4f);
      Assert.AreEqual(v, Matrix.Identity * v);
      Assert.AreEqual(Vector4.Zero, Matrix.Zero * v);

      Matrix m = new Matrix(12, 23, 45, 56,
                                  67, 89, 90, 12,
                                  43, 65, 87, 43,
                                  34, -12, 84, 44);
      Assert.IsTrue(Vector4.AreNumericallyEqual(v, m * m.Inverse * v));

      for (int i = 0; i < 4; i++)
        Assert.AreEqual(Vector4.Dot(m.GetRow(i), v), (m * v)[i]);
    }


    [Test]
    public void MultiplyVector()
    {
      Vector4 v = new Vector4(2.34f, 3.45f, 4.56f, 23.4f);
      Assert.AreEqual(v, Matrix.Multiply(Matrix.Identity, v));
      Assert.AreEqual(Vector4.Zero, Matrix.Multiply(Matrix.Zero, v));

      Matrix m = new Matrix(12, 23, 45, 56,
                                  67, 89, 90, 12,
                                  43, 65, 87, 43,
                                  34, -12, 84, 44);
      Assert.IsTrue(Vector4.AreNumericallyEqual(v, Matrix.Multiply(m * m.Inverse, v)));

      for (int i = 0; i < 4; i++)
        Assert.AreEqual(Vector4.Dot(m.GetRow(i), v), Matrix.Multiply(m, v)[i]);
    }


    [Test]
    public void ImplicitMatrix44DCast()
    {
      float m00 = 23.5f; float m01 = 0.0f; float m02 = -11.0f; float m03 = 0.3f;
      float m10 = 33.5f; float m11 = 1.1f; float m12 = -12.0f; float m13 = 0.4f;
      float m20 = 43.5f; float m21 = 2.2f; float m22 = -13.0f; float m23 = 0.5f;
      float m30 = 53.5f; float m31 = 3.3f; float m32 = -14.0f; float m33 = 0.6f;
      Matrix44D matrix44D = new Matrix(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
      Assert.IsTrue(Numeric.AreEqual(m00, (float)matrix44D[0, 0]));
      Assert.IsTrue(Numeric.AreEqual(m01, (float)matrix44D[0, 1]));
      Assert.IsTrue(Numeric.AreEqual(m02, (float)matrix44D[0, 2]));
      Assert.IsTrue(Numeric.AreEqual(m03, (float)matrix44D[0, 3]));
      Assert.IsTrue(Numeric.AreEqual(m10, (float)matrix44D[1, 0]));
      Assert.IsTrue(Numeric.AreEqual(m11, (float)matrix44D[1, 1]));
      Assert.IsTrue(Numeric.AreEqual(m12, (float)matrix44D[1, 2]));
      Assert.IsTrue(Numeric.AreEqual(m13, (float)matrix44D[1, 3]));
      Assert.IsTrue(Numeric.AreEqual(m20, (float)matrix44D[2, 0]));
      Assert.IsTrue(Numeric.AreEqual(m21, (float)matrix44D[2, 1]));
      Assert.IsTrue(Numeric.AreEqual(m22, (float)matrix44D[2, 2]));
      Assert.IsTrue(Numeric.AreEqual(m23, (float)matrix44D[2, 3]));
      Assert.IsTrue(Numeric.AreEqual(m30, (float)matrix44D[3, 0]));
      Assert.IsTrue(Numeric.AreEqual(m31, (float)matrix44D[3, 1]));
      Assert.IsTrue(Numeric.AreEqual(m32, (float)matrix44D[3, 2]));
      Assert.IsTrue(Numeric.AreEqual(m33, (float)matrix44D[3, 3]));
    }


    [Test]
    public void ToMatrix44D()
    {
      float m00 = 23.5f; float m01 = 0.0f; float m02 = -11.0f; float m03 = 0.3f;
      float m10 = 33.5f; float m11 = 1.1f; float m12 = -12.0f; float m13 = 0.4f;
      float m20 = 43.5f; float m21 = 2.2f; float m22 = -13.0f; float m23 = 0.5f;
      float m30 = 53.5f; float m31 = 3.3f; float m32 = -14.0f; float m33 = 0.6f;
      Matrix44D matrix44D = new Matrix(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33).ToMatrix44D();
      Assert.IsTrue(Numeric.AreEqual(m00, (float)matrix44D[0, 0]));
      Assert.IsTrue(Numeric.AreEqual(m01, (float)matrix44D[0, 1]));
      Assert.IsTrue(Numeric.AreEqual(m02, (float)matrix44D[0, 2]));
      Assert.IsTrue(Numeric.AreEqual(m03, (float)matrix44D[0, 3]));
      Assert.IsTrue(Numeric.AreEqual(m10, (float)matrix44D[1, 0]));
      Assert.IsTrue(Numeric.AreEqual(m11, (float)matrix44D[1, 1]));
      Assert.IsTrue(Numeric.AreEqual(m12, (float)matrix44D[1, 2]));
      Assert.IsTrue(Numeric.AreEqual(m13, (float)matrix44D[1, 3]));
      Assert.IsTrue(Numeric.AreEqual(m20, (float)matrix44D[2, 0]));
      Assert.IsTrue(Numeric.AreEqual(m21, (float)matrix44D[2, 1]));
      Assert.IsTrue(Numeric.AreEqual(m22, (float)matrix44D[2, 2]));
      Assert.IsTrue(Numeric.AreEqual(m23, (float)matrix44D[2, 3]));
      Assert.IsTrue(Numeric.AreEqual(m30, (float)matrix44D[3, 0]));
      Assert.IsTrue(Numeric.AreEqual(m31, (float)matrix44D[3, 1]));
      Assert.IsTrue(Numeric.AreEqual(m32, (float)matrix44D[3, 2]));
      Assert.IsTrue(Numeric.AreEqual(m33, (float)matrix44D[3, 3]));
    }


    [Test]
    public void TransformPosition()
    {
      Matrix scale = Matrix.CreateScale(2.5f);
      Matrix rotation = Matrix.CreateRotation(Vector3.One, 0.3f);
      Matrix translation = Matrix.CreateTranslation(new Vector3(1.0f, 2.0f, 3.0f));

      // Random transformation
      Matrix transform = translation * rotation * scale * translation.Inverse * rotation.Inverse;
      Vector4 v4 = new Vector4(1.0f, 2.0f, 0.5f, 1.0f);
      Vector3 v3 = new Vector3(1.0f, 2.0f, 0.5f);

      v4 = transform * v4;
      v3 = transform.TransformPosition(v3);
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(v4.X / v4.W, v4.Y / v4.W, v4.Z / v4.W), v3));

      // Test that involves a homogenous coordinate W component.
      translation = Matrix.CreateTranslation(2, 4, 6);
      translation.M33 = 2;
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(5, 8, 11) / 2, translation.TransformPosition(new Vector3(3, 4, 5))));
    }


    [Test]
    public void TransformVector()
    {
      Matrix scale = Matrix.CreateScale(2.5f);
      Matrix rotation = Matrix.CreateRotation(Vector3.One, 0.3f);
      Matrix translation = Matrix.CreateTranslation(new Vector3(1.0f, 2.0f, 3.0f));

      // Random transformation
      Matrix transform = translation * rotation * scale * translation.Inverse * rotation.Inverse;
      Vector4 p1 = new Vector4(1.0f, 2.0f, 0.5f, 1.0f);
      Vector4 p2 = new Vector4(-3.4f, 5.5f, -0.5f, 1.0f);
      Vector4 d = p1 - p2;
      Vector3 v = new Vector3(d.X, d.Y, d.Z);

      p1 = transform * p1;
      p1 /= p1.W;
      p2 = transform * p2;
      p2 /= p2.W;
      d = p1 - p2;
      v = transform.TransformDirection(v);
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(d.X, d.Y, d.Z), v));
    }


    [Test]
    public void TransformNormal()
    {
      // Random matrix
      Matrix transform = new Matrix(1, 2, 3, 4,
                                2, 5, 8, 3,
                                7, 6, -1, 1,
                                0, 0, 0, 1);

      Vector3 p3 = new Vector3(1.0f, 2.0f, 0.5f);
      Vector3 x3 = new Vector3(-3.4f, 5.5f, -0.5f);
      Vector3 d = (x3 - p3);
      Vector3 n3 = d.Orthonormal1;

      Vector4 p4 = new Vector4(p3.X, p3.Y, p3.Z, 1.0f);
      Vector4 x4 = new Vector4(x3.X, x3.Y, x3.Z, 1.0f);
      Vector4 n4 = new Vector4(n3.X, n3.Y, n3.Z, 0.0f);
      float planeEquation = Vector4.Dot((x4 - p4), n4);
      Assert.IsTrue(Numeric.IsZero(planeEquation));

      p4 = transform * p4;
      x4 = transform * x4;
      n3 = transform.TransformNormal(n3);
      n4 = new Vector4(n3.X, n3.Y, n3.Z, 0.0f);
      planeEquation = Vector4.Dot((x4 - p4), n4);
      Assert.IsTrue(Numeric.IsZero(planeEquation));
    }


    [Test]
    public void SerializationXml()
    {
      Matrix m1 = new Matrix(12, 23, 45, 56,
                                67, 89, 90, 12,
                                43, 65, 87, 43,
                                34, -12, 84, 44.3f);
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
      m2 = (Matrix)serializer.Deserialize(fileStream);
      Assert.AreEqual(m1, m2);
    }


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      Matrix m1 = new Matrix(12, 23, 45, 56,
                                   67, 89, 90, 12,
                                   43, 65, 87, 43,
                                   34, -12, 84, 44.3f);
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
      m2 = (Matrix)formatter.Deserialize(fs);
      fs.Close();

      Assert.AreEqual(m1, m2);
    }


    [Test]
    public void SerializationXml2()
    {
      Matrix m1 = new Matrix(12, 23, 45, 56,
                                   67, 89, 90, 12,
                                   43, 65, 87, 43,
                                   34, -12, 84, 44.3f);
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
      Matrix m1 = new Matrix(12, 23, 45, 56,
                                   67, 89, 90, 12,
                                   43, 65, 87, 43,
                                   34, -12, 84, 44.3f);
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
      Matrix absoluteM = new Matrix(-1, -2, -3, -4,
                                          -5, -6, -7, -8,
                                          -9, -10, -11, -12,
                                          -13, -14, -15, -16);
      absoluteM.Absolute();

      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M03);
      Assert.AreEqual(5, absoluteM.M10);
      Assert.AreEqual(6, absoluteM.M11);
      Assert.AreEqual(7, absoluteM.M12);
      Assert.AreEqual(8, absoluteM.M13);
      Assert.AreEqual(9, absoluteM.M20);
      Assert.AreEqual(10, absoluteM.M21);
      Assert.AreEqual(11, absoluteM.M22);
      Assert.AreEqual(12, absoluteM.M23);
      Assert.AreEqual(13, absoluteM.M30);
      Assert.AreEqual(14, absoluteM.M31);
      Assert.AreEqual(15, absoluteM.M32);
      Assert.AreEqual(16, absoluteM.M33);

      absoluteM = new Matrix(1, 2, 3, 4,
                                5, 6, 7, 8,
                                9, 10, 11, 12,
                                13, 14, 15, 16);
      absoluteM.Absolute();
      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M03);
      Assert.AreEqual(5, absoluteM.M10);
      Assert.AreEqual(6, absoluteM.M11);
      Assert.AreEqual(7, absoluteM.M12);
      Assert.AreEqual(8, absoluteM.M13);
      Assert.AreEqual(9, absoluteM.M20);
      Assert.AreEqual(10, absoluteM.M21);
      Assert.AreEqual(11, absoluteM.M22);
      Assert.AreEqual(12, absoluteM.M23);
      Assert.AreEqual(13, absoluteM.M30);
      Assert.AreEqual(14, absoluteM.M31);
      Assert.AreEqual(15, absoluteM.M32);
      Assert.AreEqual(16, absoluteM.M33);
    }

    [Test]
    public void AbsoluteStatic()
    {
      Matrix m = new Matrix(-1, -2, -3, -4,
                                  -5, -6, -7, -8,
                                  -9, -10, -11, -12,
                                  -13, -14, -15, -16);
      Matrix absoluteM = Matrix.Absolute(m);

      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M03);
      Assert.AreEqual(5, absoluteM.M10);
      Assert.AreEqual(6, absoluteM.M11);
      Assert.AreEqual(7, absoluteM.M12);
      Assert.AreEqual(8, absoluteM.M13);
      Assert.AreEqual(9, absoluteM.M20);
      Assert.AreEqual(10, absoluteM.M21);
      Assert.AreEqual(11, absoluteM.M22);
      Assert.AreEqual(12, absoluteM.M23);
      Assert.AreEqual(13, absoluteM.M30);
      Assert.AreEqual(14, absoluteM.M31);
      Assert.AreEqual(15, absoluteM.M32);
      Assert.AreEqual(16, absoluteM.M33);

      m = new Matrix(1, 2, 3, 4,
                        5, 6, 7, 8,
                        9, 10, 11, 12,
                        13, 14, 15, 16);
      absoluteM = Matrix.Absolute(m);
      Assert.AreEqual(1, absoluteM.M00);
      Assert.AreEqual(2, absoluteM.M01);
      Assert.AreEqual(3, absoluteM.M02);
      Assert.AreEqual(4, absoluteM.M03);
      Assert.AreEqual(5, absoluteM.M10);
      Assert.AreEqual(6, absoluteM.M11);
      Assert.AreEqual(7, absoluteM.M12);
      Assert.AreEqual(8, absoluteM.M13);
      Assert.AreEqual(9, absoluteM.M20);
      Assert.AreEqual(10, absoluteM.M21);
      Assert.AreEqual(11, absoluteM.M22);
      Assert.AreEqual(12, absoluteM.M23);
      Assert.AreEqual(13, absoluteM.M30);
      Assert.AreEqual(14, absoluteM.M31);
      Assert.AreEqual(15, absoluteM.M32);
      Assert.AreEqual(16, absoluteM.M33);
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
      Matrix m = new Matrix(1, 2, 3, 4,
                                  5, 6, 7, 8,
                                  9, 10, 11, 12,
                                  13, 14, 15, 16);
      float[] array = m.ToArray1D(MatrixOrder.RowMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], array[i]);
      array = m.ToArray1D(MatrixOrder.ColumnMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(columnMajor[i], array[i]);
    }


    [Test]
    public void ToList()
    {
      Matrix m = new Matrix(1, 2, 3, 4,
                                  5, 6, 7, 8,
                                  9, 10, 11, 12,
                                  13, 14, 15, 16);
      IList<float> list = m.ToList(MatrixOrder.RowMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(rowMajor[i], list[i]);
      list = m.ToList(MatrixOrder.ColumnMajor);
      for (int i = 0; i < 16; i++)
        Assert.AreEqual(columnMajor[i], list[i]);
    }


    [Test]
    public void ToArray2D()
    {
      Matrix m = new Matrix(1, 2, 3, 4,
                                  5, 6, 7, 8,
                                  9, 10, 11, 12, 13,
                                  14, 15, 16);

      float[,] array = m.ToArray2D();
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Assert.AreEqual(i * 4 + j + 1, array[i, j]);

      array = (float[,])m;
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Assert.AreEqual(i * 4 + j + 1, array[i, j]);
    }


    [Test]
    public void ToArrayJagged()
    {
      Matrix m = new Matrix(1, 2, 3, 4,
                                  5, 6, 7, 8,
                                  9, 10, 11, 12,
                                  13, 14, 15, 16);

      float[][] array = m.ToArrayJagged();
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Assert.AreEqual(i * 4 + j + 1, array[i][j]);

      array = (float[][])m;
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Assert.AreEqual(i * 4 + j + 1, array[i][j]);
    }


    [Test]
    public void ToMatrixF()
    {
      Matrix m44 = new Matrix(1, 2, 3, 4,
                                    5, 6, 7, 8,
                                    9, 10, 11, 12,
                                    13, 14, 15, 16);

      MatrixF m = m44.ToMatrixF();
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Assert.AreEqual(i * 4 + j + 1, m[i, j]);

      m = m44;
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Assert.AreEqual(i * 4 + j + 1, m[i, j]);
    }


    [Test]
    public void ExplicitFromXnaCast()
    {
      Matrix xna = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
      Matrix v = (Matrix)xna;

      Assert.AreEqual(xna.M11, v.M00);
      Assert.AreEqual(xna.M12, v.M10);
      Assert.AreEqual(xna.M13, v.M20);
      Assert.AreEqual(xna.M14, v.M30);
      Assert.AreEqual(xna.M21, v.M01);
      Assert.AreEqual(xna.M22, v.M11);
      Assert.AreEqual(xna.M23, v.M21);
      Assert.AreEqual(xna.M24, v.M31);
      Assert.AreEqual(xna.M31, v.M02);
      Assert.AreEqual(xna.M32, v.M12);
      Assert.AreEqual(xna.M33, v.M22);
      Assert.AreEqual(xna.M34, v.M32);
      Assert.AreEqual(xna.M41, v.M03);
      Assert.AreEqual(xna.M42, v.M13);
      Assert.AreEqual(xna.M43, v.M23);
      Assert.AreEqual(xna.M44, v.M33);
    }


    [Test]
    public void FromXna()
    {
      Matrix xna = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
      Matrix v = Matrix.FromXna(xna);

      Assert.AreEqual(xna.M11, v.M00);
      Assert.AreEqual(xna.M12, v.M10);
      Assert.AreEqual(xna.M13, v.M20);
      Assert.AreEqual(xna.M14, v.M30);
      Assert.AreEqual(xna.M21, v.M01);
      Assert.AreEqual(xna.M22, v.M11);
      Assert.AreEqual(xna.M23, v.M21);
      Assert.AreEqual(xna.M24, v.M31);
      Assert.AreEqual(xna.M31, v.M02);
      Assert.AreEqual(xna.M32, v.M12);
      Assert.AreEqual(xna.M33, v.M22);
      Assert.AreEqual(xna.M34, v.M32);
      Assert.AreEqual(xna.M41, v.M03);
      Assert.AreEqual(xna.M42, v.M13);
      Assert.AreEqual(xna.M43, v.M23);
      Assert.AreEqual(xna.M44, v.M33);
    }


    [Test]
    public void ExplicitToXnaCast()
    {
      Matrix v = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
      Matrix xna = (Matrix)v;

      Assert.AreEqual(xna.M11, v.M00);
      Assert.AreEqual(xna.M12, v.M10);
      Assert.AreEqual(xna.M13, v.M20);
      Assert.AreEqual(xna.M14, v.M30);
      Assert.AreEqual(xna.M21, v.M01);
      Assert.AreEqual(xna.M22, v.M11);
      Assert.AreEqual(xna.M23, v.M21);
      Assert.AreEqual(xna.M24, v.M31);
      Assert.AreEqual(xna.M31, v.M02);
      Assert.AreEqual(xna.M32, v.M12);
      Assert.AreEqual(xna.M33, v.M22);
      Assert.AreEqual(xna.M34, v.M32);
      Assert.AreEqual(xna.M41, v.M03);
      Assert.AreEqual(xna.M42, v.M13);
      Assert.AreEqual(xna.M43, v.M23);
      Assert.AreEqual(xna.M44, v.M33);
    }


    [Test]
    public void ToXna()
    {
      Matrix v = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
      Matrix xna = v.ToXna();

      Assert.AreEqual(xna.M11, v.M00);
      Assert.AreEqual(xna.M12, v.M10);
      Assert.AreEqual(xna.M13, v.M20);
      Assert.AreEqual(xna.M14, v.M30);
      Assert.AreEqual(xna.M21, v.M01);
      Assert.AreEqual(xna.M22, v.M11);
      Assert.AreEqual(xna.M23, v.M21);
      Assert.AreEqual(xna.M24, v.M31);
      Assert.AreEqual(xna.M31, v.M02);
      Assert.AreEqual(xna.M32, v.M12);
      Assert.AreEqual(xna.M33, v.M22);
      Assert.AreEqual(xna.M34, v.M32);
      Assert.AreEqual(xna.M41, v.M03);
      Assert.AreEqual(xna.M42, v.M13);
      Assert.AreEqual(xna.M43, v.M23);
      Assert.AreEqual(xna.M44, v.M33);
    }


    [Test]
    public void TranslationProperty()
    {
      Vector3 translation = new Vector3(1, 2, 3);
      Matrix srt = Matrix.CreateTranslation(translation) * Matrix.CreateRotation(new Vector3(4, 5, 6), MathHelper.ToRadians(37)) * Matrix.CreateScale(2.3f);
      Assert.AreEqual(translation, srt.Translation);

      translation = new Vector3(-3, 0, 9);
      srt.Translation = translation;
      Assert.AreEqual(translation, srt.Translation);
    }


    [Test]
    public void DecomposeTest()
    {
      Vector3 scale = new Vector3(1.0f, 2.0f, 3.0f);
      Quaternion rotation = Quaternion.CreateFromRotationMatrix(new Vector3(4, 5, 6), MathHelper.ToRadians(37));
      Vector3 translation = new Vector3(-3.0f, 0.5f, 9.0f);

      Matrix srt = Matrix.CreateTranslation(translation) * Matrix.CreateRotation(rotation) * Matrix.CreateScale(scale);

      Vector3 scaleOfMatrix;
      Quaternion rotationOfMatrix;
      Vector3 translationOfMatrix;
      bool result = srt.Decompose(out scaleOfMatrix, out rotationOfMatrix, out translationOfMatrix);
      Assert.IsTrue(result);
      Assert.IsTrue(Vector3.AreNumericallyEqual(scale, scaleOfMatrix));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(rotation, rotationOfMatrix));
      Assert.IsTrue(Vector3.AreNumericallyEqual(translation, translationOfMatrix));
    }


    [Test]
    public void DecomposeWithNegativeScaleTest()
    {
      Vector3 scale = new Vector3(-2.0f, 3.0f, 4.0f);
      Quaternion rotation = Quaternion.CreateFromRotationMatrix(new Vector3(4, 5, 6), MathHelper.ToRadians(37));
      Vector3 translation = new Vector3(-3.0f, 0.5f, 9.0f);

      Matrix srt = Matrix.CreateTranslation(translation) * Matrix.CreateRotation(rotation) * Matrix.CreateScale(scale);

      Vector3 scaleOfMatrix;
      Quaternion rotationOfMatrix;
      Vector3 translationOfMatrix;
      bool result = srt.Decompose(out scaleOfMatrix, out rotationOfMatrix, out translationOfMatrix);
      Assert.IsTrue(result);
      Matrix srt2 = Matrix.CreateTranslation(translationOfMatrix) * Matrix.CreateRotation(rotationOfMatrix) * Matrix.CreateScale(scaleOfMatrix);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt, srt2));
    }


    [Test]
    public void DecomposeShouldFail()
    {
      Matrix matrix = new Matrix();

      Vector3 scaleOfMatrix;
      Quaternion rotationOfMatrix;
      Vector3 translationOfMatrix;
      bool result = matrix.Decompose(out scaleOfMatrix, out rotationOfMatrix, out translationOfMatrix);
      Assert.IsFalse(result);

      matrix = new Matrix(rowMajor, MatrixOrder.RowMajor);
      result = matrix.Decompose(out scaleOfMatrix, out rotationOfMatrix, out translationOfMatrix);
      Assert.IsFalse(result);
    }


    [Test]
    public void DecomposeWithZeroScale()
    {
      Vector3 s0;
      Quaternion r0 = Quaternion.CreateFromRotationMatrix(new Vector3(4, 5, 6), MathHelper.ToRadians(37));
      Vector3 t0 = new Vector3(-3.0f, 0.5f, 9.0f);

      s0 = new Vector3(0, -2, 3);
      Matrix srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);

      Vector3 s1;
      Quaternion r1;
      Vector3 t1;
      bool result = srt0.Decompose(out s1, out r1, out t1);
      Matrix srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(result);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));

      s0 = new Vector3(-2, 0, 3);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);

      result = srt0.Decompose(out s1, out r1, out t1);
      srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(result);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));

      s0 = new Vector3(2, -3, 0);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);

      result = srt0.Decompose(out s1, out r1, out t1);
      srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(result);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));

      s0 = new Vector3(1, 0, 0);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);
      result = srt0.Decompose(out s1, out r1, out t1);
      Assert.IsFalse(result);

      s0 = new Vector3(0, 1, 0);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);
      result = srt0.Decompose(out s1, out r1, out t1);
      Assert.IsFalse(result);

      s0 = new Vector3(0, 0, 1);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);
      result = srt0.Decompose(out s1, out r1, out t1);
      Assert.IsFalse(result);
    }


    [Test]
    public void DecomposeFast()
    {
      Vector3 s0;
      Quaternion r0 = Quaternion.CreateFromRotationMatrix(new Vector3(4, 5, 6), MathHelper.ToRadians(37));
      Vector3 t0 = new Vector3(-3.0f, 0.5f, 9.0f);

      s0 = new Vector3(-4, -2, 3);
      Matrix srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);

      Vector3 s1;
      Quaternion r1;
      Vector3 t1;
      srt0.DecomposeFast(out s1, out r1, out t1);
      Matrix srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));

      s0 = new Vector3(2, -2, 3);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);
      srt0.DecomposeFast(out s1, out r1, out t1);
      srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));

      s0 = new Vector3(0, -2, 3);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);
      srt0.DecomposeFast(out s1, out r1, out t1);
      srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));

      s0 = new Vector3(-2, 0, 3);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);

      srt0.DecomposeFast(out s1, out r1, out t1);
      srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));

      s0 = new Vector3(2, -3, 0);
      srt0 = Matrix.CreateTranslation(t0) * Matrix.CreateRotation(r0) * Matrix.CreateScale(s0);

      srt0.DecomposeFast(out s1, out r1, out t1);
      srt1 = Matrix.CreateTranslation(t1) * Matrix.CreateRotation(r1) * Matrix.CreateScale(s1);
      Assert.IsTrue(Matrix.AreNumericallyEqual(srt0, srt1));
    }



    [Test]
    public void CreateLookAtTest()
    {
      Vector3 cameraPosition = new Vector3(100, 10, -50);
      Vector3 cameraForward = new Vector3(0.5f, -2, -3);
      Vector3 cameraTarget = cameraPosition + cameraForward * 100;
      Vector3 cameraUpVector = new Vector3(2, 3, 4).Normalized;
      Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, cameraUpVector);

      // ---- Construct a point that is at position (1, 2, -3) in view space:
      // Move point to (0, 0, -3) in view space.
      Vector3 cameraRight = Vector3.Cross(cameraForward, cameraUpVector).Normalized;
      Vector3 cameraUp = Vector3.Cross(cameraRight, cameraForward).Normalized;
      Vector3 point = cameraPosition + 3 * cameraForward.Normalized;
      // Move point to (0, 2, -3) in view space.
      point += 2 * cameraUp;
      // Move point to (1, 2, -3) in view space.
      point += 1 * cameraRight;

      point = view.TransformPosition(point);
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 2, -3), point));
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateLookAtException()
    {
      Vector3 cameraPosition = new Vector3(100, 10, -50);
      Vector3 cameraTarget = new Vector3(100, 10, -50);
      Vector3 cameraUpVector = new Vector3(2, 3, 4).Normalized;
      Matrix.CreateLookAt(cameraPosition, cameraTarget, cameraUpVector);
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateLookAtException2()
    {
      Vector3 cameraPosition = new Vector3(100, 10, -50);
      Vector3 cameraTarget = new Vector3(110, 10, -50);
      Vector3 cameraUpVector = new Vector3(0, 0, 0);
      Matrix.CreateLookAt(cameraPosition, cameraTarget, cameraUpVector);
    }


    

    

    [Test]
    public void CreateOrthographicTest()
    {
      Matrix projection = Matrix.CreateOrthographic(4, 3, 1, 11);

      Vector3 pointProjectionSpace = projection.TransformPosition(new Vector3(2, 0.75f, -6));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 0.5f, 0.5f), pointProjectionSpace));

      // zNear = 0 should be allowed.
      Matrix.CreateOrthographic(4, 3, 0, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrthographicException()
    {
      Matrix.CreateOrthographic(0, 3, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrthographicException2()
    {
      Matrix.CreateOrthographic(4, 0, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrthographicException3()
    {
      Matrix.CreateOrthographic(4, 3, 1, 1);
    }

    [Test]
    public void OrthographicWithNegativeNear()
    {
      Matrix projection = Matrix.CreateOrthographic(4, 3, -1, 11);
      Assert.IsTrue(Vector4.AreNumericallyEqual(new Vector4(-1, 1, 0, 1), projection * new Vector4(-2, 1.5f, 1, 1)));
      Assert.IsTrue(Vector4.AreNumericallyEqual(new Vector4(1, 1, 0.5f, 1), projection * new Vector4(2, 1.5f, -5, 1)));
      Assert.IsTrue(Vector4.AreNumericallyEqual(new Vector4(1, -1, 1, 1), projection * new Vector4(2, -1.5f, -11, 1)));
    }

    [Test]
    public void CreateOrthographicOffCenterTest()
    {
      Matrix projection = Matrix.CreateOrthographicOffCenter(0, 4, 0, 3, 1, 11);

      Vector3 pointProjectionSpace = projection.TransformPosition(new Vector3(4, 2.25f, -6));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 0.5f, 0.5f), pointProjectionSpace));

      // zNear = 0 should be allowed.
      Matrix.CreateOrthographicOffCenter(0, 4, 0, 3, 0, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrthographicOffCenterException()
    {
      Matrix.CreateOrthographicOffCenter(4, 4, 0, 3, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrthographicOffCenterException2()
    {
      Matrix.CreateOrthographicOffCenter(0, 4, 3, 3, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrthographicOffCenterException3()
    {
      Matrix.CreateOrthographicOffCenter(0, 4, 0, 3, 1, 1);
    }

    [Test]
    public void OrthographicOffCenterWithNegativeNear()
    {
      Matrix projection = Matrix.CreateOrthographicOffCenter(0, 4, 0, 3, -1, 11);
      Assert.IsTrue(Vector4.AreNumericallyEqual(new Vector4(-1, 1, 0, 1), projection * new Vector4(0, 3, 1, 1)));
      Assert.IsTrue(Vector4.AreNumericallyEqual(new Vector4(1, 1, 0.5f, 1), projection * new Vector4(4, 3, -5, 1)));
      Assert.IsTrue(Vector4.AreNumericallyEqual(new Vector4(1, -1, 1, 1), projection * new Vector4(4, 0, -11, 1)));
    }

    [Test]
    public void CreatePerspectiveTest()
    {
      Matrix projection = Matrix.CreatePerspective(4, 3, 1, 11);

      Vector3 p = Vector4.HomogeneousDivide(projection * new Vector4(-2, -1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(-1, -1, 0), p));
      p = Vector4.HomogeneousDivide(projection * new Vector4(2, 1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 1, 0), p));
      p = Vector4.HomogeneousDivide(projection * new Vector4(0, 0, -11, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(0, 0, 1), p));
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveException()
    {
      Matrix.CreatePerspective(0, 3, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveException2()
    {
      Matrix.CreatePerspective(4, 0, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveException3()
    {
      Matrix.CreatePerspective(4, 3, 0, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveException4()
    {
      Matrix.CreatePerspective(4, 3, 1, 0);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePerspectiveException5()
    {
      Matrix.CreatePerspective(4, 3, 1, 1);
    }

    [Test]
    public void CreatePerspectiveFieldOfViewTest()
    {
      // Use same field of view as in CreatePerspectiveTest
      float fieldOfView = (float)(2 * Math.Atan(1.5 / 1));
      Matrix projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, 4f / 3f, 1, 11);

      Vector3 p = Vector4.HomogeneousDivide(projection * new Vector4(-2, -1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(-1, -1, 0), p));
      p = Vector4.HomogeneousDivide(projection * new Vector4(2, 1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 1, 0), p));
      p = Vector4.HomogeneousDivide(projection * new Vector4(0, 0, -11, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(0, 0, 1), p));
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveFieldOfViewException()
    {
      Matrix.CreatePerspectiveFieldOfView(0, 4f / 3f, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveFieldOfViewException2()
    {
      Matrix.CreatePerspectiveFieldOfView(ConstantsF.Pi, 4f / 3f, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveFieldOfViewException3()
    {
      Matrix.CreatePerspectiveFieldOfView(ConstantsF.PiOver2, 0, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveFieldOfViewException4()
    {
      Matrix.CreatePerspectiveFieldOfView(ConstantsF.PiOver2, 4f / 3f, 0, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveFieldOfViewException5()
    {
      Matrix.CreatePerspectiveFieldOfView(ConstantsF.PiOver2, 4f / 3f, 1, 0);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePerspectiveFieldOfViewException6()
    {
      Matrix.CreatePerspectiveFieldOfView(ConstantsF.PiOver2, 4f / 3f, 1, 1);
    }

    [Test]
    public void CreatePerspectiveOffCenterTest()
    {
      Matrix projection = Matrix.CreatePerspectiveOffCenter(0, 4, 0, 3, 1, 11);

      Vector3 p = Vector4.HomogeneousDivide(projection * new Vector4(0, 0, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(-1, -1, 0), p));
      p = Vector4.HomogeneousDivide(projection * new Vector4(4, 3, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 1, 0), p));
      p = Vector4.HomogeneousDivide(projection * new Vector4(0, 0, -11, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(-1, -1, 1), p));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePerspectiveOffCenterException()
    {
      Matrix.CreatePerspectiveOffCenter(4, 4, 0, 3, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePerspectiveOffCenterException2()
    {
      Matrix.CreatePerspectiveOffCenter(0, 4, 3, 3, 1, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveOffCenterException3()
    {
      Matrix.CreatePerspectiveOffCenter(0, 4, 0, 3, 0, 11);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreatePerspectiveOffCenterException4()
    {
      Matrix.CreatePerspectiveOffCenter(0, 4, 0, 3, 1, 0);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePerspectiveOffCenterException5()
    {
      Matrix.CreatePerspectiveOffCenter(0, 4, 0, 3, 1, 1);
    }

    [Test]
    public void CreateInfinitePerspectiveTest()
    {
      Matrix pInfinite = Matrix.CreatePerspective(4, 3, 1, float.PositiveInfinity);

      Vector3 v = Vector4.HomogeneousDivide(pInfinite * new Vector4(-2, -1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(-1, -1, 0), v));
      v = Vector4.HomogeneousDivide(pInfinite * new Vector4(2, 1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 1, 0), v));
      v = Vector4.HomogeneousDivide(pInfinite * new Vector4(0, 0, -1000, 1));
      Assert.AreEqual(0, v.X);
      Assert.AreEqual(0, v.Y);
      Assert.LessOrEqual(0.999, v.Z);
    }

    [Test]
    public void CreateInfinitePerspectiveFieldOfViewTest()
    {
      // Use same field of view as in CreatePerspectiveTest
      float fieldOfView = (float)(2 * Math.Atan(1.5 / 1));
      Matrix pInfinite = Matrix.CreatePerspectiveFieldOfView(fieldOfView, 4f / 3f, 1, float.PositiveInfinity);

      Vector3 v = Vector4.HomogeneousDivide(pInfinite * new Vector4(-2, -1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(-1, -1, 0), v));
      v = Vector4.HomogeneousDivide(pInfinite * new Vector4(2, 1.5f, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 1, 0), v));
      v = Vector4.HomogeneousDivide(pInfinite * new Vector4(0, 0, -1000, 1));
      Assert.AreEqual(0, v.X);
      Assert.AreEqual(0, v.Y);
      Assert.LessOrEqual(0.999, v.Z);
    }

    [Test]
    public void CreateInfinitePerspectiveOffCenterTest()
    {
      Matrix pInfinite = Matrix.CreatePerspectiveOffCenter(0, 4, 0, 3, 1, float.PositiveInfinity);

      Vector3 v = Vector4.HomogeneousDivide(pInfinite * new Vector4(0, 0, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(-1, -1, 0), v));
      v = Vector4.HomogeneousDivide(pInfinite * new Vector4(4, 3, -1, 1));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(1, 1, 0), v));
      v = Vector4.HomogeneousDivide(pInfinite * new Vector4(0, 0, -1000, 1));
      Assert.AreEqual(-1, v.X);
      Assert.AreEqual(-1, v.Y);
      Assert.LessOrEqual(0.999, v.Z);
    }

  }
}
