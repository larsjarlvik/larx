#version 330
uniform sampler2DArray uTexture;

in vec3 position;
in vec2 texCoord;
in vec3 normal;

out vec3 outputColor;

const vec4 lightPosition = vec4(350, 350, 350, 1);
const vec3 ambientLight = vec3(0.4, 0.4, 0.4);
const vec3 diffuseLight = vec3(0.8, 0.8, 0.8);
const vec3 specularLight = vec3(0.1, 0.1, 0.1);

uniform vec3 uAmbient;
uniform vec3 uDiffuse;
uniform vec3 uSpecular;
uniform float uShininess;

vec3 getTriPlanarTexture(float textureId) {
    vec3 n = normalize(normal);
    vec3 blending = abs(n);
    blending = normalize(max(blending, 0.00001));
    float b = (blending.x + blending.y + blending.z);
    blending /= vec3(b, b, b);

    vec3 xaxis = texture(uTexture, vec3(position.yz, textureId)).rgb;
    vec3 yaxis = texture(uTexture, vec3(position.xz, textureId)).rgb;
    vec3 zaxis = texture(uTexture, vec3(position.xy, textureId)).rgb;

    return xaxis * blending.x + yaxis * blending.y + zaxis * blending.z;
}

vec3 blendSlope(float slope, float start, float stop, vec3 texture1, vec3 texture2, vec3 textureDefault) {
    if (slope < start || slope > stop) return textureDefault;

    float slopeBlend = (slope - start) * (1.0 / (stop - start));
    return mix(texture1, texture2, slopeBlend);
}

void main() {
    vec3 n = normalize(normal);
    vec3 s = normalize(vec3(lightPosition) - position);
    vec3 v = normalize(vec3(-position));
    vec3 r = reflect(-s, n);

    vec3 ambientReflection = ambientLight + uAmbient;
    vec3 diffuseReflection = diffuseLight + uDiffuse;
    vec3 specularReflection = max(dot(s, n), 0.0) + specularLight * pow(max(dot(r,v), 0.0), uShininess);

    vec3 grass = getTriPlanarTexture(0.0);
    vec3 sand = getTriPlanarTexture(1.0);
    vec3 rock = getTriPlanarTexture(2.0);
    
    float slope = 1.0 - n.y;
    vec3 finalColor = grass;
    finalColor = blendSlope(slope, 0.1, 0.2, grass, sand, finalColor);
    finalColor = blendSlope(slope, 0.2, 0.3, sand, rock, finalColor);
    finalColor = blendSlope(slope, 0.3, 1.0, rock, rock, finalColor);

    outputColor = finalColor * uSpecular * (ambientReflection + diffuseReflection * specularReflection);
}
