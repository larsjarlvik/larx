#version 330
#include shadow-factor
#include calculate-light

uniform float uRoughness;

uniform sampler2D uBaseColorTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uRoughnessTexture;

in vec2 vert_texCoord;
in vec3 vert_normal;
in LightVectors vert_lightVectors;
in vec4 vert_shadowCoords;

out vec4 outputColor;


void main() {
    vec4 tex = texture(uBaseColorTexture, vert_texCoord);
    if (tex.a < 0.7) discard;

    vec3 normalMap = texture(uNormalTexture, vert_texCoord).rgb * 2.0 - 1.0;
    vec3 n = normalize(normalMap);

    float roughness = uRoughness;
    if (roughness < 0.0) {
        roughness = texture(uRoughnessTexture, vert_texCoord).r;
    }

    outputColor = tex * vec4(calculateLight(vert_lightVectors, n, roughness, 1.0) * getShadowFactor(vert_shadowCoords, 0.3), tex.a);
}
