#include "lib/shared/hash-functions.hlsl"
#include "lib/shared/noise-functions.hlsl"
#include "lib/shared/point.hlsl"

cbuffer Params : register(b0)
{
    float2 AmountXY;
    float Amount;

    float Confinment;
    float DepthConcentration;
    float CenterDepth;
}

cbuffer Transforms : register(b1)
{
    float4x4 CameraToClipSpace;
    float4x4 ClipSpaceToCamera;
    float4x4 WorldToCamera;
    float4x4 CameraToWorld;
    float4x4 WorldToClipSpace;
    float4x4 ClipSpaceToWorld;
    float4x4 ObjectToWorld;
    float4x4 WorldToObject;
    float4x4 ObjectToCamera;
    float4x4 ObjectToClipSpace;
};

sampler texSampler : register(s0);

Texture2D<float4> FxTexture : register(t0);

RWStructuredBuffer<Particle> Particles : u0; 

[numthreads(64,1,1)]
void main(uint3 i : SV_DispatchThreadID)
{
    uint maxParticleCount, _;
    Particles.GetDimensions(maxParticleCount, _);

    if(i.x >= maxParticleCount)
        return;

    float4 posInObject = float4(Particles[i.x].p.position,1);
    float4 posInCamera = mul(posInObject, ObjectToCamera);
    float4 pos = mul(float4(posInCamera.xyz, 1), CameraToClipSpace);
    float depth = pos.z;
    pos.xyz /= pos.w;
    
    float4 normalMap = FxTexture.SampleLevel(texSampler, (pos.xy * float2(1, -1)  + 1) / 2, 0);
    float3 offset = normalMap.rgb * float3(1,-1,1) * Amount * float3( AmountXY,0) * normalMap.a;
    
    // Confine particles outside of view
    float2 dXYFromCenter = abs(pos.xy);
    float distanceFromCenter = max(dXYFromCenter.x, dXYFromCenter.y);
    float confineFactor= smoothstep(0.9,1.2, distanceFromCenter);
    if(distanceFromCenter > 0.01) {
        offset.xy -= normalize(pos.xy) * confineFactor * Confinment;
    }

    float accelerationToDepthCenter = depth - CenterDepth;
    offset.z += accelerationToDepthCenter * DepthConcentration;
    offset = mul(float4(offset.xyz, 0), CameraToWorld);

    Particles[i.x].velocity += offset;
}
