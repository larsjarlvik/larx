#version 330

out vec4 outputColor;

uniform sampler2D uBaseColorTexture;
uniform vec4 uClearColor;
uniform vec3 uLightDirection;

in vec2 vert_texCoord;
in vec3 vert_position;
in vec3 vert_normal;

void main() {
    outputColor = texture(uBaseColorTexture, vert_texCoord);

    float sunFactor = pow(max(dot(normalize(vert_normal), normalize(uLightDirection)), 0.0), 200);
    outputColor += sunFactor;
    outputColor = mix(uClearColor, outputColor, clamp(vert_position.y * 15 - 0.05, 0.0, 1.0));
}