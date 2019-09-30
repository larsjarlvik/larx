#version 330

out vec4 outputColor;

uniform sampler2D uBaseColorTexture;
uniform vec4 uClearColor;

in vec2 texCoord;
in float yPosition;

void main() {
    outputColor = mix(uClearColor, texture(uBaseColorTexture, texCoord), clamp(yPosition * 15 - 0.05, 0.0, 1.0));
}