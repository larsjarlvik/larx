#version 430

layout(vertices = 16) out;

const int AB = 2;
const int BC = 3;
const int CD = 0;
const int DA = 1;

uniform int uTessFactor;
uniform float uTessSlope;
uniform float uTessShift;
uniform vec3 uCameraPosition;

in vec2 vs_texCoord[];
out vec2 tc_texCoord[];

float lodFactor(float dist) {
    return max(0.0, uTessFactor / pow(dist, uTessSlope) + uTessShift);
}

void main() {
    if(gl_InvocationID == 0) {
        vec3 abMid = vec3(gl_in[ 0].gl_Position + gl_in[ 3].gl_Position) / 2.0;
        vec3 bcMid = vec3(gl_in[ 3].gl_Position + gl_in[15].gl_Position) / 2.0;
        vec3 cdMid = vec3(gl_in[15].gl_Position + gl_in[12].gl_Position) / 2.0;
        vec3 daMid = vec3(gl_in[12].gl_Position + gl_in[ 0].gl_Position) / 2.0;

        float distanceAb = distance(abMid, uCameraPosition);
        float distanceBc = distance(bcMid, uCameraPosition);
        float distanceCd = distance(cdMid, uCameraPosition);
        float distanceDa = distance(daMid, uCameraPosition);

        gl_TessLevelOuter[AB] = mix(1, gl_MaxTessGenLevel, lodFactor(distanceAb));
        gl_TessLevelOuter[BC] = mix(1, gl_MaxTessGenLevel, lodFactor(distanceBc));;
        gl_TessLevelOuter[CD] = mix(1, gl_MaxTessGenLevel, lodFactor(distanceCd));;
        gl_TessLevelOuter[DA] = mix(1, gl_MaxTessGenLevel, lodFactor(distanceDa));;

        gl_TessLevelInner[0] = (gl_TessLevelOuter[BC] + gl_TessLevelOuter[DA]) / 4.0;
        gl_TessLevelInner[1] = (gl_TessLevelOuter[AB] + gl_TessLevelOuter[CD]) / 4.0;
    }

    tc_texCoord[gl_InvocationID] = vs_texCoord[gl_InvocationID];
    gl_out[gl_InvocationID].gl_Position = gl_in[gl_InvocationID].gl_Position;
}