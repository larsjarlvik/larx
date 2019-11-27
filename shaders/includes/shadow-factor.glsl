uniform sampler2DShadow uShadowMap[3];
uniform int uEnableShadows;

#define TEXTURE_SAMPLES 16
#define BIAS 0.003

vec2 poissonDisk[TEXTURE_SAMPLES] = vec2[](
   vec2( -0.94201624, -0.39906216 ),
   vec2( 0.94558609, -0.76890725 ),
   vec2( -0.094184101, -0.92938870 ),
   vec2( 0.34495938, 0.29387760 ),
   vec2( -0.91588581, 0.45771432 ),
   vec2( -0.81544232, -0.87912464 ),
   vec2( -0.38277543, 0.27676845 ),
   vec2( 0.97484398, 0.75648379 ),
   vec2( 0.44323325, -0.97511554 ),
   vec2( 0.53742981, -0.47373420 ),
   vec2( -0.26496911, -0.41893023 ),
   vec2( 0.79197514, 0.19090188 ),
   vec2( -0.24188840, 0.99706507 ),
   vec2( -0.81409955, 0.91437590 ),
   vec2( 0.19984126, 0.78641367 ),
   vec2( 0.14383161, -0.14100790 )
);

float getShadowFactor(vec4[3] shadowCoords, float distance, float strength) {
    float shadowFactor = clamp(min(
        1.0 - abs((shadowCoords[2].x * 2.0) - 1.0),
        1.0 - abs((shadowCoords[2].y * 2.0) - 1.0)
    ) * 4.0, 0, 1);

    if(shadowFactor == 0.0 || uEnableShadows != 1.0) {
        return 1.0;
    }

    float total = 0.0;

    for (int i = 0; i < TEXTURE_SAMPLES; i++) {
        if (distance < 100) {
            float nearestLight = texture(uShadowMap[0], vec3(shadowCoords[0].xy + poissonDisk[i] / 3000.0, (shadowCoords[0].z - BIAS) / shadowCoords[0].w));
            if(shadowCoords[0].z - BIAS > nearestLight) {
                total += strength / TEXTURE_SAMPLES;
            }
        } else if (distance < 300) {
            float nearestLight = texture(uShadowMap[1], vec3(shadowCoords[1].xy + poissonDisk[i] / 2000.0, (shadowCoords[1].z - BIAS) / shadowCoords[1].w));
            if(shadowCoords[1].z - BIAS > nearestLight) {
                total += strength / TEXTURE_SAMPLES;
            }
        } else {
            float nearestLight = texture(uShadowMap[2], vec3(shadowCoords[2].xy + poissonDisk[i] / 1000.0, (shadowCoords[2].z - BIAS) / shadowCoords[2].w));
            if(shadowCoords[2].z - BIAS > nearestLight) {
                total += strength / TEXTURE_SAMPLES;
            }
        }
    }

    return 1.0 - (total * shadowFactor);
}