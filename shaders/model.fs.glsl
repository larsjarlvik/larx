#version 330

// TODO: Move to uniforms
const vec3 uAmbient = vec3(0.3, 0.3, 0.3);
const vec3 uDiffuse = vec3(0.6, 0.6, 0.6);
const vec3 uSpecular = vec3(0.8, 0.8, 0.8);

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

uniform sampler2D uBaseColorTexture;
uniform sampler2D uNormalTexture;

in vec3 lightVector;
in vec3 normalVector;
in vec3 eyeVector;
in vec2 texCoord;

out vec4 outputColor;

vec3 calculateLight() {
    vec3 normal = texture(uNormalTexture, texCoord).rgb * 2.0 - 1.0;

    vec3 n = normalize(normalVector + normal);
    vec3 ambient = uAmbient * uLightAmbient;
    vec3 diffuse = max(dot(lightVector, n), 0.0) * uDiffuse * uLightDiffuse;
    vec3 halfwayVector = normalize(lightVector + eyeVector);
    vec3 specular = pow(max(dot(n, halfwayVector), 0.0), 0.4 * 3.0) * uSpecular * uLightSpecular;

    return ambient + diffuse + specular;
}

void main() {
    outputColor = texture(uBaseColorTexture, texCoord) * vec4(calculateLight(), 1.0);
}
