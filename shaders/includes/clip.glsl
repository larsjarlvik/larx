uniform int uClipPlane;

const int CLIP_BOTTOM = 1;
const int CLIP_TOP = 2;

float clip(vec3 position) {
    if (uClipPlane == CLIP_BOTTOM) {
        return position.y + 0.05;
    }

    if (uClipPlane == CLIP_TOP) {
        return -position.y + 0.05;
    }
}