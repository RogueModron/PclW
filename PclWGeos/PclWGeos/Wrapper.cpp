#include "geos.h"
#include "geos\geom\CoordinateSequenceFilter.h"




namespace PclWGeos{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;

using namespace std;





public ref class GeosWrapper
{
public:
    static Region^ GetDifference(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GeosClipType::geosDifference);
    }

    static Region^ GetIntersection(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GeosClipType::geosIntersection);
    }

    static Region^ GetUnion(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GeosClipType::geosUnion);
    }

    static Region^ GetXor(Region^ subject, Region^ clip)
    {
        return Clip(subject, clip, GeosClipType::geosXor);
    }

private:
    enum class GeosClipType
    {
        geosDifference,
        geosIntersection,
        geosUnion,
        geosXor
    };

    static Region^ Clip(Region^ subject, Region^ clip, GeosClipType operation)
    {
        GeometryFactory* geosFactory = new GeometryFactory();
        MultiPolygon* geosSubject;
        FromRegionToGeosMultiPolygon(subject, *geosFactory, geosSubject);
        MultiPolygon* geosClip;
        FromRegionToGeosMultiPolygon(clip, *geosFactory, geosClip);

        Geometry* geosResult;
        switch (operation)
        {
            case GeosClipType::geosDifference:
                geosResult = geosSubject->difference(geosClip);
                break;
            case GeosClipType::geosIntersection:
                geosResult = geosSubject->intersection(geosClip);
                break;
            case GeosClipType::geosUnion:
                geosResult = geosSubject->Union(geosClip);
                break;
            case GeosClipType::geosXor:
                geosResult = geosSubject->symDifference(geosClip);
                break;
        }

        geosFactory->destroyGeometry(geosSubject);
        geosFactory->destroyGeometry(geosClip);

        Region^ result = gcnew Region;
        FromGeosMultiPolygonToRegion(*geosResult, result);

        geosFactory->destroyGeometry(geosResult);
        delete geosFactory;

        return result;
    }
    
    static void FromRegionToGeosMultiPolygon(Region^ region,
        const GeometryFactory& geosFactory, MultiPolygon*& geosMultiPolygon)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        std::vector<Geometry*>* geosPolygons = 
            new std::vector<Geometry*>(polygonSets->Count);
        for (Int32 i = 0; i < polygonSets->Count; i++)
        {
            geos::geom::Polygon* geosPolygon;
            FromPolygonSetToGeosPolygon(polygonSets->default[i], geosFactory,
                geosPolygon);
            (*geosPolygons)[i] = geosPolygon;
        }
        geosMultiPolygon = geosFactory.createMultiPolygon(geosPolygons);
    }
    
    static void FromPolygonSetToGeosPolygon(PolygonSet^ polygonSet,
        const GeometryFactory& geosFactory, geos::geom::Polygon*& geosPolygon)
    {
        Collection<PclWCommon::Polygon^>^ polygons = polygonSet->Polygons;
        LinearRing* geosOuterRing;
        switch (polygons->Count)
        {
            case 1:
                FromPolygonToGeosRing(polygons->default[0], geosFactory, 
                    geosOuterRing);
                break;
            case 0:
                return;
            default:
                throw gcnew ArgumentException();
        }
        
        Collection<PclWCommon::Polygon^>^ holes = polygonSet->Holes;
        if (holes->Count == 0)
        {
            geosPolygon = geosFactory.createPolygon(geosOuterRing, NULL);
            return;
        }
        std::vector<Geometry*>* geosHoles = 
			new std::vector<Geometry*>(holes->Count);
        for (Int32 i = 0; i < holes->Count; i++)
        {
            LinearRing* geosRing;
            FromPolygonToGeosRing(holes->default[i], geosFactory, geosRing);
            (*geosHoles)[i] = geosRing;
        }
        geosPolygon = geosFactory.createPolygon(geosOuterRing, geosHoles);
    }
    
    static void FromPolygonToGeosRing(PclWCommon::Polygon^ polygon, 
        const GeometryFactory& geosFactory, LinearRing*& geosRing)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        Int32 verticesCount = vertices->Count;

        if (verticesCount == 0)
        {
            return;
        }
        
        CoordinateSequence* geosCoordinates = new CoordinateArraySequence();
        for (Int32 i = 0; i < verticesCount; i++)
        {
            Vertex vertex = vertices->default[i];
            geosCoordinates->add(Coordinate(vertex.X, vertex.Y));
        }
        geosRing = geosFactory.createLinearRing(geosCoordinates);
    }
    
    static void FromGeosMultiPolygonToRegion(
        const Geometry& geosMultiGeometry, Region^ region)
    {
        Collection<PolygonSet^>^ polygonSets = region->PolygonSets;
        for (unsigned int i = 0; i < geosMultiGeometry.getNumGeometries(); 
            i++)
        {
            const Geometry* geosGeometry = geosMultiGeometry.getGeometryN(i);
            if (geosGeometry->getGeometryTypeId() != GEOS_POLYGON)
            {
                continue;
            }
            
            if (geosGeometry->getNumGeometries() == 0)
            {
                continue;
            }
            const geos::geom::Polygon* geosPolygon = 
                dynamic_cast<const geos::geom::Polygon*> (geosGeometry);

            PolygonSet^ polygonSet = gcnew PolygonSet;
            PclWCommon::Polygon^ polygon = gcnew PclWCommon::Polygon;
            const LineString* geosLineString = geosPolygon->getExteriorRing();
            FromGeosLineStringToPolygon(*geosLineString, polygon);
            polygonSet->Polygons->Add(polygon);
            
            Collection<PclWCommon::Polygon^>^ holes = polygonSet->Holes;
            for (unsigned int j = 0; j < geosPolygon->getNumInteriorRing();
                j++)
            {
                polygon = gcnew PclWCommon::Polygon;
                geosLineString = geosPolygon->getInteriorRingN(j);
                FromGeosLineStringToPolygon(*geosLineString, polygon);
                holes->Add(polygon);
            }
            
            region->PolygonSets->Add(polygonSet);
        }
    }
    
    static void FromGeosLineStringToPolygon(const LineString& geosLineString,
        PclWCommon::Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        const CoordinateSequence* geosCoordinates = 
            geosLineString.getCoordinatesRO();
        for (unsigned int i = 0; i < geosCoordinates->getSize(); i++)
        {
            const Coordinate geosCoordinate = geosCoordinates->getAt(i);
            vertices->Add(Vertex(geosCoordinate.x, geosCoordinate.y));
        }
    }
};

}