using Godot;

namespace Dust.MeshGeneration;

public static class FloorMeshGenerator
{
	private const int resolutionMultiplier = 3;

	public static Mesh GenerateArc(Vector3 center, Vector3 pointA, Vector3 pointB)
	{
		// Math is in radians!
		bool isReflex = (pointA - center).Cross(pointB - center).Y > 0;
		GD.Print($"isReflex: {isReflex}");
		float radius = Mathf.Abs((center - pointA).Length());
		GD.Print($"radius: {radius}");
		float chord = (pointA - pointB).Length();
		GD.Print($"chord: {chord}");
		float angle = Mathf.Asin(chord / (2 * radius)) * 2;
		GD.Print($"angle: {angle}");
		if (isReflex)
			angle = (2 * Mathf.Pi) - angle;

		float arc = angle * radius;
		GD.Print($"Arc Length: {arc}");
		var immediate_mesh = new ImmediateMesh();

		immediate_mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		const float stepSize = 1f / resolutionMultiplier;
		Vector3 previous = Vector3.Right;
		for (float a = stepSize; a < arc; a += stepSize)
		{
			Vector3 next = new(Mathf.Cos(a) * radius, 0, Mathf.Sin(a) * radius);
			immediate_mesh.SurfaceAddVertex(previous);
			immediate_mesh.SurfaceAddVertex(next);
			previous = next;
		}
		immediate_mesh.SurfaceAddVertex(previous);
		immediate_mesh.SurfaceAddVertex(new Vector3(Mathf.Cos(arc) * radius, 0, Mathf.Sin(arc) * radius));
		immediate_mesh.SurfaceEnd();

		return immediate_mesh;
	}
}
