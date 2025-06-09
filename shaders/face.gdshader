shader_type canvas_item;

uniform float line_width : hint_range(0.001,0.02) = 0.008;

float segment_distance(vec2 point, vec2 seg_a, vec2 seg_b){
    vec2 pa = point - seg_a;
    vec2 ba = seg_b - seg_a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

void fragment(){
    vec2 pos = (UV - vec2(0.5)) * 2.0;
    pos.y = -pos.y;
    vec3 color = vec3(1.0);
    float dist;

    dist = abs(length(vec2(pos.x / 0.35, pos.y / 0.55)) - 1.0);
    if(dist < line_width) color = vec3(0.0);

    dist = abs(length(vec2((pos.x + 0.63) / 0.27, (pos.y - 0.12) / 0.15)) - 1.0);
    if(dist < line_width) color = vec3(0.0);
    dist = abs(length(vec2((pos.x - 0.63) / 0.27, (pos.y - 0.12) / 0.15)) - 1.0);
    if(dist < line_width) color = vec3(0.0);

    dist = segment_distance(pos, vec2(-0.24, 0.55), vec2(-0.90, 0.12));
    if(dist < line_width) color = vec3(0.0);
    dist = segment_distance(pos, vec2( 0.24, 0.55), vec2( 0.90, 0.12));
    if(dist < line_width) color = vec3(0.0);

    float curl_radius = 0.10;
    vec2 curl_center;
    curl_center = vec2(-0.24, 0.52); dist = abs(length(pos - curl_center) - curl_radius); if(dist < line_width) color = vec3(0.0);
    curl_center = vec2(-0.08, 0.55); dist = abs(length(pos - curl_center) - curl_radius); if(dist < line_width) color = vec3(0.0);
    curl_center = vec2( 0.08, 0.55); dist = abs(length(pos - curl_center) - curl_radius); if(dist < line_width) color = vec3(0.0);
    curl_center = vec2( 0.24, 0.52); dist = abs(length(pos - curl_center) - curl_radius); if(dist < line_width) color = vec3(0.0);

    if(length(pos - vec2(-0.15, 0.12)) < 0.05) color = vec3(0.0);
    if(length(pos - vec2( 0.15, 0.12)) < 0.05) color = vec3(0.0);

    if(length(pos - vec2(-0.04, -0.02)) < 0.02) color = vec3(0.0);
    if(length(pos - vec2( 0.04, -0.02)) < 0.02) color = vec3(0.0);

    float mouth_half_width = 0.15 + 0.07 * (sin(TIME * 1.5) * 0.5 + 0.5);
    float clamped_x = clamp(pos.x, -mouth_half_width, mouth_half_width);
    float mouth_curve_y = -0.28 + 0.12 * sqrt(max(0.0, 1.0 - (clamped_x / mouth_half_width) * (clamped_x / mouth_half_width)));
    if(abs(pos.y - mouth_curve_y) < line_width && abs(pos.x) <= mouth_half_width + 0.01) color = vec3(0.0);

    COLOR = vec4(color, 1.0);
}
