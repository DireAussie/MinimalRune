using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

using MinimalRune.Mathematics;
using MinimalRune.Mathematics.Algebra;

using Microsoft.Xna.Framework;

namespace MinimalRune.Geometry
{
  /// <summary>
  /// A pose defines the position and orientation of a shape in world space (or the parent 
  /// coordinate space).
  /// </summary>
  /// <remarks>
  /// <para>
  /// This type represents an affine transformation consisting only of a rotation followed by a 
  /// translation - no scaling. This transformation transforms coordinates from local space to world 
  /// space (or a parent space). Every <see cref="IGeometricObject"/> has a <see cref="Pose"/> which 
  /// defines position and orientation of the figure in the parent's space which is usually the 
  /// world space. 
  /// </para>
  /// <para>
  /// For hierarchical objects, like <see cref="CompositeShape"/>s, a pose defines the relationship 
  /// of a local coordinates system to a parent coordinate system. For example: The children of a 
  /// <see cref="CompositeShape"/> are of type <see cref="IGeometricObject"/>. So each child of a 
  /// <see cref="CompositeShape"/> has a <see cref="Pose"/> which defines the position and 
  /// orientation of the child in the local space of the <see cref="CompositeShape"/>. The local 
  /// space of the <see cref="CompositeShape"/> is the parent space of the child. 
  /// </para>
  /// <para>
  /// <strong>Important:</strong> When creating a new <see cref="Pose"/> do not use <c>Pose p = new 
  /// Pose();</c> instead use <c>Pose p = Pose.Identity</c>. The constructor <c>new Pose()</c> 
  /// initializes the orientation quaternion elements with 0 and thus is not a valid transformation. 
  /// </para>
  /// <para>
  /// <strong>Notes:</strong> The name "pose" comes from the definition: <i>"to pose = to put or set 
  /// in place".</i>
  /// </para>
  /// </remarks>
  [Serializable]
  [DataContract]
  public struct Pose : IEquatable<Pose>
  {
    

    

    /// <summary>
    /// A pose with no translation and no rotation.
    /// </summary>
    public static readonly Pose Identity = new Pose(Vector3.Zero, Matrix.Identity);


    /// <summary>
    /// The position.
    /// </summary>

    [DataMember]

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
    public Vector3 Position;


    /// <summary>
    /// The orientation.
    /// </summary>
    /// <remarks>
    /// The orientation is stored as a 3x3 matrix. The matrix must represent a rotation.
    /// </remarks>

    [DataMember]

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
    public Matrix Orientation;


    /// <summary>
    /// Gets the inverse of this pose.
    /// </summary>
    /// <value>An inverse of this pose.</value>
    public Pose Inverse
    {
      get
      {
        Pose result = this;
        result.Invert();
        return result;
      }
    }


    /// <summary>
    /// Gets a value indicating whether the position is not 0.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the position describes a non-zero translation; otherwise, 
    /// <see langword="false"/>.
    /// </value>
    public bool HasTranslation
    {
      get { return !Position.IsNumericallyZero(); }
    }


    /// <summary>
    /// Gets a value indicating whether the orientation is not the default rotation.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the orientation describes a non-zero rotation; otherwise, 
    /// <see langword="false"/> if the orientation is not used (rotation angle is zero;
    /// <see cref="Orientation"/> is an identity matrix).
    /// </value>
    public bool HasRotation
    {
      get
      {
        if (!Numeric.AreEqual(1, Orientation.M11))
          return true;
        if (!Numeric.AreEqual(1, Orientation.M22))
          return true;

        return false;
      }
    }


    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="Pose"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="Pose"/> class from position and orientation.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="orientation">The orientation.</param>
    public Pose(Vector3 position, Matrix orientation)
    {
      Position = position;
      Orientation = orientation;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Pose"/> class from position.
    /// </summary>
    /// <param name="position">The position.</param>
    public Pose(Vector3 position)
    {
      Position = position;
      Orientation = Matrix.Identity;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Pose"/> class from orientation.
    /// </summary>
    /// <param name="orientation">The orientation.</param>
    public Pose(Matrix orientation)
    {
      Position = Vector3.Zero;
      Orientation = orientation;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Pose"/> class from position and orientation.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="orientation">The orientation.</param>
    public Pose(Vector3 position, Quaternion orientation)
    {
      Position = position;
      Orientation = Matrix.CreateFromQuaternion(orientation);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Pose"/> class from orientation.
    /// </summary>
    /// <param name="orientation">The orientation.</param>
    public Pose(Quaternion orientation)
    {
      Position = Vector3.Zero;
      Orientation = Matrix.CreateFromQuaternion(orientation);
    }


    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true"/> if the current object is equal to the other parameter; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool Equals(Pose other)
    {
      return this == other;
    }


    /// <summary>
    /// Inverts the pose.
    /// </summary>
    public void Invert()
    {
      // Let 
      //   R = Rotation as matrix
      //   t = translation as vector
      //   v = any vector
      //   v' = v transformed by the current pose
      //   Pose(v) = current pose
      //   Pose'(v) = inverse of current pose
      // 
      // Pose(v) = Rv + t
      // v' = Rv + t
      // v' - t = Rv
      // R'(v' - t) = v
      // v = R'v' + R'(-t)
      // => Pose'(v) = R'v + R'(-t)

      // Calculate the inverse of the orientation. 
      // (The inverse of an orthogonal is the same as the transposed matrix.)
      Orientation = Matrix.Transpose(Orientation);     // R'

      // Calculate the new translation
      Position = (Orientation * Matrix.CreateTranslation(-Position)).Translation; // R'(-t)
    }


    /// <summary>
    /// Converts a direction vector from local space to world space (or the parent space for nested 
    /// coordinate spaces).
    /// </summary>
    /// <param name="localDirection">The local direction.</param>
    /// <returns>
    /// The direction in world space (or the parent space for nested coordinate spaces).
    /// </returns>
    /// <remarks>
    /// This method can be used to transform direction vectors. It applies only the rotation to the 
    /// vector. The translation is ignored. 
    /// </remarks>
    public Vector3 ToWorldDirection(Vector3 localDirection)
    {
      // return Orientation * localDirection;

      // ----- Optimized version:
      Vector3 result;
      result.X = Orientation.M11 * localDirection.X + Orientation.M21 * localDirection.Y + Orientation.M31 * localDirection.Z;
      result.Y = Orientation.M12 * localDirection.X + Orientation.M22 * localDirection.Y + Orientation.M32 * localDirection.Z;
      result.Z = Orientation.M13 * localDirection.X + Orientation.M23 * localDirection.Y + Orientation.M33 * localDirection.Z;
      return result;
    }


    /// <summary>
    /// Converts a direction vector from world space (or the parent space for nested coordinate 
    /// spaces) to local space.
    /// </summary>
    /// <param name="worldDirection">
    /// The direction vector in world space (or the parent space for nested coordinate spaces).
    /// </param>
    /// <returns>The direction in local space.</returns>
    /// <remarks>
    /// This method can be used to transform direction vectors. It applies only the rotation to the 
    /// vector. The translation is ignored. 
    /// </remarks>
    public Vector3 ToLocalDirection(Vector3 worldDirection)
    {
      //return Matrix.MultiplyTransposed(Orientation, worldDirection);

      // ----- Optimized version:
      Vector3 result;
      result.X = Orientation.M11 * worldDirection.X + Orientation.M12 * worldDirection.Y + Orientation.M13 * worldDirection.Z;
      result.Y = Orientation.M21 * worldDirection.X + Orientation.M22 * worldDirection.Y + Orientation.M23 * worldDirection.Z;
      result.Z = Orientation.M31 * worldDirection.X + Orientation.M32 * worldDirection.Y + Orientation.M33 * worldDirection.Z;
      return result;
    }


    /// <summary>
    /// Converts a position vector from local space to world space (or the parent space for nested 
    /// coordinate spaces).
    /// </summary>
    /// <param name="localPosition">The local position.</param>
    /// <returns>
    /// The position in world space (or the parent space for nested coordinate spaces).
    /// </returns>
    public Vector3 ToWorldPosition(Vector3 localPosition)
    {
      //return Orientation * localPosition + Position;

      // ----- Optimized version:
      Vector3 result;
      result.X = Orientation.M11 * localPosition.X + Orientation.M21 * localPosition.Y + Orientation.M31 * localPosition.Z + Position.X;
      result.Y = Orientation.M12 * localPosition.X + Orientation.M22 * localPosition.Y + Orientation.M32 * localPosition.Z + Position.Y;
      result.Z = Orientation.M13 * localPosition.X + Orientation.M23 * localPosition.Y + Orientation.M33 * localPosition.Z + Position.Z;
      return result;
    }


    /// <summary>
    /// Converts a direction vector from world space (or the parent space for nested coordinate 
    /// spaces) to local space.
    /// </summary>
    /// <param name="worldPosition">
    /// The position vector in world space (or the parent space for nested coordinate spaces).
    /// </param>
    /// <returns>The position in local space.</returns>
    public Vector3 ToLocalPosition(Vector3 worldPosition)
    {
      //return Matrix.MultiplyTransposed(Orientation, worldPosition - Position);

      // ----- Optimized version:
      Vector3 diff;
      diff.X = worldPosition.X - Position.X;
      diff.Y = worldPosition.Y - Position.Y;
      diff.Z = worldPosition.Z - Position.Z;
      Vector3 result;
      result.X = Orientation.M11 * diff.X + Orientation.M12 * diff.Y + Orientation.M13 * diff.Z;
      result.Y = Orientation.M21 * diff.X + Orientation.M22 * diff.Y + Orientation.M23 * diff.Z;
      result.Z = Orientation.M31 * diff.X + Orientation.M32 * diff.Y + Orientation.M33 * diff.Z;
      return result;
    }


    /// <summary>
    /// Creates a <see cref="Pose"/> from a matrix that contains a translation and a rotation.
    /// </summary>
    /// <param name="poseMatrix">The pose matrix.</param>
    /// <returns>A pose that represents the same transformation as the 4x4-matrix.</returns>
    /// <remarks>
    /// <paramref name="poseMatrix"/> must only contain rotations and translations, otherwise the
    /// result is undefined.
    /// </remarks>
    public static Pose FromMatrix(Matrix poseMatrix)
    {
      Debug.Assert(IsValid(poseMatrix), "Matrix is not a valid pose matrix. Pose matrix must only contain rotations and translations.");

      return new Pose(
        new Vector3(poseMatrix.M41, poseMatrix.M42, poseMatrix.M43),
        new Matrix(poseMatrix.M11, poseMatrix.M12, poseMatrix.M13, 0f,
                      poseMatrix.M21, poseMatrix.M22, poseMatrix.M23, 0f,
                      poseMatrix.M31, poseMatrix.M32, poseMatrix.M33, 0f,
                      0f, 0f, 0f, 1f));
    }


    /// <summary>
    /// Converts this pose to a 4x4 transformation matrix.
    /// </summary>
    /// <returns>
    /// A 4x4-matrix that represents the same transformation as the pose.
    /// </returns>
    public Matrix ToMatrix()
    {
        return this;
    }


    /// <overloads>
    /// <summary>
    /// Determines whether two poses are equal (regarding a given tolerance).
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Determines whether two poses are equal (regarding the tolerance 
    /// <see cref="Numeric.EpsilonF"/>).
    /// </summary>
    /// <param name="pose1">The first pose.</param>
    /// <param name="pose2">The second pose.</param>
    /// <returns>
    /// <see langword="true"/> if the poses are equal (within the tolerance 
    /// <see cref="Numeric.EpsilonF"/>); otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The two vectors are compared component-wise. If the differences of the components are less
    /// than <see cref="Numeric.EpsilonF"/> the vectors are considered as being equal.
    /// </remarks>
    public static bool AreNumericallyEqual(Pose pose1, Pose pose2)
    {
      return Numeric.AreEqual(pose1.Position.X, pose2.Position.X)
          && Numeric.AreEqual(pose1.Position.Y, pose2.Position.Y)
          && Numeric.AreEqual(pose1.Position.Z, pose2.Position.Z)
          && Numeric.AreEqual(pose1.Orientation.M11, pose2.Orientation.M11)
          && Numeric.AreEqual(pose1.Orientation.M21, pose2.Orientation.M21)
          && Numeric.AreEqual(pose1.Orientation.M31, pose2.Orientation.M31)
          && Numeric.AreEqual(pose1.Orientation.M12, pose2.Orientation.M12)
          && Numeric.AreEqual(pose1.Orientation.M22, pose2.Orientation.M22)
          && Numeric.AreEqual(pose1.Orientation.M32, pose2.Orientation.M32)
          && Numeric.AreEqual(pose1.Orientation.M13, pose2.Orientation.M13)
          && Numeric.AreEqual(pose1.Orientation.M23, pose2.Orientation.M23)
          && Numeric.AreEqual(pose1.Orientation.M33, pose2.Orientation.M33);
    }


    /// <summary>
    /// Determines whether two poses are equal (regarding a specific tolerance).
    /// </summary>
    /// <param name="pose1">The first pose.</param>
    /// <param name="pose2">The second pose.</param>
    /// <param name="epsilon">The tolerance value.</param>
    /// <returns>
    /// <see langword="true"/> if the poses are equal (within the tolerance 
    /// <paramref name="epsilon"/>); otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The two vectors are compared component-wise. If the differences of the components are less
    /// than <paramref name="epsilon"/> the poses are considered as being equal.
    /// </remarks>
    public static bool AreNumericallyEqual(Pose pose1, Pose pose2, float epsilon)
    {
      return Numeric.AreEqual(pose1.Position.X, pose2.Position.X, epsilon)
          && Numeric.AreEqual(pose1.Position.Y, pose2.Position.Y, epsilon)
          && Numeric.AreEqual(pose1.Position.Z, pose2.Position.Z, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M11, pose2.Orientation.M11, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M21, pose2.Orientation.M21, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M31, pose2.Orientation.M31, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M12, pose2.Orientation.M12, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M22, pose2.Orientation.M22, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M32, pose2.Orientation.M32, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M13, pose2.Orientation.M13, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M23, pose2.Orientation.M23, epsilon)
          && Numeric.AreEqual(pose1.Orientation.M33, pose2.Orientation.M33, epsilon);
    }


    /// <summary>
    /// Interpolates two poses.
    /// </summary>
    /// <param name="startPose">The start pose.</param>
    /// <param name="endPose">The end pose.</param>
    /// <param name="parameter">
    /// The interpolation parameter. If the value is 0, the <paramref name="startPose"/> is
    /// returned. If the value is 1, the <paramref name="endPose"/> is returned. For values between
    /// 0 and 1 an interpolated pose is returned.
    /// </param>
    /// <returns>An interpolated pose.</returns>
    public static Pose Interpolate(Pose startPose, Pose endPose, float parameter)
    {
      // Linearly interpolate position.
      var interpolatedPosition = startPose.Position * (1 - parameter) + endPose.Position * parameter;

      // Slerp orientation.
      var interpolatedOrientation = Quaternion.Lerp(
        Quaternion.CreateFromRotationMatrix(startPose.Orientation),
        Quaternion.CreateFromRotationMatrix(endPose.Orientation),
        parameter);

      return new Pose(interpolatedPosition, interpolatedOrientation);
    }


    /// <summary>
    /// Determines whether the specified matrix is a valid pose matrix.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns>
    /// <see langword="true"/> if the specified matrix is a valid pose matrix; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method makes a simple, low-performance test.
    /// </remarks>
    public static bool IsValid(Matrix matrix)
    {
      Vector4 v1 = matrix.Multiply(Vector4.UnitX);
      Vector4 v2 = matrix.Multiply(Vector4.UnitY);
      Vector4 v3 = matrix.Multiply(Vector4.UnitZ);

      return Numeric.AreEqual(v1.LengthSquared(), 1)
             && Numeric.AreEqual(v2.LengthSquared(), 1)
             && Numeric.AreEqual(v3.LengthSquared(), 1)
             && Numeric.IsZero(Vector4.Dot(v1, v2))
             && Numeric.IsZero(Vector4.Dot(v2, v3))
             && Numeric.IsZero(Vector4.Dot(v1, v3))
             && Numeric.AreEqual(1.0f, matrix.Determinant())
             && Numeric.IsZero(matrix.M14)
             && Numeric.IsZero(matrix.M24)
             && Numeric.IsZero(matrix.M34)
             && Numeric.AreEqual(matrix.M44, 1);
    }


    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer that is the hash code for this instance.
    /// </returns>
    public override int GetHashCode()
    {
      // ReSharper disable NonReadonlyFieldInGetHashCode
      unchecked
      {
        int hashCode = Position.GetHashCode();
        hashCode = (hashCode * 397) ^ Orientation.GetHashCode();
        return hashCode;
      }
      // ReSharper restore NonReadonlyFieldInGetHashCode
    }


    /// <overloads>
    /// <summary>
    /// Indicates whether the current object is equal to another object.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">Another object to compare to.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> and this instance are the same type and
    /// represent the same value; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object obj)
    {
      return obj is Pose && this == ((Pose)obj);
    }


    /// <overloads>
    /// <summary>
    /// Returns the string representation of this pose.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Returns the string representation of this pose.
    /// </summary>
    /// <returns>The string representation of this pose.</returns>
    public override string ToString()
    {
      return ToString(CultureInfo.InvariantCulture);
    }


    /// <summary>
    /// Returns the string representation of this pose using the specified culture-specific format
    /// information.
    /// </summary>
    /// <param name="provider">
    /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.
    /// </param>
    /// <returns>The string representation of this pose.</returns>
    public string ToString(IFormatProvider provider)
    {
      return string.Format(provider, "Pose {{ Position = {0}, Orientation = {1} }}", Position, Orientation);
    }


    /// <overloads>
    /// <summary>
    /// Multiplies a <see cref="Pose"/> with another value.
    /// </summary>
    /// </overloads> 
    /// 
    /// <summary>
    /// Multiplies two poses.
    /// </summary>
    /// <param name="p1">The first pose p1.</param>
    /// <param name="p2">The second pose p2.</param>
    /// <returns>The product of p1 and p2: p1 * p2.</returns>
    /// <remarks>
    /// <para>
    /// When product (<paramref name="p1"/> * <paramref name="p2"/>) is applied to a vector <i>v</i> 
    /// the transformation are applied in the following order: <i>v'</i> = p1 * p2 * <i>v</i><br/>
    /// That means, the vector is first transformed by <paramref name="p2"/> and then by 
    /// <paramref name="p1"/>.
    /// </para>
    /// <para>
    /// Example: If <paramref name="p1"/> is the <see cref="Pose"/> of a 
    /// <see cref="CompositeShape"/> and <paramref name="p2"/> is the <see cref="Pose"/> of a child 
    /// <see cref="IGeometricObject"/> in the <see cref="CompositeShape"/>, the pose 
    /// <paramref name="p2"/> transforms from child's space to the <see cref="CompositeShape"/>'s
    /// space and <paramref name="p1"/> transforms from the <see cref="CompositeShape"/>'s space to
    /// world space. The result of the multiplication p1 * p2 is a pose that transforms directly
    /// from the child's space to world space.
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static Pose operator *(Pose p1, Pose p2)
    {
      // Pose'(v) = Pose1(Pose2(v)) = Pose1(R2 v + t2) = R1 (R2 v + t2) + t1
      //          = R1 R2 v + R1 t2 + t1
      // => R' = R1 R2
      //    t' = R1 t2 + t1
      return new Pose(p1.Orientation.Multiply(p2.Position) + p1.Position, p1.Orientation * p2.Orientation);
    }


    /// <overloads>
    /// <summary>
    /// Multiplies a <see cref="Pose"/> with another value.
    /// </summary>
    /// </overloads> 
    /// 
    /// <summary>
    /// Multiplies two poses.
    /// </summary>
    /// <param name="p1">The first pose p1.</param>
    /// <param name="p2">The second pose p2.</param>
    /// <returns>The multiplication of p1 and p2: p1 * p2.</returns>
    /// <remarks>
    /// <para>
    /// When product (<paramref name="p1"/> * <paramref name="p2"/>) is applied to a vector <i>v</i> 
    /// the transformation are applied in the following order: <i>v'</i> = p1 * p2 * <i>v</i><br/>
    /// That means, the vector is first transformed by <paramref name="p2"/> and then by 
    /// <paramref name="p1"/>.
    /// </para>
    /// <para>
    /// Example: If <paramref name="p1"/> is the <see cref="Pose"/> of a
    /// <see cref="CompositeShape"/> and <paramref name="p2"/> is the <see cref="Pose"/> of a child 
    /// <see cref="IGeometricObject"/> in the <see cref="CompositeShape"/>, the pose 
    /// <paramref name="p2"/> transforms from child's space to the <see cref="CompositeShape"/>'s
    /// space and <paramref name="p1"/> transforms from the <see cref="CompositeShape"/>'s space to
    /// world space. The result of the multiplication p1 * p2 is a pose that transforms directly
    /// from the child's space to world space.
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public static Pose Multiply(Pose p1, Pose p2)
    {
      // Pose'(v) = Pose1(Pose2(v)) = Pose1(R2 v + t2) = R1 (R2 v + t2) + t1
      //          = R1 R2 v + R1 t2 + t1
      // => R' = R1 R2
      //    t' = R1 t2 + t1
      return new Pose(p1.Orientation.Multiply(p2.Position) + p1.Position, p1.Orientation * p2.Orientation);
    }


    /// <summary>
    /// Multiplies the pose with a vector.
    /// </summary>
    /// <param name="pose">The pose.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The transformed vector.</returns>
    /// <remarks>
    /// Multiplying a pose matrix with a vector is equal to transforming a vector from local space
    /// to world space (or parent space for nested coordinate spaces).
    /// </remarks>
    public static Vector4 operator *(Pose pose, Vector4 vector)
    {
      return pose.ToMatrix().Multiply(vector);
    }


    /// <summary>
    /// Multiplies the pose with a vector.
    /// </summary>
    /// <param name="pose">The pose.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The transformed vector.</returns>
    /// <remarks>
    /// Multiplying a pose matrix with a vector is equal to transforming a vector from local space
    /// to world space (or parent space for nested coordinate spaces).
    /// </remarks>
    public static Vector4 Multiply(Pose pose, Vector4 vector)
    {
      return pose.ToMatrix().Multiply(vector);
    }


    /// <summary>
    /// Compares two <see cref="Pose"/>s to determine whether they are the same.
    /// </summary>
    /// <param name="pose1">The first pose.</param>
    /// <param name="pose2">The second pose.</param>
    /// <returns>
    /// <see langword="true"/> if the poses are equal; otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>Two poses are equal if their positions and orientations are equal.</remarks>
    public static bool operator ==(Pose pose1, Pose pose2)
    {
      return pose1.Position == pose2.Position && pose1.Orientation == pose2.Orientation;
    }


    /// <summary>
    /// Compares two <see cref="Pose"/>s to determine whether they are different.
    /// </summary>
    /// <param name="pose1">The first pose.</param>
    /// <param name="pose2">The second pose.</param>
    /// <returns>
    /// <see langword="true"/> if the poses are different; otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>Two poses are equal if their positions and orientations are equal.</remarks>
    public static bool operator !=(Pose pose1, Pose pose2)
    {
      return !(pose1 == pose2);
    }

    /// <summary>
    /// Converts a pose to a 4x4 transformation matrix.
    /// </summary>
    /// <param name="pose">The pose.</param>
    /// <returns>A 4x4-matrix that represents the same transformation as the pose.</returns>
    public static implicit operator Matrix(Pose pose)
    {
      Vector3 v = pose.Position;
      Matrix m = pose.Orientation;
      return new Matrix(m.M11, m.M21, m.M31, v.X,
                           m.M12, m.M22, m.M32, v.Y,
                           m.M13, m.M23, m.M33, v.Z,
                           0, 0, 0, 1);
    }
  }
}
