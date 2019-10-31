#version 330
uniform sampler2D uTexture;
uniform vec4 uColor;
uniform float uBuffer;
uniform float uGamma;

in vec2 vert_texCoord;

out vec4 outputColor;

void main() {
    float dist = texture(uTexture, vert_texCoord).r;
    float alpha = smoothstep(uBuffer - uGamma, uBuffer + uGamma, dist);
    outputColor = vec4(uColor.rgb, alpha * uColor.a);
}
