#version 330

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;
uniform float uRoughness;

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
    vec3 diffuse = max(dot(lightVector, n), 0.0) * uLightDiffuse;
    vec3 halfwayVector = normalize(lightVector + eyeVector);
    vec3 specular = pow(max(dot(n, halfwayVector), 0.0), uRoughness) * uLightSpecular;

    return uLightAmbient + diffuse + specular;
}

void main() {
    vec4 tex = texture(uBaseColorTexture, texCoord);
    if (tex.a < 0.5) discard;

    outputColor = tex * vec4(calculateLight(), 1.0);
}
