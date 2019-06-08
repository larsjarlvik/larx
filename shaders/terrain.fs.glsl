#version 330
precision highp float;

uniform sampler2DArray uTexture;
uniform sampler2DArray uSplatMap;
uniform sampler2D uTextureNoise;

in vec3 position;
in vec2 texCoord;
in vec3 normal;
in vec3 lightVector;
in vec3 normalVector;
in vec3 eyeVector;

out vec3 outputColor;

uniform vec3 uAmbient;
uniform vec3 uDiffuse;
uniform vec3 uSpecular;

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

uniform float uShininess;
uniform int uGridLines;
uniform int uSplatCount;

vec3 getTriPlanarTexture(int textureId) {
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

float gridLine() {
    if (uGridLines == 0) return 0.0;

    vec2 coord = position.xz;
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / fwidth(coord);
    float line = min(grid.x, grid.y);

    return (1.0 - min(line, 1.0)) / 3;
}

vec3 calculateLight(vec3 normalMap, vec3 roughMap) {
    vec3 n = normalize(normalVector * normalMap);
    vec3 ambient = uAmbient * uLightAmbient;
    vec3 diffuse = max(dot(lightVector, n), 0.0) * uDiffuse * uLightDiffuse;
    vec3 halfwayVector = normalize(lightVector + eyeVector);
    vec3 specular = pow(max(dot(n, halfwayVector), 0.0), (roughMap.r * 50.0)) * uSpecular * uLightSpecular;

    return ambient + diffuse + specular;
}

vec3 finalTexture(int index) {
    float n1 = (texture(uTextureNoise, texCoord).r * 0.05) + 0.95;
    float n2 = (texture(uTextureNoise, texCoord / 5).r * 0.1) + 0.90;

    float noise = (n1 + n2) / 2;

    return (getTriPlanarTexture(index * 3) * noise) * calculateLight(getTriPlanarTexture(index * 3 + 1), getTriPlanarTexture(index * 3 + 2));
}

void main() {

    vec3 color = vec3(0);
    for (int i = 0; i < uSplatCount; i++) {
        float intesity = texture(uSplatMap, vec3(texCoord.x, texCoord.y, i)).r;
        if (intesity > 0.0) {
            color += finalTexture(i) * intesity;
        }
    }

    outputColor = mix(color, vec3(0.3, 0.3, 0.3), gridLine());
}
