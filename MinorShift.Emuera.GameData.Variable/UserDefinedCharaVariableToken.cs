using MinorShift.Emuera.GameProc;

namespace MinorShift.Emuera.GameData.Variable;

internal abstract class UserDefinedCharaVariableToken : CharaVariableToken
{
	public readonly UserDefinedVariableData DimData;

	public readonly int ArrayIndex;

	protected UserDefinedCharaVariableToken(VariableCode varCode, UserDefinedVariableData data, VariableData varData, int arrayIndex)
		: base(varCode, varData)
	{
		ArrayIndex = arrayIndex;
		DimData = data;
		varName = data.Name;
		sizes = data.Lengths;
		base.IsGlobal = data.Global;
		base.IsSavedata = data.Save;
		totalSize = 1;
		for (int i = 0; i < sizes.Length; i++)
		{
			totalSize *= sizes[i];
		}
		base.IsForbid = totalSize == 0;
	}

	public override object GetArrayChara(int charano)
	{
		return varData.CharacterList[charano].UserDefCVarDataList[ArrayIndex];
	}
}
