﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Diagnostics;


namespace MinimalRune.Mathematics.Analysis
{
  /// <summary>
  /// A base class for methods which solve a single equation (double-precision). 
  /// </summary>
  /// <remarks>
  /// An <i>x</i> is searched such that <i>f(x) = 0</i> or <i>f(x) = y</i> (the function <i>f</i>
  /// and the <i>y</i> must be given).
  /// </remarks>
  public abstract class RootFinderD
  {
    

    

    private int _maxNumberOfIterations = 100;



    

    

    /// <summary>
    /// Gets the function f(x), which root we want to find.
    /// </summary>
    /// <value>The function f(x), which root we want to find.</value>
    public Func<double, double> Function { get; private set; }


    /// <summary>
    /// Gets or sets the number of iterations of the last 
    /// <see cref="FindRoot(double,double)"/> method call.
    /// </summary>
    /// <value>The number of iterations.</value>
    /// <remarks>
    /// This property is not thread-safe.
    /// </remarks>
    public int NumberOfIterations { get; protected set; }


    /// <summary>
    /// Gets or sets the maximum number of iterations.
    /// </summary>
    /// <value>The maximum number number of iterations. The default value is 100.</value>
    /// <remarks>
    /// In one call of <strong>FindRoot</strong> no more than <see cref="MaxNumberOfIterations"/>
    /// are performed.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is negative.
    /// </exception>
    public int MaxNumberOfIterations
    {
      get { return _maxNumberOfIterations; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("value", "The max number of iterations must be greater than 0.");

        _maxNumberOfIterations = value;
      }
    }


    /// <summary>
    /// Gets or sets the tolerance value used for comparing x values (same as <see cref="EpsilonX"/>). 
    /// </summary>
    /// <value>
    /// The tolerance value used for comparing x values. 
    /// The default is <see cref="Numeric"/>.<see cref="Numeric.EpsilonD"/>.
    /// </value>
    /// <remarks>
    /// If the absolute difference of x from the new iteration and the x from the last iteration is
    /// less than this tolerance, the refinement of x is stopped.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is negative or 0.
    /// </exception>
    [Obsolete("The property Epsilon is obsolete. Use EpsilonX and EpsilonY instead.")]
    public double Epsilon
    {
      get { return _epsilonX; }
      set
      {
        EpsilonX = value;
      }
    }


    /// <summary>
    /// Gets or sets the tolerance value used for comparing x values. 
    /// </summary>
    /// <value>
    /// The tolerance value used for comparing x values. 
    /// The default is <see cref="Numeric"/>.<see cref="Numeric.EpsilonD"/>.
    /// </value>
    /// <remarks>
    /// If the absolute difference of x from the new iteration and the x from the last iteration is
    /// less than this tolerance, the refinement of x is stopped.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is negative or 0.
    /// </exception>
    public double EpsilonX
    {
      get { return _epsilonX; }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException("value", "The tolerance value must be greater than zero");

        _epsilonX = value;
      }
    }
    private double _epsilonX = Numeric.EpsilonD;


    /// <summary>
    /// Gets or sets the tolerance value used for comparing function values. 
    /// </summary>
    /// <value>
    /// The tolerance value used for comparing function values.
    /// The default is <see cref="Numeric"/>.<see cref="Numeric.EpsilonD"/>.
    /// </value>
    /// <remarks>
    /// If the absolute difference of f(x) and the searched y value is less than this tolerance, 
    /// the refinement of x is stopped.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is negative or 0.
    /// </exception>
    public double EpsilonY
    {
      get { return _epsilonY; }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException("value", "The tolerance value must be greater than zero");

        _epsilonY = value;
      }
    }
    private double _epsilonY = Numeric.EpsilonD;



    

    

    /// <summary>
    /// Initializes a new instance of the <see cref="RootFinderD"/> class.
    /// </summary>
    /// <param name="function">The function f(x), which root we want to find.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="function"/> is <see langword="null"/>.
    /// </exception>
    protected RootFinderD(Func<double, double> function)
    {
      if (function == null)
        throw new ArgumentNullException("function");

      Function = function;
    }



    

    

    /// <overloads>
    /// <summary>
    /// Takes the initial guessed range [<paramref name="x0"/>, <paramref name="x1"/>] and expands
    /// this interval such that the root is in the interval.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Takes the initial guessed range [<paramref name="x0"/>, <paramref name="x1"/>] and expands
    /// this interval such that the root <i>x<sub>root</sub></i> where 
    /// <i>f(x<sub>root</sub>) = 0</i> is in the interval.
    /// </summary>
    /// <param name="x0">The left bound of the interval.</param>
    /// <param name="x1">The right bound of the interval.</param>
    /// <returns>
    /// <see langword="true"/> if a valid bracket was found; otherwise <see langword="false"/>.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public bool ExpandBracket(ref double x0, ref double x1)
    {
      return ExpandBracket(Function, ref x0, ref x1);
    }


    /// <summary>
    /// Takes the initial guessed range [<paramref name="x0"/>, <paramref name="x1"/>] and expands
    /// this interval such that <i>x</i> with <i>f(x) = y</i>
    /// is in the interval.
    /// </summary>
    /// <param name="x0">The left bound of the interval.</param>
    /// <param name="x1">The right bound of the interval.</param>
    /// <param name="y">The y for which an x is searched for such that <i>f(x) = y</i>.</param>
    /// <returns>
    /// <see langword="true"/> if a valid bracket was found; otherwise <see langword="false"/>.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public bool ExpandBracket(ref double x0, ref double x1, double y)
    {
      // Reformulate f(x) = y as simple root finding f(x) - y = 0.
      return ExpandBracket(x => Function(x) - y, ref x0, ref x1);
    }


    /// <summary>
    /// Takes the initial guessed range [<paramref name="x0"/>, <paramref name="x1"/>] and expands
    /// this interval such that the root <i>x<sub>root</sub></i> where 
    /// <i>f(x<sub>root</sub>) = 0</i> is in the interval.
    /// </summary>
    /// <param name="function">The function f(x).</param>
    /// <param name="x0">The left bound of the interval.</param>
    /// <param name="x1">The right bound of the interval.</param>
    /// <returns>
    /// <see langword="true"/> if a valid bracket was found; otherwise <see langword="false"/>.
    /// </returns>
    private bool ExpandBracket(Func<double, double> function, ref double x0, ref double x1)
    {
      Debug.Assert(function != null);

      // See Numerical Recipes, p. 356.

      const double expansionFactor = 1.6;

      // If interval has size 0, make it a useful interval.
      if (Numeric.AreEqual(x0, x1))
        x1 += 1;

      double y0 = function(x0);
      double y1 = function(x1);

      for (int i = 0; i < MaxNumberOfIterations; i++)
      {
        if (y0 * y1 < 0)
          return true;

        if (Math.Abs(y0) < Math.Abs(y1))
        {
          x0 += expansionFactor * (x0 - x1);
          y0 = function(x0);
        }
        else
        {
          x1 += expansionFactor * (x1 - x0);
          y1 = function(x1);
        }
      }
      return false;
    }


    /// <overloads>
    /// <summary>
    /// Finds the root of the given function.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Finds the root of the given function.
    /// </summary>
    /// <param name="x0">
    /// An x value such that the root lies between <paramref name="x0"/> and <paramref name="x1"/>.
    /// </param>
    /// <param name="x1">
    /// An x value such that the root lies between <paramref name="x0"/> and <paramref name="x1"/>.
    /// </param>
    /// <returns>The x value such that <i>f(x) = 0</i>; or <i>NaN</i> if no root is found.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public double FindRoot(double x0, double x1)
    {
      return FindRoot(Function, x0, x1);
    }


    /// <summary>
    /// Finds the x for the given function, such that <i>f(x) = y</i>.
    /// </summary>
    /// <param name="x0">
    /// An x value such that the root lies between <paramref name="x0"/> and <paramref name="x1"/>.
    /// </param>
    /// <param name="x1">
    /// An x value such that the root lies between <paramref name="x0"/> and <paramref name="x1"/>.
    /// </param>
    /// <param name="y">
    /// The y for which an x is searched for such that <i>f(x) = y</i>.
    /// </param>
    /// <returns>
    /// The x value such that <i>f(x) = y</i>; or <i>NaN</i> if no suitable x is found.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public double FindRoot(double x0, double x1, double y)
    {
      // Reformulate as simple root finding.
      return FindRoot(x => Function(x) - y, x0, x1);
    }


    /// <summary>
    /// Finds the root of the given function.
    /// </summary>
    /// <param name="function">The function f(x).</param>
    /// <param name="x0">
    /// An x value such that the root lies between <paramref name="x0"/> and <paramref name="x1"/>.
    /// </param>
    /// <param name="x1">
    /// An x value such that the root lies between <paramref name="x0"/> and <paramref name="x1"/>.
    /// </param>
    /// <returns>The x value such that <i>f(x) = 0</i>; or <i>NaN</i> if no root is found.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
    protected abstract double FindRoot(Func<double, double> function, double x0, double x1);

  }
}
