extern uniform float4x4 Model;
extern uniform float4x4 Proj;
extern uniform float4x4 View;
extern uniform float3 CamPos;
extern uniform float3 Amb;
extern uniform float3 Dif;
extern uniform float3 Spe;
extern uniform float K;

static const int NUM_LIGHTS = 4;

static const float4 Lights[NUM_LIGHTS] = {
    float4(30.0, 65.0, -30.0,  0.1),
    float4(-35.0, 25.0, 0.0,  0.1),
    float4( 0.0, 2.0, 25.0,  0.1),
    float4( 40.0, 2.0, 25.0,  0.1),
};

struct PSOutput {
    float4 color : SV_TARGET;
};

struct VSInput {
    float4 pos      : POSITION0;
    float2 texCoord : TEXCOORD0;
    float4 color    : COLOR;
    float3 normal   : NORMAL0;
};

struct VSOutput {
    float4 screenPos : POSITION0;
    float3 worldPos  : TEXCOORD1;
    float2 texCoord  : TEXCOORD0;
    float3 normal    : TEXCOORD2;
    float4 color     : COLOR;
};

void ps_main(in VSOutput x, out PSOutput r) {
    r.color = float4(Amb, 1.0);

    for (int i = 0; i < NUM_LIGHTS; i++) {
        float3 l = normalize(Lights[i].xyz - x.worldPos);
        float3 n = normalize(x.normal);
        float3 rn = reflect(-l, n);
        float3 v = normalize(CamPos - x.worldPos);
        float3 vl = length(CamPos - x.worldPos);
        float  d = length(Lights[i].xyz - x.worldPos);
        float  j = Lights[i].w;
        float  dif = max(0.0, dot(l, n));
        float  spe = pow(max(0.0, dot(rn, v)), K);

        float3 c = 0.0;

        c += dif*Dif;
        c += spe*Spe;
        c *= j;
        c = sqrt(c);

        r.color += float4(c, 1.0);
    }
}

void vs_main(in VSInput x, out VSOutput r) {
    float4 modelPos = mul(x.pos   , Model);
    float4 viewPos  = mul(modelPos, View );

    r.screenPos = mul(viewPos, Proj);
    r.worldPos  = modelPos.xyz;
    r.texCoord  = x.texCoord;
    r.normal    = mul(float4(x.normal.xyz, 0.0), Model).xyz;
    r.color     = x.color;
}

technique Technique1 {
    pass Pass0 {
        PixelShader  = compile ps_3_0 ps_main();
        VertexShader = compile vs_3_0 vs_main();
    }
}