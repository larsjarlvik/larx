#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uLightPosition;

out vec3 position;
out vec2 texCoord;
out vec3 normal;
out vec3 lightVector;
out vec3 normalVector;
out vec3 eyeVector;

void main()
{
    position = vPosition;
    texCoord = vTexCoord;
    normal = vNormal;

    vec3 worldPosition = vec3(uViewMatrix * vec4(vPosition, 1.0));

    lightVector = normalize(uViewMatrix * vec4(uLightPosition - worldPosition, 1.0)).xyz;
    normalVector = normalize((uViewMatrix * vec4(normal, 1.0)).xyz);
    eyeVector = -normalize(worldPosition);

    gl_Position = uProjectionMatrix * vec4(worldPosition, 1.0);
}
