#version 330
precision highp float;

uniform sampler2DArray uTexture;
uniform sampler2DArray uSplatMap;
uniform sampler2D uTextureNoise;

in vec3 position;
in vec2 texCoord;
in vec3 normal;

out vec4 outputColor;

uniform int uGridLines;
uniform int uSplatCount;
uniform int uShowOverlays;

uniform vec3 uMousePosition;
uniform float uSelectionSize;

#include shadow-factor
#include calculate-light

vec3 getTriPlanarTexture(int textureId) {
    vec3 n = normalize(normal);
    vec3 blending = abs(n);
    blending = normalize(max(blending, 0.00001));
    float b = (blending.x + blending.y + blending.z);
    blending /= vec3(b, b, b);

    vec3 xaxis = texture(uTexture, vec3(position.yz, textureId)).rgb;
    vec3 yaxis = texture(uTexture, vec3(position.xz, textureId)).rgb;
    vec3 zaxis = texture(uTexture, vec3(position.xy, textureId)).rgb;

    return xaxis * blending.x + yaxis * blending.y + zaxis * blending.z;
}

float gridLine() {
    if (uGridLines == 0) return 0.0;

    vec2 coord = position.xz;
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / fwidth(coord);
    float line = min(grid.x, grid.y);

    return (1.0 - min(line, 1.0)) / 3;
}

vec3 finalTexture(int index) {
    float n1 = (texture(uTextureNoise, texCoord / 0.3).r * 0.05) + 0.95;
    float n2 = (texture(uTextureNoise, texCoord / 4.5).r * 0.1) + 0.90;

    float noise = (n1 + n2) / 2;

    vec3 n = texture(uTexture, getTriPlanarTexture(index * 3 + 1)).rgb * 2.0 - 1.0;
    float r = texture(uTexture, getTriPlanarTexture(index * 3 + 2)).r;
    return getTriPlanarTexture(index * 3) * calculateLight(n, r * 5.0) * noise;
}

float circle() {
    float radius = uSelectionSize;
    float border = 0.08;
    float dist = distance(uMousePosition.xz, position.xz);

    return 1.0 + smoothstep(radius, radius + border, dist)
               - smoothstep(radius - border, radius, dist);
}

void main() {
    vec3 color = vec3(0);
    for (int i = 0; i < uSplatCount; i++) {
        float intesity = texture(uSplatMap, vec3(texCoord.x, texCoord.y, i)).r;
        if (intesity > 0.0) {
            color += finalTexture(i) * intesity;
        }
    }

    if (uShowOverlays == 0) {
        outputColor = vec4(color, 1.0);
        return;
    }

    color *= getShadowFactor(0.3);

    vec3 terrainGridLines = mix(color, vec3(0.3, 0.3, 0.3), gridLine());
    outputColor = vec4(mix(vec3(1.0, 1.0, 1.0), terrainGridLines, circle()), 1.0);
}
