#version 330
uniform sampler2D uTexture;

in vec2 texCoord;

out vec4 outputColor;

const float shade_strength = 4;
const float border_width = 0.04;

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

        outputColor = vec4(mix(texColor, gradient, intensity), 1.0);
    } else {
        outputColor = vec4(1.0);
    }
}
