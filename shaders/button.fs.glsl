#version 330

const float shade_strength = 4;
const float border_width = 0.04;
const int state_hover = 1;
const int state_pressed = 2;

uniform sampler2D uTexture;
uniform int uState;
uniform int uActive;

in vec2 vert_texCoord;

out vec4 outputColor;

void main() {
    vec3 texColor = texture(uTexture, vert_texCoord).rgb;
    vec3 gradient = vec3(0.0);

    float maxX = 1.0 - border_width;
    float minX = border_width;
    float maxY = maxX;
    float minY = minX;

    if (vert_texCoord.x < maxX && vert_texCoord.x > minX &&
        vert_texCoord.y < maxY && vert_texCoord.y > minY) {
        float intensity = clamp(
            pow(abs(vert_texCoord.x - 0.5) * 2, shade_strength) +
            pow(abs(vert_texCoord.y - 0.5) * 2, shade_strength),
            0.0, 1.0
        ) * 0.6;

        outputColor = vec4(mix(texColor, gradient, intensity), uState == 0 ? 0.85 : 1.0);
    } else {
        if (uActive == 1) {
            outputColor = vec4(1.0, 0.54, 0.0, 1.0);
        } else {
            outputColor = vec4(1.0);
        }

        if (uState == state_pressed) {
            outputColor.a = 0.5;
        }
    }
}
