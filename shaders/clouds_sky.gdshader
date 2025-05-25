shader_type spatial;
render_mode unshaded, depth_prepass_alpha, blend_mix;

uniform sampler2D cloud_noise;
uniform vec3  sky_color : source_color = vec3(0.6, 0.8, 1.0);
uniform vec3  cloud_color : source_color = vec3(1.0, 1.0, 1.0);
uniform float cloud_scale : hint_range(0.1, 10.0) = 2.0;
uniform float contrast_pow : hint_range(0.1, 5.0) = 1.5;
uniform float opacity_mul : hint_range(0.0, 1.0) = 0.8;

void fragment() {
    vec2 uv = UV * cloud_scale;
    float n = texture(cloud_noise, uv).r;
    float density = pow(n, contrast_pow);

    ALBEDO = mix(sky_color, cloud_color, density);
    ALPHA  = opacity_mul * density;
}
