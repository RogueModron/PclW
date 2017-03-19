#include "bbox_2.h"
#include "booleanop.h"
#include "point_2.h"
#include "polygon.h"
#include "segment_2.h"
#include "utilities.h"





namespace PclWBop{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;

namespace bop = cbop;





typedef bop::BooleanOpType BopClipType;
typedef bop::Point_2 BopPoint;
typedef bop::Contour BopContour;
typedef bop::Polygon BopPolygon;

public ref class BopWrapper
{
public:
    static Region^ GetDifference(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, bop::DIFFERENCE);
    }

    static Region^ GetIntersection(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, bop::INTERSECTION);
    }

    static Region^ GetUnion(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, bop::UNION);
    }

    static Region^ GetXor(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, bop::XOR);
    }    

private:
    static const double SCALE = 1000;

    static Region^ Clip(Region^ subject, Region^ clip, 
        BopClipType operation)
    {
        BopPolygon bopSubject;
        FromRegionToBopPolygon(subject, bopSubject);
        BopPolygon bopClip;
        FromRegionToBopPolygon(clip, bopClip);

        BopPolygon bopResult;
		compute(bopSubject, bopClip, bopResult, operation);
        //bopResult.computeHoles();

        Region^ result = gcnew Region;
        FromBopPolygonToRegion(bopResult, result);
        return result;
    }

    static void FromRegionToBopPolygon(Region^ region, 
        BopPolygon& bopPolygon)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for (Int32 i = 0; i < polygonSets->Count; i++)
        {
            FromPolygonSetToBopPolygon(polygonSets->default[i],
                bopPolygon);
        }
    }

    static void FromPolygonSetToBopPolygon(PolygonSet^ polygonSet,
        BopPolygon& bopPolygon)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        switch (polygons->Count)
        {
            case 1:
                AddContourToBopPolygon(polygons->default[0], bopPolygon);
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
            AddContourToBopPolygon(holes->default[i], bopPolygon);
        }
    }

    static void AddContourToBopPolygon(Polygon^ polygon, 
        BopPolygon& bopPolygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        BopContour bopContour;
        for (Int32 i = 0; i < vertices->Count; i++)
        {
            Vertex vertex = vertices->default[i];
            Int32 x = Int32(vertex.X * SCALE);
            Int32 y = Int32(vertex.Y * SCALE);
            bopContour.add(BopPoint(x, y));
        }
        bopPolygon.push_back(bopContour);
    }

    static void FromBopPolygonToRegion(const BopPolygon& bopPolygon, 
        Region^ region)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for (unsigned int i = 0; i < bopPolygon.ncontours(); i++)
        {
            if (!bopPolygon.contour(i).external())
            {
                continue;
            }
            AddPolygonSetToPolygonSets(bopPolygon, i, polygonSets);
        }
    }

    static void AddPolygonSetToPolygonSets(const BopPolygon& bopPolygon,
        unsigned int contourIndex, Collection<PolygonSet^>^ polygonSets)
    {
        PolygonSet^ polygonSet = gcnew PolygonSet;

        BopContour bopContour = bopPolygon.contour(contourIndex);
        Polygon^ polygon = gcnew Polygon;
        FromBopContourToPolygon(bopContour, polygon);
        polygonSet->Polygons->Add(polygon);

        Collection<Polygon^>^ holes = polygonSet->Holes;
        for (unsigned int i = 0; i < bopContour.nholes(); i++)
        {
            BopContour bopHole = bopPolygon.contour(bopContour.hole(i));
            Polygon^ hole = gcnew Polygon;
            FromBopContourToPolygon(bopHole, hole);
            polygonSet->Holes->Add(hole);

            for (unsigned int j = 0; j < bopHole.nholes(); j++)
            {
                AddPolygonSetToPolygonSets(bopPolygon, bopHole.hole(j),
                    polygonSets);
            }
        }

        polygonSets->Add(polygonSet);
    }

    static void FromBopContourToPolygon(const BopContour& bopContour,
        Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (BopContour::const_iterator pointsIterator = bopContour.begin();
            pointsIterator != bopContour.end();
            pointsIterator++)
        {
            double x = pointsIterator->x() / SCALE;
            double y = pointsIterator->y() / SCALE;
            vertices->Add(Vertex(x, y));
        }
    }
};

}