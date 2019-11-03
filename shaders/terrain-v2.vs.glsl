#version 430

layout (location = 0) in vec2 vPosition;

uniform vec3 uCameraPosition;
uniform int uLod;
uniform vec2 uIndex;
uniform float uSize;
uniform float uScale;
uniform vec2 uPosition;
uniform int uLodMorphAreas[8];

uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;
uniform mat4 uLocalMatrix;
uniform mat4 uWorldMatrix;

out vec2 vs_texCoord;

float morphLatitude(vec2 position) {
    if (uIndex == vec2(0, 0)) {
        float morph = position.x - position.y;
        if (morph > 0.0) return -morph;
    }
    if (uIndex == vec2(0, 1)) {
        float morph = position.x + position.y - uSize;
        if (morph > 0.0) return morph;
    }
    if (uIndex == vec2(1, 0)) {
        float morph = uSize - position.x - position.y;
        if (morph > 0.0) return -morph;
    }
    if (uIndex == vec2(1, 1)) {
        float morph = position.y - position.x;
        if (morph > 0.0) return morph;
    }
    return 0.0;
}

float morphLongitude(vec2 position) {
    if (uIndex == vec2(0, 0)) {
        float morph = position.y - position.x;
        if (morph > 0.0) return morph;
    }
    if (uIndex == vec2(0, 1)) {
        float morph = uSize - position.y - position.x;
        if (morph > 0.0) return morph;
    }
    if (uIndex == vec2(1, 0)) {
        float morph = position.y - (uSize - position.x);
        if (morph > 0.0) return -morph;
    }
    if (uIndex == vec2(1, 1)) {
        float morph = position.x - position.y;
        if (morph > 0.0) return -morph;
    }
    return 0.0;
}

vec2 morph() {
    vec2 fixPointLatitude = vec2(0.0, 0.0);
    vec2 fixPointLongitude = vec2(0.0, 0.0);

    if (uIndex == vec2(0, 0)) {
        fixPointLatitude = uPosition + vec2(uSize, 0.0);
        fixPointLongitude = uPosition + vec2(0.0, uSize);
    } else if (uIndex == vec2(0, 1)) {
        fixPointLatitude = uPosition + vec2(uSize, uSize);
        fixPointLongitude = uPosition;
    } else if (uIndex == vec2(1, 0)) {
        fixPointLatitude = uPosition;
        fixPointLongitude = uPosition + vec2(uSize, uSize);
    } else if (uIndex == vec2(1, 1)) {
        fixPointLatitude = uPosition + vec2(0.0, uSize);
        fixPointLongitude = uPosition + vec2(uSize, 0.0);
    }

    float distLatitude = length(uCameraPosition - (uWorldMatrix * vec4(fixPointLatitude.x, 0.0, fixPointLatitude.y, 1.0)).xyz);
    float distLongitude = length(uCameraPosition - (uWorldMatrix * vec4(fixPointLongitude.x, 0.0, fixPointLongitude.y, 1.0)).xyz);

    return vec2(
        distLatitude > uLodMorphAreas[uLod - 1] ? morphLatitude(vPosition * uSize) : 0.0,
        distLongitude > uLodMorphAreas[uLod - 1] ? morphLongitude(vPosition * uSize) : 0.0
    );
}

void main() {
    vec2 position = (uLocalMatrix * vec4(vPosition.x, 0, vPosition.y, 1)).xz;
    if (uLod > 0) position += morph();

    vec4 worldPosition = uWorldMatrix * vec4(position.x, 0.0, position.y, 1.0);

    vs_texCoord = position;
    gl_Position = worldPosition;
}