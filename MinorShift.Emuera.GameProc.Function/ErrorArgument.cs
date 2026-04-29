namespace MinorShift.Emuera.GameProc.Function;

internal sealed class ErrorArgument : Argument
{
	private readonly string errorMes;

	public ErrorArgument(string errorMes)
	{
		this.errorMes = errorMes;
	}
}
