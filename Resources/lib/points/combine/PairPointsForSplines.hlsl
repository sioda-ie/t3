#include "lib/shared/point.hlsl"

cbuffer Params : register(b0)
{
    float3 TangentDirection;

    float InitWTo01;
    float SegmentCount;

    float TangentA;
    float TangentA_WFactor;
    float TangentB;
    float TangentB_WFactor;

    float Debug;
}

StructuredBuffer<Point> PointsA : t0;        // input
StructuredBuffer<Point> PointsB : t1;        // input
RWStructuredBuffer<Point> ResultPoints : u0; // output

inline float3 Interpolate(float t, float3 pA, float3 tA, float3 tB, float3 pB) 
{
    float t2 = t * t;
    float t3 = t2 * t;

    return (2 * t3 - 3 * t2 + 1) * pA +
                (t3 - 2 * t2 + t) * tA +
                (-2 * t3 + 3 * t2) * pB +
                (t3 - t2) * -tB;
}


[numthreads(64, 1, 1)] void main(uint3 i
                                 : SV_DispatchThreadID)
{
    uint resultCount, countA, countB, stride;
    ResultPoints.GetDimensions(resultCount, stride);
    PointsA.GetDimensions(countA, stride);
    PointsB.GetDimensions(countB, stride);

    int segmentCount = (int)(SegmentCount + 0.5);

    int pointsPerSegment = segmentCount ;

    uint pairIndex = i.x / pointsPerSegment;
    uint indexInLine = i.x % pointsPerSegment;
    float f = (float)indexInLine / (float)(pointsPerSegment - 2);

    if (indexInLine == pointsPerSegment - 1)
    {
        ResultPoints[i.x].w = sqrt(-1); // NaN for divider
        return;
    }

    uint indexA = pairIndex % countA;
    uint indexB = pairIndex % countB;

    float3 pA1 = PointsA[indexA].position;
    float3 pB1 = PointsB[indexB].position;
    float3 forward = TangentDirection;

    float paW = PointsA[indexA].w;
    float pbW = PointsA[indexB].w;
    float3 tA = rotate_vector(forward, PointsA[indexA].rotation) * (TangentA + paW * TangentA_WFactor);
    float3 tB = rotate_vector(forward, PointsB[indexB].rotation) * (TangentB + pbW * TangentB_WFactor);

    float3 pF = Interpolate(f, pA1, tA, tB, pB1);
    ResultPoints[i.x].position = pF;

    float3 pF2 = Interpolate(f+0.001, pA1, tA, tB, pB1);
    float3 forward2= normalize(pF-pF2);
    float3 up = float3(0,0,1);
    float fade = 1-abs(dot(up,forward2));
    float3 refUp = lerp(tA, tB, f);
    ResultPoints[i.x].rotation =  q_look_at(forward2, refUp);
    float w = isnan(paW) || isnan(paW) ? sqrt(-1) : 1;
    ResultPoints[i.x].w = InitWTo01 > 0.5 ? f : w;
}
