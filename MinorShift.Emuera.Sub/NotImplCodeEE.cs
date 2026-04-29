using System;

namespace MinorShift.Emuera.Sub;

[Serializable]
internal sealed class NotImplCodeEE : CodeEE
{
	public NotImplCodeEE(ScriptPosition position)
		: base("この機能は現バージョンでは使えません", position)
	{
	}

	public NotImplCodeEE()
		: base("この機能は現バージョンでは使えません")
	{
	}
}
