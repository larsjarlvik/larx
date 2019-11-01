uniform sampler2DShadow uShadowMap;
uniform int uEnableShadows;

#define TEXTURE_SAMPLES 4
#define BIAS 0.002

vec2 poissonDisk[TEXTURE_SAMPLES] = vec2[](
  vec2( -0.94201624, -0.39906216 ),
  vec2( 0.94558609, -0.76890725 ),
  vec2( -0.094184101, -0.92938870 ),
  vec2( 0.34495938, 0.29387760 )
);

float getShadowFactor(vec4 shadowCoords, float strength) {
    float shadowFactor = clamp(min(
        1.0 - abs((shadowCoords.x * 2.0) - 1.0),
        1.0 - abs((shadowCoords.y * 2.0) - 1.0)
    ) * 4.0, 0, 1);

    if(shadowFactor == 0.0 || uEnableShadows != 1.0) {
        return 1.0;
    }

    float total = 0.0;

    for (int i = 0; i < TEXTURE_SAMPLES; i++){
        float nearestLight = texture(uShadowMap, vec3(shadowCoords.xy + poissonDisk[i] / 1000.0, (shadowCoords.z - BIAS) / shadowCoords.w));
        if(shadowCoords.z - BIAS > nearestLight) {
            total += strength / TEXTURE_SAMPLES;
        }
    }

    return 1.0 - (total * shadowFactor);
}