namespace MinorShift.Emuera.Content;

internal abstract class AContentItem
{
	public readonly string Name;

	public bool Enabled;

	protected AContentItem(string name)
	{
		Name = name;
	}
}
