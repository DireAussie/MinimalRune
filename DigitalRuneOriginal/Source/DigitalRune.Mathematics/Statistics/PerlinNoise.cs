﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.
//
// Part of this code is based on 
//   Public Domain code by Stefan Gustavson, 2003-2005 (stegu@itn.liu.se).
//   https://github.com/kev009/craftd/blob/master/plugins/survival/mapgen/noise/noise1234.c


using System;
using System.Diagnostics;


namespace MinimalRune.Mathematics.Statistics
{
  /// <summary>
  /// Computes <i>Improved Perlin Noise</i>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class implements "improved noise" by Ken Perlin for 1D, 2D, 3D and 4D. 
  /// </para>
  /// <para>
  /// General noise is simply a collection of random values, like the random dot pattern on old TVs
  /// when there is no input signal. General noise is not smooth. Band-limited noise is a noise
  /// which is smooth. This can be generated by zooming into the noise pattern and applying a
  /// Gaussian blur. A more efficient version of band-limited noise is Perlin Noise.
  /// </para>
  /// <para>
  /// Perlin Noise is a Gradient Noise: It use a grid of gradients. The noise is 0 at the grid
  /// points, i.e. for 3D <c>Compute((int)x, (int)y, (int)z) == 0</c>.
  /// </para>
  /// <para>
  /// Between grid points the gradients are interpolated to created smooth noise. There is at most
  /// one oscillation between two grid points.
  /// </para>
  /// <para>
  /// Perlin noise is consistent: For a constant position this method always returns the same noise
  /// value.
  /// </para>
  /// <para>
  /// The resulting noise is scaled to keep values the range [-1, 1]. (This is only approximated
  /// using scaling factors determined by experimentation. Some returned noise values might still be 
  /// outside [-1, 1].)
  /// </para>
  /// <para>
  /// Random values are generated using a lookup table. Since the table is limited, the noise
  /// function repeats itself in each dimension after 256 values, for example:<br/>
  /// <c>Compute(x, y, z) == Compute(x + 256, y + 256, z + 256)</c>.
  /// </para>
  /// <para>
  /// This class can also compute periodic/tileable noise for any integer period, such that:<br/>
  /// <c>Compute(x, y, z, periodX, periodY, periodZ) == Compute(x + periodX, y + periodY, z + periodZ, periodX, periodY, periodZ)</c>
  /// </para>
  /// </remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
  public static class PerlinNoise
  {
    // Notes: 
    // See also 
    // - The reference implementation can be found at http://mrl.nyu.edu/~perlin/noise/ 
    // - "Implementing Improved Perlin Noise" in "GPU Gems",
    // - http://www.noisemachine.com/talk1/index.html.
    // - Public Domain code by Stefan Gustavson, 2003-2005 (stegu@itn.liu.se).
    //   https://github.com/kev009/craftd/blob/master/plugins/survival/mapgen/noise/noise1234.c
    //   Gustavson uses scale factors to make the result similar to classic Perlin Noise.
    //   We use different scale factors determined by experiments to scale the noise to [-1, 1].


    

    

    /// <summary>
    /// Permutation table for <i>improved noise</i> by Ken Perlin.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly")]
    public static readonly byte[] Permutation;



    

    

    /// <summary>
    /// Initializes static members of the <see cref="PerlinNoise"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static PerlinNoise()
    {
      var values = new byte[]
      { 
        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 
        142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 
        203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 
        175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 
        230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 
        209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 
        198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 
        212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 
        2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 
        110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 
        144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 
        199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 
        222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
      };

      Debug.Assert(values.Length == 256, "256 permutation values expected.");

      // Fill array with two times the permutation values.
      Permutation = new byte[512];
      Array.Copy(values, 0, Permutation, 0, 256);
      Array.Copy(values, 0, Permutation, 256, 256);
    }



    

    



    private static double Lerp(double source, double target, double parameter)
    {
      return source + (target - source) * parameter;
    }


    // This function describes a fade curve (a 5th order interpolation curve that goes smoothly
    // from (0, 0) to (1, 1)), similar to HermiteSmoothStep. 
    private static double Fade(double t)
    {
      return t * t * t * (t * (t * 6 - 15) + 10);
    }


    // Functions which return gradient directions for 1D, 2D, 3D and 4D noise.
    private static double Grad1(int hash, double x)
    {
      int h = hash & 15;
      double grad = 1.0 + (h & 7);      // Gradient value 1.0, 2.0, ..., 8.0
      if ((h & 8) != 0) grad = -grad;   // and a random sign for the gradient
      return (grad * x);                // Multiply the gradient with the distance
    }


    private static double Grad2(int hash, double x, double y)
    {
      int h = hash & 7;            // Convert low 3 bits of hash code
      double u = (h < 4) ? x : y;  // into 8 simple gradient directions,
      double v = (h < 4) ? y : x;  // and compute the dot product with (x,y).
      return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0 * v : 2.0 * v);
    }


    private static double Grad3(int hash, double x, double y, double z)
    {
      // Convert low 4 bits of hash code into 12 gradient directions.
      int h = hash & 15;
      double u = (h < 8) ? x : y;
      double v = (h < 4) ? y : (h == 12 || h == 14) ? x : z;
      return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }


    private static double Grad4(int hash, double x, double y, double z, double t)
    {
      int h = hash & 31;      // Convert low 5 bits of hash code into 32 simple
      double u = (h < 24) ? x : y; // gradient directions, and compute dot product.
      double v = (h < 16) ? y : z;
      double w = (h < 8) ? z : t;
      return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v) + ((h & 4) != 0 ? -w : w);
    }



    /// <overloads>
    /// <summary>
    /// Computes a noise value using <i>Improved Perlin Noise</i>.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Computes a noise value for a 1D position given by (x).
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <returns>The noise value for the given position (x).</returns>
    /// <remarks>
    /// See class description of <see cref="PerlinNoise" /> for more details.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x)
    {
      int ix0 = (int)Math.Floor(x);
      double fx0 = x - ix0;
      double fx1 = fx0 - 1.0f;
      int ix1 = (ix0 + 1) & 0xff;
      ix0 = ix0 & 0xff;    // Wrap to 0..255

      double s = Fade(fx0);

      var perm = Permutation;
      double n0 = Grad1(perm[ix0], fx0);
      double n1 = Grad1(perm[ix1], fx1);
      return 0.25 * (Lerp(n0, n1, s));
    }


    /// <summary>
    /// Computes a noise value for a 1D position given by (x).
    /// The noise is tileable with the given periods.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="periodX">The period in x. Must be greater than 0.</param>
    /// <returns>The noise value for the given position (x).</returns>
    /// <remarks>
    /// See class description of <see cref="PerlinNoise" /> for more details.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x, int periodX)
    {
      // Make position positive using "positive modulo".
      if (x < 0)
        x = (x % periodX + periodX) % periodX;

      int ix0 = (int)Math.Floor(x);
      double fx0 = x - ix0;
      double fx1 = fx0 - 1.0f;
      int ix1 = ((ix0 + 1) % periodX) & 0xff;
      ix0 = (ix0 % periodX) & 0xff;      // (because periodX might be greater than 256)

      double s = Fade(fx0);

      var perm = Permutation;
      double n0 = Grad1(perm[ix0], fx0);
      double n1 = Grad1(perm[ix1], fx1);
      return 0.25 * (Lerp(n0, n1, s));
    }


    /// <summary>
    /// Computes a noise value for a 2D position given by (x, y).
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <returns>The noise value for the given position (x, y).</returns>
    /// <remarks>
    /// See class description of <see cref="PerlinNoise" /> for more details.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x, double y)
    {
      int ix0 = (int)Math.Floor(x);
      int iy0 = (int)Math.Floor(y);
      double fx0 = x - ix0;
      double fy0 = y - iy0;
      double fx1 = fx0 - 1.0f;
      double fy1 = fy0 - 1.0f;
      int ix1 = (ix0 + 1) & 0xff;
      int iy1 = (iy0 + 1) & 0xff;
      ix0 = ix0 & 0xff;
      iy0 = iy0 & 0xff;

      double t = Fade(fy0);
      double s = Fade(fx0);

      var perm = Permutation;
      double nx0 = Grad2(perm[ix0 + perm[iy0]], fx0, fy0);
      double nx1 = Grad2(perm[ix0 + perm[iy1]], fx0, fy1);
      double n0 = Lerp(nx0, nx1, t);

      nx0 = Grad2(perm[ix1 + perm[iy0]], fx1, fy0);
      nx1 = Grad2(perm[ix1 + perm[iy1]], fx1, fy1);
      double n1 = Lerp(nx0, nx1, t);

      return 0.661 * (Lerp(n0, n1, s));
    }


    /// <summary>
    /// Computes a noise value for a 2D position given by (x, y).
    /// The noise is tileable with the given periods.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="periodX">The period in x. Must be greater than 0.</param>
    /// <param name="periodY">The period in y. Must be greater than 0.</param>
    /// <returns>The noise value for the given position (x, y).</returns>
    /// <remarks>
    /// See class description of <see cref="PerlinNoise" /> for more details.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x, double y, int periodX, int periodY)
    {
      // Make position positive using "positive modulo".
      if (x < 0)
        x = (x % periodX + periodX) % periodX;
      if (y < 0)
        y = (y % periodY + periodY) % periodY;

      int ix0 = (int)Math.Floor(x);
      int iy0 = (int)Math.Floor(y);
      double fx0 = x - ix0;
      double fy0 = y - iy0;
      double fx1 = fx0 - 1.0f;
      double fy1 = fy0 - 1.0f;
      int ix1 = ((ix0 + 1) % periodX) & 0xff;
      int iy1 = ((iy0 + 1) % periodY) & 0xff;
      ix0 = (ix0 % periodX) & 0xff;
      iy0 = (iy0 % periodY) & 0xff;

      double t = Fade(fy0);
      double s = Fade(fx0);

      var perm = Permutation;
      double nx0 = Grad2(perm[ix0 + perm[iy0]], fx0, fy0);
      double nx1 = Grad2(perm[ix0 + perm[iy1]], fx0, fy1);
      double n0 = Lerp(nx0, nx1, t);

      nx0 = Grad2(perm[ix1 + perm[iy0]], fx1, fy0);
      nx1 = Grad2(perm[ix1 + perm[iy1]], fx1, fy1);
      double n1 = Lerp(nx0, nx1, t);

      return 0.661 * (Lerp(n0, n1, s));
    }


    
    /// <summary>
    /// Computes a noise value for a 3D position given by (x, y, z).
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <returns>The noise value for the given position (x, y, z).</returns>
    /// <remarks>
    /// See <see cref="PerlinNoise"/> for details about Perlin Noise.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x, double y, double z)
    {
      int ix0 = (int)Math.Floor(x);
      int iy0 = (int)Math.Floor(y);
      int iz0 = (int)Math.Floor(z);
      double fx0 = x - ix0;
      double fy0 = y - iy0;
      double fz0 = z - iz0;
      double fx1 = fx0 - 1.0f;
      double fy1 = fy0 - 1.0f;
      double fz1 = fz0 - 1.0f;
      int ix1 = (ix0 + 1) & 0xff;
      int iy1 = (iy0 + 1) & 0xff;
      int iz1 = (iz0 + 1) & 0xff;
      ix0 = ix0 & 0xff;
      iy0 = iy0 & 0xff;
      iz0 = iz0 & 0xff;

      double r = Fade(fz0);
      double t = Fade(fy0);
      double s = Fade(fx0);

      var perm = Permutation;
      double nxy0 = Grad3(perm[ix0 + perm[iy0 + perm[iz0]]], fx0, fy0, fz0);
      double nxy1 = Grad3(perm[ix0 + perm[iy0 + perm[iz1]]], fx0, fy0, fz1);
      double nx0 = Lerp(nxy0, nxy1, r);

      nxy0 = Grad3(perm[ix0 + perm[iy1 + perm[iz0]]], fx0, fy1, fz0);
      nxy1 = Grad3(perm[ix0 + perm[iy1 + perm[iz1]]], fx0, fy1, fz1);
      double nx1 = Lerp(nxy0, nxy1, r);

      double n0 = Lerp(nx0, nx1, t);

      nxy0 = Grad3(perm[ix1 + perm[iy0 + perm[iz0]]], fx1, fy0, fz0);
      nxy1 = Grad3(perm[ix1 + perm[iy0 + perm[iz1]]], fx1, fy0, fz1);
      nx0 = Lerp(nxy0, nxy1, r);

      nxy0 = Grad3(perm[ix1 + perm[iy1 + perm[iz0]]], fx1, fy1, fz0);
      nxy1 = Grad3(perm[ix1 + perm[iy1 + perm[iz1]]], fx1, fy1, fz1);
      nx1 = Lerp(nxy0, nxy1, r);

      double n1 = Lerp(nx0, nx1, t);

      return Lerp(n0, n1, s);
    }


    /// <summary>
    /// Computes a noise value for a 3D position given by (x, y, z).
    /// The noise is tileable with the given periods.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <param name="periodX">The period in x. Must be greater than 0.</param>
    /// <param name="periodY">The period in y. Must be greater than 0.</param>
    /// <param name="periodZ">The period in z. Must be greater than 0.</param>
    /// <returns>The noise value for the given position (x, y, z).</returns>
    /// <remarks>
    /// See class description of <see cref="PerlinNoise" /> for more details.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x, double y, double z, int periodX, int periodY, int periodZ)
    {
      // Make position positive using "positive modulo".
      if (x < 0)
        x = (x % periodX + periodX) % periodX;
      if (y < 0)
        y = (y % periodY + periodY) % periodY;
      if (z < 0)
        z = (z % periodZ + periodZ) % periodZ;

      int ix0 = (int)Math.Floor(x);
      int iy0 = (int)Math.Floor(y);
      int iz0 = (int)Math.Floor(z);
      double fx0 = x - ix0;
      double fy0 = y - iy0;
      double fz0 = z - iz0;
      double fx1 = fx0 - 1.0f;
      double fy1 = fy0 - 1.0f;
      double fz1 = fz0 - 1.0f;
      int ix1 = ((ix0 + 1) % periodX) & 0xff;
      int iy1 = ((iy0 + 1) % periodY) & 0xff;
      int iz1 = ((iz0 + 1) % periodZ) & 0xff;
      ix0 = (ix0 % periodX) & 0xff;
      iy0 = (iy0 % periodY) & 0xff;
      iz0 = (iz0 % periodZ) & 0xff;

      double r = Fade(fz0);
      double t = Fade(fy0);
      double s = Fade(fx0);

      var perm = Permutation;
      double nxy0 = Grad3(perm[ix0 + perm[iy0 + perm[iz0]]], fx0, fy0, fz0);
      double nxy1 = Grad3(perm[ix0 + perm[iy0 + perm[iz1]]], fx0, fy0, fz1);
      double nx0 = Lerp(nxy0, nxy1, r);

      nxy0 = Grad3(perm[ix0 + perm[iy1 + perm[iz0]]], fx0, fy1, fz0);
      nxy1 = Grad3(perm[ix0 + perm[iy1 + perm[iz1]]], fx0, fy1, fz1);
      double nx1 = Lerp(nxy0, nxy1, r);

      double n0 = Lerp(nx0, nx1, t);

      nxy0 = Grad3(perm[ix1 + perm[iy0 + perm[iz0]]], fx1, fy0, fz0);
      nxy1 = Grad3(perm[ix1 + perm[iy0 + perm[iz1]]], fx1, fy0, fz1);
      nx0 = Lerp(nxy0, nxy1, r);

      nxy0 = Grad3(perm[ix1 + perm[iy1 + perm[iz0]]], fx1, fy1, fz0);
      nxy1 = Grad3(perm[ix1 + perm[iy1 + perm[iz1]]], fx1, fy1, fz1);
      nx1 = Lerp(nxy0, nxy1, r);

      double n1 = Lerp(nx0, nx1, t);

      return Lerp(n0, n1, s);
    }


    /// <summary>
    /// Computes a noise value for a 4D position given by (x, y, z, w).
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <param name="w">The w position.</param>
    /// <returns>The noise value for the given position (x, y, z, w).</returns>
    /// <remarks>
    /// See class description of <see cref="PerlinNoise" /> for more details.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x, double y, double z, double w)
    {
      int ix0 = (int)Math.Floor(x);
      int iy0 = (int)Math.Floor(y);
      int iz0 = (int)Math.Floor(z);
      int iw0 = (int)Math.Floor(w);
      double fx0 = x - ix0;
      double fy0 = y - iy0;
      double fz0 = z - iz0;
      double fw0 = w - iw0;
      double fx1 = fx0 - 1.0f;
      double fy1 = fy0 - 1.0f;
      double fz1 = fz0 - 1.0f;
      double fw1 = fw0 - 1.0f;
      int ix1 = (ix0 + 1) & 0xff;
      int iy1 = (iy0 + 1) & 0xff;
      int iz1 = (iz0 + 1) & 0xff;
      int iw1 = (iw0 + 1) & 0xff;
      ix0 = ix0 & 0xff;
      iy0 = iy0 & 0xff;
      iz0 = iz0 & 0xff;
      iw0 = iw0 & 0xff;

      double q = Fade(fw0);
      double r = Fade(fz0);
      double t = Fade(fy0);
      double s = Fade(fx0);

      var perm = Permutation;
      double nxyz0 = Grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx0, fy0, fz0, fw0);
      double nxyz1 = Grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx0, fy0, fz0, fw1);
      double nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx0, fy0, fz1, fw0);
      nxyz1 = Grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx0, fy0, fz1, fw1);
      double nxy1 = Lerp(nxyz0, nxyz1, q);

      double nx0 = Lerp(nxy0, nxy1, r);

      nxyz0 = Grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx0, fy1, fz0, fw0);
      nxyz1 = Grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx0, fy1, fz0, fw1);
      nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx0, fy1, fz1, fw0);
      nxyz1 = Grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx0, fy1, fz1, fw1);
      nxy1 = Lerp(nxyz0, nxyz1, q);

      double nx1 = Lerp(nxy0, nxy1, r);

      double n0 = Lerp(nx0, nx1, t);

      nxyz0 = Grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx1, fy0, fz0, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx1, fy0, fz0, fw1);
      nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx1, fy0, fz1, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx1, fy0, fz1, fw1);
      nxy1 = Lerp(nxyz0, nxyz1, q);

      nx0 = Lerp(nxy0, nxy1, r);

      nxyz0 = Grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx1, fy1, fz0, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx1, fy1, fz0, fw1);
      nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx1, fy1, fz1, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx1, fy1, fz1, fw1);
      nxy1 = Lerp(nxyz0, nxyz1, q);

      nx1 = Lerp(nxy0, nxy1, r);

      double n1 = Lerp(nx0, nx1, t);

      return 0.845 * Lerp(n0, n1, s);
    }


    /// <summary>
    /// Computes a noise value for a 4D position given by (x, y, z, w).
    /// The noise is tileable with the given periods.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <param name="w">The w position.</param>
    /// <param name="periodX">The period in x. Must be greater than 0.</param>
    /// <param name="periodY">The period in y. Must be greater than 0.</param>
    /// <param name="periodZ">The period in z. Must be greater than 0.</param>
    /// <param name="periodW">The period in w. Must be greater than 0.</param>
    /// <returns>The noise value for the given position (x, y, z, w).</returns>
    /// <remarks>
    /// See class description of <see cref="PerlinNoise" /> for more details.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static double Compute(double x, double y, double z, double w,
                                 int periodX, int periodY, int periodZ, int periodW)
    {
      // Make position positive using "positive modulo".
      if (x < 0)
        x = (x % periodX + periodX) % periodX;
      if (y < 0)
        y = (y % periodY + periodY) % periodY;
      if (z < 0)
        z = (z % periodZ + periodZ) % periodZ;
      if (w < 0)
        w = (w % periodW + periodW) % periodW;

      int ix0 = (int)Math.Floor(x);
      int iy0 = (int)Math.Floor(y);
      int iz0 = (int)Math.Floor(z);
      int iw0 = (int)Math.Floor(w);
      double fx0 = x - ix0;
      double fy0 = y - iy0;
      double fz0 = z - iz0;
      double fw0 = w - iw0;
      double fx1 = fx0 - 1.0f;
      double fy1 = fy0 - 1.0f;
      double fz1 = fz0 - 1.0f;
      double fw1 = fw0 - 1.0f;
      int ix1 = ((ix0 + 1) % periodX) & 0xff;
      int iy1 = ((iy0 + 1) % periodY) & 0xff;
      int iz1 = ((iz0 + 1) % periodZ) & 0xff;
      int iw1 = ((iw0 + 1) % periodW) & 0xff;
      ix0 = (ix0 % periodX) & 0xff;
      iy0 = (iy0 % periodY) & 0xff;
      iz0 = (iz0 % periodZ) & 0xff;
      iw0 = (iw0 % periodW) & 0xff;

      double q = Fade(fw0);
      double r = Fade(fz0);
      double t = Fade(fy0);
      double s = Fade(fx0);

      var perm = Permutation;
      double nxyz0 = Grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx0, fy0, fz0, fw0);
      double nxyz1 = Grad4(perm[ix0 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx0, fy0, fz0, fw1);
      double nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx0, fy0, fz1, fw0);
      nxyz1 = Grad4(perm[ix0 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx0, fy0, fz1, fw1);
      double nxy1 = Lerp(nxyz0, nxyz1, q);

      double nx0 = Lerp(nxy0, nxy1, r);

      nxyz0 = Grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx0, fy1, fz0, fw0);
      nxyz1 = Grad4(perm[ix0 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx0, fy1, fz0, fw1);
      nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx0, fy1, fz1, fw0);
      nxyz1 = Grad4(perm[ix0 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx0, fy1, fz1, fw1);
      nxy1 = Lerp(nxyz0, nxyz1, q);

      double nx1 = Lerp(nxy0, nxy1, r);

      double n0 = Lerp(nx0, nx1, t);

      nxyz0 = Grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw0]]]], fx1, fy0, fz0, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy0 + perm[iz0 + perm[iw1]]]], fx1, fy0, fz0, fw1);
      nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw0]]]], fx1, fy0, fz1, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy0 + perm[iz1 + perm[iw1]]]], fx1, fy0, fz1, fw1);
      nxy1 = Lerp(nxyz0, nxyz1, q);

      nx0 = Lerp(nxy0, nxy1, r);

      nxyz0 = Grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw0]]]], fx1, fy1, fz0, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy1 + perm[iz0 + perm[iw1]]]], fx1, fy1, fz0, fw1);
      nxy0 = Lerp(nxyz0, nxyz1, q);

      nxyz0 = Grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw0]]]], fx1, fy1, fz1, fw0);
      nxyz1 = Grad4(perm[ix1 + perm[iy1 + perm[iz1 + perm[iw1]]]], fx1, fy1, fz1, fw1);
      nxy1 = Lerp(nxyz0, nxyz1, q);

      nx1 = Lerp(nxy0, nxy1, r);

      double n1 = Lerp(nx0, nx1, t);

      return 0.845 * Lerp(n0, n1, s);
    }


    /// <overloads>
    /// <summary>
    /// Computes a noise value. <i>Obsolete</i>
    /// </summary>
    /// </overloads>
    /// <summary>
    /// Computes a 3d noise value for a 3d position given by (x, y, z).
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <returns>The 3d noise value for the given position (x, y, z).</returns>
    /// <remarks>
    /// <para>
    /// <i>This method is obsolete. Use <see cref="Compute(double, double, double)"/> instead.</i>
    /// </para>
    /// <para>
    /// Perlin noise is consistent: For a constant position this method always returns the same
    /// noise value.
    /// </para>
    /// <para>
    /// The noise should be in the range [-1, 1]. The noise value at integer positions is 0, for
    /// example Noise(11, 12, 13) = 0.
    /// </para>
    /// <para>
    /// There is at max one oscillation (1 whole sine period) between two consecutive integer value.
    /// </para>
    /// <para>
    /// Random values are generated per table lookup. Since the table is limited, the noise function
    /// repeats itself after 256 values: <c>Noise(i, 0, 0) == Noise(i + 256, 0, 0)</c>
    /// </para>
    /// </remarks>
    [Obsolete("This method is obsolete. Use PerlinNoise.Compute instead.")]
    public static double Noise(double x, double y, double z)
    {
      // ReSharper disable InconsistentNaming

      // Find unit cube that contains point.
      int X = (int)Math.Floor(x) & 255;
      int Y = (int)Math.Floor(y) & 255;
      int Z = (int)Math.Floor(z) & 255;

      // Find relative x, y, z of point in cube.
      x -= Math.Floor(x);
      y -= Math.Floor(y);
      z -= Math.Floor(z);

      // Compute fade curves for each of x, y, z.
      double u = Fade(x);
      double v = Fade(y);
      double w = Fade(z);

      // Hash coordinates of the 8 cube corners,
      var p = Permutation;
      int A = p[X] + Y;
      int AA = p[A] + Z;
      int AB = p[A + 1] + Z;
      int B = p[X + 1] + Y;
      int BA = p[B] + Z;
      int BB = p[B + 1] + Z;

      // and add blended results from 8 corners of the cube.
      double lerp0 = Lerp(Grad3(p[AA], x, y, z), Grad3(p[BA], x - 1, y, z), u);
      double lerp1 = Lerp(Grad3(p[AB], x, y - 1, z), Grad3(p[BB], x - 1, y - 1, z), u);
      double lerp2 = Lerp(Grad3(p[AA + 1], x, y, z - 1), Grad3(p[BA + 1], x - 1, y, z - 1), u);
      double lerp3 = Lerp(Grad3(p[AB + 1], x, y - 1, z - 1), Grad3(p[BB + 1], x - 1, y - 1, z - 1), u);
      double lerp01 = Lerp(lerp0, lerp1, v);
      double lerp23 = Lerp(lerp2, lerp3, v);
      return Lerp(lerp01, lerp23, w);
      // ReSharper restore InconsistentNaming
    }


    /// <summary>
    /// Computes a 3d noise value for a 3d position given by (x, y, z).
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <param name="numberOfOctaves">The number of octaves.</param>
    /// <returns>
    /// The 3d noise value for the given position (x, y, z).
    /// </returns>
    /// <remarks>
    /// <para>
    /// <i>This method is obsolete. It is recommended to manually combine noise generated by the
    /// <strong>PerlinNoise.Compute</strong> methods as required. For example:
    /// </i>.
    /// </para>
    /// <code lang="csharp">
    /// <![CDATA[
    /// double noise = 0;
    /// for (int i = 0; i < numberOfOctaves; i++)
    /// {
    ///   double k = 1 << i;
    ///   noise += 1.0 / k * PerlinNoise.Compute(k * x, k * y, k * z);
    /// }
    /// ]]>
    /// </code>
    /// <para>
    /// A noise value is computed as in <see cref="Noise(double, double, double)"/> except that 
    /// several noise functions with different frequencies (octaves) are added, for example for 
    /// 3 octaves: <c>noiseValue = Noise(x,y,z) + 1/2*Noise(2x, 2y, 2z) + 1/4*Noise(4x,4y,4z)</c>
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="numberOfOctaves"/> is negative or 0.
    /// </exception>
    [Obsolete("This method is obsolete. See remarks.")]
    public static double Noise(double x, double y, double z, int numberOfOctaves)
    {
      if (numberOfOctaves <= 0)
        throw new ArgumentOutOfRangeException("numberOfOctaves", "The number of octaves must be greater than 0.");

      double noiseValue = 0;

      for (int i = 0; i < numberOfOctaves; i++)
      {
        double k = 1 << i;
        noiseValue += 1.0 / k * Noise(k * x, k * y, k * z);
      }

      return noiseValue;
    }

  }
}
