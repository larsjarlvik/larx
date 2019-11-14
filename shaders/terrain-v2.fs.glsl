#version 330
#include calculate-light

uniform sampler2DArray uTexture;
uniform sampler2D uNormalMap;

uniform int uGridLines;

uniform vec3 uMousePosition;
uniform float uSelectionSize;

in vec2 gs_texCoord;
in vec3 gs_position;
in LightVectors gs_lightVectors;

out vec4 outputColor;

float circle() {
    float radius = uSelectionSize;
    float border = 0.08;
    float dist = distance(uMousePosition.xz, gs_position.xz);

    return 1.0 + smoothstep(radius, radius + border, dist)
               - smoothstep(radius - border, radius, dist);
}

float gridLine() {
    if (uGridLines == 0) return 0.0;

    vec2 coord = gs_position.xz;
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / fwidth(coord);
    float line = min(grid.x, grid.y);

    return (1.0 - min(line, 1.0)) / 3;
}

vec3 getTriPlanarTexture(int textureId, vec3 n) {
    vec3 blending = abs(n);
    blending = normalize(max(blending, 0.00001));
    float b = (blending.x + blending.y + blending.z);
    blending /= vec3(b, b, b);

    vec3 xaxis = texture(uTexture, vec3(gs_position.yz * 0.1, textureId)).rgb;
    vec3 yaxis = texture(uTexture, vec3(gs_position.xz * 0.1, textureId)).rgb;
    vec3 zaxis = texture(uTexture, vec3(gs_position.xy * 0.1, textureId)).rgb;

    return xaxis * blending.x + yaxis * blending.y + zaxis * blending.z;
}

vec3 finalTexture(int index, vec3 normal, LightVectors lv) {
    float noise = 1.0;
    vec3 n = texture(uTexture, getTriPlanarTexture(index * 3 + 1, normal)).rgb * 2.0 - 1.0;
    float r = texture(uTexture, getTriPlanarTexture(index * 3 + 2, normal)).r;
    return getTriPlanarTexture(index * 3, normal) * calculateLight(lv, n, r * 5.0, 1.0) * noise;
}

void main() {
    vec3 normal = (texture(uNormalMap, gs_texCoord).zyx * 2.0) - 1.0;
    vec3 color = finalTexture(0, normal, gs_lightVectors);

    vec3 terrainGridLines = mix(color, vec3(0.3, 0.3, 0.3), gridLine());
    outputColor = vec4(mix(vec3(1.0, 1.0, 1.0), terrainGridLines, circle()), 1.0);
}