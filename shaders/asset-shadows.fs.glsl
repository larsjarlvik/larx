#version 330
precision highp float;

uniform sampler2D uBaseColorTexture;
in vec2 texCoord;

void main() {
    vec4 tex = texture(uBaseColorTexture, texCoord);
    if (tex.a < 0.7) discard;
}