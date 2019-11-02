#version 330

out vec4 outputColor;

uniform float uDepth;

void main() {
    outputColor = vec4(uDepth, 0.0, 0.0, 1.0);
}