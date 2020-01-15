// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

// The MonoGame portable DLL does not have an Accelerometer class. Since it also 
// doesn't wrap the other sensors (compass, gyrometer, etc.), we leave sensor handling
// to the user.



using MinimalRune.Mathematics.Algebra;
#else
using Vector2F = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;



using System.Diagnostics;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Input;



namespace MinimalRune.Game.Input
{
  partial class InputManager
  {
    

    

    private bool _isAccelerometerInitialized;


    // The accelerometer sensor on the device.
    private readonly Accelerometer _accelerometer = new Accelerometer();

    // Value is set asynchronously in callback.
    private Vector3 _accelerometerCallbackValue;
    private readonly object _syncRoot = new object();




    

    

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public bool IsAccelerometerActive { get; private set; }


    /// <inheritdoc/>
    public Vector3 AccelerometerValue
    {
      get
      {
        if (!_isAccelerometerInitialized)
        {
          _isAccelerometerInitialized = true;


          try
          {
            _accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
            _accelerometer.Start();
            IsAccelerometerActive = true;
          }
          catch (AccelerometerFailedException)
          {
            IsAccelerometerActive = false;
          }

        }

        return _accelerometerValue;
      }
    }
    private static Vector3 _accelerometerValue = new Vector3(0, 0, -1);



    

    


    // Accelerometer callback.
    private void OnAccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs eventArgs)
    {
      // Store the accelerometer value in our variable to be used on the next Update. This callback
      // can come from another thread!
      lock (_syncRoot)
      {
        _accelerometerCallbackValue = new Vector3((float)eventArgs.X, (float)eventArgs.Y, (float)eventArgs.Z);
      }
    }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void UpdateAccelerometer()
    {

      if (_isAccelerometerInitialized)
      {
        if (IsAccelerometerActive)
        {
          lock (_syncRoot)
          {
            _accelerometerValue = _accelerometerCallbackValue;
          }
        }
      }

    }

  }
}
