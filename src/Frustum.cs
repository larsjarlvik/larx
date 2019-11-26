using System;
using OpenTK;

namespace Larx
{
    public class Frustum
    {
        public static Vector4[] ExtractFrustum(Camera camera)
        {
            var proj = camera.ProjectionMatrix;
            var modl = camera.ViewMatrix;
            var frustum = new Vector4[6];
            var clip = camera.ViewMatrix * camera.ProjectionMatrix;

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
    }
}