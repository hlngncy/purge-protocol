
void expand_float(float3 world, float3 normal, float3 view, out float3 Out)
{
#if defined(_GEO_RESIZING_ON)
	float d = distance(world, view);
	float resize = saturate((d - _GeoNearFadeDistance) / (_GeoFarFadeDistance - _GeoNearFadeDistance)) * _ResizeAmount;
	world += resize * normal;
#endif
	Out = world;
}

void reject_float(float3 a, float3 b, out float3 Out)
{
	Out = a - (dot(a, b) / dot(b, b)) * b;
}
