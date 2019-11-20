#version 430
#include clip

layout(quads, fractional_odd_spacing, cw) in;

uniform sampler2D uHeightMap;
uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;
uniform float uHeightMapScale;

in vec2 tc_texCoord[];

void main() {
    float u = gl_TessCoord.x;
    float v = gl_TessCoord.y;

    vec4 position = (
        (1 - u) * (1 - v) * gl_in[12].gl_Position +
        u * (1 - v) * gl_in[0].gl_Position +
        u * v * gl_in[3].gl_Position +
        (1 - u) * v * gl_in[15].gl_Position
    );

    vec2 texCoord = (
        (1 - u) * (1 - v) * tc_texCoord[12] +
        u * (1 - v) * tc_texCoord[0] +
        u * v * tc_texCoord[3] +
        (1 - u) * v * tc_texCoord[15]
    );

    float height = texture(uHeightMap, texCoord).r;
    position.y = height * uHeightMapScale;

    gl_ClipDistance[0] = clip(position.xyz);
    gl_Position = uProjectionMatrix * uViewMatrix * position;
}