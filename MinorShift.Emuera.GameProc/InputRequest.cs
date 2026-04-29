namespace MinorShift.Emuera.GameProc;

internal sealed class InputRequest
{
	public readonly long ID;

	public InputType InputType;

	public bool OneInput;

	public bool StopMesskip;

	public bool IsSystemInput;

	public bool HasDefValue;

	public long DefIntValue;

	public string DefStrValue;

	public long Timelimit = -1L;

	public bool DisplayTime;

	public string TimeUpMes;

	private static long LastRequestID;

	public bool NeedValue
	{
		get
		{
			if (InputType != InputType.IntValue)
			{
				return InputType == InputType.StrValue;
			}
			return true;
		}
	}

	public InputRequest()
	{
		ID = LastRequestID++;
	}
}
