#include <TeGeometry.h>
#include <TeOverlay.h>





namespace PclWTerraLib{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;





public ref class TerraLibWrapper
{
public:
    static Region^ GetDifference(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, TerraLibClipType::terraDifference);
    }

    static Region^ GetIntersection(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, TerraLibClipType::terraIntersection);
    }

    static Region^ GetUnion(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, TerraLibClipType::terraUnion);
    }

    static Region^ GetXor(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, TerraLibClipType::terraXor);
    }

private:
    enum class TerraLibClipType
    {
        terraDifference,
        terraIntersection,
        terraUnion,
        terraXor
    };

    static Region^ Clip(Region^ subject, Region^ clip, 
        TerraLibClipType operation)
    {
        TePolygonSet terraSubject;
        FromRegionToTePolygonSet(subject, terraSubject);
        TePolygonSet terraClip;
        FromRegionToTePolygonSet(clip, terraClip);

        TePolygonSet terraResult;
        bool r;
        switch (operation)
        {
            case TerraLibClipType::terraDifference:
                r = TeOVERLAY::TeDifference(terraSubject, terraClip, 
                    terraResult);
                break;
            case TerraLibClipType::terraIntersection:
                r = TeOVERLAY::TeIntersection(terraSubject, terraClip, 
                    terraResult);
                break;
            case TerraLibClipType::terraUnion:
                r = TeOVERLAY::TeUnion(terraSubject, terraClip, 
                    terraResult);
                break;
            case TerraLibClipType::terraXor:
                //TerraLib doesn't handle xor directly.
                TePolygonSet terraResult1;
                r = TeOVERLAY::TeUnion(terraSubject, terraClip, 
                    terraResult1);
                TePolygonSet terraResult2;
                r = TeOVERLAY::TeIntersection(terraSubject, terraClip, 
                    terraResult2);
                r = TeOVERLAY::TeDifference(terraResult1, terraResult2, 
                    terraResult);
                break;
        }

        Region^ result = gcnew Region;
        FromTePolygonSetToRegion(terraResult, result);
        return result;
    }

    static void FromRegionToTePolygonSet(Region^ region, 
        TePolygonSet& terraPolygonSet)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for (Int32 i = 0; i < polygonSets->Count; i++)
        {
            TePolygon terraPolygon;
            FromPolygonSetToTePolygon(polygonSets->default[i],
                terraPolygon);
            terraPolygonSet.add(terraPolygon);
        }
    }
    
    static void FromPolygonSetToTePolygon(PolygonSet^ polygonSet,
        TePolygon& terraPolygon)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        TeLinearRing terraOuterRing;
        switch (polygons->Count)
        {
            case 1:
                FromPolygonToTeLinearRing(polygons->default[0],
                    terraOuterRing);
                terraPolygon.add(terraOuterRing);
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
            TeLinearRing terraRing;
            FromPolygonToTeLinearRing(holes->default[i], terraRing);
            terraPolygon.add(terraRing);
        }
    }
    
    static void FromPolygonToTeLinearRing(Polygon^ polygon, 
        TeLinearRing& terraRing)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (Int32 i = 0; i < vertices->Count; i++)
        {
            Vertex vertex = vertices->default[i];
            terraRing.add(TeCoord2D(vertex.X, vertex.Y));
        }
    }

    static void FromTePolygonSetToRegion(const TePolygonSet& terraPolygonSet, 
        Region^ region)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for(unsigned int i = 0; i < terraPolygonSet.size(); i++)
        {
            PolygonSet^ polygonSet = gcnew PolygonSet;
            FromTePolygonToPolygonSet(terraPolygonSet[i], polygonSet);
            polygonSets->Add(polygonSet);
        }
    }
    
    static void FromTePolygonToPolygonSet(const TePolygon& terraPolygon,
        PolygonSet^ polygonSet)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        if (terraPolygon.size() > 0)
        {
            TeLinearRing terraOuterRing = terraPolygon[0];
            Polygon^ polygon = gcnew Polygon;
            FromTeLinearRingToPolygon(terraOuterRing, polygon);
            polygons->Add(polygon);
        }
        
        Collection<Polygon^>^ holes = polygonSet->Holes;
        for (unsigned int i = 1; i < terraPolygon.size(); i++)
        {
            Polygon^ polygon = gcnew Polygon;
            FromTeLinearRingToPolygon(terraPolygon[i], polygon);
            holes->Add(polygon);
        }
    }

    static void FromTeLinearRingToPolygon(const TeLinearRing& terraRing, 
        Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (unsigned int i = 0; i < terraRing.size(); i++)
        {
            TeCoord2D terraCoord = terraRing[i];
            vertices->Add(Vertex(terraCoord.x(), terraCoord.y()));
        }
    }
};

}