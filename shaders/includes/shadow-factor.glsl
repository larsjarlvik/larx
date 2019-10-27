uniform sampler2DShadow uShadowMap;
uniform int uEnableShadows;

in vec4 shadowCoords;

#define TEXTURE_SAMPLES 16
#define BIAS 0.002

vec2 poissonDisk[TEXTURE_SAMPLES] = vec2[](
   vec2(-0.94201624,-0.39906216),
   vec2( 0.94558609,-0.76890725),
   vec2(-0.09418410,-0.92938870),
   vec2( 0.34495938, 0.29387760),
   vec2(-0.91588581, 0.45771432),
   vec2(-0.81544232,-0.87912464),
   vec2(-0.38277543, 0.27676845),
   vec2( 0.97484398, 0.75648379),
   vec2( 0.44323325,-0.97511554),
   vec2( 0.53742981,-0.47373420),
   vec2(-0.26496911,-0.41893023),
   vec2( 0.79197514, 0.19090188),
   vec2(-0.24188840, 0.99706507),
   vec2(-0.81409955, 0.91437590),
   vec2( 0.19984126, 0.78641367),
   vec2( 0.14383161,-0.14100790)
);

float getShadowFactor(float strength) {
    if(shadowCoords.z > 1.0 || uEnableShadows != 1.0) {
        return 1.0;
    }

    float total = 0.0;

    for (int i = 0; i < TEXTURE_SAMPLES; i++){
        float nearestLight = texture(uShadowMap, vec3(shadowCoords.xy + poissonDisk[i] / 1000.0, (shadowCoords.z - BIAS) / shadowCoords.w));
        if(shadowCoords.z - BIAS > nearestLight) {
            total += strength / TEXTURE_SAMPLES;
        }
    }

    float shadowFactor = clamp(min(
        1.0 - abs((shadowCoords.x * 2.0) - 1.0),
        1.0 - abs((shadowCoords.y * 2.0) - 1.0)
    ) * 4.0, 0, 1);

    return 1.0 - (total * shadowFactor);
}