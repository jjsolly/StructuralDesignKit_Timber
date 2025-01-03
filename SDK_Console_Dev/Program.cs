using StructuralDesignKitLibrary.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDK = StructuralDesignKitLibrary;

namespace SDK_Console_Dev
{
	internal class Program
	{
		static void Main(string[] args)
		{


			MaterialPanelOSB OSB = new MaterialPanelOSB(MaterialPanelOSB.Grades.OSB3.ToString(), 18);

			Type type = OSB.GetType();
			var prop = type.GetProperties();

			foreach (var property in prop)
			{
				Console.WriteLine("{0} -> {1}", property.Name.ToString(), property.GetValue(OSB).ToString());
			}

			Console.WriteLine(OSB.Grade_Thickness.ToString());
			Console.ReadLine();
		}
	}

}