﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using MinimalRune.Animation.Character;
using MinimalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Content;


namespace MinimalRune.Animation.Content
{
  /// <summary>
  /// Reads a <see cref="SkeletonKeyFrameAnimation"/> from binary format. 
  /// (Only available in the XNA-compatible build.)
  /// </summary>
  /// <remarks>
  /// This type is available only in the XNA-compatible build of the DigitalRune.Animation.dll.
  /// </remarks>
  public class SkeletonKeyFrameAnimationReader : ContentTypeReader<SkeletonKeyFrameAnimation>
  {

    /// <summary>
    /// Determines if deserialization into an existing object is possible.
    /// </summary>
    public override bool CanDeserializeIntoExistingObject
    {
      get { return true; }
    }



    /// <summary>
    /// Reads a strongly typed object from the current stream.
    /// </summary>
    /// <param name="input">The <see cref="ContentReader"/> used to read the object.</param>
    /// <param name="existingInstance">An existing object to read into.</param>
    /// <returns>The type of object to read.</returns>
    protected override SkeletonKeyFrameAnimation Read(ContentReader input, SkeletonKeyFrameAnimation existingInstance)
    {
      if (existingInstance == null)
        existingInstance = new SkeletonKeyFrameAnimation();

      // _totalDuration
      existingInstance._totalDuration = input.ReadRawObject<TimeSpan>();

      // _times
      int numberOfTimes = input.ReadInt32();
      TimeSpan[] times = new TimeSpan[numberOfTimes];
      for (int i = 0; i < numberOfTimes; i++)
        times[i] = input.ReadRawObject<TimeSpan>();

      existingInstance._times = times;

      // _channels
      int numberOfChannels = input.ReadInt32();
      int[] channels = new int[numberOfChannels];
      for (int i = 0; i < numberOfChannels; i++)
        channels[i] = input.ReadInt32();

      existingInstance._channels = channels;

      // _weights
      float[] weights = new float[numberOfChannels];
      for (int i = 0; i < numberOfChannels; i++)
        weights[i] = input.ReadSingle();

      existingInstance._weights = weights;

      // _indices
      int[] indices = new int[numberOfChannels * numberOfTimes];
      for (int i = 0; i < indices.Length; i++)
        indices[i] = input.ReadInt32();

      existingInstance._indices = indices;

      // _keyFrameTypes, _keyFrames
      existingInstance._keyFrameTypes = new BoneKeyFrameType[numberOfChannels];
      existingInstance._keyFrames = new object[numberOfChannels];

      for (int i = 0; i < numberOfChannels; i++)
      {
        var boneKeyFrameType = (BoneKeyFrameType)input.ReadInt32();
        existingInstance._keyFrameTypes[i] = boneKeyFrameType;

        int numberOfKeyFrames = input.ReadInt32();

        if (boneKeyFrameType == BoneKeyFrameType.R)
        {
          var keyFrames = new BoneKeyFrameR[numberOfKeyFrames];
          for (int keyFrameIndex = 0; keyFrameIndex < numberOfKeyFrames; keyFrameIndex++)
          {
            keyFrames[keyFrameIndex] = new BoneKeyFrameR
            {
              Time = input.ReadRawObject<TimeSpan>(),
              Rotation = input.ReadRawObject<Quaternion>(),
            };
          }

          existingInstance._keyFrames[i] = keyFrames;
        }
        else if (boneKeyFrameType == BoneKeyFrameType.RT)
        {
          var keyFrames = new BoneKeyFrameRT[numberOfKeyFrames];
          for (int keyFrameIndex = 0; keyFrameIndex < numberOfKeyFrames; keyFrameIndex++)
          {
            keyFrames[keyFrameIndex] = new BoneKeyFrameRT
            {
              Time = input.ReadRawObject<TimeSpan>(),
              Rotation = input.ReadRawObject<Quaternion>(),
              Translation = input.ReadRawObject<Vector3>(),
            };
          }

          existingInstance._keyFrames[i] = keyFrames;
        }
        else
        {
          var keyFrames = new BoneKeyFrameSRT[numberOfKeyFrames];
          for (int keyFrameIndex = 0; keyFrameIndex < numberOfKeyFrames; keyFrameIndex++)
          {
            keyFrames[keyFrameIndex] = new BoneKeyFrameSRT
            {
              Time = input.ReadRawObject<TimeSpan>(),
              Transform = new SrtTransform(
                input.ReadRawObject<Vector3>(), 
                input.ReadRawObject<Quaternion>(), 
                input.ReadRawObject<Vector3>())
            };
          }

          existingInstance._keyFrames[i] = keyFrames;
        }
      }

      existingInstance.EnableInterpolation = input.ReadBoolean();
      existingInstance.FillBehavior = (FillBehavior)input.ReadInt32();
      existingInstance.IsAdditive = input.ReadBoolean();

      if (input.ReadBoolean())
        existingInstance.TargetObject = input.ReadString();

      if (input.ReadBoolean())
        existingInstance.TargetProperty = input.ReadString();

      return existingInstance;
    }
  }
}
