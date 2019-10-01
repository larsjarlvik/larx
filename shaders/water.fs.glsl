#version 330

const float waveStrength = 0.01;
const float waveScale = 0.08;

const float uShininess = 0.4;
const float reflectivity = 0.2;

in vec3 lightVector;
in vec4 clipSpace;
in vec2 texCoord;
in vec3 eyeVector;
in vec3 position;

uniform sampler2D uRefractionColorTexture;
uniform sampler2D uRefractionDepthTexture;
uniform sampler2D uReflectionColorTexture;
uniform sampler2D uDuDvMap;
uniform sampler2D uNormalMap;

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;
uniform vec3 uCameraPosition;

uniform float uNear;
uniform float uFar;
uniform float uTimeOffset;

out vec4 outputColor;

vec3 calculateLight() {
    vec3 normalMap = texture(uNormalMap, vec2(texCoord.x + uTimeOffset, texCoord.y) / waveScale).rgb;
    vec3 n = normalize(vec3(normalMap.x * 2.0 - 1.0, normalMap.y, normalMap.z * 2.0 - 1.0));

    vec3 lightDir = normalize(lightVector);
    vec3 reflectDir = reflect(lightDir, n);
    vec3 specular = pow(max(dot(eyeVector, reflectDir), 0.0), uShininess) * uLightSpecular;

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
    vec3 waterColor = mix(refractionTexture, reflectionTexture, clamp(waterDepth / 25.0 + 0.3, 0.0, 1.0)) + calculateLight();
    outputColor = vec4(waterColor, clamp(waterDepth, 0.0, 1.0));
    // outputColor = vec4(calculateLight(), 1.0);
}
