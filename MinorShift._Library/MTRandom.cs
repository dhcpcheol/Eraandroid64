using System;

namespace MinorShift._Library;

public sealed class MTRandom
{
	private const int MEXP = 19937;

	private const int POS1 = 122;

	private const int SL1 = 18;

	private const int SL2 = 1;

	private const int SR1 = 11;

	private const int SR2 = 1;

	private const uint MSK1 = 3758096367u;

	private const uint MSK2 = 3724462975u;

	private const uint MSK3 = 3220897791u;

	private const uint MSK4 = 3221225462u;

	private const uint PARITY1 = 1u;

	private const uint PARITY2 = 0u;

	private const uint PARITY3 = 0u;

	private const uint PARITY4 = 331998852u;

	private const int N = 156;

	private const int N32 = 624;

	private const int SL2_x8 = 8;

	private const int SR2_x8 = 8;

	private const int SL2_ix8 = 56;

	private const int SR2_ix8 = 56;

	private uint[] sfmt;

	private int idx;

	public MTRandom()
		: this(Environment.TickCount)
	{
	}

	public MTRandom(long seed)
	{
		init_gen_rand((uint)seed);
	}

	public long NextInt64(long max)
	{
		if (max <= 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		return (long)(NextUInt64() % (ulong)max);
	}

	public long NextInt64()
	{
		return (long)NextUInt64();
	}

	public ulong NextUInt64()
	{
		return ((ulong)NextUInt32() << 32) + NextUInt32();
	}

	public double NextDouble()
	{
		return (double)NextUInt32() * 2.3283064365386963E-10;
	}

	public void SetRand(long[] array)
	{
		if (array == null || array.Length != 625)
		{
			throw new ArgumentOutOfRangeException();
		}
		for (int i = 0; i < 624; i++)
		{
			sfmt[i] = (uint)array[i];
		}
		idx = (int)array[624];
	}

	public void GetRand(long[] array)
	{
		if (array == null || array.Length != 625)
		{
			throw new ArgumentOutOfRangeException();
		}
		for (int i = 0; i < 624; i++)
		{
			array[i] = sfmt[i];
		}
		array[624] = idx;
	}

	private uint NextUInt32()
	{
		if (idx >= 624)
		{
			gen_rand_all();
			idx = 0;
		}
		return sfmt[idx++];
	}

	private void init_gen_rand(uint seed)
	{
		sfmt = new uint[624];
		sfmt[0] = seed;
		for (int i = 1; i < 624; i++)
		{
			sfmt[i] = (uint)(1812433253 * (sfmt[i - 1] ^ (sfmt[i - 1] >> 30)) + i);
		}
		period_certification();
		idx = 624;
	}

	private void period_certification()
	{
		uint[] array = new uint[4] { 1u, 0u, 0u, 331998852u };
		uint num = 0u;
		for (int i = 0; i < 4; i++)
		{
			num ^= sfmt[i] & array[i];
		}
		for (int i = 16; i > 0; i >>= 1)
		{
			num ^= num >> i;
		}
		num &= 1;
		if (num == 1)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			uint num2 = 1u;
			for (int j = 0; j < 32; j++)
			{
				if ((num2 & array[i]) != 0)
				{
					sfmt[i] ^= num2;
					return;
				}
				num2 <<= 1;
			}
		}
	}

	private void gen_rand_all()
	{
		gen_rand_all_19937();
	}

	private void gen_rand_all_19937()
	{
		uint[] array = sfmt;
		int num = 0;
		int num2 = 488;
		int num3 = 616;
		int num4 = 620;
		do
		{
			array[num + 3] = array[num + 3] ^ (array[num + 3] << 8) ^ (array[num + 2] >> 24) ^ (array[num3 + 3] >> 8) ^ ((array[num2 + 3] >> 11) & 0xBFFFFFF6u) ^ (array[num4 + 3] << 18);
			array[num + 2] = array[num + 2] ^ (array[num + 2] << 8) ^ (array[num + 1] >> 24) ^ (array[num3 + 3] << 24) ^ (array[num3 + 2] >> 8) ^ ((array[num2 + 2] >> 11) & 0xBFFAFFFFu) ^ (array[num4 + 2] << 18);
			array[num + 1] = array[num + 1] ^ (array[num + 1] << 8) ^ (array[num] >> 24) ^ (array[num3 + 2] << 24) ^ (array[num3 + 1] >> 8) ^ ((array[num2 + 1] >> 11) & 0xDDFECB7Fu) ^ (array[num4 + 1] << 18);
			array[num] = array[num] ^ (array[num] << 8) ^ (array[num3 + 1] << 24) ^ (array[num3] >> 8) ^ ((array[num2] >> 11) & 0xDFFFFFEFu) ^ (array[num4] << 18);
			num3 = num4;
			num4 = num;
			num += 4;
			num2 += 4;
			if (num2 >= 624)
			{
				num2 = 0;
			}
		}
		while (num < 624);
	}
}
