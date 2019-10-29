uniform int uClipPlane;

const int CLIP_BOTTOM = 1;
const int CLIP_TOP = 2;

void clip(vec3 position) {
    if (uClipPlane == CLIP_BOTTOM) {
        gl_ClipDistance[0] = position.y + 0.05;
    } else if (uClipPlane == CLIP_TOP) {
        gl_ClipDistance[0] = -position.y + 0.05;
    }
}