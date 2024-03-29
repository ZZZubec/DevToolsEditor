#include "Uniforms.hlsl"
#include "Samplers.hlsl"
#include "Transform.hlsl"
#include "Lighting.hlsl"
#include "ScreenPos.hlsl"
#include "Fog.hlsl"

varying highp vec2 vScreenPos;

#ifdef COMPILEVS

void VS()
{
    mat4 modelMatrix = iModelMatrix;
    vec3 worldPos = GetWorldPos(modelMatrix);
    gl_Position = GetClipPos(worldPos);
    vScreenPos = GetScreenPosPreDiv(gl_Position);
}

#endif


#ifdef COMPILEPS
uniform highp vec2 cScreenSize;

// Port from: https://github.com/jsj2008/Zombie-Blobs/blob/278e16229ccb77b2e11d788082b2ccebb9722ace/src/postproc.fs

// see T M?ller, 1999: Efficiently building a matrix to rotate one vector to another
mat3 rotateNormalVecToAnother(vec3 f, vec3 t) {
    vec3 v = cross(f, t);
    float c = dot(f, t);
    float h = (1.0 - c) / (1.0 - c * c);
    return mat3(c + h * v.x * v.x, h * v.x * v.y + v.z, h * v.x * v.z - v.y,
                h * v.x * v.y - v.z, c + h * v.y * v.y, h * v.y * v.z + v.x,
                h * v.x * v.z + v.y, h * v.y * v.z - v.x, c + h * v.z * v.z);
}

vec3 normal_from_depth(float depth, highp vec2 texcoords) {
    // One pixel: 0.001 = 1 / 1000 (pixels)
    const vec2 offset1 = vec2(0.0, 0.001);
    const vec2 offset2 = vec2(0.001, 0.0);
    
    float depth1 = DecodeDepth(texture2D(sEmissiveMap, texcoords + offset1).rgb);
    float depth2 = DecodeDepth(texture2D(sEmissiveMap, texcoords + offset2).rgb);
    
    vec3 p1 = vec3(offset1, depth1 - depth);
    vec3 p2 = vec3(offset2, depth2 - depth);
    
    highp vec3 normal = cross(p1, p2);
    normal.z = -normal.z;
    
    return normalize(normal);
}

void PS()
{
    const float aoStrength = 1.0;
    
    highp vec2 tx = vScreenPos;
    highp vec2 px = vec2(1.0 / cScreenSize.x, 1.0 / cScreenSize.y);
    
    float depth = DecodeDepth(texture2D(sEmissiveMap, vScreenPos).rgb);
    vec3  normal = normal_from_depth(depth, vScreenPos);
    
    // radius is in world space unit
    const float radius = 1.0;
    float zRange = radius / (cFarClipPS - cNearClipPS);
    
    // calculate inverse matrix of the normal by rotate it to identity
    mat3 InverseNormalMatrix = rotateNormalVecToAnother(normal, vec3(0.0, 0.0, 1.0));
    
    // result of line sampling
    // See Loos & Sloan: Volumetric Obscurance
    // http://www.cs.utah.edu/~loos/publications/vo/vo.pdf
    float hemi = 0.0;
    float maxi = 0.0;
    
    for (int x = -2; x <= 2; ++x) {
        for (int y = -2; y <= 2; ++y) {
            // make virtual sphere of unit volume, more closer to center, more ambient occlusion contributions
            float rx = 0.3 * float(x);
            float ry = 0.3 * float(y);
            float rz = sqrt(1.0 - rx * rx - ry * ry);
            
            highp vec3 screenCoord = vec3(float(x) * px.x, float(y) * px.y, 0.0);
            // 0.25 times smaller when farest, 5.0 times bigger when nearest.
            highp vec2 coord = tx + (5.0 - 4.75 * depth) * screenCoord.xy;
            // fetch depth from texture
            screenCoord.z = DecodeDepth(texture2D(sEmissiveMap, coord).rgb);
            // move to origin
            screenCoord.z -= depth;

            // ignore occluders which are too far away
            if (screenCoord.z < -zRange) continue;

            // Transform to normal-oriented hemisphere space
            highp vec3 localCoord = InverseNormalMatrix * screenCoord;
            // ralative depth in the world space radius
            float dr = localCoord.z / zRange;
            // calculate contribution
            float v = clamp(rz + dr * aoStrength, 0.0, 2.0 * rz);

            maxi += rz;
            hemi += v;
        }
    }

    float ao = clamp(hemi / maxi, 0.0, 1.0);

    gl_FragColor = vec4(texture2D(sDiffMap, vScreenPos).rgb * ao, 1.0);
}

#endif
