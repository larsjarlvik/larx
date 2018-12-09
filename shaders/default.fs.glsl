#version 330

in vec3 position;
in vec3 color;
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

void main() {
    vec3 n = normalize(normal);
    vec3 s = normalize(vec3(lightPosition) - position);
    vec3 v = normalize(vec3(-position));
    vec3 r = reflect(-s, n);

    vec3 ambientReflection = ambientLight + uAmbient;
    vec3 diffuseReflection = diffuseLight + uDiffuse;
    vec3 specularReflection = max(dot(s, n), 0.0) + specularLight * pow(max(dot(r,v), 0.0), uShininess);

    outputColor = color * uSpecular * (ambientReflection + diffuseReflection * specularReflection);
}
