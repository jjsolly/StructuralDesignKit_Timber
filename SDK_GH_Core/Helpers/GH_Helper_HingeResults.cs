using Dlubal.WS.Rfem6.Model;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using SDK = StructuralDesignKitLibrary;

namespace StructuralDesignKitGH_Core.Helpers
{
	/// <summary>
	/// Object to consolidate the data necessary to check results on line hinge
	/// </summary>
	public class GH_Helper_HingeResults
	{
		public int ID { get; set; }
		public int SurfaceID { get; set; }
		public int LineID { get; set; }
		public Line BaseLine { get; set; }
		public List<double> PositionParams = new List<double>();
		public List<Point3d> Positions = new List<Point3d>();
		public List<int> LC = new List<int>();
		public List<string> LC_Names = new List<string>();
		public List<SDK.EC5.EC5_Utilities.LoadDuration> loadDuration = new List<SDK.EC5.EC5_Utilities.LoadDuration>();
		public List<List<double>> N = new List<List<double>>();
		public List<List<double>> Vy = new List<List<double>>();
		public List<List<double>> Vz = new List<List<double>>();
		public List<List<double>> N_Vy_Combined = new List<List<double>>();
		public List<List<double>> Mx = new List<List<double>>();
		public List<double> N_Average = new List<double>();
		public List<double> Vy_Average = new List<double>();
		public List<double> Vz_Average = new List<double>();
		public List<double> N_Vy_Combined_Average = new List<double>();
		public List<double> Mx_Average = new List<double>();
		public Plane Orientation = new Plane();


		//public GH_Helper_HingeResults(int id, Line line, int surfaceID, int lineID)
		//{
		//	ID = id;
		//	BaseLine = line;
		//	SurfaceID = surfaceID;
		//	LineID = lineID;
		//}

		public GH_Helper_HingeResults(int id, int surfaceID, int lineID)
		{
			ID = id;
			SurfaceID = surfaceID;
			LineID = lineID;
		}


		public void UpdateLine(List<line> lines, List<node> nodes, List<SurfaceEdge> srfEdges)
		{
			line L = lines.Where(p => p.no == LineID).First();
			
			Line tempLine = ConvertLineRFEM_ToRhino(L, nodes);

			Point3d midTempLine = tempLine.ToNurbsCurve().PointAtNormalizedLength(0.5);

			List<Point3d> midPoints = new List<Point3d>();
			SurfaceEdge edgesList = srfEdges.Where(p=>p.SrfID == this.SurfaceID).First();
			foreach(Curve crv in edgesList.Edges)
			{
				midPoints.Add(crv.PointAtNormalizedLength(0.5));
			}


			int index = midPoints.IndexOf(midTempLine);
			Line line = new Line(edgesList.Edges[index].PointAtStart, edgesList.Edges[index].PointAtEnd);
			this.BaseLine = line;
		}

		public void UpdateOrientation(List<surface> surfaces, List<line> lines, List<node> nodes)
		{
			surface srf = surfaces.Where(p => p.no == SurfaceID).First();
			Surface RhinoSrf = ConvertRFEMSurfaceToRhinoSurface(srf, lines, nodes);

			Curve hingeLineCrv = this.BaseLine.ToNurbsCurve();

			//define middle point on curve
			Point3d pt = hingeLineCrv.PointAtNormalizedLength(0.5);
			Plane plane;

			double t;

			hingeLineCrv.ClosestPoint(pt, out t);
			
			double[] d = { 0, t };

			this.Orientation = hingeLineCrv.GetPerpendicularFrames(d)[1];

			//this.Orientation = new Plane(pt, this.BaseLine.Direction, RhinoSrf.NormalAt(0, 0));
			//Orientation.Rotate(Math.PI / 2, Orientation.YAxis);
			//Orientation.Rotate(Math.PI, Orientation.ZAxis);
		}

		public void UpdatePositions()
		{
			Curve lineCrv = this.BaseLine.ToNurbsCurve();
			foreach (double p in this.PositionParams)
			{

				Positions.Add(lineCrv.PointAt(p));
			}
		}


		public static List<GH_Helper_HingeResults> CreateHinges(line_hinge hinge)
		{
			int id = 0;
			////"1/1-4; 2/3,6-8; 3/7,9-11"
			List<GH_Helper_HingeResults> results = new List<GH_Helper_HingeResults>();

			List<string> surfaceAttribution = hinge.assigned_to.Split(';').ToList();
			foreach (string s in surfaceAttribution)
			{
				List<int> lineID = new List<int>();

				List<string> split1 = s.Split('/').ToList();

				int surfaceID = Int32.Parse(split1[0]);

				List<string> split2 = split1[1].Split(',').ToList();


				foreach (string s1 in split2)
				{
					List<string> StartEnd = s1.Split('-').ToList();
					if (StartEnd.Count == 1)
					{
						lineID.Add(Int32.Parse(StartEnd[0]));
						id++;
					}
					else
					{
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
					results.Add(new GH_Helper_HingeResults(id, surfaceID, line));
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
