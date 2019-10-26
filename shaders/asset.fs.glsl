#version 330

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;
uniform float uRoughness;
uniform int uEnableShadows;

uniform sampler2D uBaseColorTexture;
uniform sampler2D uNormalTexture;
uniform sampler2DShadow uShadowMap;

in vec3 lightVector;
in vec3 eyeVector;
in vec2 texCoord;
in vec3 normal;
in vec4 shadowCoords;

out vec4 outputColor;

const float PCF_COUNT = 2.0;
const float PCF_SAMLE_SIZE = 1.0;
float getShadowFactor() {
    if(shadowCoords.z > 1.0 || uEnableShadows != 1.0) {
        return 1.0;
    }

    float totalTexels = (PCF_COUNT * 2.0 + PCF_SAMLE_SIZE) * (PCF_COUNT * 2.0 + PCF_SAMLE_SIZE);
    float texelSize = 1.0 / 4096.0;
    float total = 0.0;

    for(float x = -PCF_COUNT; x <= PCF_COUNT; x += PCF_SAMLE_SIZE) {
        for(float y = -PCF_COUNT; y <= PCF_COUNT; y += PCF_SAMLE_SIZE) {
            float nearestLight = texture(uShadowMap, vec3(shadowCoords.xy + vec2(x, y) * texelSize, shadowCoords.z));
            if(shadowCoords.z - 0.001 > nearestLight) {
                total += 0.3;
            }
        }
    }

    total /= totalTexels;
    return 1.0 - (total * shadowCoords.w);
}

vec3 calculateLight() {
    vec3 normalMap = texture(uNormalTexture, texCoord).rgb * 2.0 - 1.0;
    vec3 n = normalize(normalMap);

    vec3 diffuse = max(dot(n, normalize(lightVector)), 0.0) * uLightDiffuse;
    vec3 reflectedLightVector = reflect(-normalize(lightVector), n);
    float specularFactor = max(dot(reflectedLightVector, normalize(-eyeVector)), 0.0);
    vec3 specular = pow(specularFactor, uRoughness * 5.0) * uLightSpecular;

    return uLightAmbient + diffuse + specular;
}

void main() {
    vec4 tex = texture(uBaseColorTexture, texCoord);
    if (tex.a < 0.7) discard;

    outputColor = tex * vec4(calculateLight() * getShadowFactor(), 1.0);
}
