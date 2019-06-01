#version 330
uniform sampler2D uTexture;
uniform int uState;

in vec2 texCoord;

out vec4 outputColor;

const float shade_strength = 4;
const float border_width = 0.04;

const int state_hover = 1;
const int state_pressed = 2;

void main() {
    vec3 texColor = texture2D(uTexture, texCoord * 1.0).rgb;
    vec3 gradient = vec3(0.0);

    float maxX = 1.0 - border_width;
    float minX = border_width;
    float maxY = maxX;
    float minY = minX;

    if (texCoord.x < maxX && texCoord.x > minX &&
        texCoord.y < maxY && texCoord.y > minY) {
        float intensity = clamp(
            pow(abs(texCoord.x - 0.5) * 2, shade_strength) +
            pow(abs(texCoord.y - 0.5) * 2, shade_strength),
            0.0, 1.0
        ) * 0.6;

        outputColor = vec4(mix(texColor, gradient, intensity), 0.8);
    } else {
        if (uState == state_hover) {
            outputColor = vec4(0.41, 0.56, 0.64, 1.0);
        } else if (uState == state_pressed) {
            outputColor = vec4(0.53, 0.75, 0.87, 1.0);
        } else {
            outputColor = vec4(1.0);
        }
    }
}
