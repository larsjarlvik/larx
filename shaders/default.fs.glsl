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

void main() {
    vec3 n = normalize(normal);
    vec3 s = normalize(vec3(lightPosition) - position);
    vec3 v = normalize(vec3(-position));
    vec3 r = reflect(-s, n);

    vec3 ambientReflection = ambientLight + uAmbient;
    vec3 diffuseReflection = diffuseLight + uDiffuse;
    vec3 specularReflection = max(dot(s, n), 0.0) + specularLight * pow(max(dot(r,v), 0.0), uShininess);
    
    float slope = 1.0 - n.y;
    float slopeBlend;

    vec3 grass = texture(uTexture, vec3(texCoord.xy, 0)).xyz;
    vec3 sand = texture(uTexture, vec3(texCoord.xy, 1)).xyz;
    vec3 rock = texture(uTexture, vec3(texCoord.xy, 2)).xyz;

    vec3 finalColor;

    if(slope < 0.1) {
        slopeBlend = slope / 0.1;
        finalColor = mix(grass, sand, slopeBlend);
    } else if(slope >= 0.1 && slope < 0.25) {
        slopeBlend = (slope - 0.1) * (1.0 / (0.25 - 0.1));
        finalColor = mix(sand, rock, slopeBlend);
    } else if(slope >= 0.25) {
        finalColor = rock;
    }

    outputColor = finalColor * uSpecular * (ambientReflection + diffuseReflection * specularReflection);
}
