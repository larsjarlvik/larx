#version 330

layout(location = 0) in vec2 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uMatrix;
uniform vec2 uTexSize;
uniform vec2 uPosition;

out vec2 texCoord;

void main() {
    gl_Position = uMatrix * vec4(vPosition + uPosition, 0.0, 1.0);
    texCoord = vTexCoord / uTexSize;
}
