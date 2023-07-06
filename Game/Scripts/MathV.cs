using Godot;

namespace Dust;
public static class MathV
{
	public static Vector3I Min(params Vector3I[] vectors)
	{
		int x = vectors[0].X;
		int y = vectors[0].Y;
		int z = vectors[0].Z;

		foreach (var v in vectors)
		{
			x = Mathf.Min(v.X, x);
			y = Mathf.Min(v.Y, y);
			z = Mathf.Min(v.Z, z);
		}

		return new Vector3I(x, y, z);
	}

	public static Vector3I Max(params Vector3I[] vectors)
	{
		int x = vectors[0].X;
		int y = vectors[0].Y;
		int z = vectors[0].Z;

		foreach (var v in vectors)
		{
			x = Mathf.Max(v.X, x);
			y = Mathf.Max(v.Y, y);
			z = Mathf.Max(v.Z, z);
		}

		return new Vector3I(x, y, z);
	}
}
