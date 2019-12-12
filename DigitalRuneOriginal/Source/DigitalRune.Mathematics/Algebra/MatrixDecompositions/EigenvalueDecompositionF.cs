// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;


namespace DigitalRune.Mathematics.Algebra
{
  /// <summary>
  /// Computes the eigenvalues and eigenvectors of a real square matrix A (single-precision).
  /// </summary>
  /// <remarks>
  /// <para>
  /// If the matrix A is symmetric, then A = V * D * V<sup>T</sup> where the eigenvalue matrix D is 
  /// a diagonal matrix and the eigenvector matrix V is orthogonal.
  /// </para>
  /// <para>
  /// If A is not symmetric, then the eigenvalue matrix D is block diagonal with the real
  /// eigenvalues in 1 x 1 blocks and any complex eigenvalues (λ + i*μ) in 2 x 2 blocks
  /// ((λ, μ),(-μ, λ)). The columns of V represent the eigenvectors in the sense that
  /// A * V = V * D. The matrix V may be badly conditioned or even singular; so if the inverse of V
  /// can be computed depends on the condition number of V. (The condition number can be checked
  /// with <see cref="SingularValueDecompositionF"/>.)
  /// </para>
  /// </remarks>
  public class EigenvalueDecompositionF
  {
    //--------------------------------------------------------------

    //--------------------------------------------------------------

    private readonly MatrixF _v;
    private readonly VectorF _d; // Real eigenvalues.
    private MatrixF _matrixD;
    private readonly VectorF _e; // Imaginary eigenvalues.
    private readonly int _n;
    private readonly bool _isSymmetric;



    //--------------------------------------------------------------

    //--------------------------------------------------------------

    /// <summary>
    /// Gets the vector of the imaginary parts of the eigenvalues. 
    /// </summary>
    /// <value>The vector of the imaginary parts of the eigenvalues.</value>
    public VectorF ImaginaryEigenvalues
    {
      get { return _e; }
    }


    /// <summary>
    /// Gets the block diagonal eigenvalue matrix D. (This property returns the internal matrix, 
    /// not a copy.)
    /// </summary>
    /// <value>The block diagonal eigenvalue matrix D.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public MatrixF D
    {
      get 
      {
        if (_matrixD != null)
          return _matrixD;

        _matrixD = new MatrixF(_n, _n);
        for (int i = 0; i < _n; i++)
        {
          //for (int j = 0; j < _n; j++)
            //D[i, j] = 0.0;
          _matrixD[i, i] = _d[i];
          if (_e[i] > 0)
            _matrixD[i, i + 1] = _e[i];
          else if (_e[i] < 0)
            _matrixD[i, i - 1] = _e[i];
        }
        return _matrixD;
      }
    }


    /// <summary>
    /// Gets the eigenvector matrix V. (This property returns the internal matrix, 
    /// not a copy.)
    /// </summary>
    /// <value>The eigenvector matrix V.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public MatrixF V
    {
      get { return _v; }
    }


    /// <summary>
    /// Gets the vector of the real parts of the eigenvalues (the diagonal of D). 
    /// </summary>
    /// <value>The vector of the real parts of the eigenvalues.</value>
    public VectorF RealEigenvalues
    {
      get { return _d; }
    }    



    //--------------------------------------------------------------

    //--------------------------------------------------------------

    /// <summary>
    /// Creates the eigenvalue decomposition of the given matrix.
    /// </summary>
    /// <param name="matrixA">The square matrix A.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="matrixA"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="matrixA"/> is non-square (rectangular).
    /// </exception>
    public EigenvalueDecompositionF(MatrixF matrixA)
    {
      if (matrixA == null)
        throw new ArgumentNullException("matrixA");
      if (matrixA.IsSquare == false)
        throw new ArgumentException("The matrix A must be square.", "matrixA");

      _n = matrixA.NumberOfColumns;
      _d = new VectorF(_n);
      _e = new VectorF(_n);

      _isSymmetric = matrixA.IsSymmetric;
      
      if (_isSymmetric)
      {
        _v = matrixA.Clone();

        // Tridiagonalize.
        ReduceToTridiagonal();

        // Diagonalize.
        TridiagonalToQL();
      }
      else
      {
        _v = new MatrixF(_n, _n);

        // Abort if A contains NaN values.
        // If we continue with NaN values, we run into an infinite loop.
        for (int i = 0; i < _n; i++)
        {
          for (int j = 0; j < _n; j++)
          {
            if (Numeric.IsNaN(matrixA[i, j]))
            {
              _e.Set(float.NaN);
              _v.Set(float.NaN);
              _d.Set(float.NaN);
              return;
            }
          }
        }        

        // Storage of nonsymmetric Hessenberg form.
        MatrixF matrixH = matrixA.Clone();
        // Working storage for nonsymmetric algorithm.
        float[] ort = new float[_n];

        // Reduce to Hessenberg form.
        ReduceToHessenberg(matrixH, ort);

        // Reduce Hessenberg to real Schur form.
        HessenbergToRealSchur(matrixH);
      }
    }



    //--------------------------------------------------------------

    //--------------------------------------------------------------

    // Symmetric Householder reduction to tridiagonal form.
    private void ReduceToTridiagonal()
    {
      //  This is derived from the Algol procedures tred2 by
      //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
      //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      for (int j = 0; j < _n; j++)
        _d[j] = _v[_n - 1, j];

      // Householder reduction to tridiagonal form.

      for (int i = _n - 1; i > 0; i--)
      {
        // Scale to avoid under/overflow.
        float scale = 0;
        float h = 0;
        for (int k = 0; k < i; k++)
          scale = scale + Math.Abs(_d[k]);
        if (scale == 0)
        {
          _e[i] = _d[i - 1];
          for (int j = 0; j < i; j++)
          {
            _d[j] = _v[i - 1, j];
            _v[i, j] = 0;
            _v[j, i] = 0;
          }
        }
        else
        {
          // Generate Householder vector.

          for (int k = 0; k < i; k++)
          {
            _d[k] /= scale;
            h += _d[k] * _d[k];
          }
          float f = _d[i - 1];
          float g = (float)Math.Sqrt(h);
          if (f > 0)
            g = -g;
          _e[i] = scale * g;
          h = h - f * g;
          _d[i - 1] = f - g;
          for (int j = 0; j < i; j++)
            _e[j] = 0;

          // Apply similarity transformation to remaining columns.

          for (int j = 0; j < i; j++)
          {
            f = _d[j];
            _v[j, i] = f;
            g = _e[j] + _v[j, j] * f;
            for (int k = j + 1; k <= i - 1; k++)
            {
              g += _v[k, j] * _d[k];
              _e[k] += _v[k, j] * f;
            }
            _e[j] = g;
          }
          f = 0;
          for (int j = 0; j < i; j++)
          {
            _e[j] /= h;
            f += _e[j] * _d[j];
          }
          float hh = f / (h + h);
          for (int j = 0; j < i; j++)
          {
            _e[j] -= hh * _d[j];
          }
          for (int j = 0; j < i; j++)
          {
            f = _d[j];
            g = _e[j];
            for (int k = j; k <= i - 1; k++)
            {
              _v[k, j] -= (f * _e[k] + g * _d[k]);
            }
            _d[j] = _v[i - 1, j];
            _v[i, j] = 0;
          }
        }
        _d[i] = h;
      }

      // Accumulate transformations.

      for (int i = 0; i < _n - 1; i++)
      {
        _v[_n - 1, i] = _v[i, i];
        _v[i, i] = 1;
        float h = _d[i + 1];
        if (h != 0.0)
        {
          for (int k = 0; k <= i; k++)
            _d[k] = _v[k, i + 1] / h;
          for (int j = 0; j <= i; j++)
          {
            float g = 0;
            for (int k = 0; k <= i; k++)
              g += _v[k, i + 1] * _v[k, j];
            for (int k = 0; k <= i; k++)
              _v[k, j] -= g * _d[k];
          }
        }
        for (int k = 0; k <= i; k++)
          _v[k, i + 1] = 0;
      }
      for (int j = 0; j < _n; j++)
      {
        _d[j] = _v[_n - 1, j];
        _v[_n - 1, j] = 0;
      }
      _v[_n - 1, _n - 1] = 1;
      _e[0] = 0;
    }


    // Symmetric tridiagonal QL algorithm.
    private void TridiagonalToQL()
    {
      //  This is derived from the Algol procedures tql2, by
      //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
      //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      for (int i = 1; i < _n; i++)
        _e[i - 1] = _e[i];
      _e[_n - 1] = 0;

      float f = 0;
      float tst1 = 0;
      float eps = (float)Math.Pow(2, -23);
      for (int l = 0; l < _n; l++)
      {

        // Find small subdiagonal element

        tst1 = Math.Max(tst1, Math.Abs(_d[l]) + Math.Abs(_e[l]));
        int m = l;
        while (m < _n)
        {
          if (Math.Abs(_e[m]) <= eps * tst1)
            break;
          m++;
        }

        // If m == l, d[l] is an eigenvalue,
        // otherwise, iterate.

        if (m > l)
        {
          int iter = 0;
          do
          {
            iter = iter + 1;  // (Could check iteration count here.)

            // Compute implicit shift

            float g = _d[l];
            float p = (_d[l + 1] - g) / (2 * _e[l]);
            float r = MathHelper.Hypotenuse(p, 1);
            if (p < 0)
            {
              r = -r;
            }
            _d[l] = _e[l] / (p + r);
            _d[l + 1] = _e[l] * (p + r);
            float dl1 = _d[l + 1];
            float h = g - _d[l];
            for (int i = l + 2; i < _n; i++)
            {
              _d[i] -= h;
            }
            f = f + h;

            // Implicit QL transformation.

            p = _d[m];
            float c = 1;
            float c2 = c;
            float c3 = c;
            float el1 = _e[l + 1];
            float s = 0;
            float s2 = 0;
            for (int i = m - 1; i >= l; i--)
            {
              c3 = c2;
              c2 = c;
              s2 = s;
              g = c * _e[i];
              h = c * p;
              r = MathHelper.Hypotenuse(p, _e[i]);
              _e[i + 1] = s * r;
              s = _e[i] / r;
              c = p / r;
              p = c * _d[i] - s * g;
              _d[i + 1] = h + s * (c * g + s * _d[i]);

              // Accumulate transformation.

              for (int k = 0; k < _n; k++)
              {
                h = _v[k, i + 1];
                _v[k, i + 1] = s * _v[k, i] + c * h;
                _v[k, i] = c * _v[k, i] - s * h;
              }
            }
            p = -s * s2 * c3 * el1 * _e[l] / dl1;
            _e[l] = s * p;
            _d[l] = c * p;

            // Check for convergence.

          } while (Math.Abs(_e[l]) > eps * tst1);
        }
        _d[l] = _d[l] + f;
        _e[l] = 0;
      }

      // Sort eigenvalues and corresponding vectors.

      for (int i = 0; i < _n - 1; i++)
      {
        int k = i;
        float p = _d[i];
        for (int j = i + 1; j < _n; j++)
        {
          if (_d[j] < p)
          {
            k = j;
            p = _d[j];
          }
        }
        if (k != i)
        {
          _d[k] = _d[i];
          _d[i] = p;
          for (int j = 0; j < _n; j++)
          {
            p = _v[j, i];
            _v[j, i] = _v[j, k];
            _v[j, k] = p;
          }
        }
      }
    }


    // Nonsymmetric reduction to Hessenberg form.
    private void ReduceToHessenberg(MatrixF matrixH, float[] ort)
    {
      //  This is derived from the Algol procedures orthes and ortran,
      //  by Martin and Wilkinson, Handbook for Auto. Comp.,
      //  Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutines in EISPACK.

      int low = 0;
      int high = _n - 1;

      for (int m = low + 1; m <= high - 1; m++)
      {
        // Scale column.

        float scale = 0;
        for (int i = m; i <= high; i++)
          scale = scale + Math.Abs(matrixH[i, m - 1]);
        if (scale != 0)
        {
          // Compute Householder transformation.
          float h = 0;
          for (int i = high; i >= m; i--)
          {
            ort[i] = matrixH[i, m - 1] / scale;
            h += ort[i] * ort[i];
          }
          float g = (float)Math.Sqrt(h);
          if (ort[m] > 0)
            g = -g;
          h = h - ort[m] * g;
          ort[m] = ort[m] - g;

          // Apply Householder similarity transformation
          // H = (I-u*u'/h)*H*(I-u*u')/h)

          for (int j = m; j < _n; j++)
          {
            float f = 0;
            for (int i = high; i >= m; i--)
              f += ort[i] * matrixH[i, j];
            f = f / h;
            for (int i = m; i <= high; i++)
              matrixH[i, j] -= f * ort[i];
          }

          for (int i = 0; i <= high; i++)
          {
            float f = 0;
            for (int j = high; j >= m; j--)
              f += ort[j] * matrixH[i, j];
            f = f / h;
            for (int j = m; j <= high; j++)
              matrixH[i, j] -= f * ort[j];
          }
          ort[m] = scale * ort[m];
          matrixH[m, m - 1] = scale * g;
        }
      }

      // Accumulate transformations (Algol's ortran).

      for (int i = 0; i < _n; i++)
        for (int j = 0; j < _n; j++)
          _v[i, j] = (i == j ? 1 : 0);

      for (int m = high - 1; m >= low + 1; m--)
      {
        if (matrixH[m, m - 1] != 0.0)
        {
          for (int i = m + 1; i <= high; i++)
            ort[i] = matrixH[i, m - 1];
          for (int j = m; j <= high; j++)
          {
            float g = 0;
            for (int i = m; i <= high; i++)
              g += ort[i] * _v[i, j];
            // Double division avoids possible underflow
            g = (g / ort[m]) / matrixH[m, m - 1];
            for (int i = m; i <= high; i++)
              _v[i, j] += g * ort[i];
          }
        }
      }
    }


    // Nonsymmetric reduction from Hessenberg to real Schur form.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void HessenbergToRealSchur(MatrixF matrixH)
    {
      //  This is derived from the Algol procedure hqr2,
      //  by Martin and Wilkinson, Handbook for Auto. Comp.,
      //  Vol.ii-Linear Algebra, and the corresponding
      //  Fortran subroutine in EISPACK.

      // Initialize
      int nn = _n;
      int n = nn - 1;
      int low = 0;
      int high = nn - 1;
      float eps = (float)Math.Pow(2, -23);
      float exshift = 0;
      float p = 0, q = 0, r = 0, s = 0, z = 0, w, x, y;

      // Store roots isolated by balance and compute matrix norm

      float norm = 0;
      for (int i = 0; i < nn; i++)
      {
        if (i < low | i > high)
        {
          _d[i] = matrixH[i, i];
          _e[i] = 0;
        }
        for (int j = Math.Max(i - 1, 0); j < nn; j++)
          norm = norm + Math.Abs(matrixH[i, j]);
      }

      // Outer loop over eigenvalue index

      int iter = 0;
      while (n >= low)
      {
        // Look for single small sub-diagonal element

        int l = n;
        while (l > low)
        {
          s = Math.Abs(matrixH[l - 1, l - 1]) + Math.Abs(matrixH[l, l]);
          if (s == 0)
            s = norm;
          if (Math.Abs(matrixH[l, l - 1]) < eps * s)
            break;
          l--;
        }

        // Check for convergence
        // One root found

        if (l == n)
        {
          matrixH[n, n] = matrixH[n, n] + exshift;
          _d[n] = matrixH[n, n];
          _e[n] = 0;
          n--;
          iter = 0;

          // Two roots found

        }
        else if (l == n - 1)
        {
          w = matrixH[n, n - 1] * matrixH[n - 1, n];
          p = (matrixH[n - 1, n - 1] - matrixH[n, n]) / 2;
          q = p * p + w;
          z = (float)Math.Sqrt(Math.Abs(q));
          matrixH[n, n] = matrixH[n, n] + exshift;
          matrixH[n - 1, n - 1] = matrixH[n - 1, n - 1] + exshift;
          x = matrixH[n, n];

          // Real pair

          if (q >= 0)
          {
            if (p >= 0)
            {
              z = p + z;
            }
            else
            {
              z = p - z;
            }
            _d[n - 1] = x + z;
            _d[n] = _d[n - 1];
            if (z != 0.0)
            {
              _d[n] = x - w / z;
            }
            _e[n - 1] = 0;
            _e[n] = 0;
            x = matrixH[n, n - 1];
            s = Math.Abs(x) + Math.Abs(z);
            p = x / s;
            q = z / s;
            r = (float)Math.Sqrt(p * p + q * q);
            p = p / r;
            q = q / r;

            // Row modification

            for (int j = n - 1; j < nn; j++)
            {
              z = matrixH[n - 1, j];
              matrixH[n - 1, j] = q * z + p * matrixH[n, j];
              matrixH[n, j] = q * matrixH[n, j] - p * z;
            }

            // Column modification

            for (int i = 0; i <= n; i++)
            {
              z = matrixH[i, n - 1];
              matrixH[i, n - 1] = q * z + p * matrixH[i, n];
              matrixH[i, n] = q * matrixH[i, n] - p * z;
            }

            // Accumulate transformations

            for (int i = low; i <= high; i++)
            {
              z = _v[i, n - 1];
              _v[i, n - 1] = q * z + p * _v[i, n];
              _v[i, n] = q * _v[i, n] - p * z;
            }

            // Complex pair

          }
          else
          {
            _d[n - 1] = x + p;
            _d[n] = x + p;
            _e[n - 1] = z;
            _e[n] = -z;
          }
          n = n - 2;
          iter = 0;

          // No convergence yet

        }
        else
        {

          // Form shift

          x = matrixH[n, n];
          y = 0;
          w = 0;
          if (l < n)
          {
            y = matrixH[n - 1, n - 1];
            w = matrixH[n, n - 1] * matrixH[n - 1, n];
          }

          // Wilkinson's original ad hoc shift

          if (iter == 10)
          {
            exshift += x;
            for (int i = low; i <= n; i++)
            {
              matrixH[i, i] -= x;
            }
            s = Math.Abs(matrixH[n, n - 1]) + Math.Abs(matrixH[n - 1, n - 2]);
            x = y = 0.75f * s;
            w = -0.4375f * s * s;
          }

          // MATLAB's new ad hoc shift

          if (iter == 30)
          {
            s = (y - x) / 2;
            s = s * s + w;
            if (s > 0)
            {
              s = (float)Math.Sqrt(s);
              if (y < x)
              {
                s = -s;
              }
              s = x - w / ((y - x) / 2 + s);
              for (int i = low; i <= n; i++)
              {
                matrixH[i, i] -= s;
              }
              exshift += s;
              x = y = w = 0.964f;
            }
          }

          iter = iter + 1;   // (Could check iteration count here.)

          // Look for two consecutive small sub-diagonal elements

          int m = n - 2;
          while (m >= l)
          {
            z = matrixH[m, m];
            r = x - z;
            s = y - z;
            p = (r * s - w) / matrixH[m + 1, m] + matrixH[m, m + 1];
            q = matrixH[m + 1, m + 1] - z - r - s;
            r = matrixH[m + 2, m + 1];
            s = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
            p = p / s;
            q = q / s;
            r = r / s;
            if (m == l)
            {
              break;
            }
            if (Math.Abs(matrixH[m, m - 1]) * (Math.Abs(q) + Math.Abs(r)) <
               eps * (Math.Abs(p) * (Math.Abs(matrixH[m - 1, m - 1]) + Math.Abs(z) +
               Math.Abs(matrixH[m + 1, m + 1]))))
            {
              break;
            }
            m--;
          }

          for (int i = m + 2; i <= n; i++)
          {
            matrixH[i, i - 2] = 0;
            if (i > m + 2)
            {
              matrixH[i, i - 3] = 0;
            }
          }

          // Double QR step involving rows l:n and columns m:n

          for (int k = m; k <= n - 1; k++)
          {
            bool notlast = (k != n - 1);
            if (k != m)
            {
              p = matrixH[k, k - 1];
              q = matrixH[k + 1, k - 1];
              r = (notlast ? matrixH[k + 2, k - 1] : 0);
              x = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
              if (x != 0.0)
              {
                p = p / x;
                q = q / x;
                r = r / x;
              }
            }
            if (x == 0.0)
            {
              break;
            }
            s = (float)Math.Sqrt(p * p + q * q + r * r);
            if (p < 0)
            {
              s = -s;
            }
            if (s != 0)
            {
              if (k != m)
              {
                matrixH[k, k - 1] = -s * x;
              }
              else if (l != m)
              {
                matrixH[k, k - 1] = -matrixH[k, k - 1];
              }
              p = p + s;
              x = p / s;
              y = q / s;
              z = r / s;
              q = q / p;
              r = r / p;

              // Row modification

              for (int j = k; j < nn; j++)
              {
                p = matrixH[k, j] + q * matrixH[k + 1, j];
                if (notlast)
                {
                  p = p + r * matrixH[k + 2, j];
                  matrixH[k + 2, j] = matrixH[k + 2, j] - p * z;
                }
                matrixH[k, j] = matrixH[k, j] - p * x;
                matrixH[k + 1, j] = matrixH[k + 1, j] - p * y;
              }

              // Column modification

              for (int i = 0; i <= Math.Min(n, k + 3); i++)
              {
                p = x * matrixH[i, k] + y * matrixH[i, k + 1];
                if (notlast)
                {
                  p = p + z * matrixH[i, k + 2];
                  matrixH[i, k + 2] = matrixH[i, k + 2] - p * r;
                }
                matrixH[i, k] = matrixH[i, k] - p;
                matrixH[i, k + 1] = matrixH[i, k + 1] - p * q;
              }

              // Accumulate transformations

              for (int i = low; i <= high; i++)
              {
                p = x * _v[i, k] + y * _v[i, k + 1];
                if (notlast)
                {
                  p = p + z * _v[i, k + 2];
                  _v[i, k + 2] = _v[i, k + 2] - p * r;
                }
                _v[i, k] = _v[i, k] - p;
                _v[i, k + 1] = _v[i, k + 1] - p * q;
              }
            }  // (s != 0)
          }  // k loop
        }  // check convergence
      }  // while (n >= low)

      // Backsubstitute to find vectors of upper triangular form

      if (norm == 0.0)
        return;

      for (n = nn - 1; n >= 0; n--)
      {
        p = _d[n];
        q = _e[n];

        // Real vector

        if (q == 0)
        {
          int l = n;
          matrixH[n, n] = 1;
          for (int i = n - 1; i >= 0; i--)
          {
            w = matrixH[i, i] - p;
            r = 0;
            for (int j = l; j <= n; j++)
            {
              r = r + matrixH[i, j] * matrixH[j, n];
            }
            if (_e[i] < 0.0)
            {
              z = w;
              s = r;
            }
            else
            {
              float t;

              l = i;
              if (_e[i] == 0)
              {
                if (w != 0)
                  matrixH[i, n] = -r / w;
                else
                  matrixH[i, n] = -r / (eps * norm);

                // Solve real equations
              }
              else
              {
                x = matrixH[i, i + 1];
                y = matrixH[i + 1, i];
                q = (_d[i] - p) * (_d[i] - p) + _e[i] * _e[i];
                t = (x * s - z * r) / q;
                matrixH[i, n] = t;
                if (Math.Abs(x) > Math.Abs(z))
                  matrixH[i + 1, n] = (-r - w * t) / x;
                else
                  matrixH[i + 1, n] = (-s - y * t) / z;
              }

              // Overflow control

              t = Math.Abs(matrixH[i, n]);
              if ((eps * t) * t > 1)
                for (int j = i; j <= n; j++)
                  matrixH[j, n] = matrixH[j, n] / t;
            }
          }

          // Complex vector

        }
        else if (q < 0)
        {
          int l = n - 1;

          // Last vector component imaginary so matrix is triangular

          if (Math.Abs(matrixH[n, n - 1]) > Math.Abs(matrixH[n - 1, n]))
          {
            matrixH[n - 1, n - 1] = q / matrixH[n, n - 1];
            matrixH[n - 1, n] = -(matrixH[n, n] - p) / matrixH[n, n - 1];
          }
          else
          {
            float cdivr, cdivi;
            cdiv(0, -matrixH[n - 1, n], matrixH[n - 1, n - 1] - p, q, out cdivr, out cdivi);
            matrixH[n - 1, n - 1] = cdivr;
            matrixH[n - 1, n] = cdivi;
          }
          matrixH[n, n - 1] = 0;
          matrixH[n, n] = 1;
          for (int i = n - 2; i >= 0; i--)
          {
            float ra = 0;
            float sa = 0;
            for (int j = l; j <= n; j++)
            {
              ra = ra + matrixH[i, j] * matrixH[j, n - 1];
              sa = sa + matrixH[i, j] * matrixH[j, n];
            }
            w = matrixH[i, i] - p;

            if (_e[i] < 0)
            {
              z = w;
              r = ra;
              s = sa;
            }
            else
            {
              l = i;
              if (_e[i] == 0)
              {
                float cdivr, cdivi;
                cdiv(-ra, -sa, w, q, out cdivr, out cdivi);
                matrixH[i, n - 1] = cdivr;
                matrixH[i, n] = cdivi;
              }
              else
              {
                // Solve complex equations

                x = matrixH[i, i + 1];
                y = matrixH[i + 1, i];
                float vr = (_d[i] - p) * (_d[i] - p) + _e[i] * _e[i] - q * q;
                float vi = (_d[i] - p) * 2 * q;
                if (vr == 0 & vi == 0)
                {
                  vr = eps * norm * (Math.Abs(w) + Math.Abs(q) +
                  Math.Abs(x) + Math.Abs(y) + Math.Abs(z));
                }
                float cdivr, cdivi;
                cdiv(x * r - z * ra + q * sa, x * s - z * sa - q * ra, vr, vi, out cdivr, out cdivi);
                matrixH[i, n - 1] = cdivr;
                matrixH[i, n] = cdivi;
                if (Math.Abs(x) > (Math.Abs(z) + Math.Abs(q)))
                {
                  matrixH[i + 1, n - 1] = (-ra - w * matrixH[i, n - 1] + q * matrixH[i, n]) / x;
                  matrixH[i + 1, n] = (-sa - w * matrixH[i, n] - q * matrixH[i, n - 1]) / x;
                }
                else
                {
                  cdiv(-r - y * matrixH[i, n - 1], -s - y * matrixH[i, n], z, q, out cdivr, out cdivi);
                  matrixH[i + 1, n - 1] = cdivr;
                  matrixH[i + 1, n] = cdivi;
                }
              }

              // Overflow control

              float t = Math.Max(Math.Abs(matrixH[i, n - 1]), Math.Abs(matrixH[i, n]));
              if ((eps * t) * t > 1)
              {
                for (int j = i; j <= n; j++)
                {
                  matrixH[j, n - 1] = matrixH[j, n - 1] / t;
                  matrixH[j, n] = matrixH[j, n] / t;
                }
              }
            }
          }
        }
      }

      // Vectors of isolated roots

      for (int i = 0; i < nn; i++)
        if (i < low | i > high)
          for (int j = i; j < nn; j++)
            _v[i, j] = matrixH[i, j];

      // Back transformation to get eigenvectors of original matrix

      for (int j = nn - 1; j >= low; j--)
      {
        for (int i = low; i <= high; i++)
        {
          z = 0;
          for (int k = low; k <= Math.Min(j, high); k++)
            z = z + _v[i, k] * matrixH[k, j];
          _v[i, j] = z;
        }
      }
    }


    // Complex scalar division.
    private static void cdiv(float xr, float xi, float yr, float yi, out float cdivr, out float cdivi)
    {
      float r, d;
      if (Math.Abs(yr) > Math.Abs(yi))
      {
        r = yi / yr;
        d = yr + r * yi;
        cdivr = (xr + r * xi) / d;
        cdivi = (xi - r * xr) / d;
      }
      else
      {
        r = yr / yi;
        d = yi + r * yr;
        cdivr = (r * xr + xi) / d;
        cdivi = (r * xi - xr) / d;
      }
    }

  }
}
