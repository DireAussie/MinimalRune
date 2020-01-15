using System;
using System.Collections.Generic;

using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace MinimalRune.Mathematics.Algebra
{
    public static class MatrixExtensions
    {
        public static Vector4 Multiply(this Matrix matrix, Vector4 vector)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");
            if (vector == null)
                throw new ArgumentNullException("vector");

            var product = new Vector4();
            
            for (var i = 0; i < 4; i++)
                for (var k = 0; k < 4; k++)
                    product.SetIndex(i, product.Index(i) + matrix[i, k] * vector.Index(k));

            return product;
        }

        public static Vector3 Multiply(this Matrix matrix, Vector3 vector)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");
            if (vector == null)
                throw new ArgumentNullException("vector");

            var product = new Vector3();

            for (var i = 0; i < 3; i++)
                for (var k = 0; k < 3; k++)
                    product.SetIndex(i, product.Index(i) + matrix[i, k] * vector.Index(k));

            return product;
        }
    }
}