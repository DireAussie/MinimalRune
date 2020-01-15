// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.Collections.Generic;
using MinimalRune.Collections;
using MinimalRune.Mathematics.Algebra;


using System.Linq;



namespace MinimalRune.Geometry.Partitioning
{
  partial class DualPartition<T>
  {
    /// <inheritdoc/>
    public override IEnumerable<Pair<T>> GetOverlaps(ISpatialPartition<T> otherPartition)
    {
      UpdateInternal();
      var overlapsStatic = StaticPartition.GetOverlaps(otherPartition);
      var overlapsDynamic = DynamicPartition.GetOverlaps(otherPartition);


      return overlapsStatic.Concat(overlapsDynamic);
#else
      return ConcatWork<Pair<T>>.Create(overlapsStatic, overlapsDynamic);

    }


    /// <inheritdoc/>
    public override IEnumerable<Pair<T>> GetOverlaps(Vector3 scale, Pose pose, ISpatialPartition<T> otherPartition, Vector3 otherScale, Pose otherPose)
    {
      UpdateInternal();
      var overlapsStatic = StaticPartition.GetOverlaps(scale, pose, otherPartition, otherScale, otherPose);
      var overlapsDynamic = DynamicPartition.GetOverlaps(scale, pose, otherPartition, otherScale, otherPose);


      return overlapsStatic.Concat(overlapsDynamic);
#else
      return ConcatWork<Pair<T>>.Create(overlapsStatic, overlapsDynamic);

    }
  }



  internal sealed class ConcatWork<T> : PooledEnumerable<T>
  {
    // ReSharper disable StaticFieldInGenericType
    private static readonly ResourcePool<ConcatWork<T>> Pool = new ResourcePool<ConcatWork<T>>(() => new ConcatWork<T>(), x => x.Initialize(), null);
    // ReSharper restore StaticFieldInGenericType
    private IEnumerator<T> _enumerator;
    private IEnumerable<T> _nextEnumerable;

    public static IEnumerable<T> Create(IEnumerable<T> enumerableA, IEnumerable<T> enumerableB)
    {
      var enumerable = Pool.Obtain();
      enumerable._enumerator = enumerableA.GetEnumerator();
      enumerable._nextEnumerable = enumerableB;
      return enumerable;
    }

    protected override bool OnNext(out T current)
    {
      while (true)
      {
        if (_enumerator == null)
        {
          if (_nextEnumerable != null)
          {
            _enumerator = _nextEnumerable.GetEnumerator();
            _nextEnumerable = null;
          }
          else
          {
            current = default(T);
            return false;
          }
        }

        if (_enumerator.MoveNext())
        {
          current = _enumerator.Current;
          return true;
        }
        else
        {
          _enumerator.Dispose();
          _enumerator = null;
        }
      }
    }

    protected override void OnRecycle()
    {
      if (_enumerator != null)
      {
        _enumerator.Dispose();
        _enumerator = null;
      }
      _nextEnumerable = null;
      Pool.Recycle(this);
    }
  }

}
