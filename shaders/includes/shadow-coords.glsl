uniform mat4 uShadowMatrix[3];
uniform int uEnableShadows;

vec4 getShadowCoords(vec4 position) {
    if (uEnableShadows == 1) {
        return uShadowMatrix[0] * position;
    }
}
