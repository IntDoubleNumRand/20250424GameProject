
#ifndef UTIL_RANDOM
#define UTIL_RANDOM

// Cheap, but cryptographically unsecure RNG
// Multiplier constant from: https://www.ams.org/journals/mcom/1999-68-225/S0025-5718-99-00996-5/S0025-5718-99-00996-5.pdf
// Addition constant is arbitrary (only needs to be odd).
uint lcg_stateless (uint seed) {
	return ((seed*2891336453u) + 1234567u) & 0xFFFFFFFFu;
}

uint lcg (inout uint seed_state) {
	uint result = seed_state;
	seed_state = lcg_stateless(seed_state);
	return result;
}

// Helper for `hash` function
void mmix(inout uint h, inout uint k, uint m, uint r) {
	k *= m;
	k ^= k >> r;
	k *= m;
	h *= m;
	h ^= k;
}

// Cheap, but cryptographically unsecure hash, derived from MurmurHash2A
uint hash_uint (uint seed, uint key )
{
  const uint m = 0x5bd1e995u;
  const uint r = 24u;
  uint l = 4u;
  uint h = seed;
  mmix(h,key,m,r);
  uint t = 0u;
  mmix(h,t,m,r);
  mmix(h,l,m,r);
  h ^= h >> 13u;
  h *= m;
  h ^= h >> 15u;
  return h;
}

uint hash_uvec2(uint seed, uvec2 key) {
	return hash_uint(hash_uint(seed,key.x),key.y);
}
uint hash_uvec3(uint seed, uvec3 key) {
	return hash_uint(hash_uvec2(seed,key.xy),key.z);
}
uint hash_uvec4(uint seed, uvec4 key) {
	return hash_uint(hash_uvec3(seed,key.xyz),key.w);
}


uint hash_int(uint seed, int key) {
	return hash_uint(seed,uint(key));
}
uint hash_ivec2(uint seed, ivec2 key) {
	return hash_int(hash_int(seed,key.x),key.y);
}
uint hash_ivec3(uint seed, ivec3 key) {
	return hash_int(hash_ivec2(seed,key.xy),key.z);
}
uint hash_ivec4(uint seed, ivec4 key) {
	return hash_int(hash_ivec3(seed,key.xyz),key.w);
}

float lcg_float(inout uint seed_state) {
	return float(lcg(seed_state)) / float(0x80000000u);
}
vec2 lcg_vec2(inout uint seed_state) {
	return vec2(lcg_float(seed_state),lcg_float(seed_state));
}
vec3 lcg_vec3(inout uint seed_state) {
	return vec3(
		lcg_float(seed_state),
		lcg_float(seed_state),
		lcg_float(seed_state)
	);
}
vec4 lcg_vec4(inout uint seed_state) {
	return vec4(
		lcg_float(seed_state),
		lcg_float(seed_state),
		lcg_float(seed_state),
		lcg_float(seed_state)
	);
}

float random_float(uint seed_state) {
	return float(lcg(seed_state)) / float(0x80000000u);
}
vec2 random_vec2(uint seed_state) {
	return vec2(lcg_float(seed_state),random_float(seed_state));
}
vec3 random_vec3(uint seed_state) {
	return vec3(
		lcg_float(seed_state),
		lcg_float(seed_state),
		lcg_float(seed_state)
	);
}
vec4 random_vec4(uint seed_state) {
	return vec4(
		lcg_float(seed_state),
		lcg_float(seed_state),
		lcg_float(seed_state),
		lcg_float(seed_state)
	);
}

#endif // UTIL_RANDOM
