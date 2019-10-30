#version 330

uniform float uRoughness;

uniform sampler2D uBaseColorTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uRoughnessTexture;

in vec2 texCoord;
in vec3 normal;

out vec4 outputColor;

#include shadow-factor
#include calculate-light

void main() {
    vec4 tex = texture(uBaseColorTexture, texCoord);
    if (tex.a < 0.7) discard;

    vec3 normalMap = texture(uNormalTexture, texCoord).rgb * 2.0 - 1.0;
    vec3 n = normalize(normalMap);

    float roughness = uRoughness;
    if (roughness < 0.0) {
        roughness = texture(uRoughnessTexture, texCoord).r;
    }

    outputColor = tex * vec4(calculateLight(n, roughness, 1.0) * getShadowFactor(0.3), tex.a);
}
