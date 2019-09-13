#version 330

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

out vec3 outputColor;

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

    outputColor = mix(texture(uRefractionColorTexture, ndc).rgb, vec3(0.61, 0.81, 0.82), clamp(waterDepth / 3.0, 0.4, 1.0));
}
