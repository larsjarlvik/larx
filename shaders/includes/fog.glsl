uniform float uFarPlane;
uniform vec4 uFogColor;

const float SIGHT_RANGE = 0.4;

vec3 fog(vec3 inputColor, float dist)
{
    float fogFactor = clamp(-0.0002 / SIGHT_RANGE * (dist - (uFarPlane) / 10 * SIGHT_RANGE) + 1, 0.0, 1.0);
    return mix(uFogColor.rgb, inputColor, clamp(fogFactor, 0.0, 1.0));
}
