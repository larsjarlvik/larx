#version 330

// TODO: Move to uniform
const vec4 uClearColor = vec4(0.754, 0.832, 0.898, 1.0);

out vec4 outputColor;

uniform sampler2D uBaseColorTexture;

in vec2 texCoord;
in float yPosition;

void main() {
    outputColor = mix(uClearColor, texture(uBaseColorTexture, texCoord), clamp(yPosition * 15 - 0.05, 0.0, 1.0));
}