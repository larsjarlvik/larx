#version 330

out vec4 outputColor;

uniform sampler2D uBaseColorTexture;
uniform vec4 uClearColor;
uniform vec3 uLightDirection;

in vec2 texCoord;
in vec3 position;
in vec3 normal;

void main() {
    outputColor = texture(uBaseColorTexture, texCoord);

    float sunFactor = pow(max(dot(normalize(normal), normalize(-uLightDirection)), 0.0), 200);
    outputColor += sunFactor;
    outputColor = mix(uClearColor, outputColor, clamp(position.y * 15 - 0.05, 0.0, 1.0));
}