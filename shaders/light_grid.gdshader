shader_type spatial;
render_mode cull_disabled;

uniform float scale = 0.1;
uniform float grid_scale = 0.05;

varying vec3 model_vertex;

void vertex() {
	// Called for every vertex the material is visible on.
	model_vertex = VERTEX;
}

void fragment() {
	vec3 bumped_coordinate = model_vertex + 0.5 * scale;
	vec3 rounded_coordinate = floor(bumped_coordinate/scale);
	vec3 inner_coordinate = (bumped_coordinate - rounded_coordinate*scale)/scale;
	vec3 threshold  = vec3(grid_scale,grid_scale,grid_scale);
	if (any(lessThanEqual(inner_coordinate,threshold))) {
		EMISSION = vec3(1,1,1);
	} else {
		discard;
	}
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
