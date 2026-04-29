namespace MinorShift.Emuera.GameData.Expression;

internal enum TermEndWith
{
	None = 0,
	EoL = 1,
	Comma = 2,
	RightParenthesis = 4,
	RightBracket = 8,
	Assignment = 16,
	RightParenthesis_Comma = 6,
	RightBracket_Comma = 10,
	Comma_Assignment = 18,
	RightParenthesis_Comma_Assignment = 22,
	RightBracket_Comma_Assignment = 26
}
