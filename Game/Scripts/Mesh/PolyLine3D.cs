using Godot;

namespace Dust.MeshGeneration;

[GlobalClass]
public partial class PolyLine3D : Path3D
{
	[Export(PropertyHint.Range, "0, .1")] private float Radius = 0.1f;
	[Export(PropertyHint.Range, "0, 100")] private int Resolution = 10;

	public override void _Ready()
	{
		CsgPolygon3D poly = new CsgPolygon3D
		{
			Mode = CsgPolygon3D.ModeEnum.Path,
			PathRotation = CsgPolygon3D.PathRotationEnum.Path,
			SmoothFaces = true,
			PathIntervalType = CsgPolygon3D.PathIntervalTypeEnum.Subdivide,
			PathInterval = 0.1f

		};

		Vector2[] verts = new Vector2[Resolution];
		for (int i = 0; i < Resolution; i++)
		{
			verts[i] = new Vector2(
				Mathf.Sin(Mathf.Pi * 2 * i / Resolution) * Radius,
				Mathf.Cos(Mathf.Pi * 2 * i / Resolution) * Radius
				);
		}
		poly.Polygon = verts;

		_ = CallDeferred("add_child", poly);
		poly.SetDeferred("path_node", GetPath());
	}
}
