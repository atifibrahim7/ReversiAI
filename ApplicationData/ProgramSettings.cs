using System;
using System.Drawing;
using System.Xml;


namespace FullSailAFI.GamePlaying
{
	/// <summary>
	/// Summary description for ProgramSettings.
	/// </summary>
	public class ProgramSettings
	{
		// The file name used to save the program settings.
		private string fileName;

		// An Xml document used to store and save program settings.
		private XmlDocument document;

		public ProgramSettings(string fileName)
		{
			//
			// TODO: Add constructor logic here
			//

			// Assign the file name.
			this.fileName = fileName;

			// If the file already exists, load it. Otherwise, create a new document.
			this.document = new XmlDocument();
			try
			{
				document.Load(this.fileName);
			}
			catch (Exception ex)
			{
				// Create a new XML document and set the root node.
//                Console.WriteLine(ex.Message);
				document = new XmlDocument();
				document.AppendChild(document.CreateElement("ProgramSettings"));
                ex.GetType();  // just shutting up a warning here about ex never being used
			}
		}

		//
		// Saves the settings XML file.
		//
		public void Save()
		{
			this.document.Save(this.fileName);
		}

		//
		// Reads a value from the settings file.
		//
		public string GetValue(string section, string name)
		{
			try
			{
                if (this.document.DocumentElement.SelectSingleNode(section + "/" + name) == null)
                    return "";
				return this.document.DocumentElement.SelectSingleNode(section + "/" + name).InnerText;
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
				return "";
			}
		}

		//
		// Writes a value to the settings file.
		//
		public void SetValue(string section, string name, string value)
		{
			// If the section does not exist, create it.
			XmlNode sectionNode = this.document.DocumentElement.SelectSingleNode(section);
			if (sectionNode == null)
				sectionNode = this.document.DocumentElement.AppendChild(this.document.CreateElement(section));
			// If the node does not exist, create it.
			XmlNode node = sectionNode.SelectSingleNode(name);
			if (node == null)
				node = sectionNode.AppendChild(this.document.CreateElement(name));

			// Set the value.
			node.InnerText = value;
		}
	}
}
