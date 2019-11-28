#version 330
precision highp float;

uniform sampler2D uBaseColorTexture;
in vec2 vert_texCoord;

void main() {
    vec4 tex = texture(uBaseColorTexture, vert_texCoord);
    if (tex.a < 0.1) discard;
}