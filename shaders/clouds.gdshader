shader_type canvas_item;
#include "util/voronoi.gdshaderinc"

uniform vec3  sky_color   : source_color;
uniform vec3  cloud_color : source_color;
uniform float grid_scale;
uniform float offset;
uniform uint  breadth;
uniform bool  wrap;
uniform float k;

float cs(vec2 p, vec2 c, float r) { return length(p - c) - r; }
float su(float a, float b) { float h = clamp(0.5 + 0.5*(b - a)/k, 0., 1.); return mix(b, a, h) - k*h*(1. - h); }

float cloudD(vec2 uv) {
    float db;
    VoronoiScanEntry2D e = voronoi_2d(uv, grid_scale, offset, breadth, wrap, db);
    vec2 cu = uv * grid_scale;
    vec2 p  = fract(cu);
    ivec2 cell = ivec2(floor(cu));
    uint h0 = hash_ivec2(0u, cell);
    uint h1 = hash_ivec2(1u, cell);
    uint h2 = hash_ivec2(2u, cell);
    uint h3 = hash_ivec2(3u, cell);
    uint h4 = hash_ivec2(4u, cell);
    uint h5 = hash_ivec2(5u, cell);
    vec2 c0 = random_vec3(h0).xy; float r0 = 0.2 + random_vec3(h0).z * 0.1;
    vec2 c1 = random_vec3(h1).xy; float r1 = 0.2 + random_vec3(h1).z * 0.1;
    vec2 c2 = random_vec3(h2).xy; float r2 = 0.2 + random_vec3(h2).z * 0.1;
    vec2 c3 = random_vec3(h3).xy; float r3 = 0.2 + random_vec3(h3).z * 0.1;
    vec2 c4 = random_vec3(h4).xy; float r4 = 0.2 + random_vec3(h4).z * 0.1;
    vec2 c5 = random_vec3(h5).xy; float r5 = 0.2 + random_vec3(h5).z * 0.1;
    float d = cs(p, c0, r0);
    d = su(d, cs(p, c1, r1));
    d = su(d, cs(p, c2, r2));
    d = su(d, cs(p, c3, r3));
    d = su(d, cs(p, c4, r4));
    d = su(d, cs(p, c5, r5));
    return d;
}

void fragment() {
    float d = cloudD(UV);
    float a = smoothstep(fwidth(d) * 0.5, -fwidth(d) * 0.5, d);
    COLOR.rgb = mix(sky_color, cloud_color, a);
}
