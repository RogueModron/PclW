#include "clipper.hpp"





namespace PclWClipper{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;

namespace clpr = ClipperLib;
using namespace clpr;





typedef clpr::ClipType ClprClipType;
typedef clpr::IntPoint ClprPoint;
typedef clpr::Polygon ClprPolygon;
typedef clpr::Polygons ClprPolygons;
typedef clpr::Clipper Clipper;

public ref class ClipperWrapper
{
public:
    static PolygonSet^ GetDifference(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, ctDifference);
    }

    static PolygonSet^ GetIntersection(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, ctIntersection);
    }

    static PolygonSet^ GetUnion(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, ctUnion);
    }

    static PolygonSet^ GetXor(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, ctXor);
    }    

private:
    static const double SCALE = 1000;

    static PolygonSet^ Clip(PolygonSet^ subject, PolygonSet^ clip,
        ClprClipType operation)
    {
        ClprPolygons clprSubject;
        FromPolygonSetToClprPolygons(subject, clprSubject);
        ClprPolygons clprClip;
        FromPolygonSetToClprPolygons(clip, clprClip);

        Clipper clipperEngine;
        //clipperEngine.UseFullCoordinateRange(false);
        clipperEngine.AddPolygons(clprSubject, ptSubject);
        clipperEngine.AddPolygons(clprClip, ptClip);
        ClprPolygons clprResult;
        clipperEngine.Execute(operation, clprResult);
        
        PolygonSet^ result = gcnew PolygonSet;
        FromClprPolygonsToPolygonSet(clprResult, result);
        return result;
    }

    static void FromClprPolygonsToPolygonSet(
        const ClprPolygons& clprPolygons, PolygonSet^ polygonSet)
    {
        Collection<PclWCommon::Polygon^>^ polygons = polygonSet->Polygons;
        for (unsigned int i = 0; i < clprPolygons.size(); i++)
        {
            PclWCommon::Polygon^ polygon = gcnew PclWCommon::Polygon;
            FromClprPolygonToPolygon(clprPolygons[i], polygon);
            polygons->Add(polygon);
        }
    }
    
    static void FromClprPolygonToPolygon(const ClprPolygon& clprPolygon,
        PclWCommon::Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (unsigned int j = 0; j < clprPolygon.size(); j++)
        {
            ClprPoint point = clprPolygon[j];
            Double x = point.X / SCALE;
            Double y = point.Y / SCALE;
            vertices->Add(Vertex(x, y));
        }
    }

    static void FromPolygonSetToClprPolygons(PolygonSet^ polygonSet,
        ClprPolygons& clprPolygons)
    {
        Collection<PclWCommon::Polygon^>^ polygons = polygonSet->Polygons;
        clprPolygons.resize(polygons->Count);
        for (Int32 i = 0; i < polygons->Count; i++)
        {
            FromPolygonToClprPolygon(polygons->default[i], 
                clprPolygons[i]);
        }
    }
    
    static void FromPolygonToClprPolygon(PclWCommon::Polygon^ polygon, 
        ClprPolygon& clprPolygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        clprPolygon.resize(vertices->Count);
        for (Int32 j = 0; j < vertices->Count; j++)
        {
            Vertex vertex = vertices->default[j];
            ClprPoint clprPoint;
            clprPoint.X = Int32(vertex.X * SCALE);
            clprPoint.Y = Int32(vertex.Y * SCALE);
            clprPolygon[j] = clprPoint;
        }
    }
};

}