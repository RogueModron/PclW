#include <boost/geometry/geometry.hpp>
#include <boost/geometry/geometries/point_xy.hpp>
#include <boost/geometry/geometries/polygon.hpp>
#include <boost/geometry/multi/geometries/multi_polygon.hpp>

#if TTMATH
    #include <boost/geometry/extensions/contrib/ttmath_stub.hpp>
#elif MPIR
    #include <boost/numeric_adaptor/numeric_adaptor.hpp>
    #include <boost/numeric_adaptor/gmp_value_type.hpp>
#endif





namespace PclWGgl{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;

namespace ggl = boost::geometry;
using namespace ggl;





#if TTMATH
    typedef ggl::model::d2::point_xy<boost::numeric_adaptor::gmp_value_type> GglPoint;
#elif MPIR
    typedef ggl::model::d2::point_xy<ttmath_big> GglPoint;
#else
    typedef ggl::model::d2::point_xy<double> GglPoint;
#endif

typedef ggl::model::ring<GglPoint> GglPolygon;
typedef ggl::model::polygon<GglPoint> GglHoledPolygon;
typedef ggl::model::multi_polygon<GglHoledPolygon> GglRegion;


public ref class GglWrapper
{
public:
    static Region^ GetDifference(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GglClipType::gglDifference);
    }

    static Region^ GetIntersection(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GglClipType::gglIntersection);
    }

    static Region^ GetUnion(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GglClipType::gglUnion);
    }

    static Region^ GetXor(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GglClipType::gglXor);
    }

private:
    static const double SCALE = 1000;

    enum class GglClipType
    {
        gglDifference,
        gglIntersection,
        gglUnion,
        gglXor
    };

    static Region^ Clip(Region^ subject, Region^ clip, GglClipType operation)
    {
        GglRegion gglSubject; 
        FromRegionToGglRegion(subject, gglSubject);
        //correct(gglSubject);
        GglRegion gglClip; 
        FromRegionToGglRegion(clip, gglClip);
        //correct(gglSubject);

        GglRegion gglResult;
        switch (operation)
        {
            case GglClipType::gglDifference:
                difference(gglSubject, gglClip, gglResult);
                break;
            case GglClipType::gglIntersection:
                intersection(gglSubject, gglClip, gglResult);
                break;
            case GglClipType::gglUnion:
                union_(gglSubject, gglClip, gglResult);
                break;
            case GglClipType::gglXor:
                sym_difference(gglSubject, gglClip, gglResult);
                break;
        }
        //unique(gglResult);

        Region^ result = gcnew Region;
        FromGglRegionToRegion(gglResult, result);
        return result;
    }
   
    static void FromRegionToGglRegion(Region^ region, GglRegion& gglRegion)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        gglRegion.resize(polygonSets->Count);
        for (Int32 i = 0; i < polygonSets->Count; i++)
        {
            FromPolygonSetToGglHoledPolygon(polygonSets->default[i],
                gglRegion[i]);
        }
    }
    
    static void FromPolygonSetToGglHoledPolygon(PolygonSet^ polygonSet,
        GglHoledPolygon& gglHoledPolygon)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        switch (polygons->Count)
        {
            case 1:
                FromPolygonToGglPolygon(polygons->default[0], 
                    gglHoledPolygon.outer());
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
        gglHoledPolygon.inners().resize(holes->Count);
        for (Int32 i = 0; i < holes->Count; i++)
        {
            FromPolygonToGglPolygon(holes->default[i], 
                gglHoledPolygon.inners()[i]);
        }
    }
    
    static void FromPolygonToGglPolygon(Polygon^ polygon, 
        GglPolygon& gglPolygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        gglPolygon.resize(vertices->Count);
        for (Int32 i = 0; i < vertices->Count; i++)
        {
            Vertex vertex = vertices->default[i];
            Int32 x = Int32(vertex.X * SCALE);
            Int32 y = Int32(vertex.Y * SCALE);
            gglPolygon[i] = make<GglPoint>(x, y);

#if TTMATH || MPIR
            gglPolygon[i] = make<GglPoint>(vertex.X, vertex.Y);
#endif
        }
    }

    static void FromGglRegionToRegion(const GglRegion& gglRegion, 
        Region^ region)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for (unsigned int i = 0; i < gglRegion.size(); i++)
        {
            PolygonSet^ polygonSet = gcnew PolygonSet;
            FromGglHoledPolygonToPolygonSet(gglRegion[i], polygonSet);
            polygonSets->Add(polygonSet);
        }
    }
    
    static void FromGglHoledPolygonToPolygonSet(
        const GglHoledPolygon& gglHoledPolygon, PolygonSet^ polygonSet)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        if (gglHoledPolygon.outer().size() > 0)
        {
            GglPolygon gglPolygon = gglHoledPolygon.outer();
            Polygon^ polygon = gcnew Polygon;
            FromGglPolygonToPolygon(gglPolygon, polygon);
            polygons->Add(polygon);
        }
        
        Collection<Polygon^>^ holes = polygonSet->Holes;
        for (std::vector<GglPolygon>::const_iterator holesIterator = 
            gglHoledPolygon.inners().begin();
            holesIterator != gglHoledPolygon.inners().end();
            holesIterator++)
        {
            GglPolygon gglPolygon = *holesIterator;
            Polygon^ polygon = gcnew Polygon;
            FromGglPolygonToPolygon(gglPolygon, polygon);
            holes->Add(polygon);
        }
    }

    static void FromGglPolygonToPolygon(const GglPolygon& gglPolygon, 
        Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (unsigned int i = 0; i < gglPolygon.size(); i++)
        {
            GglPoint gglPoint = gglPolygon[i];
#if TTMATH
            vertices->Add(Vertex(gglPoint.x().ToDouble(), 
                gglPoint.y().ToDouble()));
#elif MPIR
            vertices->Add(Vertex(gglPoint.x(), gglPoint.y()));
#else
            double x = gglPoint.x() / SCALE;
            double y = gglPoint.y() / SCALE;
            vertices->Add(Vertex(x, y));
#endif
        }
    }
};

}