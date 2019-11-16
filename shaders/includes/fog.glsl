uniform float uFarPlane;

float fog(float dist)
{
    float sightRange = uFarPlane * 0.00015;
    return clamp(-0.0002 / sightRange * (dist - (uFarPlane) / 10.0 * sightRange) + 1.0, 0.0, 1.0);
}
