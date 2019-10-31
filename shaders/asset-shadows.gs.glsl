#version 330

layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;

in vec2 vert_texCoord[3];
in float vert_variation[3];

out vec2 geom_texCoord;

uniform float uRenderDistance;

void main() {
    for(int i = 0; i < 3; i++) {
        float distance = length(gl_in[i].gl_Position);
        float renderDistance = uRenderDistance - vert_variation[i];

        if (distance < renderDistance) {
            geom_texCoord = vert_texCoord[i];

            gl_Position = gl_in[i].gl_Position;
            EmitVertex();
        }
    }

    EndPrimitive();
}