shader_type canvas_item;

uniform float blue = 1.0;

void vertex() {
	// Called for every vertex the material is visible on.
	// Nah, adding animation using shader just makes the detection kinda inconsistent
	// But it would be the same for manual animation isn't it?
	// perhaps tone it down a bit
	//VERTEX += vec2(cos(TIME) * 2.0, sin(TIME) * 2.0);
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR.r = 0.2;
	COLOR.b = 0.2;
	COLOR.g = 0.2;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
