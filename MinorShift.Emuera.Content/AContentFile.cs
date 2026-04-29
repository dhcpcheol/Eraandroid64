using System;

namespace MinorShift.Emuera.Content;

internal abstract class AContentFile : IDisposable
{
	public readonly string Name;

	public readonly string Filepath;

	protected bool Loaded;

	public bool Enabled { get; protected set; }

	public AContentFile(string name, string path)
	{
		Name = name;
		Filepath = path;
	}

	public abstract void Dispose();
}
