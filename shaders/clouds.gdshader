shader_type canvas_item;
#include "util/random.gdshaderinc"

uniform vec4 bg_color : source_color = vec4(0.28, 0.43, 0.74, 1.0);
uniform vec4 fill_color : source_color = vec4(0.86, 0.82, 0.75, 1.0);
uniform vec4 shadow_color : source_color = vec4(0.68, 0.73, 0.70, 1.0);

uniform vec2 tile_count = vec2(6.0, 4.0);
uniform float layer1_scale = 0.14;
uniform float layer2_scale = 0.4;

uniform float jitter_amount = 0.2;
uniform vec2 move_dir = vec2(-1.0, 1.0);
uniform float speed1 = 0.05;
uniform float speed2  = 0.03;
uniform vec2 shadow_offset = vec2(0.05, -0.05);
uniform float cluster_scale = 0.5;

const int N = 6;
const vec2 offsets[N] = vec2[](
  vec2(-0.10,  0.05), vec2(-0.03, -0.10),
  vec2( 0.15,  0.10), vec2( 0.10, -0.10),
  vec2(-0.20, -0.10), vec2(-0.30,  0.10)
);
const vec2 radii[N] = vec2[](
  vec2(0.20, 0.20), vec2(0.16, 0.20),
  vec2(0.18, 0.15), vec2(0.20, 0.13),
  vec2(0.13, 0.12), vec2(0.17, 0.14)
);

float rnd1(uint s) { return fract(sin(float(s))*43758.5453); }
vec2  rnd2(uint s) { return vec2(rnd1(s), rnd1(s+1u)); }

void sample_layer(vec2 UVbase, vec2 dir, float speed, vec2 tcount,
                  out bool main_hit, out bool shadow_hit) {
  vec2 uv_moved = UVbase + dir * speed * TIME;
  vec2 gv = uv_moved * tcount;
  ivec2 cell = ivec2(floor(gv));
  vec2 uvCell = fract(gv);

  uint seed = hash_cell(cell);
  vec2 jitter = (rnd2(seed) - 0.5) * jitter_amount;
  vec2 center= vec2(0.5) + jitter;

  main_hit = false;
  shadow_hit = false;
  // fill
  for (int i = 0; i < N; i++) {
    vec2 c = center + offsets[i] * cluster_scale;
    vec2 r = radii[i] * cluster_scale;
    if (length((uvCell - c) / r) < 1.0) {
      main_hit = true;
      return;
    }
  }
  // shadow
  for (int i = 0; i < N; i++) {
    vec2 c = center + offsets[i] * cluster_scale + shadow_offset;
    vec2 r = radii[i] * cluster_scale;
    if (length((uvCell - c) / r) < 1.0) {
      shadow_hit = true;
      return;
    }
  }
}

void fragment() {
  vec2 dir = normalize(move_dir);
  bool mA, sA;
  sample_layer(UV, dir, speed1, tile_count * layer1_scale, mA, sA);
  bool mB, sB;
  sample_layer(UV, dir, speed2, tile_count * layer2_scale, mB, sB);

  if (mA || mB) {
    COLOR = fill_color;
  } else if (sA || sB) {
    COLOR = shadow_color;
  } else {
    COLOR = bg_color;
  }
}
