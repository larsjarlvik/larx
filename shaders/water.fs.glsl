#version 330

const float waveStrength = 0.01;
const float waveScale = 0.08;

const float uShininess = 0.8;
const float reflectivity = 0.05;

in vec3 lightVector;
in vec3 normalVector;
in vec4 clipSpace;
in vec2 texCoord;
in vec3 eyeVector;

uniform sampler2D uRefractionColorTexture;
uniform sampler2D uRefractionDepthTexture;
uniform sampler2D uReflectionColorTexture;
uniform sampler2D uDuDvMap;
uniform sampler2D uNormalMap;

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

uniform float uNear;
uniform float uFar;
uniform float uTimeOffset;

out vec4 outputColor;

vec3 calculateLight(vec3 normal) {
    vec3 n = normalize(normalVector * normal);
    vec3 halfwayVector = normalize(lightVector + eyeVector);
    vec3 specular = pow(max(dot(n, halfwayVector), 0.0), uShininess) * uLightSpecular;

    return specular * reflectivity;
}

float calculateDepth(vec2 ndc) {
    float depth = texture(uRefractionDepthTexture, ndc).r;

    float floorDistance = 2.0 * uNear * uFar / (uFar + uNear - (2.0 * depth - 1.0) * (uFar - uNear));
    depth = gl_FragCoord.z;
    float waterDistance = 2.0 * uNear * uFar / (uFar + uNear - (2.0 * depth - 1.0) * (uFar - uNear));

    return floorDistance - waterDistance;
}

vec2 getDistortion(float waterDepth) {
    vec2 distortion1 = texture(uDuDvMap, vec2(texCoord.x + uTimeOffset, texCoord.y) / waveScale).rg * 2.0 - 1.0;
    vec2 distortion2 = texture(uDuDvMap, vec2(-texCoord.x + uTimeOffset, texCoord.y + uTimeOffset) / waveScale).rg * 2.0 - 1.0;
    return (distortion1 + distortion2) * waveStrength * clamp(waterDepth / 5.0, 0, 1);
}

void main() {
    vec2 ndc = (clipSpace.xy / clipSpace.w) / 2.0 + 0.5;
    float waterDepth = calculateDepth(ndc);

    vec2 totalDistortion = getDistortion(waterDepth);
    vec3 refractionTexture = texture(uRefractionColorTexture, clamp(ndc + totalDistortion, 0.001, 0.999)).rgb;

    vec3 reflectionTexture = texture(uReflectionColorTexture, clamp(vec2(1.0 - ndc.x, ndc.y) + totalDistortion, 0.001, 0.999)).rgb;

    vec3 normalTexture = texture(uNormalMap, vec2(texCoord.x + uTimeOffset, texCoord.y) * 16.0).rgb;
    vec3 normal = vec3(normalTexture.r * 2.0 - 1.0, normalTexture.b, normalTexture.g * 2.0 - 1.0);

    vec3 waterColor = mix(refractionTexture, reflectionTexture, clamp(waterDepth / 25.0 + 0.3, 0.0, 1.0)) + calculateLight(normal);
    outputColor = vec4(waterColor, clamp(waterDepth, 0.0, 1.0));
}
