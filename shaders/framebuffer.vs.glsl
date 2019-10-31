#version 330

layout(location = 0) in vec2 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uMatrix;
uniform vec2 uSize;
uniform vec2 uPosition;

out vec2 vert_texCoord;

void main() {
    vert_texCoord = vTexCoord;
    gl_Position = uMatrix * vec4(vPosition * uSize + uPosition, 0.0, 1.0);
}
