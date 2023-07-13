using Godot;

namespace Dust;
public static class MathV
{
	public static Vector3I Min(params Vector3I[] vectors)
	{
		int x = vectors[0].X;
		int y = vectors[0].Y;
		int z = vectors[0].Z;

		foreach (Vector3I v in vectors)
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

		foreach (Vector3I v in vectors)
		{
			x = Mathf.Max(v.X, x);
			y = Mathf.Max(v.Y, y);
			z = Mathf.Max(v.Z, z);
		}

		return new Vector3I(x, y, z);
	}

	public static Vector3I RoundToInt(this Vector3 vector) => new(Mathf.RoundToInt(vector.X), Mathf.RoundToInt(vector.Y), Mathf.RoundToInt(vector.Z));
	/// <summary>
	/// Creates a copy of the vector with certain components replaced. Null values are kept the same as the original.
	/// </summary>
	/// <param name="vector">The vector being modified.</param>
	/// <param name="x">The new x component, or null to keep it the same.</param>
	/// <param name="y">The new y component, or null to keep it the same.</param>
	/// <param name="z">The new z component, or null to keep it the same.</param>
	/// <returns>The vector with it's components replaced.</returns>
	public static Vector3 With(this Vector3 vector, float? x, float? y, float? z)
	{
		return new Vector3(x ?? vector.X, y ?? vector.Y, z ?? vector.Z);
	}
}
