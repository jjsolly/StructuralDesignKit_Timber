using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using SDK = StructuralDesignKitLibrary;
using StructuralDesignKitLibrary.RFEM;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;
using System.Linq;
using StructuralDesignKitGH_Core.Helpers;
using Grasshopper.Kernel.Types.Transforms;

namespace StructuralDesignKitGH_Core
{
	public class GH_RFEM_TimberHingeDisplay : GH_Component
	{
		/// <summary>
		/// Initializes a new instance of the TimberHingeDisplay class.
		/// </summary>
		public GH_RFEM_TimberHingeDisplay()
		  : base("DisplayTimberHinge", "Display_T_Hinge",
			  "Display the forces from the surface hinges",
			  "SDK", "RFEM6")
		{
			//Needed to allow the Value List to be updated
			this.Params.ParameterSourcesChanged += new GH_ComponentParamServer.ParameterSourcesChangedEventHandler(this.ParamSourcesChanged);

		}

		//Script to change the value of the valueList connected to the component - needs to be refactored in a global GH utility to be reused on other components
		private void ParamSourcesChanged(object sender, GH_ParamServerEventArgs e)
		{
			List<string> keys = new List<string>()
			{
				"N",
				"Vy",
				"Vz",
				"√(N²+Vy²)",
				"Mx"
			};

			List<int> value = new List<int>() { 0, 1, 2, 3, 4 };


			if ((e.ParameterSide == GH_ParameterSide.Input) && (e.ParameterIndex == 1))
			{

				// Update Value Lists
				foreach (IGH_Param source in e.Parameter.Sources)
				{
					if (source is Grasshopper.Kernel.Special.GH_ValueList)
					{

						Grasshopper.Kernel.Special.GH_ValueList valueList = source as Grasshopper.Kernel.Special.GH_ValueList;

						// Clear the contents of ValueList
						string selectedItem = valueList.SelectedItems[0].Name;

						valueList.ListItems.Clear();

						int index = 0;
						// Add double-cotation to item
						string addQuotation;
						for (int i = 0; i < value.Count; i++)
						{
							addQuotation = "\"" + value[i] + "\"";
							// Add item to ValueList
							valueList.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(keys[i], addQuotation));
							//search for the previously selected item
							if (selectedItem == keys[i])
							{
								index = i;
							}
						}
						// Reset Selection
						valueList.SelectItem(index);
						valueList.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;
						valueList.ExpireSolution(false);

					}
				}
			}
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("Surface Hinge objects", "Hinges", "Surface hinges from the component SDK GetTimberHinge", GH_ParamAccess.list);
			pManager.AddIntegerParameter("Force", "F", "Force to display", GH_ParamAccess.item);
			pManager.AddNumberParameter("Scale factor", "S", "Scale factor for results display", GH_ParamAccess.item);
			pManager.AddIntegerParameter("Load case index", "LC", "Loadcase to consider", GH_ParamAccess.item);
			pManager.AddBooleanParameter("Average", "A", "Average force on the hinge line", GH_ParamAccess.item);

			pManager[1].Optional = true;
			pManager[2].Optional = true;
			pManager[3].Optional = true;
			pManager[3].Optional = true;
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddGeometryParameter("ForceDisplay", "F", "Mesh representing the force", GH_ParamAccess.list);
			pManager.AddNumberParameter("ForceValue", "V", "force or moment in  8kN] or [kN.m]", GH_ParamAccess.list);
			pManager.AddTextParameter("LoadCase", "LC", "Load case description", GH_ParamAccess.item);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{


			//input
			int forceSelection = 0;
			int LCID = 0;
			double scale = 1;
			bool average = false;
			List<GH_Helper_HingeResults> hinges = new List<GH_Helper_HingeResults>();


			DA.GetDataList(0, hinges);
			DA.GetData(1, ref forceSelection);
			DA.GetData(2, ref scale);
			DA.GetData(3, ref LCID);
			DA.GetData(4, ref average);




			//output
			List<Mesh> resultMeshes = new List<Mesh>();
			List<double> resultValues = new List<double>();
			string LCDescription = null;

			foreach (GH_Helper_HingeResults H in hinges)
			{
				//Display N

				if (H.LC.Contains(LCID))
				{
					int index = H.LC.IndexOf(LCID);

					LCDescription = H.LC_Names[index];


					//case N
					if (forceSelection == 0)
					{
						for (int i = 0; i < H.Positions.Count - 1; i++)
						{
							double value1 = 0;
							double value2 = 0;

							if (average)
							{
								value1 = H.N_Average[index] / 1000;
								value2 = H.N_Average[index] / 1000;
							}
							else
							{
								value1 = H.N[index][i] / 1000;
								value2 = H.N[index][i + 1] / 1000;
							}


							Point3d pt1 = H.Positions[i];
							Point3d pt2 = H.Positions[i + 1];

							resultMeshes.Add(ComputeMesh(pt1, pt2, value1, value2, H.Orientation.YAxis, scale));
						}

						foreach (double v in H.N[index]) resultValues.Add(v / 1000);
					}

					//case Vy
					else if (forceSelection == 1)
					{
						for (int i = 0; i < H.Positions.Count - 1; i++)
						{
							double value1 = 0;
							double value2 = 0;

							if (average)
							{
								value1 = H.Vy_Average[index] / 1000;
								value2 = H.Vy_Average[index] / 1000;
							}
							else
							{
								value1 = H.Vy[index][i] / 1000;
								value2 = H.Vy[index][i + 1] / 1000;
							}

							Point3d pt1 = H.Positions[i];
							Point3d pt2 = H.Positions[i + 1];

							resultMeshes.Add(ComputeMesh(pt1, pt2, value1, value2, H.Orientation.YAxis, scale));
						}

						foreach (double v in H.Vy[index]) resultValues.Add(v / 1000);
					}

					//case Vz
					else if (forceSelection == 2)
					{
						for (int i = 0; i < H.Positions.Count - 1; i++)
						{
							double value1 = 0;
							double value2 = 0;

							if (average)
							{
								value1 = H.Vz_Average[index] / 1000;
								value2 = H.Vz_Average[index] / 1000;
							}
							else
							{
								value1 = H.Vz[index][i] / 1000;
								value2 = H.Vz[index][i + 1] / 1000;
							}

							Point3d pt1 = H.Positions[i];
							Point3d pt2 = H.Positions[i + 1];

							resultMeshes.Add(ComputeMesh(pt1, pt2, value1, value2, H.Orientation.XAxis, scale));
						}

						foreach (double v in H.Vz[index]) resultValues.Add(v / 1000);
					}

					//case √(N²+Vy²)
					else if (forceSelection == 3)
					{
						for (int i = 0; i < H.Positions.Count - 1; i++)
						{
							double value1 = 0;
							double value2 = 0;

							if (average)
							{
								value1 = H.N_Vy_Combined_Average[index] / 1000;
								value2 = H.N_Vy_Combined_Average[index] / 1000;
							}
							else
							{
								value1 = H.N_Vy_Combined[index][i] / 1000;
								value2 = H.N_Vy_Combined[index][i + 1] / 1000;
							}

							Point3d pt1 = H.Positions[i];
							Point3d pt2 = H.Positions[i + 1];

							Vector3d V = H.Orientation.YAxis;
							V.Unitize();
							resultMeshes.Add(ComputeMesh(pt1, pt2, value1, value2, V, scale));
						}

						foreach (double v in H.N_Vy_Combined[index]) resultValues.Add(v / 1000);
					}

					//case Mx
					else if (forceSelection == 4)
					{
						for (int i = 0; i < H.Positions.Count - 1; i++)
						{
							double value1 = 0;
							double value2 = 0;

							if (average)
							{
								value1 = H.Mx_Average[index] / 1000;
								value2 = H.Mx_Average[index] / 1000;
							}
							else
							{
								value1 = H.Mx[index][i] / 1000;
								value2 = H.Mx[index][i + 1] / 1000;
							}

							Point3d pt1 = H.Positions[i];
							Point3d pt2 = H.Positions[i + 1];

							resultMeshes.Add(ComputeMesh(pt1, pt2, value1, value2, H.Orientation.YAxis, scale));
						}

						foreach (double v in H.Mx[index]) resultValues.Add(v / 1000);
					}

					else throw new Exception("Force type note correctly defined- 0-> N, 1-> Vy, 2-> Vz, 3->√(N²+Vy²), 4-> Mx\n Plug a value List");

				}
				else throw new Exception("Load case not available");


			}





			DA.SetDataList(0, resultMeshes);
			DA.SetDataList(1, resultValues);
			DA.SetData(2, LCDescription);



		}

		/// <summary>
		/// Provides an Icon for the component.
		/// </summary>
		protected override System.Drawing.Bitmap Icon
		{
			get
			{
				//You can add image files to your project resources and access them like this:
				// return Resources.IconForThisComponent;
				return null;
			}
		}

		/// <summary>
		/// Gets the unique ID for this component. Do not change this ID after release.
		/// </summary>
		public override Guid ComponentGuid
		{
			get { return new Guid("CD48228F-4085-477C-90AC-A4242F739A81"); }
		}

		private Mesh ComputeMesh(Point3d pt1, Point3d pt2, double v1, double v2, Vector3d dir, double scale)
		{
			Mesh mesh = new Mesh();

			Vector3d V1 = dir;
			V1 *= scale * v1;

			Vector3d V2 = dir;
			V2 *= scale * v2;

			Transform T1 = Transform.Translation(V1);
			Transform T2 = Transform.Translation(V2);

			Point3d pt3 = new Point3d(pt1);
			Point3d pt4 = new Point3d(pt2);

			pt3.Transform(T1);
			pt4.Transform(T2);


			mesh.Vertices.AddVertices(new List<Point3d>() { pt1, pt2, pt4, pt3 });
			mesh.Faces.AddFace(new MeshFace(0, 1, 2, 3));

			return mesh;
		}
	}
}