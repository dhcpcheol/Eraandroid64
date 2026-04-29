using System.IO;
using System.Xml;
using Android.OS;
using Android.App;

namespace EraAndroid;

internal static class DB
{
	private static readonly string DataFolder;

	private static readonly string XmlPath;

	private static readonly string version;

	private static readonly XmlDocument doc;

    static DB()
    {
        DataFolder = Android.App.Application.Context.FilesDir.AbsolutePath + "/";

        if (!Directory.Exists(DataFolder))
        {
            Directory.CreateDirectory(DataFolder);
        }

        XmlPath = Path.Combine(DataFolder, "DB.xml");
        version = "1.0";
        doc = new XmlDocument();

        if (File.Exists(XmlPath))
        {
            try
            {
                doc.Load(XmlPath);
                if (doc.DocumentElement.Attributes["Version"].Value != version)
                {
                    CreateDB();
                }
                return;
            }
            catch
            {
                CreateDB();
                return;
            }
        }

        Clear();
        CreateDB();
    }

    private static void CreateDB()
	{
		XmlDeclaration newChild = doc.CreateXmlDeclaration("1.0", "utf-8", "no");
		doc.AppendChild(newChild);
		XmlElement xmlElement = doc.CreateElement("DataBase");
		XmlAttribute xmlAttribute = doc.CreateAttribute("Version");
		xmlAttribute.InnerText = version;
		xmlElement.Attributes.Append(xmlAttribute);
		doc.AppendChild(xmlElement);
		doc.Save(XmlPath);
	}

	public static void Save(string name, string contents)
	{
		FindElement(name).InnerText = contents;
		doc.Save(XmlPath);
	}

	public static string Load(string name)
	{
		string innerText = FindElement(name).InnerText;
		if (innerText == string.Empty)
		{
			return null;
		}
		return innerText;
	}

	private static XmlElement FindElement(string name)
	{
		XmlElement xmlElement = doc.DocumentElement.SelectSingleNode(name) as XmlElement;
		if (xmlElement == null)
		{
			xmlElement = doc.CreateElement(name);
			doc.DocumentElement.AppendChild(xmlElement);
		}
		return xmlElement;
	}

	internal static void Clear()
	{
		string[] files = Directory.GetFiles(DataFolder, "*");
		for (int i = 0; i < files.Length; i++)
		{
			File.Delete(files[i]);
		}
	}
}
