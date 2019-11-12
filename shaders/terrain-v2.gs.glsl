#version 430
#include calculate-light-vectors
#include clip

layout(triangles) in;
layout(triangle_strip, max_vertices = 4) out;

uniform sampler2D uNormalMap;

uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;

in vec2 te_texCoord[];

out vec2 gs_texCoord;
out vec3 gs_position;
out LightVectors gs_lightVectors;


vec3 calcTangent() {
    vec3 v0 = gl_in[0].gl_Position.xyz;
    vec3 v1 = gl_in[1].gl_Position.xyz;
    vec3 v2 = gl_in[2].gl_Position.xyz;

    vec3 e1 = v1 - v0;
    vec3 e2 = v2 - v0;

    vec2 uv0 = te_texCoord[0];
    vec2 uv1 = te_texCoord[1];
    vec2 uv2 = te_texCoord[2];

    vec2 deltaUV1 = uv1 - uv0;
    vec2 deltaUV2 = uv2 - uv0;

    float r = 1.0 / (deltaUV1.x * deltaUV2.y - deltaUV1.y * deltaUV2.x);

    return normalize((e1 * deltaUV2.y - e2 * deltaUV1.y) * r);
}

void main() {
    vec3 tangent = calcTangent();

    for (int i = 0; i < gl_in.length(); ++i) {
        vec4 position = gl_in[i].gl_Position * vec4(1, 25, 1, 1);
        vec3 normal = (texture(uNormalMap, te_texCoord[i]).zyx * 2.0) - 1.0;

        gs_lightVectors = calculateLightVectors(normal, tangent, position.xyz, mat3(1.0));
        gs_texCoord = te_texCoord[i];
        gs_position = position.xyz;

        gl_ClipDistance[0] = clip(position.xyz);
        gl_Position = uProjectionMatrix * uViewMatrix * position;
        EmitVertex();
    }

    EndPrimitive();
}