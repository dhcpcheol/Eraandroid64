namespace MinorShift.Emuera;

internal abstract class AConfigItem
{
	public readonly ConfigCode Code;

	public readonly string Name;

	public readonly string Text;

	public bool Fixed;

	public AConfigItem(ConfigCode code, string text)
	{
		Code = code;
		Name = code.ToString();
		Text = text;
	}

	public static ConfigItem<T> Copy<T>(ConfigItem<T> other)
	{
		if (other == null)
		{
			return null;
		}
		return new ConfigItem<T>(other.Code, other.Text, other.Value)
		{
			Fixed = other.Fixed
		};
	}

	public abstract void CopyTo(AConfigItem other);

	public abstract bool TryParse(string tokens);

	public abstract void SetValue<U>(U p);

	public abstract U GetValue<U>();

	public abstract string ValueToString();
}
