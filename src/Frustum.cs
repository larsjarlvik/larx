using System;
using OpenTK;

namespace Larx
{
    public static class Frustum
    {
        public static Vector4[] extractFrustum(Matrix4 mMat, Matrix4 pMat) {
            var clip = new Matrix4();
            var frustum = new Vector4[6];

            clip.M11 = mMat.M11 * pMat.M11 + mMat.M12 * pMat.M21 + mMat.M13 * pMat.M31 + mMat.M14 * pMat.M41;
            clip.M12 = mMat.M11 * pMat.M12 + mMat.M12 * pMat.M22 + mMat.M13 * pMat.M32 + mMat.M14 * pMat.M42;
            clip.M13 = mMat.M11 * pMat.M13 + mMat.M12 * pMat.M23 + mMat.M13 * pMat.M33 + mMat.M14 * pMat.M43;
            clip.M14 = mMat.M11 * pMat.M14 + mMat.M12 * pMat.M24 + mMat.M13 * pMat.M34 + mMat.M14 * pMat.M44;

            clip.M21 = mMat.M21 * pMat.M11 + mMat.M22 * pMat.M21 + mMat.M23 * pMat.M31 + mMat.M24 * pMat.M41;
            clip.M22 = mMat.M21 * pMat.M12 + mMat.M22 * pMat.M22 + mMat.M23 * pMat.M32 + mMat.M24 * pMat.M42;
            clip.M23 = mMat.M21 * pMat.M13 + mMat.M22 * pMat.M23 + mMat.M23 * pMat.M33 + mMat.M24 * pMat.M43;
            clip.M24 = mMat.M21 * pMat.M14 + mMat.M22 * pMat.M24 + mMat.M23 * pMat.M34 + mMat.M24 * pMat.M44;

            clip.M31 = mMat.M31 * pMat.M11 + mMat.M32 * pMat.M21 + mMat.M33 * pMat.M31 + mMat.M34 * pMat.M41;
            clip.M32 = mMat.M31 * pMat.M12 + mMat.M32 * pMat.M22 + mMat.M33 * pMat.M32 + mMat.M34 * pMat.M42;
            clip.M33 = mMat.M31 * pMat.M13 + mMat.M32 * pMat.M23 + mMat.M33 * pMat.M33 + mMat.M34 * pMat.M43;
            clip.M34 = mMat.M31 * pMat.M14 + mMat.M32 * pMat.M24 + mMat.M33 * pMat.M34 + mMat.M34 * pMat.M44;

            clip.M41 = mMat.M41 * pMat.M11 + mMat.M42 * pMat.M21 + mMat.M43 * pMat.M31 + mMat.M44 * pMat.M41;
            clip.M42 = mMat.M41 * pMat.M12 + mMat.M42 * pMat.M22 + mMat.M43 * pMat.M32 + mMat.M44 * pMat.M42;
            clip.M43 = mMat.M41 * pMat.M13 + mMat.M42 * pMat.M23 + mMat.M43 * pMat.M33 + mMat.M44 * pMat.M43;
            clip.M44 = mMat.M41 * pMat.M14 + mMat.M42 * pMat.M24 + mMat.M43 * pMat.M34 + mMat.M44 * pMat.M44;

            /* Extract the numbers for the RIGHT plane */
            frustum[0].X = clip.M14 - clip.M11;
            frustum[0].Y = clip.M24 - clip.M21;
            frustum[0].Z = clip.M34 - clip.M31;
            frustum[0].W = clip.M44 - clip.M41;
            frustum[0].Normalize();

            /* Extract the numbers for the LEFT plane */
            frustum[1].X = clip.M14 + clip.M11;
            frustum[1].Y = clip.M24 + clip.M21;
            frustum[1].Z = clip.M34 + clip.M31;
            frustum[1].W = clip.M44 + clip.M41;
            frustum[1].Normalize();

            /* Extract the BOTTOM plane */
            frustum[2].X = clip.M14 + clip.M12;
            frustum[2].Y = clip.M24 + clip.M22;
            frustum[2].Z = clip.M34 + clip.M32;
            frustum[2].W = clip.M44 + clip.M42;
            frustum[2].Normalize();

            /* Extract the TOP plane */
            frustum[3].X = clip.M14 - clip.M12;
            frustum[3].Y = clip.M24 - clip.M22;
            frustum[3].Z = clip.M34 - clip.M32;
            frustum[3].W = clip.M44 - clip.M42;
            frustum[3].Normalize();

            /* Extract the FAR plane */
            frustum[4].X = clip.M14 - clip.M13;
            frustum[4].Y = clip.M24 - clip.M23;
            frustum[4].Z = clip.M34 - clip.M33;
            frustum[4].W = clip.M44 - clip.M43;
            frustum[4].Normalize();

            /* Extract the NEAR plane */
            frustum[5].X = clip.M14 + clip.M13;
            frustum[5].Y = clip.M24 + clip.M23;
            frustum[5].Z = clip.M34 + clip.M33;
            frustum[5].W = clip.M44 + clip.M43;
            frustum[5].Normalize();

            return frustum;
        }

        public static bool inFrustum(Vector3 v1, Vector3 v2, Vector4[] frustum)
        {
            for(var p = 0; p < 6; p++) {
                if(frustum[p].X * v1.X + frustum[p].Y * v1.Y + frustum[p].Z * v1.Z + frustum[p].W > -1.0) continue;
                if(frustum[p].X * v2.X + frustum[p].Y * v1.Y + frustum[p].Z * v1.Z + frustum[p].W > -1.0) continue;
                if(frustum[p].X * v1.X + frustum[p].Y * v2.Y + frustum[p].Z * v1.Z + frustum[p].W > -1.0) continue;
                if(frustum[p].X * v2.X + frustum[p].Y * v2.Y + frustum[p].Z * v1.Z + frustum[p].W > -1.0) continue;
                if(frustum[p].X * v1.X + frustum[p].Y * v1.Y + frustum[p].Z * v2.Z + frustum[p].W > -1.0) continue;
                if(frustum[p].X * v2.X + frustum[p].Y * v1.Y + frustum[p].Z * v2.Z + frustum[p].W > -1.0) continue;
                if(frustum[p].X * v1.X + frustum[p].Y * v2.Y + frustum[p].Z * v2.Z + frustum[p].W > -1.0) continue;
                if(frustum[p].X * v2.X + frustum[p].Y * v2.Y + frustum[p].Z * v2.Z + frustum[p].W > -1.0) continue;
                return false;
            }

            return true;
        }

        public static Vector3[] getFrustumCorners(Vector4[] frustum) {
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

        private static Vector3 getFrustumCorner(Vector4 f1, Vector4 f2, Vector4 f3) {
            var vf1 = f1.Xyz;
            var vf2 = f2.Xyz;
            var vf3 = f3.Xzy;
            var normals = Matrix3.Identity;

            normals.Row0 = vf1;
            normals.Row1 = vf2;
            normals.Row2 = vf3;

            var det = normals.Determinant;
            var v1 = Vector3.Cross(vf2, vf3);
            var v2 = Vector3.Cross(vf3, vf1);
            var v3 = Vector3.Cross(vf1, vf2);

            v1 *= -f1.W;
            v2 *= -f2.W;
            v3 *= -f3.W;

            return (v1 + v2 + v3) / det;
        }
    }
}