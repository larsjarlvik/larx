#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uCameraPosition;
uniform vec3 uLightDirection;

out vec4 clipSpace;
out vec3 lightVector;
out vec3 eyeVector;
out vec2 texCoord;
out vec3 position;

void main() {
    vec4 worldPosition = uViewMatrix * vec4(vPosition, 1.0);

    texCoord = vTexCoord;
    clipSpace = uProjectionMatrix * worldPosition;
    position = vPosition;

    vec3 normal = normalize(vec3(0, 1, 0));
    vec3 tangent = normalize((uViewMatrix * vec4(1, 0, 0, 0)).xyz);
    vec3 biTangent = normalize(cross(normal, tangent));
    mat3 toTangentSpace = mat3(
        tangent.x, biTangent.x, normal.x,
        tangent.y, biTangent.y, normal.y,
        tangent.z, biTangent.z, normal.z
    );

    lightVector = toTangentSpace * -uLightDirection;
    eyeVector = toTangentSpace * -(uCameraPosition - position);

    gl_Position = clipSpace;
}
