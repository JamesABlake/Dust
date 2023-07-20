using Godot;

namespace Dust;

public readonly record struct Ray
{
	/// <summary>
	/// The origin of the ray.
	/// </summary>
	public readonly Vector3 Origin;

	/// <summary>
	/// The direction of the ray.
	/// </summary>
	/// <remarks>
	/// This is always normalized. Assigning a vector will normalize it.
	/// </remarks>
	public Vector3 Direction { readonly get => _direction; init => _direction = value.Normalized(); }
	private readonly Vector3 _direction;

	public Ray(Vector3 origin, Vector3 direction) : this()
	{
		Origin = origin;
		Direction = direction;
	}

	public readonly Vector3 GetPoint(float distance) => Origin + (Direction * distance);
}