#version 330

layout(location = 0) in vec3 vPosition;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

uniform vec3 uLightPosition;

out vec4 clipSpace;
out vec3 lightVector;

void main() {
    vec4 worldPosition = (uViewMatrix * vec4(vPosition, 1.0));

    lightVector = normalize(uViewMatrix * vec4(uLightPosition, 1.0) - worldPosition).xyz;

    clipSpace = uProjectionMatrix * worldPosition;
    gl_Position = clipSpace;
}
