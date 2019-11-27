uniform mat4 uShadowMatrix[3];
uniform int uEnableShadows;

vec4[3] getShadowCoords(vec4 position) {
    if (uEnableShadows == 1) {
        return vec4[](
            uShadowMatrix[0] * position,
            uShadowMatrix[1] * position,
            uShadowMatrix[2] * position
        );
    }
}
