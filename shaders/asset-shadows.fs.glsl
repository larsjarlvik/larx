#version 330

uniform sampler2D uBaseColorTexture;

in vec2 texCoord;

out vec4 out_colour;

void main() {
    vec4 tex = texture(uBaseColorTexture, texCoord);
    if (tex.a < 0.7) discard;

    out_colour = vec4(1.0);
}