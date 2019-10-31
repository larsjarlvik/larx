#version 330
#include shadow-factor
#include calculate-light

const float waveStrength = 0.03;
const float waveScale = 0.1;

const float uShininess = 50.0;
const float reflectivity = 1.0;

in vec4 vert_position;
in vec2 vert_texCoord;
in vec3 vert_lightVector;
in vec3 vert_eyeVector;
in vec4 vert_shadowCoords;

uniform sampler2D uRefractionColorTexture;
uniform sampler2D uRefractionDepthTexture;
uniform sampler2D uReflectionColorTexture;
uniform sampler2D uDuDvMap;
uniform sampler2D uNormalMap;

uniform float uNear;
uniform float uFar;
uniform float uTimeOffset;

out vec4 outputColor;

float calculateDepth(vec2 ndc) {
    float depth = texture(uRefractionDepthTexture, ndc).r;

    float floorDistance = 2.0 * uNear * uFar / (uFar + uNear - (2.0 * depth - 1.0) * (uFar - uNear));
    depth = gl_FragCoord.z;
    float waterDistance = 2.0 * uNear * uFar / (uFar + uNear - (2.0 * depth - 1.0) * (uFar - uNear));

    return floorDistance - waterDistance;
}

vec2 getDistortion(float waterDepth) {
    vec2 distortion1 = texture(uDuDvMap, vec2(vert_texCoord.x + uTimeOffset, vert_texCoord.y) / waveScale).rg * 2.0 - 1.0;
    vec2 distortion2 = texture(uDuDvMap, vec2(-vert_texCoord.x + uTimeOffset, vert_texCoord.y + uTimeOffset) / waveScale).rg * 2.0 - 1.0;
    return (distortion1 + distortion2) * waveStrength * clamp(waterDepth / 15.0, 0, 1);
}

void main() {
    vec2 ndc = (vert_position.xy / vert_position.w) / 2.0 + 0.5;
    float waterDepth = calculateDepth(ndc);

    vec2 totalDistortion = getDistortion(waterDepth);
    vec3 refractionTexture = texture(uRefractionColorTexture, clamp(ndc + totalDistortion, 0.001, 0.999)).rgb;

    vec3 nm1 = texture(uNormalMap, vec2(vert_texCoord.x + uTimeOffset, vert_texCoord.y) / waveScale).rgb;
    vec3 nm2 = texture(uNormalMap, vec2(-vert_texCoord.x + uTimeOffset, vert_texCoord.y + uTimeOffset) / waveScale).rgb;
    vec3 n = normalize(nm1 + nm2 - 1.0);
    vec3 light = calculateLight(vert_lightVector, vert_eyeVector, n, uShininess, 0.0) * clamp(waterDepth / 5.0, 0, 1);

    vec3 reflectionTexture = texture(uReflectionColorTexture, clamp(vec2(1.0 - ndc.x, ndc.y) + totalDistortion, 0.001, 0.999)).rgb * getShadowFactor(vert_shadowCoords, 0.2);
    vec3 waterColor = mix(refractionTexture, reflectionTexture, clamp(waterDepth / 35.0 + 0.3, 0.0, 1.0)) + light;

    outputColor = vec4(waterColor, clamp(waterDepth, 0.0, 1.0));
}
