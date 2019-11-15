uniform vec3 uCameraPosition;
uniform vec2 uPosition;
uniform float uSize;
uniform int uLodMorphAreas[8];
uniform vec2 uIndex;

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

vec2 morph(mat4 worldMatrix, vec2 position, int lod) {
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

    float distLatitude = length(uCameraPosition - (worldMatrix * vec4(fixPointLatitude.x, 0.0, fixPointLatitude.y, 1.0)).xyz);
    float distLongitude = length(uCameraPosition - (worldMatrix * vec4(fixPointLongitude.x, 0.0, fixPointLongitude.y, 1.0)).xyz);

    return vec2(
        distLatitude > uLodMorphAreas[lod - 1] ? morphLatitude(position * uSize) : 0.0,
        distLongitude > uLodMorphAreas[lod - 1] ? morphLongitude(position * uSize) : 0.0
    );
}