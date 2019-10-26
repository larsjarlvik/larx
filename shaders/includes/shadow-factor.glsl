uniform sampler2DShadow uShadowMap;

in vec4 shadowCoords;

uniform int uEnableShadows;

const float PCF_COUNT = 2.0;
const float PCF_SAMLE_SIZE = 1.0;

float getShadowFactor(float strength) {
    if(shadowCoords.z > 1.0 || uEnableShadows != 1.0) {
        return 1.0;
    }

    float totalTexels = (PCF_COUNT * 2.0 + PCF_SAMLE_SIZE) * (PCF_COUNT * 2.0 + PCF_SAMLE_SIZE);
    float texelSize = 1.0 / 4096.0;
    float total = 0.0;

    for(float x = -PCF_COUNT; x <= PCF_COUNT; x += PCF_SAMLE_SIZE) {
        for(float y = -PCF_COUNT; y <= PCF_COUNT; y += PCF_SAMLE_SIZE) {
            float nearestLight = texture(uShadowMap, vec3(shadowCoords.xy + vec2(x, y) * texelSize, shadowCoords.z));
            if(shadowCoords.z - 0.001 > nearestLight) {
                total += strength;
            }
        }
    }

    total /= totalTexels;
    return 1.0 - (total * shadowCoords.w);
}