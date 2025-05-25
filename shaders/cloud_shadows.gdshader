shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, unshaded;
#include "util/random.gdshaderinc"

uniform vec4 shadow_color : source_color = vec4(0.0, 0.0, 0.0, 0.5);
uniform float time_scale = 1.0;
uniform vec2 move_dir = vec2(-1.0, 1.0);
uniform float speed = 0.05;
uniform vec2 tile_count = vec2(1.0, 1.0);
uniform float cluster_scale = 1.5;
uniform float jitter_amount = 0.2;

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

float rnd1(uint s) { return fract(sin(float(s)) * 43758.5453); }
vec2 rnd2(uint s) { return vec2(rnd1(s), rnd1(s + 1u)); }

void fragment() {
	vec2 dir = normalize(move_dir);
	vec2 uv_moved = UV + dir * speed * TIME * time_scale;
	vec2 gv = uv_moved * tile_count;
	ivec2 base_cell = ivec2(floor(gv));
	vec2 uvCell = fract(gv);

	float alpha = 0.0;

	// Sample a 3x3 neighborhood of cells
	for (int ox = -1; ox <= 1; ++ox) {
		for (int oy = -1; oy <= 1; ++oy) {
			ivec2 cell = base_cell + ivec2(ox, oy);
			uint seed = hash_cell(cell);
			vec2 jitter = (rnd2(seed) - 0.5) * jitter_amount;
			vec2 center = vec2(cell) + vec2(0.5) + jitter;

			vec2 cell_uv = gv - vec2(cell);

			for (int i = 0; i < N; i++) {
				vec2 c = center + offsets[i] * cluster_scale;
				vec2 r = radii[i] * cluster_scale;
				if (length((gv - c) / r) < 1.0) {
					alpha = shadow_color.a;
				}
			}
		}
	}

	ALBEDO = shadow_color.rgb;
	ALPHA = alpha;
}