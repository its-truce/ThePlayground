sampler2D uTexture : register(s0);
float uTime;
float uLerpCoff;
// Shader from: https://www.shadertoy.com/view/MtcGD7

float3 rgb2hsv(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float rand(float2 n) {
    return frac(sin(cos(dot(n, float2(12.9898, 12.1414)))) * 83758.5453);
}

float noise(float2 n) {
    const float2 d = float2(0.0, 1.0);
    float2 b = floor(n), f = smoothstep(float2(0.0, 0.0), float2(1.0, 1.0), frac(n));
    return lerp(lerp(rand(b), rand(b + d.yx), f.x), lerp(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

float fbm(float2 n) {
    float total = 0.0, amplitude = 1.0;
    for (int i = 0; i < 5; i++) {
        total += noise(n) * amplitude;
        n += n * 1.7;
        amplitude *= 0.47;
    }
    return total;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    const float3 c1 = float3(0.5, 0.0, 0.1);
    const float3 c2 = float3(0.9, 0.1, 0.0);
    const float3 c3 = float3(0.2, 0.1, 0.7);
    const float3 c4 = float3(1.0, 0.9, 0.1);
    const float3 c5 = float3(0.1, 0.1, 0.1);
    const float3 c6 = float3(0.9, 0.9, 0.9);

    float2 speed = float2(1.2, 0.1);
    float shift = 1.327 + sin(uTime * 2.0) / 2.4;
    float alpha = 1.0;

    // Change the constant term for all kinds of cool distance versions,
    // make plus/minus to switch between 
    // ground fire and fire rain!
    float dist = 3.5 - sin(uTime * 0.4) / 1.89;

    float2 p = uv;

    p.x -= uTime / 1.1;
    float q = fbm(p - uTime * 0.01 + 1.0 * sin(uTime) / 10.0);
    float qb = fbm(p - uTime * 0.002 + 0.1 * cos(uTime) / 5.0);
    float q2 = fbm(p - uTime * 0.44 - 5.0 * cos(uTime) / 7.0) - 6.0;
    float q3 = fbm(p - uTime * 0.9 - 10.0 * cos(uTime) / 30.0) - 4.0;
    float q4 = fbm(p - uTime * 2.0 - 20.0 * sin(uTime) / 20.0) + 2.0;
    q = (q + qb - 0.4 * q2 - 2.0 * q3 + 0.6 * q4) / 3.8;
    float2 r = float2(fbm(p + q / 2.0 + uTime * speed.x - p.x - p.y), fbm(p + p - uTime * speed.y));
    float3 c = lerp(c1, c2, fbm(p + r)) + lerp(c3, c4, r.x) - lerp(c5, c6, r.y);
    float3 color = c * cos(shift * p.y);
    color += float3(0.05, 0.05, 0.05);
    color.r *= 0.8;
    float3 hsv = rgb2hsv(color);
    hsv.y *= hsv.z * 1.1;
    hsv.z *= hsv.y * 1.13;
    hsv.y = (2.2 - hsv.z * 0.9) * 1.20;
    color = hsv2rgb(hsv);
    
    float4 spriteColor = tex2D(uTexture, uv);
    float4 finalColor = float4(color, alpha);
    
    if (any(spriteColor.rgb))
        return lerp(finalColor, spriteColor, uLerpCoff);
    else
        return spriteColor;
}

technique FireTechnique 
{
    pass FirePass 
    {
        PixelShader = compile ps_3_0 main();
    }
}