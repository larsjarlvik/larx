uniform mat4 uShadowMatrix;
uniform int uEnableShadows;

vec4 getShadowCoords(vec4 position) {
    if (uEnableShadows == 1) {
        return uShadowMatrix * position;
    }
}
