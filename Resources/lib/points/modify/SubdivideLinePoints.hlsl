#include "lib/shared/hash-functions.hlsl"
#include "lib/shared/noise-functions.hlsl"
#include "lib/shared/point.hlsl"

cbuffer Params : register(b0)
{
    float InsertCount;
}

StructuredBuffer<Point> SourcePoints : t0;         // input
RWStructuredBuffer<Point> ResultPoints : u0;    // output


static uint sourceCount;
float3 SamplePosAtF(float f, out float weight) 
{
    float sourceF = saturate(f) * (sourceCount -1);
    int index = (int)sourceF;
    float fraction = sourceF - index;    
    index = clamp(index,0, sourceCount -1);
    weight = lerp(SourcePoints[index].w, SourcePoints[index+1].w, fraction );
    return lerp(SourcePoints[index].position, SourcePoints[index+1].position, fraction );
}

float4 SampleRotationAtF(float f) 
{
    float sourceF = saturate(f) * (sourceCount -1);
    int index = (int)sourceF;
    float fraction = sourceF - index;    
    index = clamp(index,0, sourceCount -1);
    return q_slerp(SourcePoints[index].rotation, SourcePoints[index+1].rotation, fraction );
}




[numthreads(64,1,1)]
void main(uint3 i : SV_DispatchThreadID)
{
    uint pointCount, stride;
    ResultPoints.GetDimensions(pointCount, stride);

    if(i.x >= pointCount) {
        return;
    }

    uint stride2;
    SourcePoints.GetDimensions(sourceCount, stride);

    int subdiv = (int)(InsertCount + 1);

    int segmentIndex = i.x / (subdiv);
    int segmentPointIndex = (i.x % (subdiv));

    float f = (float)segmentPointIndex / subdiv;

    if(f <= 0.001)  {
        ResultPoints[i.x].position = SourcePoints[segmentIndex].position;
        ResultPoints[i.x].w =        SourcePoints[segmentIndex].w;
        ResultPoints[i.x].rotation = SourcePoints[segmentIndex].rotation;   // use qlerp
    }
    else {
        ResultPoints[i.x].position = lerp( SourcePoints[segmentIndex].position,  SourcePoints[segmentIndex + 1].position, f);
        ResultPoints[i.x].w = lerp( SourcePoints[segmentIndex].w,  SourcePoints[segmentIndex + 1].w, f);
        ResultPoints[i.x].rotation = lerp( SourcePoints[segmentIndex].rotation,  SourcePoints[segmentIndex + 1].rotation, f);   // use qlerp

    }
    

}

