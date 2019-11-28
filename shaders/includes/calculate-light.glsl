struct LightVectors {
    vec3 lightVector;
    vec3 eyeVector;
};

uniform vec3 uLightAmbient;
uniform vec3 uLightDiffuse;
uniform vec3 uLightSpecular;

vec3 calculateLight(LightVectors vectors, vec3 n, float shininess, float ambientDiffuseFactor, float shadowFactor) {
    vec3 diffuse = max(dot(n, normalize(vectors.lightVector)), 0.0) * uLightDiffuse;
    vec3 reflectedLightVector = reflect(-normalize(vectors.lightVector), n);
    float specularFactor = max(dot(reflectedLightVector, normalize(-vectors.eyeVector)), 0.0);
    float specular = pow(specularFactor, shininess);

    vec3 diffuseSpecular = (diffuse * ambientDiffuseFactor) + specular * uLightSpecular;
    diffuseSpecular *= shadowFactor;

    return (uLightAmbient * ambientDiffuseFactor) + diffuseSpecular;
}