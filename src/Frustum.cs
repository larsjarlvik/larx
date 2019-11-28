using System;
using OpenTK;

namespace Larx
{
    public class Frustum
    {
        public static Vector4[] ExtractFrustum(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            var frustum = new Vector4[6];
            var clip = viewMatrix * projectionMatrix;

            frustum[0] = (clip.Column3 - clip.Column0).Normalized(); // right plane
            frustum[1] = (clip.Column3 + clip.Column0).Normalized(); // left plane
            frustum[2] = (clip.Column3 + clip.Column1).Normalized(); // bottom plane
            frustum[3] = (clip.Column3 - clip.Column1).Normalized(); // top plane
            frustum[4] = (clip.Column3 - clip.Column2).Normalized(); // far plane
            frustum[5] = (clip.Column3 + clip.Column2).Normalized(); // near plane

            return frustum;
        }

        public static bool CubeInFrustum(Vector4[] f, Vector3 c, float s)
        {
            if (f == null) return true;

            for(var i = 0; i < 6; i++ ) {
                if(f[i].X * (c.X - s) + f[i].Y * (c.Y - s) + f[i].Z * (c.Z - s) + f[i].W > 0) continue;
                if(f[i].X * (c.X + s) + f[i].Y * (c.Y - s) + f[i].Z * (c.Z - s) + f[i].W > 0) continue;
                if(f[i].X * (c.X - s) + f[i].Y * (c.Y + s) + f[i].Z * (c.Z - s) + f[i].W > 0) continue;
                if(f[i].X * (c.X + s) + f[i].Y * (c.Y + s) + f[i].Z * (c.Z - s) + f[i].W > 0) continue;
                if(f[i].X * (c.X - s) + f[i].Y * (c.Y - s) + f[i].Z * (c.Z + s) + f[i].W > 0) continue;
                if(f[i].X * (c.X + s) + f[i].Y * (c.Y - s) + f[i].Z * (c.Z + s) + f[i].W > 0) continue;
                if(f[i].X * (c.X - s) + f[i].Y * (c.Y + s) + f[i].Z * (c.Z + s) + f[i].W > 0) continue;
                if(f[i].X * (c.X + s) + f[i].Y * (c.Y + s) + f[i].Z * (c.Z + s) + f[i].W > 0) continue;
                return false;
            }

            return true;
        }

        public static Vector3[] GetFrustumCorners(Vector4[] frustum)
        {
            var points = new Vector3[8];

            points[0] = getFrustumCorner(frustum[4], frustum[3], frustum[0]);
            points[1] = getFrustumCorner(frustum[4], frustum[3], frustum[1]);
            points[2] = getFrustumCorner(frustum[4], frustum[2], frustum[0]);
            points[3] = getFrustumCorner(frustum[4], frustum[2], frustum[1]);
            points[4] = getFrustumCorner(frustum[5], frustum[3], frustum[0]);
            points[5] = getFrustumCorner(frustum[5], frustum[3], frustum[1]);
            points[6] = getFrustumCorner(frustum[5], frustum[2], frustum[0]);
            points[7] = getFrustumCorner(frustum[5], frustum[2], frustum[1]);

            return points;
        }

        private static Vector3 getFrustumCorner(Vector4 f1, Vector4 f2, Vector4 f3)
        {
            var normals = new Matrix3(f1.Xyz, f2.Xyz, f3.Xyz);
            var det = normals.Determinant;

            var v1 = Vector3.Cross(f2.Xyz, f3.Xyz) * -f1.W;
            var v2 = Vector3.Cross(f3.Xyz, f1.Xyz) * -f2.W;
            var v3 = Vector3.Cross(f1.Xyz, f2.Xyz) * -f3.W;

            return (v1 + v2 + v3) / det;
        }
    }
}