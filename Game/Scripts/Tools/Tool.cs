using Godot;

namespace Dust.Tools;

public abstract partial class Tool : Node
{
	public void Activate()
	{
		OnActivate();
	}
	public void Deactivate()
	{
		OnDeactivate();
	}
	protected virtual void OnActivate() { }
	protected virtual void OnDeactivate() { }

	public void MouseMove(Ray mouseRay)
	{
		OnMouseMove(mouseRay);
	}
	public void MouseButtonDown(MouseButton button)
	{
		OnMouseButtonDown(button);
	}
	public void MouseButtonUp(MouseButton button)
	{
		OnMouseButtonUp(button);
	}
	protected virtual void OnMouseMove(Ray mouseRay) { }
	protected virtual void OnMouseButtonDown(MouseButton button) { }
	protected virtual void OnMouseButtonUp(MouseButton button) { }


	public void KeyDown(Key key)
	{
		OnKeyDown(key);
	}
	public void KeyUp(Key key)
	{
		OnKeyUp(key);
	}
	protected virtual void OnKeyDown(Key key) { }
	protected virtual void OnKeyUp(Key key) { }
}