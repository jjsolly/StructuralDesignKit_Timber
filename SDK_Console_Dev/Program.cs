using StructuralDesignKitLibrary.Connections.Fasteners;
using StructuralDesignKitLibrary.Connections.SteelTimberShear;
using StructuralDesignKitLibrary.Connections.TimberTimberShear;
using StructuralDesignKitLibrary.EC5;
using StructuralDesignKitLibrary.Materials;
using StructuralDesignKitLibrary.RFEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDK = StructuralDesignKitLibrary;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;
using StructuralDesignKitLibrary.CrossSections;

namespace SDK_Console_Dev
{
	internal class Program
	{
		static void Main(string[] args)
		{


			//MaterialPanelOSB OSB = new MaterialPanelOSB(MaterialPanelOSB.Grades.OSB3.ToString(), 18);
			//MaterialTimberHardwood hardWood = new MaterialTimberHardwood(MaterialTimberHardwood.Grades.D30);
			//MaterialTimberSoftwood softWood = new MaterialTimberSoftwood(MaterialTimberSoftwood.Grades.C24);
			//MaterialTimberGlulam glulam= new MaterialTimberGlulam(MaterialTimberGlulam.Grades.GL32h);

			//FastenerNail Nail = new FastenerNail(FastenerNail.NailTypes.AnnularRing,5,57 , 600, true,0,0,3.64,13.53,100,4680);


			//var connection1 = new TimberTimberSingleShear(Nail, OSB, 22, 0,softWood, 100, 0, true);

			//double nef = Nail.ComputeEffectiveNumberOfFastener(10, 100, 45);

			//Nail.ComputeSpacings(45, connection1);
			//Nail.ComputeMinimumMinMemberThickness(connection1.Timber2.RhoK, true);

			//Type type = Nail.GetType();
			//var prop = type.GetProperties();

			//foreach (var property in prop)
			//{
			//	Console.WriteLine("{0} -> {1}", property.Name.ToString(), property.GetValue(Nail).ToString());
			//}

			//Console.WriteLine("nef -> {0}", nef);

			//for (int i = 0; i < connection1.Capacities.Count; i++)
			//{
			//	Console.WriteLine("{0} -> {1:0.0}", connection1.FailureModes[i], connection1.Capacities[i]);

			//}

			//double kmod = EC5_Utilities.KmodFromDifferentWood(connection1.Timber1, connection1.Timber2, EC5_Utilities.ServiceClass.SC2, EC5_Utilities.LoadDuration.ShortTerm_Instantaneous);

			//Console.WriteLine("Kmod -> {0}\nFvrd -> {1:0.000}", kmod, connection1.Capacity / 1000 / 1.3 * kmod);


			//var stiffness = connection1.Kser;
			//Console.WriteLine("Kser -> {0}", stiffness);



			////for (int i = 1; i < 20; i++)
			////{
			////	bool predrilled = false;
			////	if (i>6) predrilled = true;



			////             Console.WriteLine("Diam = {1} -> Fhk = {0}", Nail.Fhk, i);
			////}

			////Console.WriteLine(OSB.Grade_Thickness.ToString());


			//var softWood = new MaterialTimberSoftwood(MaterialTimberSoftwood.Grades.C24);

			//double diam = 4.5;
			//double spacings = 100;
			//spacings /= 1000;

			//FastenerNail Nail = new FastenerNail(FastenerNail.NailTypes.AnnularRing, diam, 57, 600, true, 0, 0, 3.64, 13.53, 100, 4680);
			//var connection1 = new TimberTimberSingleShear(Nail, softWood, 40, 0, softWood, 40, 0, true);
			//var connection2 = new SingleOuterSteelPlate(Nail, 6, 0, softWood, 40, true);

			//         Console.WriteLine("Timber/timber");
			//Console.WriteLine("Fvk = {0:0.00}kN/m", connection1.Capacity * 0.6 / 1.3 / spacings/1000);
			//Console.WriteLine("Kser = {0:0.00}kN/m", connection1.Kser/ spacings);

			//Console.WriteLine("Steel/timber");
			//Console.WriteLine("Fvk = {0:0.00}kN/m", connection2.Capacity * 0.6 / 1.3 / spacings / 1000);
			//Console.WriteLine("Kser = {0:0.00}kN/m", connection2.Kser / spacings);





			//var mat = new MaterialCLT(MaterialCLT.ProducerGrade.KLH);

			//List<double> thicknesses = new List<double> { 20, 20, 20, 20, 20 };
			//List<int> orientations = new List<int> { 0,90,0,90,0};
			//List<MaterialCLT> mats = new List<MaterialCLT>();
			//foreach (int i in thicknesses) mats.Add(mat);

			//var layup = new CLT_Layup(thicknesses, orientations, mats);

			//Console.WriteLine("CoG X : {0:0.0}mm", layup.CS_X.CenterOfGravity);
			//Console.WriteLine("CoG Y: {0:0.0}mm", layup.CS_Y.CenterOfGravity);


			//for (int i = 0; i < layup.CS_X.S0Net.Count; i++)
			//{
			//	for (int j = 0; j< layup.CS_X.S0Net[i].Count; j++)
			//	{
			//                 Console.WriteLine("Layer {0} point {1} -> S0Net = {2:0.0}",i,j, layup.CS_X.S0Net[i][j]);
			//	}
			//}


			//layup.CS_X.ComputeShearStress(13500);
			//for (int i = 0; i < layup.CS_X.TauV.Count; i++)
			//{
			//	for (int j = 0; j < layup.CS_X.TauV[i].Count; j++)
			//	{
			//		Console.WriteLine("Layer {0} point {1} -> Tau = {2:0.000}", i, j, layup.CS_X.TauV[i][j]);
			//	}
			//}


			var BM1 = new MaterialTimberSoftwood(MaterialTimberSoftwood.Grades.C24);
			var BM2 = new MaterialTimberHardwood(MaterialTimberHardwood.Grades.D30);
			var LC = new MaterialTimberGlulam(MaterialTimberGlulam.Grades.GL24h);
			var Baubuche = new MaterialTimberBaubuche(MaterialTimberBaubuche.Grades.GL75h_Cl1);

			List<IMaterial> Materials = new List<IMaterial>() { BM1, BM2, LC, Baubuche };

			foreach (var material in Materials) {
				double kv = EC5_Factors.Kv(material, 300, 150, 70,0);
				Console.WriteLine("Kv = " + kv.ToString());
			}


			Console.ReadLine();
		}
	}

}