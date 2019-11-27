#version 430
#include calculate-light-vectors
#include clip
#include shadow-coords

layout(quads, fractional_odd_spacing, cw) in;

uniform sampler2D uHeightMap;
uniform sampler2D uNormalMap;
uniform sampler2D uTextureNoise;

uniform float uHeightMapScale;
uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;

in vec2 tc_texCoord[];

out vec2 te_texCoord;
out vec3 te_position;
out vec4[3] te_shadowCoords;
out vec3 te_normal;
out float te_noise;
out LightVectors te_lightVectors;

vec3 calculate_tangent(vec3 n) {
    vec3 v = vec3(1.0, 0.0, 0.0);
    float d = dot(v, n);

    return normalize(v - d * n);
}

void main() {
    float u = gl_TessCoord.x;
    float v = gl_TessCoord.y;

    vec4 position = (
        (1 - u) * (1 - v) * gl_in[12].gl_Position +
        u * (1 - v) * gl_in[0].gl_Position +
        u * v * gl_in[3].gl_Position +
        (1 - u) * v * gl_in[15].gl_Position
    );

    vec2 texCoord = (
        (1 - u) * (1 - v) * tc_texCoord[12] +
        u * (1 - v) * tc_texCoord[0] +
        u * v * tc_texCoord[3] +
        (1 - u) * v * tc_texCoord[15]
    );

    float height = texture(uHeightMap, texCoord).r;
    position.y = height * uHeightMapScale;

    vec3 normal = normalize((texture(uNormalMap, texCoord).zyx * 2.0) - 1.0);
    float n1 = (texture(uTextureNoise, texCoord / 0.6).r * 0.1) + 0.95;
    float n2 = (texture(uTextureNoise, texCoord / 9.0).r * 0.2) + 0.90;

    te_noise = (n1 + n2) / 2;
    te_lightVectors = calculateLightVectors(normal, calculate_tangent(normal), position.xyz, mat3(1.0));
    te_texCoord = texCoord;
    te_position = position.xyz;
    te_shadowCoords = getShadowCoords(position);
    te_normal = normal;

    gl_ClipDistance[0] = clip(position.xyz);
    gl_Position = uProjectionMatrix * uViewMatrix * position;
}