#include "gpc.h"





namespace PclWGpc{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;





public ref class GpcWrapper
{
public:
    static PolygonSet^ GetDifference(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, GPC_DIFF);
    }

    static PolygonSet^ GetIntersection(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, GPC_INT);
    }

    static PolygonSet^ GetUnion(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, GPC_UNION);
    }

    static PolygonSet^ GetXor(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, GPC_XOR);
    }

private:
    static PolygonSet^ Clip(PolygonSet^ subject, PolygonSet^ clip, 
        gpc_op operation)
    {
        gpc_polygon* gpcSubject = new gpc_polygon;
        FromPolygonSetToGpcPolygon(subject, *gpcSubject);
        gpc_polygon* gpcClip = new gpc_polygon;
        FromPolygonSetToGpcPolygon(clip, *gpcClip);
        
        gpc_polygon* gpcResult = new gpc_polygon;
        gpc_polygon_clip(operation, gpcSubject, gpcClip, gpcResult);

        PolygonSet^ result = gcnew PolygonSet;
        FromGpcPolygonToPolygonSet(*gpcResult, result);

        DeleteGpcPolygon(gpcSubject);
        DeleteGpcPolygon(gpcClip);
        gpc_free_polygon(gpcResult);
        delete gpcResult;

        return result;
    }

    static void DeleteGpcPolygon(gpc_polygon* gpcPolygon)
    {
        for (int contourIndex = 0;
            contourIndex < gpcPolygon->num_contours; contourIndex++)
        {
            if (gpcPolygon->contour[contourIndex].num_vertices > 0)
            {
                delete[] gpcPolygon->contour[contourIndex].vertex;
            }
        }
        if (gpcPolygon->num_contours > 0)
        {
            delete[] gpcPolygon->contour;
            delete[] gpcPolygon->hole;
        }
        delete gpcPolygon;
    }

    static void FromGpcPolygonToPolygonSet(const gpc_polygon& gpcPolygon,
        PolygonSet^ polygonSet)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        Collection<Polygon^>^ holes = polygonSet->Holes;
        for (int i = 0; i < gpcPolygon.num_contours; i++)
        {
            Polygon^ polygon = gcnew Polygon();
            FromGpcVertexListToPolygon(gpcPolygon.contour[i], polygon);
            if (gpcPolygon.hole[i] == 0)
            {
                polygons->Add(polygon);
            }
            else
            {
                polygons->Add(polygon);
            }
        }
    }
    
    static void FromGpcVertexListToPolygon(
        const gpc_vertex_list& gpcVertexList, Polygon^ polygon)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        for (int j = 0; j < gpcVertexList.num_vertices; j++)
        {
            gpc_vertex gpcVertex = gpcVertexList.vertex[j];
            vertices->Add(Vertex(gpcVertex.x, gpcVertex.y));
        }
    }

    static void FromPolygonSetToGpcPolygon(PolygonSet^ polygonSet,
        gpc_polygon& gpcPolygon)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        Collection<Polygon^>^ holes = polygonSet->Holes;
        Int32 totalPolygonsCount = polygons->Count + holes->Count;

        gpcPolygon.num_contours = totalPolygonsCount;
        if (totalPolygonsCount == 0)
        {
            return;
        }
        
        gpcPolygon.contour = new gpc_vertex_list[totalPolygonsCount];
        gpcPolygon.hole = new int[totalPolygonsCount];

        for (Int32 i = 0; i < polygons->Count; i++)
        {
            FromPolygonToGpcVertexList(polygons->default[i], 
                gpcPolygon.contour[i]);
            gpcPolygon.hole[i] = int(0);
        }
        for (Int32 i = 0; i < holes->Count; i++)
        {
            Int32 j = polygons->Count + i;
            FromPolygonToGpcVertexList(holes->default[i], 
                gpcPolygon.contour[j]);
            gpcPolygon.hole[j] = int(1);
        }
    }
    
    static void FromPolygonToGpcVertexList(Polygon^ polygon, 
        gpc_vertex_list& gpcVertexList)
    {
        Collection<Vertex>^ vertices = polygon->Vertices;
        Int32 verticesCount = vertices->Count;

        gpcVertexList.num_vertices = verticesCount;
        if (verticesCount == 0)
        {
            return;
        }

        gpcVertexList.vertex = new gpc_vertex[verticesCount];
        for (Int32 j = 0; j < verticesCount; j++)
        {
            Vertex vertex = vertices->default[j];
            gpc_vertex& gpcVertex = gpcVertexList.vertex[j];
            gpcVertex.x = vertex.X;
            gpcVertex.y = vertex.Y;
        }
    }
};

}