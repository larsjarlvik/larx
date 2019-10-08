#version 330
uniform sampler2D uTexture;

in vec2 texCoord;

out vec4 outputColor;

void main() {
    vec3 texColor = texture(uTexture, vec2(texCoord.x, 1.0 - texCoord.y)).rgb;
    outputColor = vec4(texColor, 1.0);
}
