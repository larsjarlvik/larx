uniform vec3 uLightDirection;
uniform vec3 uCameraPosition;

vec3[2] calculateLightVectors(vec3 normal, vec3 tangent, vec3 position, mat3 transform) {
    vec3 biTangent = normalize(cross(normal, tangent));
    mat3 tangentSpace = mat3(
        tangent.x, biTangent.x, normal.x,
        tangent.y, biTangent.y, normal.y,
        tangent.z, biTangent.z, normal.z
    ) * transform;

    return vec3[2](
        tangentSpace * -uLightDirection,
        tangentSpace * -(uCameraPosition - position)
    );
}
