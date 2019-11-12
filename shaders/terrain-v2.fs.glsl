#version 330
#include calculate-light

uniform sampler2DArray uTexture;
uniform sampler2D uNormalMap;

in vec2 gs_texCoord;
in vec3 gs_position;
in LightVectors gs_lightVectors;

out vec4 outputColor;

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
    outputColor = vec4(color, 1.0);
}