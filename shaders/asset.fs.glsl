#version 330

uniform float uRoughness;

uniform sampler2D uBaseColorTexture;
uniform sampler2D uNormalTexture;

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

    outputColor = tex * vec4(calculateLight(n, uRoughness) * getShadowFactor(0.3), 1.0);
}
