#version 330

layout(location = 0) in vec2 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uMatrix;
uniform vec2 uPosition;
uniform vec2 uSize;

out vec2 vs_texCoord;

void main() {
    vs_texCoord = vTexCoord;
    gl_Position = uMatrix * vec4(round(uPosition + round(vPosition) * uSize), 0.0, 1.0);
}
