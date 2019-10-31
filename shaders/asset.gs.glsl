#version 330
#include calculate-light-vectors

layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;

in vec2 vert_texCoord[3];
in vec4 vert_shadowCoords[3];
in float vert_clip[3];
in float vert_variation[3];
in vec3 vert_position[3];
in vec3 vert_normal[3];
in vec3 vert_tangent[3];
in mat3 vert_rotation[3];

out vec2 geom_texCoord;
out vec3 geom_lightVector;
out vec3 geom_eyeVector;
out vec4 geom_shadowCoords;
out float geom_distanceFade;

uniform float uRenderDistance;

void main() {
    for(int i = 0; i < 3; i++) {
        float distance = length(gl_in[i].gl_Position);
        float renderDistance = uRenderDistance - vert_variation[i];

        if (distance < renderDistance) {
            vec3[] vectors = calculateLightVectors(vert_normal[i], vert_tangent[i], vert_position[i], vert_rotation[i]);

            geom_distanceFade = clamp((1.0 - (distance / renderDistance)) * 10, 0.0, 1.0);
            geom_lightVector = vectors[0];
            geom_eyeVector = vectors[1];
            geom_texCoord = vert_texCoord[i];
            geom_shadowCoords = vert_shadowCoords[i];

            gl_ClipDistance[0] = vert_clip[0];
            gl_Position = gl_in[i].gl_Position;
            EmitVertex();
        }
    }

    EndPrimitive();
}