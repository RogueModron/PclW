#define CGAL_DISABLE_ROUNDING_MATH_CHECK


#include <CGAL/Simple_cartesian.h>

//#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
//typedef CGAL::Exact_predicates_inexact_constructions_kernel CgalKernel;

//#include <CGAL/Exact_predicates_exact_constructions_kernel.h>
//typedef CGAL::Exact_predicates_exact_constructions_kernel CgalKernel;

//#include <CGAL/Cartesian.h>
//typedef CGAL::Cartesian<double> CgalKernel;
//typedef CGAL::Cartesian<CGAL::Gmpq> CgalKernel;

//#include <CGAL/Filtered_Kernel.h>
//typedef CGAL::Cartesian<CGAL::Gmpq> CartesianKernel;
//typedef CGAL::Filtered_kernel<CartesianKernel> CgalKernel;





#include <CGAL/Boolean_set_operations_2.h>
#include <CGAL/Polygon_2.h>
#include <CGAL/Polygon_with_holes_2.h>
#include <CGAL/Polygon_set_2.h>





namespace PclWCgal{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;





typedef CGAL::Simple_cartesian<double> CgalKernel;

typedef CgalKernel::Point_2 CgalPoint;
typedef CGAL::Polygon_2<CgalKernel> CgalPolygon;
typedef CGAL::Polygon_with_holes_2<CgalKernel> CgalHoledPolygon;
typedef CGAL::Polygon_set_2<CgalKernel> CgalRegion;


public ref class CgalWrapper
{
public:
	static Region^ GetDifference(Region^ subject, Region^ clip)
	{
		return Clip(subject, clip, CgalClipType::cgalDifference);
	}

	static Region^ GetIntersection(Region^ subject, Region^ clip)
	{
		return Clip(subject, clip, CgalClipType::cgalIntersection);
	}

	static Region^ GetUnion(Region^ subject, Region^ clip)
	{
		return Clip(subject, clip, CgalClipType::cgalUnion);
	}

	static Region^ GetXor(Region^ subject, Region^ clip)
	{
		return Clip(subject, clip, CgalClipType::cgalXor);
	}

private:
    enum class CgalClipType
    {
        cgalDifference,
        cgalIntersection,
        cgalUnion,
        cgalXor
    };
    
    static Region^ Clip(Region^ subject, Region^ clip, CgalClipType operation)
    {
        CgalRegion cgalSubject;
        FromRegionToCgalRegion(subject, cgalSubject);
        CgalRegion cgalClip;
        FromRegionToCgalRegion(clip, cgalClip);
        
        switch (operation)
        {
            case CgalClipType::cgalDifference:
                cgalSubject.difference(cgalClip);
                break;
            case CgalClipType::cgalIntersection:
                cgalSubject.intersection(cgalClip);
                break;
            case CgalClipType::cgalUnion:
                cgalSubject.join(cgalClip);
                break;
            case CgalClipType::cgalXor:
                cgalSubject.symmetric_difference(cgalClip);
                break;
        }
        
        Region^ result = gcnew Region;
        FromCgalRegionToRegion(cgalSubject, result);
        return result;
    }
    
    static void FromRegionToCgalRegion(Region^ region, CgalRegion& cgalRegion)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for (Int32 i = 0; i < polygonSets->Count; i++)
        {
            CgalHoledPolygon cgalHoledPolygon;
            FromPolygonSetToCgalHoledPolygon(polygonSets->default[i],
                cgalHoledPolygon);
            cgalRegion.insert(cgalHoledPolygon);
        }
    }
    
    static void FromPolygonSetToCgalHoledPolygon(PolygonSet^ polygonSet,
        CgalHoledPolygon& cgalHoledPolygon)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        switch (polygons->Count)
        {
            case 1:
                FromPolygonToCgalPolygon(polygons->default[0], 
                    cgalHoledPolygon.outer_boundary());
                break;
            case 0:
                return;
            default:
                throw gcnew ArgumentException();
        }
        
        Collection<Polygon^>^ holes = polygonSet->Holes;
        if (holes->Count == 0)
        {
            return;
        }
        for (Int32 i = 0; i < holes->Count; i++)
        {
            CgalPolygon cgalHole;
            FromPolygonToCgalPolygon(holes->default[i], cgalHole);
            cgalHoledPolygon.add_hole(cgalHole);
        }
    }

    static void FromPolygonToCgalPolygon(Polygon^ polygon,
        CgalPolygon& cgalPolygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (Int32 i = 0; i < vertices->Count; i++)
        {
            Vertex vertex = vertices->default[i];
            Int32 x = Int32(vertex.X);
            Int32 y = Int32(vertex.Y);
            cgalPolygon.push_back(CgalPoint(x, y));
        }
    }
    
    static void FromCgalRegionToRegion(const CgalRegion& cgalRegion,
        Region^ region)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        std::list<CgalHoledPolygon> cgalHoledPolygons;
        cgalRegion.polygons_with_holes(std::back_inserter(cgalHoledPolygons));
        for (std::list<CgalHoledPolygon>::const_iterator holedPolygonIterator =
            cgalHoledPolygons.begin();
            holedPolygonIterator != cgalHoledPolygons.end();
            holedPolygonIterator++)
        {
            CgalHoledPolygon cgalHoledPolygon = *holedPolygonIterator;
            PolygonSet^ polygonSet = gcnew PolygonSet;
            FromCgalHoledPolygonToPolygonSet(cgalHoledPolygon, polygonSet);
            polygonSets->Add(polygonSet);
        }
    }
    
    static void FromCgalHoledPolygonToPolygonSet(
        const CgalHoledPolygon& cgalHoledPolygon, PolygonSet^ polygonSet)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        if (!cgalHoledPolygon.is_unbounded())
        {
            CgalPolygon cgalPolygon = cgalHoledPolygon.outer_boundary(); 
            Polygon^ polygon = gcnew Polygon;
            FromCgalPolygonToPolygon(cgalPolygon, polygon);
            polygons->Add(polygon);
        }
        
        Collection<Polygon^>^ holes = polygonSet->Holes;
        for (CgalHoledPolygon::Hole_const_iterator holesIterator =
            cgalHoledPolygon.holes_begin();
            holesIterator != cgalHoledPolygon.holes_end();
            holesIterator++)
        {
            CgalPolygon cgalPolygon = *holesIterator;
            Polygon^ polygon = gcnew Polygon;
            FromCgalPolygonToPolygon(cgalPolygon, polygon);
            holes->Add(polygon);
        }
    }
    
    static void FromCgalPolygonToPolygon(const CgalPolygon& cgalPolygon,
        Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (CgalPolygon::Vertex_const_iterator vertexIterator =
            cgalPolygon.vertices_begin();
            vertexIterator != cgalPolygon.vertices_end();
            vertexIterator++)
        {
            CgalPoint cgalPoint = *vertexIterator;

            double x = CGAL::to_double(cgalPoint.x());
            double y = CGAL::to_double(cgalPoint.y());

            //double x = CGAL::to_double(cgalPoint.x().exact());
            //double y = CGAL::to_double(cgalPoint.y().exact());

            vertices->Add(Vertex(x, y));
        }
    }
};

}