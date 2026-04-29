namespace EmueraFramework;

public interface IScrollBar
{
	bool IsBackLog { get; }

	bool Enabled { get; set; }

	void MoveToEnd();
}
