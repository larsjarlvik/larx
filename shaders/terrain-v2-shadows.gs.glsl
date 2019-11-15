#version 430
#include calculate-light-vectors
#include clip

layout(triangles) in;
layout(triangle_strip, max_vertices = 4) out;

uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;
uniform float uHeightMapScale;


void main() {
    for (int i = 0; i < gl_in.length(); ++i) {
        vec4 position = gl_in[i].gl_Position;
        position.y *= uHeightMapScale;
        position.y = -position.y;
        gl_ClipDistance[0] = clip(position.xyz);
        gl_Position = uProjectionMatrix * uViewMatrix * position;
        EmitVertex();
    }

    EndPrimitive();
}