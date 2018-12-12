#version 330

uniform sampler2D uTexture;
uniform vec4 uColor;
uniform float uBuffer;
uniform float uGamma;

in vec2 texCoord;

out vec4 outputColor;

void main() {
    // outputColor = texture(uTexture, texCoord);
    float dist = texture(uTexture, texCoord).r;
    float alpha = smoothstep(uBuffer - uGamma, uBuffer + uGamma, dist);
    outputColor = vec4(uColor.rgb, alpha * uColor.a);
}
