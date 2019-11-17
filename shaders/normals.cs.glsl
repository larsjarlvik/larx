#version 430 core

layout (local_size_x = 16, local_size_y = 16) in;

layout (binding = 0, rgba32f) uniform writeonly image2D normalmap;

uniform sampler2D uInput;
uniform int uSize;
uniform float uNormalStrength;

void main(void)
{
    ivec2 x = ivec2(gl_GlobalInvocationID.xy);
    vec2 texCoord = gl_GlobalInvocationID.xy / float(uSize);

    float texelSize = 1.0 / uSize;

    float z0 = texture(uInput, texCoord + vec2(-texelSize, -texelSize)).r;
    float z1 = texture(uInput, texCoord + vec2(0.0, -texelSize)).r;
    float z2 = texture(uInput, texCoord + vec2(texelSize, -texelSize)).r;
    float z3 = texture(uInput, texCoord + vec2(-texelSize, 0.0)).r;
    float z4 = texture(uInput, texCoord + vec2(texelSize, 0.0)).r;
    float z5 = texture(uInput, texCoord + vec2(-texelSize, texelSize)).r;
    float z6 = texture(uInput, texCoord + vec2(0.0, texelSize)).r;
    float z7 = texture(uInput, texCoord + vec2(texelSize, texelSize)).r;

    vec3 normal;
    normal.x = z0 + 2.0 * z1 + z2 - z5 - 2.0 * z6 - z7;
    normal.y = 1.0 / uNormalStrength;
    normal.z = z0 + 2.0 * z3 + z5 - z2 - 2.0 * z4 - z7;

    imageStore(normalmap, x, vec4((normalize(normal) + 1.0) / 2.0, 1.0));
}