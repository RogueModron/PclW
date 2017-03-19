#include "Boost\Polygon\Polygon.hpp"

#if MPIR
    #include "Boost\Polygon\gmp_override.hpp"
#endif





namespace PclWGtl{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;

namespace gtl = boost::polygon;
using namespace gtl;
using namespace gtl::operators;





typedef gtl::polygon_data<int> GtlPolygon;
typedef gtl::polygon_with_holes_data<int> GtlHoledPolygon;
typedef gtl::polygon_traits<GtlHoledPolygon>::point_type GtlPoint;
typedef std::vector<GtlPoint> GtlPoints;
typedef std::vector<GtlPolygon> GtlPolygonSet;
typedef std::vector<GtlHoledPolygon> GtlRegion;


public ref class GtlWrapper
{
public:
    static Region^ GetDifference(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GtlClipType::gtlDifference);
    }

    static Region^ GetIntersection(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GtlClipType::gtlIntersection);
    }

    static Region^ GetUnion(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GtlClipType::gtlUnion);
    }

    static Region^ GetXor(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GtlClipType::gtlXor);
    }

private:
    static const double SCALE = 1000;

    enum class GtlClipType
    {
        gtlDifference,
        gtlIntersection,
        gtlUnion,
        gtlXor
    };

    static Region^ Clip(Region^ subject, Region^ clip, GtlClipType operation)
    {
        GtlRegion gtlSubject; 
        FromRegionToGtlRegion(subject, gtlSubject);
        GtlRegion gtlClip; 
        FromRegionToGtlRegion(clip, gtlClip);
        
        GtlRegion gtlResult;
        switch (operation)
        {
            case GtlClipType::gtlDifference:
                assign(gtlResult, gtlSubject - gtlClip);
                break;
            case GtlClipType::gtlIntersection:
                assign(gtlResult, gtlSubject & gtlClip);
                break;
            case GtlClipType::gtlUnion:
                assign(gtlResult, gtlSubject + gtlClip);
                break;
            case GtlClipType::gtlXor:
                assign(gtlResult, gtlSubject ^ gtlClip);
                break;
        }
        
        Region^ result = gcnew Region;
        FromGtlRegionToRegion(gtlResult, result);
        return result;
    }
   
    static void FromRegionToGtlRegion(Region^ region, GtlRegion& gtlRegion)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        gtlRegion.resize(polygonSets->Count);
        for (Int32 i = 0; i < polygonSets->Count; i++)
        {
            FromPolygonSetToGtlHoledPolygon(polygonSets->default[i],
                gtlRegion[i]);
        }
    }
    
    static void FromPolygonSetToGtlHoledPolygon(PolygonSet^ polygonSet,
        GtlHoledPolygon& gtlHoledPolygon)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        GtlPoints gtlPoints;
        switch (polygons->Count)
        {
            case 1:
                FromPolygonToGtlPoints(polygons->default[0], gtlPoints);
                set_points(gtlHoledPolygon, gtlPoints.begin(),
                    gtlPoints.end());
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
        GtlPolygonSet gtlHoles;
        gtlHoles.resize(holes->Count);
        for (Int32 i = 0; i < holes->Count; i++)
        {
            FromPolygonToGtlPoints(holes->default[i], gtlPoints);
            set_points(gtlHoles[i], gtlPoints.begin(), gtlPoints.end());
        }
        set_holes(gtlHoledPolygon, gtlHoles.begin(), gtlHoles.end());
    }
    
    static void FromPolygonToGtlPoints(Polygon^ polygon, GtlPoints& gtlPoints)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        gtlPoints.resize(vertices->Count);
        for (Int32 i = 0; i < vertices->Count; i++)
        {
            Vertex vertex = vertices->default[i];
            Int32 x = Int32(vertex.X * SCALE);
            Int32 y = Int32(vertex.Y * SCALE);
            gtlPoints[i] = construct<GtlPoint>(x, y);
        }
    }

    static void FromGtlRegionToRegion(const GtlRegion& gtlRegion, 
        Region^ region)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for (unsigned int i = 0; i < gtlRegion.size(); i++)
        {
            PolygonSet^ polygonSet = gcnew PolygonSet;
            FromGtlHoledPolygonToPolygonSet(gtlRegion[i], polygonSet);
            polygonSets->Add(polygonSet);
        }
    }
    
    static void FromGtlHoledPolygonToPolygonSet(
        const GtlHoledPolygon& gtlHoledPolygon, PolygonSet^ polygonSet)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        if (gtlHoledPolygon.self_.coords_.size() > 0)
        {
            GtlPolygon gtlPolygon = gtlHoledPolygon.self_;
            Polygon^ polygon = gcnew Polygon;
            FromGtlPolygonToPolygon(gtlPolygon, polygon);
            polygons->Add(polygon);
        }
        
        Collection<Polygon^>^ holes = polygonSet->Holes;
        for (std::list<GtlPolygon>::const_iterator holesIterator = 
            gtlHoledPolygon.holes_.begin();
            holesIterator != gtlHoledPolygon.holes_.end();
            holesIterator++)
        {
            GtlPolygon gtlPolygon = *holesIterator;
            Polygon^ polygon = gcnew Polygon;
            FromGtlPolygonToPolygon(gtlPolygon, polygon);
            holes->Add(polygon);
        }
    }

    static void FromGtlPolygonToPolygon(const GtlPolygon& gtlPolygon, 
        Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (unsigned int i = 0; i < gtlPolygon.coords_.size(); i++)
        {
            GtlPoint gtlPoint = gtlPolygon.coords_[i];
            double x = gtlPoint.x() / SCALE;
            double y = gtlPoint.y() / SCALE;
            vertices->Add(Vertex(x, y));
        }
    }
};

}