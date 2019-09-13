#version 330

const float waveStrength = 0.01;

in vec3 lightVector;
in vec4 clipSpace;
in vec2 texCoord;

uniform sampler2D uRefractionColorTexture;
uniform sampler2D uRefractionDepthTexture;
uniform sampler2D uDuDvMap;
uniform sampler2D uNormalMap;

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

uniform float uNear;
uniform float uFar;
uniform float uTimeOffset;

out vec4 outputColor;

const vec3 uAmbient = vec3(0.6, 0.6, 0.6);
const vec3 uDiffuse = vec3(0.9, 0.9, 0.9);

vec3 calculateLight() {
    vec3 n = normalize(vec3(0, 1, 0));
    vec3 ambient = uAmbient * uLightAmbient;
    vec3 diffuse = max(dot(lightVector, n), 0.0) * uDiffuse * uLightDiffuse;

    return ambient + diffuse;
}

float calculateDepth(vec2 ndc) {
    float depth = texture(uRefractionDepthTexture, ndc).r;

    float floorDistance = 2.0 * uNear * uFar / (uFar + uNear - (2.0 * depth - 1.0) * (uFar - uNear));
    depth = gl_FragCoord.z;
    float waterDistance = 2.0 * uNear * uFar / (uFar + uNear - (2.0 * depth - 1.0) * (uFar - uNear));

    return floorDistance - waterDistance;
}

void main() {
    vec2 ndc = (clipSpace.xy / clipSpace.w) / 2.0 + 0.5;
    float waterDepth = calculateDepth(ndc);

    vec2 distortion1 = texture(uDuDvMap, vec2(texCoord.x + uTimeOffset, texCoord.y) * 16.0).rg * 2.0 - 1.0;
    vec2 distortion2 = texture(uDuDvMap, vec2(-texCoord.x + uTimeOffset, texCoord.y + uTimeOffset) * 16.0).rg * 2.0 - 1.0;
    vec2 totalDistortion = (distortion1 + distortion2) * waveStrength;

    vec2 refractionTexCoords = clamp(ndc + totalDistortion, 0.001, 0.999);

    vec3 waterColor = mix(texture(uRefractionColorTexture, refractionTexCoords).rgb, vec3(0.61, 0.81, 0.82), clamp(waterDepth / 5.0, 0.4, 1.0));
    outputColor = vec4(waterColor, waterDepth);
}
