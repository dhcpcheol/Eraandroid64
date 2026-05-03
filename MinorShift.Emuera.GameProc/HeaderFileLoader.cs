using System.Collections.Generic;
using MinorShift.Emuera.GameData;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;
using System.Linq;
using System.IO;

namespace MinorShift.Emuera.GameProc;

internal sealed class HeaderFileLoader
{
	private readonly Process parentProcess;

	private readonly EmueraConsole output;

	private readonly IdentifierDictionary idDic;

	private bool noError = true;

	public HeaderFileLoader(EmueraConsole main, IdentifierDictionary idDic, Process proc)
	{
		output = main;
		parentProcess = proc;
		this.idDic = idDic;
	}

    public bool LoadHeaderFiles(string headerDir, bool displayReport)
    {
        List<KeyValuePair<string, string>> files = Config.GetFiles(headerDir, "*.ERH");

        files = files
            .OrderBy(f =>
            {
                string name = Path.GetFileName(f.Value);
                if (name.Equals("DIM.ERH", StringComparison.OrdinalIgnoreCase)) return 0;
                if (name.Equals("DIM_t.ERH", StringComparison.OrdinalIgnoreCase)) return 1;
                return 2;
            })
            .ThenBy(f => f.Value)
            .ToList();

        bool flag = true;
        try
        {
            for (int i = 0; i < files.Count; i++)
			{
				string key = files[i].Key;
				string value = files[i].Value;
				if (displayReport)
				{
					output.PrintSystemLine(key + "読み込み中・・・");
				}
				flag = loadHeaderFile(value, key);
				if (!flag)
				{
					break;
				}
			}
		}
		finally
		{
			ParserMediator.FlushWarningList();
		}
		return flag;
	}

	private bool loadHeaderFile(string filepath, string filename)
	{
		StringStream stringStream = null;
		ScriptPosition scriptPosition = null;
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: true);
		if (!eraStreamReader.Open(filepath, filename))
		{
			throw new CodeEE(eraStreamReader.Filename + "のオープンに失敗しました");
		}
		try
		{
			while ((stringStream = eraStreamReader.ReadEnabledLine()) != null)
			{
				if (!noError)
				{
					return false;
				}
				scriptPosition = new ScriptPosition(filename, eraStreamReader.LineNo, stringStream.RowString);
				LexicalAnalyzer.SkipWhiteSpace(stringStream);
				if (stringStream.Current != '#')
				{
					throw new CodeEE("ヘッダーの中に#で始まらない行があります", scriptPosition);
				}
				stringStream.ShiftNext();
				string text = LexicalAnalyzer.ReadSingleIdentifier(stringStream);
				if (text == null)
				{
					ParserMediator.Warn("解釈できない#行です", scriptPosition, 1);
					return false;
				}
				if (Config.ICFunction)
				{
					text = text.ToUpper();
				}
				LexicalAnalyzer.SkipWhiteSpace(stringStream);
				switch (text)
				{
				case "DEFINE":
					analyzeSharpDefine(stringStream, scriptPosition);
					break;
				case "FUNCTION":
				case "FUNCTIONS":
					analyzeSharpFunction(stringStream, scriptPosition, text == "FUNCTIONS");
					break;
				case "DIM":
				case "DIMS":
					analyzeSharpDim(stringStream, scriptPosition, text == "DIMS");
					break;
				default:
					throw new CodeEE("#" + text + "は解釈できないプリプロセッサです", scriptPosition);
				}
			}
		}
		catch (CodeEE codeEE)
		{
			if (codeEE.Position != null)
			{
				scriptPosition = codeEE.Position;
			}
			ParserMediator.Warn(codeEE.Message, scriptPosition, 2);
			return false;
		}
		finally
		{
			eraStreamReader.Close();
		}
		return true;
	}

	private void analyzeSharpDefine(StringStream st, ScriptPosition position)
	{
		string text = LexicalAnalyzer.ReadSingleIdentifier(st);
		if (text == null)
		{
			throw new CodeEE("置換元の識別子がありません", position);
		}
		if (Config.ICVariable)
		{
			text = text.ToUpper();
		}
		string errMes = "";
		int warnLevel = -1;
		idDic.CheckUserMacroName(ref errMes, ref warnLevel, text);
		if (warnLevel >= 0)
		{
			ParserMediator.Warn(errMes, position, warnLevel);
			if (warnLevel >= 2)
			{
				noError = false;
				return;
			}
		}
		bool flag = st.Current == '(';
		WordCollection wordCollection = LexicalAnalyzer.Analyse(st, LexEndWith.EoL, LexAnalyzeFlag.AllowAssignment);
		if (wordCollection.EOL)
		{
			DefineMacro mac = new DefineMacro(text, new WordCollection(), 0);
			idDic.AddMacro(mac);
			return;
		}
		List<string> list = new List<string>();
		if (flag)
		{
			wordCollection.ShiftNext();
			if (wordCollection.Current.Type == ')')
			{
				throw new CodeEE("関数型マクロの引数を0個にすることはできません", position);
			}
			while (!wordCollection.EOL)
			{
				IdentifierWord obj = (wordCollection.Current as IdentifierWord) ?? throw new CodeEE("置換元の引数指定の書式が間違っています", position);
				obj.SetIsMacro();
				string code = obj.Code;
				if (list.Contains(code))
				{
					throw new CodeEE("置換元の引数に同じ文字が2回以上使われています", position);
				}
				list.Add(code);
				wordCollection.ShiftNext();
				if (wordCollection.Current.Type == ',')
				{
					wordCollection.ShiftNext();
					continue;
				}
				if (wordCollection.Current.Type == ')')
				{
					break;
				}
				throw new CodeEE("置換元の引数指定の書式が間違っています", position);
			}
			if (wordCollection.EOL)
			{
				throw new CodeEE("')'が閉じられていません", position);
			}
			wordCollection.ShiftNext();
		}
		if (wordCollection.EOL)
		{
			throw new CodeEE("置換先の式がありません", position);
		}
		WordCollection wordCollection2 = new WordCollection();
		while (!wordCollection.EOL)
		{
			wordCollection2.Add(wordCollection.Current);
			wordCollection.ShiftNext();
		}
		if (flag)
		{
			while (!wordCollection2.EOL)
			{
				if (!(wordCollection2.Current is IdentifierWord identifierWord))
				{
					wordCollection2.ShiftNext();
					continue;
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (string.Equals(identifierWord.Code, list[i], Config.SCVariable))
					{
						wordCollection2.Remove();
						wordCollection2.Insert(new MacroWord(i));
						break;
					}
				}
				wordCollection2.ShiftNext();
			}
			wordCollection2.Pointer = 0;
		}
		if (flag)
		{
			throw new CodeEE("関数型マクロは宣言できません", position);
		}
		DefineMacro mac2 = new DefineMacro(text, wordCollection2, list.Count);
		idDic.AddMacro(mac2);
	}

	private void analyzeSharpDim(StringStream st, ScriptPosition position, bool dims)
	{
		UserDefinedVariableData userDefinedVariableData = UserDefinedVariableData.Create(LexicalAnalyzer.Analyse(st, LexEndWith.EoL, LexAnalyzeFlag.AllowAssignment), dims, isPrivate: false, position);
		if (userDefinedVariableData.Reference)
		{
			throw new NotImplCodeEE();
		}
		VariableToken variableToken = null;
		variableToken = ((!userDefinedVariableData.CharaData) ? ((VariableToken)parentProcess.VEvaluator.VariableData.CreateUserDefVariable(userDefinedVariableData)) : ((VariableToken)parentProcess.VEvaluator.VariableData.CreateUserDefCharaVariable(userDefinedVariableData)));
		idDic.AddUseDefinedVariable(variableToken);
	}

	private void analyzeSharpFunction(StringStream st, ScriptPosition position, bool funcs)
	{
		throw new NotImplCodeEE();
	}
}
