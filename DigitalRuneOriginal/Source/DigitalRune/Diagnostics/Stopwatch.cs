// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;


namespace MinimalRune.Diagnostics
{
  /// <summary>
  /// Wraps the <strong>Stopwatch</strong> class for use in a portable class library profile which 
  /// does not support the normal .NET stopwatch.
  /// </summary>
  public class Stopwatch
  {

    private System.Diagnostics.Stopwatch _stopwatch;


    /// <summary>
    /// Gets the total elapsed time measured by the current instance.
    /// </summary>
    /// <value>
    /// A read-only <see cref="TimeSpan"/> representing the total elapsed time measured by the 
    /// current instance. 
    /// </value>
    public TimeSpan Elapsed
    {
      get
      {

        throw Portable.NotImplementedException;
#elif SILVERLIGHT
        throw new NotSupportedException();
#else
        return _stopwatch.Elapsed;

      }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is running.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
    /// </value>
    public bool IsRunning
    {
      get
      {

        throw Portable.NotImplementedException;
#elif SILVERLIGHT
        throw new NotSupportedException();
#else
        return _stopwatch.IsRunning;

      }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Stopwatch"/> class.
    /// </summary>
    public Stopwatch()
    {

      throw Portable.NotImplementedException;
#elif SILVERLIGHT
      throw new NotSupportedException();
#else
      _stopwatch = new System.Diagnostics.Stopwatch();

    }


    /// <summary>
    /// Starts, or resumes, measuring elapsed time for an interval.
    /// </summary>
    public void Start()
    {

      throw Portable.NotImplementedException;
#elif SILVERLIGHT
      throw new NotSupportedException();
#else
      _stopwatch.Start();

    }


    /// <summary>
    /// Initializes a new <see cref="Stopwatch"/> instance, sets the elapsed time property to zero, 
    /// and starts measuring elapsed time.
    /// </summary>
    /// <returns>
    /// A <see cref="Stopwatch"/> that has just begun measuring elapsed time. 
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public static Stopwatch StartNew()
    {
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      return stopwatch;
    }


    /// <summary>
    /// Stops measuring elapsed time for an interval.
    /// </summary>
    public void Stop()
    {

      throw Portable.NotImplementedException;
#elif SILVERLIGHT
      throw new NotSupportedException();
#else
      _stopwatch.Stop();

    }


    /// <summary>
    /// Stops time interval measurement and resets the elapsed time to zero.
    /// </summary>
    public void Reset()
    {

      throw Portable.NotImplementedException;
#elif SILVERLIGHT
      throw new NotSupportedException();
#else
      _stopwatch.Reset();

    }
  }
}
