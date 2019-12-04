#version 330

const float shade_strength = 4;
const int state_hover = 1;
const int state_pressed = 2;

uniform sampler2D uTexture;
uniform int uState;
uniform int uActive;
uniform int uPanelType;
uniform int uFlat;
uniform vec3 uBackgroundColor;
uniform vec2 uSize;
uniform float uBorderWidth;

in vec2 vs_texCoord;

out vec4 outputColor;

void main() {
    vec3 background = uBackgroundColor;
    if (uPanelType == 0) {
        background = texture(uTexture, vs_texCoord).rgb;
    }

    vec3 gradient = vec3(0.0);
    vec2 border = uBorderWidth / uSize;

    float maxX = 1.0 - border.x;
    float minX = border.x;
    float maxY = 1.0 - border.y;
    float minY = border.y;

    if (vs_texCoord.x < maxX && vs_texCoord.x > minX &&
        vs_texCoord.y < maxY && vs_texCoord.y > minY) {

        if (uFlat == 0) {
            float intensity = clamp(
                pow(abs(vs_texCoord.x - 0.5) * 2, shade_strength) +
                pow(abs(vs_texCoord.y - 0.5) * 2, shade_strength),
                0.0, 1.0
            ) * 0.6;

            outputColor = vec4(mix(background, gradient, intensity), uState == 0 ? 0.85 : 1.0);
        } else {
            outputColor = vec4(background, uState == 0 ? 0.85 : 1.0);
        }
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
