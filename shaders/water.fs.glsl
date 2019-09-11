#version 330

in vec3 lightVector;

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

out vec3 outputColor;

const vec3 uAmbient = vec3(0.6, 0.6, 0.6);
const vec3 uDiffuse = vec3(0.9, 0.9, 0.9);

vec3 calculateLight() {
    vec3 n = normalize(vec3(0, 1, 0));
    vec3 ambient = uAmbient * uLightAmbient;
    vec3 diffuse = max(dot(lightVector, n), 0.0) * uDiffuse * uLightDiffuse;

    return ambient + diffuse;
}

void main() {
    outputColor = vec3(0.61, 0.90, 0.79) * calculateLight();
}
