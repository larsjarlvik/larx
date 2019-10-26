uniform mat4 uShadowMatrix;
uniform int uEnableShadows;

out vec4 shadowCoords;

void setShadowCoords(vec4 position) {
    if (uEnableShadows == 1) {
        shadowCoords = uShadowMatrix * position;
    }
}
