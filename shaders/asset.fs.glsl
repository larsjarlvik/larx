#version 330
#include shadow-factor
#include calculate-light

uniform float uRoughness;

uniform sampler2D uBaseColorTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uRoughnessTexture;

in vec2 geom_texCoord;
in vec3 geom_normal;
in vec3 geom_lightVector;
in vec3 geom_eyeVector;
in vec4 geom_shadowCoords;
in float geom_distanceFade;

out vec4 outputColor;

void main() {
    vec4 tex = texture(uBaseColorTexture, geom_texCoord);
    if (tex.a < 0.7) discard;

    vec3 normalMap = texture(uNormalTexture, geom_texCoord).rgb * 2.0 - 1.0;
    vec3 n = normalize(normalMap);

    float roughness = uRoughness;
    if (roughness < 0.0) {
        roughness = texture(uRoughnessTexture, geom_texCoord).r;
    }

    outputColor = tex * vec4(calculateLight(geom_lightVector, geom_eyeVector, n, roughness, 1.0) * getShadowFactor(geom_shadowCoords, 0.3), tex.a * geom_distanceFade);
}
