#include "booleng.h"





namespace PclWKBool{

using namespace System;
using namespace System::Collections::ObjectModel;
using namespace PclWCommon;





public ref class KBoolWrapper
{
public:
    static PolygonSet^ GetDifference(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, BOOL_A_SUB_B);
    }

    static PolygonSet^ GetIntersection(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, BOOL_AND);
    }

    static PolygonSet^ GetUnion(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, BOOL_OR);
    }

    static PolygonSet^ GetXor(PolygonSet^ subject, PolygonSet^ clip)
    {
        return Clip(subject, clip, BOOL_EXOR);
    }

private:
    static void AddPolygonSetToBoolEngine(PolygonSet^ polygonSet,
        Bool_Engine* boolEngine, GroupType group)
    {
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        for (Int32 i = 0; i < polygons->Count; i++)
        {
            Polygon^ polygon = polygons->default[i];
            Collection<Vertex>^ vertices = polygon->Vertices;
            if (vertices->Count == 0)
            {
                continue;
            }

            boolEngine->StartPolygonAdd(group);
            for (Int32 j = 0; j < vertices->Count; j++)
            {
                Vertex vertex = vertices->default[j];
                boolEngine->AddPoint(vertex.X, vertex.Y);
            }
            boolEngine->EndPolygonAdd();
        }
    }

    static PolygonSet^ Clip(PolygonSet^ subject, PolygonSet^ clip, 
        BOOL_OP operation)
    {
        Bool_Engine* boolEngine = CreateBoolEngine();
        AddPolygonSetToBoolEngine(subject, boolEngine, GROUP_A);
        AddPolygonSetToBoolEngine(clip, boolEngine, GROUP_B);
        boolEngine->Do_Operation(operation);
        PolygonSet^ result = GetPolygonSetFromBoolEngine(boolEngine);

        delete boolEngine;

        return result;
    }

    static Bool_Engine* CreateBoolEngine()
    {
        Bool_Engine* boolEngine = new Bool_Engine;
        boolEngine->SetMarge(0.001);
        boolEngine->SetGrid(10000);
        boolEngine->SetDGrid(1000);
        boolEngine->SetCorrectionFactor(500.0);
        boolEngine->SetCorrectionAber(1.0);
        boolEngine->SetSmoothAber(10.0);
        boolEngine->SetMaxlinemerge(1000.0);
        boolEngine->SetRoundfactor(1.5);
        boolEngine->SetOrientationEntryMode(true);
        boolEngine->SetLinkHoles(false);
        return boolEngine;
    }

    static PolygonSet^ GetPolygonSetFromBoolEngine(Bool_Engine* boolEngine)
    {
        PolygonSet^ polygonSet = gcnew PolygonSet;
        Collection<Polygon^>^ polygons = polygonSet->Polygons;
        while (boolEngine->StartPolygonGet())
        {
            Polygon^ polygon = gcnew Polygon;
            Collection<Vertex>^ vertices = polygon->Vertices;
            while (boolEngine->PolygonHasMorePoints() )
            {
                Double x = boolEngine->GetPolygonXPoint();
                Double y = boolEngine->GetPolygonYPoint();
                vertices->Add(Vertex(x, y));
            }
            polygons->Add(polygon);
            boolEngine->EndPolygonGet();
        }
        return polygonSet;
    }
};

}