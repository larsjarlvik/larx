uniform mat4 uShadowMatrix;
uniform float uShadowDistance;
uniform int uEnableShadows;

out vec4 shadowCoords;

void setShadowCoords(vec4 position) {
    if (uEnableShadows == 1) {
        float fade = (length(gl_Position) - uShadowDistance - 10.0) / 10.0;
        shadowCoords = uShadowMatrix * position;
        shadowCoords.w = clamp(1.0 - fade, 0.0, 1.0);
    }
}
