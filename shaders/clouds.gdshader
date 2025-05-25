shader_type canvas_item;

uniform vec4 bg_color : source_color = vec4(0.28, 0.43, 0.74, 1.0);
uniform vec4 fill_color : source_color = vec4(0.86, 0.82, 0.75, 1.0);
uniform vec4 shadow_color : source_color = vec4(0.68, 0.73, 0.70, 1.0);

uniform vec2 shadow_offset = vec2(0.05, -0.05);

const int N = 6;
const vec2 centers[N] = vec2[](
    vec2(0.40, 0.55),
    vec2(0.47, 0.40),
    vec2(0.65, 0.60),
    vec2(0.60, 0.40),
    vec2(0.30, 0.40),
    vec2(0.20, 0.60)
);
const vec2 radii[N] = vec2[](
    vec2(0.20, 0.20),
    vec2(0.16, 0.20),
    vec2(0.18, 0.15),
    vec2(0.20, 0.13),
    vec2(0.13, 0.12),
    vec2(0.17, 0.14)
);

void fragment() {
    vec2 uv = UV;
    bool hitMain   = false;
    bool hitShadow = false;

    for (int i = 0; i < N; i++) {
        vec2 p = (uv - centers[i]) / radii[i];
        if (length(p) < 1.0) {
            hitMain = true;
            break;
        }
    }

    if (!hitMain) {
        for (int i = 0; i < N; i++) {
            vec2 p = (uv - (centers[i] + shadow_offset)) / radii[i];
            if (length(p) < 1.0) {
                hitShadow = true;
                break;
            }
        }
    }

    if (hitMain) {
        COLOR = fill_color;
    } else if (hitShadow) {
        COLOR = shadow_color;
    } else {
        COLOR = bg_color;
    }
}
