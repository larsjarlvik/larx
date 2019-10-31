uniform vec3 uLightDirection;
uniform vec3 uCameraPosition;

struct LightVectors {
    vec3 lightVector;
    vec3 eyeVector;
};

LightVectors calculateLightVectors(vec3 normal, vec3 tangent, vec3 position, mat3 transform) {
    vec3 biTangent = normalize(cross(normal, tangent));
    mat3 tangentSpace = mat3(
        tangent.x, biTangent.x, normal.x,
        tangent.y, biTangent.y, normal.y,
        tangent.z, biTangent.z, normal.z
    ) * transform;

    return LightVectors(
        tangentSpace * -uLightDirection,
        tangentSpace * -(uCameraPosition - position)
    );
}
