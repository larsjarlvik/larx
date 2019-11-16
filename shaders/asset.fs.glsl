#version 330
#include shadow-factor
#include calculate-light
#include fog

uniform float uRoughness;
uniform vec3 uCameraPosition;
uniform sampler2D uBaseColorTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uRoughnessTexture;

in vec2 vs_texCoord;
in vec3 vs_normal;
in LightVectors vs_lightVectors;
in vec4 vs_shadowCoords;
in vec3 vs_position;

out vec4 outputColor;


void main() {
    vec4 tex = texture(uBaseColorTexture, vs_texCoord);
    if (tex.a < 0.7) discard;

    vec3 normalMap = texture(uNormalTexture, vs_texCoord).rgb * 2.0 - 1.0;
    vec3 n = normalize(normalMap);

    float roughness = uRoughness;
    if (roughness < 0.0) {
        roughness = texture(uRoughnessTexture, vs_texCoord).r;
    }

    float dist = length(uCameraPosition - vs_position.xyz);

    outputColor = tex * vec4(calculateLight(vs_lightVectors, n, roughness, 1.0) * getShadowFactor(vs_shadowCoords, 0.3), tex.a);
    outputColor = vec4(fog(outputColor.rgb, dist), tex.a);
}
