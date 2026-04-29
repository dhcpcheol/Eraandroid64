namespace MinorShift.Emuera.Sub;

public enum EraSaveDataType : byte
{
	Int = 0,
	IntArray = 1,
	IntArray2D = 2,
	IntArray3D = 3,
	Str = 16,
	StrArray = 17,
	StrArray2D = 18,
	StrArray3D = 19,
	Separator = 253,
	EOC = 254,
	EOF = byte.MaxValue
}
