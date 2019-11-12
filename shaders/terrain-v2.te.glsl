#version 430

layout(quads, fractional_odd_spacing, cw) in;

uniform sampler2D uHeightMap;

in vec2 tc_texCoord[];
out vec2 te_texCoord;

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
    position.y = height;

    te_texCoord = texCoord;
    gl_Position = position;
}