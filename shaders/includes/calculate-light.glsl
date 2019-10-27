in vec3 lightVector;
in vec3 eyeVector;

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

vec3 calculateLight(vec3 n, float shininess, float ambientDiffuseFactor) {
    vec3 diffuse = max(dot(n, normalize(lightVector)), 0.0) * uLightDiffuse;
    vec3 reflectedLightVector = reflect(-normalize(lightVector), n);
    float specularFactor = max(dot(reflectedLightVector, normalize(-eyeVector)), 0.0);
    float specular = pow(specularFactor, shininess);

    return ((uLightAmbient + diffuse) * ambientDiffuseFactor) + specular * uLightSpecular;
}