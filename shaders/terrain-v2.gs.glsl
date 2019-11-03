#version 430

layout(triangles) in;
layout(triangle_strip, max_vertices = 4) out;

uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;

void main() {
    for (int i = 0; i < gl_in.length(); ++i) {
        vec4 position = gl_in[i].gl_Position;
        gl_Position = uProjectionMatrix * uViewMatrix * position;
        EmitVertex();
    }

    EndPrimitive();
}