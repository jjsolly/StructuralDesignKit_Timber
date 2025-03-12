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

namespace StructuralDesignKitGH
{
	public class GH_RFEM_TimberHingeDesign : GH_Component
	{
		/// <summary>
		/// Initializes a new instance of the GH_RFEM_TimberHingeDesign class.
		/// </summary>
		public GH_RFEM_TimberHingeDesign()
		  : base("TimberHinge", "T_Hinge",
			  "Get the forces on surface hinge and compute the fastener design",
			  "SDK", "RFEM 6")
		{
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("RFEM Model", "model", "RFEM model obtained from the getModel component", GH_ParamAccess.item);
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddLineParameter("outline", "outline", "outline", GH_ParamAccess.list);
			pManager.AddPlaneParameter("Frame", "Frame", "Frame", GH_ParamAccess.list);
			pManager.AddPointParameter("Positions", "Positions", "Positions", GH_ParamAccess.list);


		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			ModelClient model = null;

			List<Line> OutputLines = new List<Line>();
			List<Plane> frames = new List<Plane>();


			List<HingeResults> results = new List<HingeResults>();
			List<HingeResults> hingeResults = new List<HingeResults>();

			DA.GetData(0, ref model);

			try
			{



				//get object info
				var listLinesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_LINE);
				var listSurfacesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_SURFACE);
				var listHingesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_LINE_HINGE);
				var listNodesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_NODE);

				List<node> nodes = new List<node>();
				List<line> lines = new List<line>();
				List<surface> surfaces = new List<surface>();
				List<line_hinge> hinges = new List<line_hinge>();

				foreach (var item in listNodesID)
				{
					nodes.Add(model.get_node(item.no));
				}

				foreach (var item in listLinesID)
				{
					lines.Add(model.get_line(item.no));
				}

				foreach (var item in listSurfacesID)
				{
					surfaces.Add(model.get_surface(item.no));
				}

				foreach (var item in listHingesID)
				{
					hinges.Add(model.get_line_hinge(item.no));
				}




				foreach (line_hinge hinge in hinges)
				{
					hingeResults = HingeResults.CreateHinges(hinge);

					foreach (HingeResults h in hingeResults)
					{
						h.UpdateLine(lines, nodes);
					}
				}


				bool modelHasAnyResults = model.has_any_results();

				if (modelHasAnyResults)
				{

					List<object_location> locationsList = new List<object_location>();

					foreach (var item in listHingesID)
					{

						object_location loc = new object_location();
						loc.no = item.no;
						loc.type = object_types.E_OBJECT_TYPE_LINE_HINGE;
						locationsList.Add(loc);
					}
					object_location[] locations = locationsList.ToArray();






					//Get load combinations ULS

					var LoadCombiID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_LOAD_COMBINATION);
					var designSituationID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_DESIGN_SITUATION);

					List<int> designSituationULS = new List<int>();

					List<load_combination> ULSLoadCombinations = new List<load_combination>();

					foreach (var item in designSituationID)
					{
						design_situation D = model.get_design_situation(item.no);
						if (D.design_situation_type == "DESIGN_SITUATION_TYPE_STR_PERMANENT_AND_TRANSIENT_6_10") designSituationULS.Add(item.no);
					}


					foreach (var item in LoadCombiID)
					{
						load_combination combi = model.get_load_combination(item.no);
						if (designSituationULS.Contains(combi.design_situation)) ULSLoadCombinations.Add(combi);
					}



					//Update forces in hinge results

					foreach (load_combination item in ULSLoadCombinations)
					{
						if (model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION, item.no))
						{

							var hingeForces = model.get_results_for_line_hinges_forces(case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION, item.no, locations);


							List<line_hinges_forces_row> cleanedHF = new List<line_hinges_forces_row>();


							//Removes the last rows of each line hinge values where info such as Extreme values, average and so on are stored
							for (int i = 0; i < hingeForces.Length; i++)
							{
								if (hingeForces[i].description == "Extremes") i += 12;
								else if (hingeForces[i].description == "Total max/min values with corresponding values") break;

								else cleanedHF.Add(hingeForces[i]);
							}


							//Populate the result objects
							foreach (var result in cleanedHF)
							{
								HingeResults hinge = hingeResults.Where(p => p.SurfaceID == result.row.surface_no).Where(p => p.LineID == result.row.line_no).First();

								if (!hinge.Positions.Contains(result.row.location)) hinge.Positions.Add(result.row.location);

								if (hinge.LC.Contains(item.no))
								{
									int index = hinge.LC.IndexOf(item.no);
									hinge.N[index].Add(result.row.line_hinge_force_n);
									hinge.Vy[index].Add(result.row.line_hinge_force_v_y);
									hinge.Vz[index].Add(result.row.line_hinge_force_v_z);
									hinge.Mx[index].Add(result.row.line_hinge_moment_m_x);
								}
								else
								{
									hinge.LC.Add(item.no);
									int index = hinge.LC.IndexOf(item.no);
									hinge.N.Add(new List<double>());
									hinge.Vy.Add(new List<double>());
									hinge.Vz.Add(new List<double>());
									hinge.Mx.Add(new List<double>());

									hinge.N[index].Add(result.row.line_hinge_force_n);
									hinge.Vy[index].Add(result.row.line_hinge_force_v_y);
									hinge.Vz[index].Add(result.row.line_hinge_force_v_z);
									hinge.Mx[index].Add(result.row.line_hinge_moment_m_x);
								}

							}
						}
					}



					//Function to correct the positions
					//It appears that RFEM API does not always return the correct position of the value
					//In some cases (still unclear when), the second position is skipped, which creates prositions which are longer than the line itself.
					foreach (HingeResults H in hingeResults)
					{
						if (H.Positions.Last() > H.BaseLine.Length)
						{
							double missingPosition = H.Positions[1] / 2;
							List<double> fixedPosition = new List<double>();
							H.Positions.Insert(1, missingPosition);

							H.Positions.RemoveAt(H.Positions.Count - 1);
						}


						//Update hinge orientation:
						H.UpdateOrientation(surfaces, lines, nodes);

					}



				}
				else
				{
					Console.WriteLine("Model has no results");
				}


				foreach (var H in hingeResults)
				{
					OutputLines.Add(H.BaseLine);
					frames.Add(H.Orientation);
				}

				model.Close();

			}

			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);


			}


			DA.SetDataList(0,OutputLines);
			DA.SetDataList(1,frames);

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
			get { return new Guid("8344A7B2-0BDD-4474-9C45-23B295931B20"); }
		}








	}





	/// <summary>
	/// Object to consolidate the data necessary to check results on line hinge
	/// </summary>
	public class HingeResults
	{
		public int ID { get; set; }
		public int SurfaceID { get; set; }
		public int LineID { get; set; }
		public Line BaseLine { get; set; }
		public List<double> Positions = new List<double>();
		public List<int> LC = new List<int>();
		public List<string> LC_Names = new List<string>();
		public List<SDK.EC5.EC5_Utilities.LoadDuration> loadDuration = new List<SDK.EC5.EC5_Utilities.LoadDuration>();
		public List<List<double>> N = new List<List<double>>();
		public List<List<double>> Vy = new List<List<double>>();
		public List<List<double>> Vz = new List<List<double>>();
		public List<List<double>> Mx = new List<List<double>>();
		public Plane Orientation = new Plane();


		public HingeResults(int id, Line line, int surfaceID, int lineID)
		{
			ID = id;
			BaseLine = line;
			SurfaceID = surfaceID;
			LineID = lineID;
		}

		public HingeResults(int id, int surfaceID, int lineID)
		{
			ID = id;
			SurfaceID = surfaceID;
			LineID = lineID;
		}


		public void UpdateLine(List<line> lines, List<node> nodes)
		{
			line L = lines.Where(p => p.no == LineID).First();
			this.BaseLine = ConvertLineRFEM_ToRhino(L, nodes);
		}

		public void UpdateOrientation(List<surface> surfaces, List<line> lines, List<node> nodes)
		{
			surface srf = surfaces.Where(p => p.no == SurfaceID).First();
			Surface RhinoSrf = ConvertRFEMSurfaceToRhinoSurface(srf, lines, nodes);

			Curve hingeLineCrv = this.BaseLine.ToNurbsCurve();


			this.Orientation = new Plane(hingeLineCrv.PointAt(0), this.BaseLine.Direction, RhinoSrf.NormalAt(0, 0));


		}


		public static List<HingeResults> CreateHinges(line_hinge hinge)
		{
			int id = 0;
			////"1/1-4; 2/3,6-8; 3/7,9-11"
			List<HingeResults> results = new List<HingeResults>();

			List<string> surfaceAttribution = hinge.assigned_to.Split(';').ToList();
			foreach (string s in surfaceAttribution)
			{
				List<int> lineID = new List<int>();

				List<string> split1 = s.Split('/').ToList();

				int surfaceID = Int32.Parse(split1[0]);

				List<string> split2 = split1[1].Split(',').ToList();


				foreach (string s1 in split2)
				{
					if (s1.Length == 1)
					{
						lineID.Add(Int32.Parse(s1));
						id++;
					}
					else
					{
						List<string> StartEnd = s1.Split('-').ToList();
						int start = Int32.Parse(StartEnd[0]);
						int end = Int32.Parse(StartEnd[1]);
						for (int i = start; i <= end; i++)
						{
							lineID.Add(i);
						}
					}
				}


				foreach (int line in lineID)
				{
					results.Add(new HingeResults(id, surfaceID, line));
					id += 1;
				}

			}
			return results;
		}

		public static Line ConvertLineRFEM_ToRhino(line L, List<node> nodes)
		{



			node startNode = nodes.Where(p => p.no == L.definition_nodes[0]).First();
			node EndNode = nodes.Where(p => p.no == L.definition_nodes[1]).First();

			Point3d startPoint = new Point3d(startNode.global_coordinate_1, startNode.global_coordinate_2, startNode.global_coordinate_3);
			Point3d endPoint = new Point3d(EndNode.global_coordinate_1, EndNode.global_coordinate_2, EndNode.global_coordinate_3);


			return new Line(startPoint, endPoint);

		}

		public static Surface ConvertRFEMSurfaceToRhinoSurface(surface srf, List<line> lines, List<node> nodes)
		{

			List<Curve> curves = new List<Curve>();
			foreach (int i in srf.boundary_lines)
			{
				curves.Add(ConvertLineRFEM_ToRhino(lines.Where(p => p.no == i).First(), nodes).ToNurbsCurve());
			}

			return Brep.CreateEdgeSurface(curves).Faces[0].DuplicateSurface();
		}
	}







}