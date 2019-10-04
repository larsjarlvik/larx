#version 330
precision highp float;

uniform sampler2DArray uTexture;
uniform sampler2DArray uSplatMap;
uniform sampler2D uTextureNoise;

in vec3 position;
in vec2 texCoord;
in vec3 normal;
in vec3 lightVector;
in vec3 eyeVector;

out vec4 outputColor;

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

uniform int uGridLines;
uniform int uSplatCount;
uniform int uShowOverlays;

uniform vec3 uMousePosition;
uniform float uSelectionSize;

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

vec3 calculateLight(int normalMap, int roughnessMap) {
    vec3 n = texture(uTexture, getTriPlanarTexture(normalMap)).rgb * 2.0 - 1.0;
    float r = texture(uTexture, getTriPlanarTexture(roughnessMap)).r;

    vec3 diffuse = max(dot(n, normalize(lightVector)), 0.0) * uLightDiffuse;

    vec3 reflectedLightVector = reflect(-normalize(lightVector), n);
    float specularFactor = max(dot(reflectedLightVector, normalize(-eyeVector)), 0.0);
    vec3 specular = pow(specularFactor, r * 5.0) * uLightSpecular;

    return uLightAmbient + diffuse + specular;
}

vec3 finalTexture(int index) {
    float n1 = (texture(uTextureNoise, texCoord / 0.3).r * 0.05) + 0.95;
    float n2 = (texture(uTextureNoise, texCoord / 4.5).r * 0.1) + 0.90;

    float noise = (n1 + n2) / 2;

    return getTriPlanarTexture(index * 3) * calculateLight(index * 3 + 1, index * 3 + 2) * noise;
}

float circle() {
    float radius = uSelectionSize;
    float border = 0.08;
    float dist = distance(uMousePosition.xz, position.xz);

    return 1.0 + smoothstep(radius, radius + border, dist)
               - smoothstep(radius - border, radius, dist);
}

void main() {
    vec3 color = vec3(0);
    for (int i = 0; i < uSplatCount; i++) {
        float intesity = texture(uSplatMap, vec3(texCoord.x, texCoord.y, i)).r;
        if (intesity > 0.0) {
            color += finalTexture(i) * intesity;
        }
    }

    if (uShowOverlays == 0) {
        outputColor = vec4(color, 0);
        return;
    }

    vec3 terrainGridLines = mix(color, vec3(0.3, 0.3, 0.3), gridLine());
    outputColor = vec4(mix(vec3(1.0, 1.0, 1.0), terrainGridLines, circle()), 1.0);
}
