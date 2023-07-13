shader_type spatial;
render_mode unshaded;

uniform sampler2D depth_texture : source_color, hint_depth_texture;
uniform sampler2D main_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform sampler3D cloud_noise;

uniform vec4 cloud_heights;
uniform vec3 cloud_offset;
uniform vec3 cloud_scale;
uniform float cloud_density_mod;
uniform float light_absorption_to_cloud;
uniform float light_absorption_to_sun;
uniform vec3 light_dir;
uniform vec3 light_color;
uniform float phase_value;

uniform int num_steps;
uniform int num_steps_light;

const float epsilon = 0.001;

vec3 hash(vec3 p) {
    p = vec3(dot(p, vec3(127.1, 311.7, 74.7)),
             dot(p, vec3(269.5, 183.3, 246.1)),
             dot(p, vec3(113.5, 271.9, 124.6)));

    return -1.0 + 2.0 * fract(sin(p) * 43758.5453123);
}

float noise(vec3 p) {
  vec3 i = floor(p);
  vec3 f = fract(p);
  vec3 u = f * f * (3.0 - 2.0 * f);

  return mix(mix(mix(dot(hash(i + vec3(0.0, 0.0, 0.0)), f - vec3(0.0, 0.0, 0.0)),
                     dot(hash(i + vec3(1.0, 0.0, 0.0)), f - vec3(1.0, 0.0, 0.0)), u.x),
                 mix(dot(hash(i + vec3(0.0, 1.0, 0.0)), f - vec3(0.0, 1.0, 0.0)),
                     dot(hash(i + vec3(1.0, 1.0, 0.0)), f - vec3(1.0, 1.0, 0.0)), u.x), u.y),
             mix(mix(dot(hash(i + vec3(0.0, 0.0, 1.0)), f - vec3(0.0, 0.0, 1.0)),
                     dot(hash(i + vec3(1.0, 0.0, 1.0)), f - vec3(1.0, 0.0, 1.0)), u.x),
                 mix(dot(hash(i + vec3(0.0, 1.0, 1.0)), f - vec3(0.0, 1.0, 1.0)),
                     dot(hash(i + vec3(1.0, 1.0, 1.0)), f - vec3(1.0, 1.0, 1.0)), u.x), u.y), u.z );
}

float density(vec3 position) {
	if(cloud_heights[1] < position.y && position.y < cloud_heights[2])
		return 1.0;
		
	return texture(cloud_noise, (position + cloud_offset + vec3(TIME * 0.1, 0, TIME * 0.1)) * cloud_scale).r * cloud_density_mod;
}

float myDepth(vec2 screen_uv, mat4 inv_proj_matrix) {
	float depth = texture(depth_texture, screen_uv).x;
	vec3 ndc = vec3(screen_uv * 2.0 - 1.0, depth);
	vec4 view = inv_proj_matrix * vec4(ndc, 1.0);
	view.xyz /= view.w;
	return -view.z;
}

// return vec3(distanceToShallow, shallowDepth, distanceToDeep)
// If a value is negative, the ray does not intersect that plane.
vec3 cloudInfo(vec3 origin, vec3 dir) {
	// t[x] = Distance to [x]
	// t[0] = lowShallow, t[1] = lowDeep, t[2] = highDeep, t[3] = highShallow
	vec4 t = (cloud_heights - vec4(origin.y)) / dir.y;
	float tMin = min(t[0], min(t[1], min(t[2], t[3])));
	float tMax = max(t[0], max(t[1], max(t[2], t[3])));
	
	// Outside cloud and pointing towards it
	if(tMin > 0.0) {
		// Below clouds
		if(abs(t[0] - tMin) <= epsilon)
			return vec3(t[0], t[1] - t[0], t[1] - t[0]);
		// Above clouds
		if(abs(t[3] - tMin) <= epsilon)
			return vec3(t[3], t[2] - t[3], t[2] - t[3]);
	}
	
	// Outside cloud and poiting away from it
	if(tMax <= 0.0)
		return vec3(-1.0);
	
	// Inside lowShallow and pointing towards outside
	if(t[0] > 0.0 && t[1] <= 0.0) 
		return vec3(0.0, t[0], -1.0);
	
	// Inside highShallow and pointing towards outside
	if(t[3] > 0.0 && t[2] <= 0.0) 
		return vec3(0.0, t[3], -1.0);
		
	// Inside lowShallow and point towards deep
	if(t[0] <= 0.0 && t[1] > 0.0)
		return vec3(0.0, t[1], t[2] - t[1]);
		
	// Inside highShallow and point towards deep
	if(t[3] <= 0.0 && t[2] > 0.0)
		return vec3(0.0, t[2], t[1] - t[2]);
	
	// Inside deep
	return vec3(-1.0, -1.0, 0);
}

vec2 cloudDistanceAndDepth(vec3 origin, vec3 dir) {
	float t0 = (cloud_heights[2] - origin.y) / dir.y;
	float t1 = (cloud_heights[3] - origin.y) / dir.y;
	
	float tMin = min(t0, t1);
	float tMax = max(t0, t1);
	
	if(tMin >= epsilon && tMax >= epsilon)
		return vec2(tMin, tMax - tMin);
		
	if(tMin < epsilon && tMax >= epsilon)
		return vec2(0.0, tMax);
		
	return vec2(-1.0);
}

float lightMarch(vec3 position) {
	vec3 dirToLight = -light_dir;
	float dstInside = cloudInfo(position, dirToLight)[1];
	
	float stepSize = dstInside / float(num_steps_light);
	float totalDensity = 0.0;
	
	for(int i = 0; i < num_steps_light; i++) {
		position += dirToLight * stepSize;
		totalDensity += max(0, density(position) * stepSize);
	}
	
	float transmittance = exp(-totalDensity * light_absorption_to_sun);
	return transmittance;
}

void vertex() {
  POSITION = vec4(VERTEX, 1.0);
}

void fragment() {
	vec4 camera = INV_VIEW_MATRIX * INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, 1, 1);
	vec4 color = texture(main_texture, SCREEN_UV);
		
	vec3 ray_origin = CAMERA_POSITION_WORLD;
	vec3 ray_dir = normalize(camera.xyz);
	
	vec3 cloudInfo = cloudInfo(ray_origin, ray_dir);
	float dstToShallow = cloudInfo.x;
	float shallowDepth = cloudInfo.y;
	float dstToDeep = cloudInfo.z;
	float depth = myDepth(SCREEN_UV, INV_PROJECTION_MATRIX) * length(camera.xyz);
	
	// Outside cloud and poiting away from it
	if(cloudInfo == vec3(-1.0)) {
		ALPHA = 0.0;
	// Inside deep
	} else if(cloudInfo == vec3(-1.0, -1.0, 0.0)) {
		ALBEDO = color.rgb * vec3(0.1);
	// Outside cloud and pointing towards it blocked
	} else if(dstToShallow > 0.0 && dstToDeep > 0.0 && depth < dstToShallow) {
		ALPHA = 0.0;
	// Outside cloud and pointing towards it unblocked
	} else {
		float dstTravelled = 0.0;
		float dstLimit =  min(min(depth - dstToShallow, shallowDepth), 4000);
		float stepSize = dstLimit / float(num_steps);
		
		float transmittance = 1.0;
		vec3 lightEnergy = vec3(0.0);
		
		while(dstTravelled < dstLimit) {
			vec3 rayPos = ray_origin + ray_dir * (dstToShallow + dstTravelled);
			float density = density(rayPos) * stepSize;
			
			if(density > 0.0) {
				float lightTrans = lightMarch(rayPos);
				lightEnergy += density * stepSize * transmittance * lightTrans * phase_value;
				transmittance *= exp(-density * stepSize * light_absorption_to_cloud);
				
				if(transmittance < 0.01)
					break;
			}
			dstTravelled += stepSize;
		}
		
		vec3 cloudColor = lightEnergy * light_color;
		ALBEDO = color.rgb * transmittance + cloudColor;
		ALPHA = 1.0;
	}
}