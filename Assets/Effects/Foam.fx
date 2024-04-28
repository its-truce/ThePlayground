#define TAU 6.28318530718
#define TILING_FACTOR 1.0
#define MAX_ITER 20

sampler2D uTexture : register(s0);
float uTime;
float uLerpCoff;
// Shader from: https://www.shadertoy.com/view/llcXW7

float waterHighlight(float2 p, float time, float foaminess)
{
    float2 i = p;
    float c = 0.0;
    float foaminess_factor = lerp(1.0, 6.0, foaminess);
    float inten = 0.0075 * foaminess_factor;

    for (int n = 0; n < MAX_ITER; n++) 
    {
        float t = time * (1.0 - (3.5 / float(n+1)));
        i = p + float2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
        c += 1.0 / length(float2(p.x / (sin(i.x+t)), p.y / (cos(i.y+t))));
    }
    
    c = 0.2 + c / (inten * float(MAX_ITER));
    c = 1.17 - pow(abs(c), 1.4);
    c = pow(abs(c), 8.0);
    return c / sqrt(foaminess_factor);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float time = uTime * 0.1 + 23.0;
    float dist_center = pow(2.0 * length(uv - 0.5), 2.0);
    
    float foaminess = smoothstep(0.0, 1.8, dist_center);
    float clearness = 1.0;
    
    float2 p = fmod(uv * TAU * TILING_FACTOR, TAU) - 250.0;
    
    float c = waterHighlight(p, time, foaminess);
    
    float3 water_color = float3(0.0, 0.2, 0.284);
    float3 color = float3(c, c, c);
    color = clamp(color + water_color, 0.0, 1.0);
    
    color = lerp(water_color, color, clearness);
    float4 spriteColor = tex2D(uTexture, uv);
    
    if (any(spriteColor.rgb))
        return float4(lerp(color, spriteColor.rgb, uLerpCoff), 1.0);
    else
        return spriteColor;
}


technique FoamTechnique 
{
    pass FoamPass 
    {
        PixelShader = compile ps_3_0 main();
    }
}