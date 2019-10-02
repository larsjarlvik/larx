#version 330

const float waveStrength = 0.03;
const float waveScale = 0.5;

const float uShininess = 50.0;
const float reflectivity = 1.0;

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

uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

uniform float uNear;
uniform float uFar;
uniform float uTimeOffset;

out vec4 outputColor;

vec3 calculateLight(float waterDepth) {
    vec3 nm1 = texture(uNormalMap, vec2(texCoord.x + uTimeOffset, texCoord.y) / waveScale).rgb;
    vec3 nm2 = texture(uNormalMap, vec2(-texCoord.x + uTimeOffset, texCoord.y + uTimeOffset) / waveScale).rgb;
    vec3 n = normalize(nm1 + nm2 - 1.0);

    vec3 diffuse = (max(dot(n, normalize(lightVector)), 0.0) - 0.5) * uLightDiffuse * 0.2;
    vec3 reflectedLightVector = reflect(-normalize(lightVector), n);
    float specularFactor = max(dot(reflectedLightVector, normalize(-eyeVector)), 0.0);
    float specular = pow(specularFactor, uShininess);

    return diffuse + specular * uLightSpecular * clamp(waterDepth / 5.0, 0, 1);
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
    return (distortion1 + distortion2) * waveStrength * clamp(waterDepth / 15.0, 0, 1);
}

void main() {
    vec2 ndc = (clipSpace.xy / clipSpace.w) / 2.0 + 0.5;
    float waterDepth = calculateDepth(ndc);

    vec2 totalDistortion = getDistortion(waterDepth);
    vec3 refractionTexture = texture(uRefractionColorTexture, clamp(ndc + totalDistortion, 0.001, 0.999)).rgb;

    vec3 reflectionTexture = texture(uReflectionColorTexture, clamp(vec2(1.0 - ndc.x, ndc.y) + totalDistortion, 0.001, 0.999)).rgb;
    vec3 waterColor = mix(refractionTexture, reflectionTexture, clamp(waterDepth / 35.0 + 0.3, 0.0, 1.0)) + calculateLight(waterDepth);

    outputColor = vec4(waterColor, clamp(waterDepth, 0.0, 1.0));

}
