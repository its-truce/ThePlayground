#define NUM_OCTAVES 5

sampler2D uTexture : register(s0);
float uTime;
float uLerpCoff;

// Shader from: https://www.shadertoy.com/view/sdlyz8

float random(in float2 _uv) 
{
    return frac(sin(dot(_uv.xy, float2(12.9898, 78.233))) * 43758.5453123);
}

float noise(in float2 _uv) 
{
    float2 i = floor(_uv);
    float2 f = frac(_uv);

    // Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + float2(1.0, 0.0));
    float c = random(i + float2(0.0, 1.0));
    float d = random(i + float2(1.0, 1.0));

    float2 u = f * f * (3.0 - 2.0 * f);

    return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
}

float fbm(in float2 _uv) 
{
    float v = 0.0;
    float a = 0.5;
    float2 shift = float2(100.0, 100.0);
    // Rotate to reduce axial bias
    float s = sin(0.5);
    float c = cos(0.5);
    float2x2 rot = float2x2(c, s, -s, c);
    for (int i = 0; i < NUM_OCTAVES; ++i) {
        v += a * noise(_uv);
        _uv = mul(rot, _uv) * 2.0 + shift; // Changed rot * _uv to mul(rot, _uv)
        a *= 0.5;
    }
    return v;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float3 color = float3(0.0, 0.0, 0.0);

    float2 q = float2(0.0, 0.0);
    q.x = fbm(uv + 0.00 * uTime);
    q.y = fbm(uv + float2(1.0, 0.0));

    float2 r = float2(0.0, 0.0);
    r.x = fbm(uv + 1.0 * q + float2(1.7, 9.2) + 0.15 * uTime);
    r.y = fbm(uv + 1.0 * q + float2(8.3, 2.8) + 0.126 * uTime);

    float f = fbm(uv + r);

    color = lerp(float3(1.0, 0.0, 0.0), float3(0.666667, 0.666667, 0.498039), clamp((f*f)*4.0, 0.0, 1.0));
    color = lerp(color, float3(1.0, 0.4, 0.164706), clamp(length(q * 2.0), 0.0, 1.0));
    color = lerp(color, float3(1.0, 1.0, 1.0), clamp(length(r.x * 0.5), 0.0, 1.0));

    float4 spriteColor = tex2D(uTexture, uv);
    float4 finalColor = float4((f * f * f + 0.6 * f * f + 0.5 * f) * color, 1.0);
    
    if (any(spriteColor.rgb))
        return float4(lerp(finalColor, spriteColor.rgb, uLerpCoff), 1.0);
    else
        return spriteColor;
}

technique SmokeTechnique 
{
    pass SmokePass 
    {
        PixelShader = compile ps_3_0 main();
    }
}
