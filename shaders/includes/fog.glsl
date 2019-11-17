uniform float uFarPlane;
uniform vec4 uFogColor;

const float SIGHT_RANGE = 0.3;

vec3 fog(vec3 inputColor, float dist)
{
    float d = dist - 250.0;
    float fogFactor = clamp(-0.0002 / SIGHT_RANGE * (d - (uFarPlane) / 10 * SIGHT_RANGE) + 1, 0.0, 1.0);
    return mix(uFogColor.rgb, inputColor, clamp(fogFactor, 0.0, 1.0));
}
