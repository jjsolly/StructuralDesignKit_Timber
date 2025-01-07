using StructuralDesignKitLibrary.Connections.Fasteners;
using StructuralDesignKitLibrary.Connections.SteelTimberShear;
using StructuralDesignKitLibrary.Connections.TimberTimberShear;
using StructuralDesignKitLibrary.EC5;
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
			MaterialTimberHardwood hardWood = new MaterialTimberHardwood(MaterialTimberHardwood.Grades.D30);
			MaterialTimberSoftwood softWood = new MaterialTimberSoftwood(MaterialTimberSoftwood.Grades.C24);
			MaterialTimberGlulam glulam= new MaterialTimberGlulam(MaterialTimberGlulam.Grades.GL32h);

			FastenerNail Nail = new FastenerNail(FastenerNail.NailTypes.AnnularRing,5,57 , 600, true,0,0,3.64,13.53,100,4680);


			var connection1 = new TimberTimberSingleShear(Nail, OSB, 22, 0,softWood, 100, 0, true);

			double nef = Nail.ComputeEffectiveNumberOfFastener(10, 100, 45);

			Nail.ComputeSpacings(45, connection1);
			Nail.ComputeMinimumMinMemberThickness(connection1.Timber2.RhoK, true);

			Type type = Nail.GetType();
			var prop = type.GetProperties();

			foreach (var property in prop)
			{
				Console.WriteLine("{0} -> {1}", property.Name.ToString(), property.GetValue(Nail).ToString());
			}

			Console.WriteLine("nef -> {0}", nef);

			for (int i = 0; i < connection1.Capacities.Count; i++)
			{
				Console.WriteLine("{0} -> {1:0.0}", connection1.FailureModes[i], connection1.Capacities[i]);

			}

			double kmod = EC5_Utilities.KmodFromDifferentWood(connection1.Timber1, connection1.Timber2, EC5_Utilities.ServiceClass.SC2, EC5_Utilities.LoadDuration.ShortTerm_Instantaneous);

			Console.WriteLine("Kmod -> {0}\nFvrd -> {1:0.000}", kmod, connection1.Capacity / 1000 / 1.3 * kmod);


			var stiffness = connection1.Kser;
			Console.WriteLine("Kser -> {0}", stiffness);



			//for (int i = 1; i < 20; i++)
			//{
			//	bool predrilled = false;
			//	if (i>6) predrilled = true;



			//             Console.WriteLine("Diam = {1} -> Fhk = {0}", Nail.Fhk, i);
			//}

			//Console.WriteLine(OSB.Grade_Thickness.ToString());


			Console.ReadLine();
		}
	}

}