#version 330
uniform sampler2D uTexture;

in vec2 vert_texCoord;

out vec4 outputColor;

void main() {
    vec3 texColor = texture(uTexture, vec2(vert_texCoord.x, 1.0 - vert_texCoord.y)).rgb;
    outputColor = vec4(texColor, 1.0);
}
