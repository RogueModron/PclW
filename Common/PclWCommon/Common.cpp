namespace PclWCommon{

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace System::Text;





public value class Vertex 
{
public:
    Vertex(Double x, Double y)
    {
        _x = CheckCoordinate(x);
        _y = CheckCoordinate(y);
    }

    property Double X
    {
        Double get()
        {
            return _x;
        }
        void set(Double value) {
            _x = CheckCoordinate(value);
        }
    }

    property Double Y
    {
        Double get()
        {
            return _y;
        }
        void set(Double value)
        {
            _y = CheckCoordinate(value);
        }
    }

    virtual String^ ToString() override
    {
        return String::Format("({0} {1})", _x, _y);
    }

private:
    Double _x;
    Double _y;

    static Double CheckCoordinate(Double value)
    {
        if (Double::IsNaN(value) || Double::IsInfinity(value))
        {
            throw gcnew ArgumentException();
        }
        return value;
    }
};





public ref class Polygon
{
public:
    Polygon()
    {
        _vertices = gcnew List<Vertex>;
        _exposedVertices = gcnew Collection<Vertex>(_vertices);
    }

    Polygon(cli::array<Double>^ coordinates)
    {
        _vertices = GetVertexListFromDoubleArray(coordinates);
        _exposedVertices = gcnew Collection<Vertex>(_vertices);
    }

    property Boolean IsClockwise
    {
        Boolean get()
        {
            return (CalculateArea(_vertices) >= 0);
        }
    }

    property Collection<Vertex>^ Vertices
    {
        Collection<Vertex>^ get()
        {
            return _exposedVertices;
        }
    }
    
    void Reverse()
    {
        _vertices->Reverse();
    }

    virtual String^ ToString() override
    {
        StringBuilder^ output = gcnew StringBuilder;
        output->Append("(");
        for (Int32 i = 0; i < _vertices->Count; i++)
        {
            output->Append(_vertices[i].ToString());
        }
        output->Append(")");
        return output->ToString();
    }

private:
    Collection<Vertex>^ _exposedVertices;
    List<Vertex>^ _vertices;

    static Double CalculateArea(List<Vertex>^ vertices)
    {
        if (vertices->Count < 3)
        {
            return 0;
        }

        Vertex vertexA = vertices[vertices->Count - 1];
        Vertex vertexB = vertices[0];
        Double area = (vertexA.Y * vertexB.X) - (vertexA.X * vertexB.Y);
        for (Int32 i = 0; i < (vertices->Count - 1); i++)
        {
            vertexA = vertices[i];
            vertexB = vertices[i + 1];
            area += (vertexA.Y * vertexB.X) - (vertexA.X * vertexB.Y);
        }
        return (area / 2);
    }

    static List<Vertex>^ GetVertexListFromDoubleArray(
        cli::array<Double>^ coordinates)
    {
        if (IsEven(coordinates->Length))
        {
            throw gcnew ArgumentException();
        }

        List<Vertex>^ vertices = gcnew List<Vertex>;
        for (Int32 i = 0; i < (coordinates->Length - 1); i += 2)
        {
            vertices->Add(Vertex(coordinates[i], coordinates[i + 1]));
        }
        return vertices;
    }

    static Boolean IsEven(Int32 value)
    {
        return ((value % 2) == 1);
    }
};





public ref class PolygonSet
{
public:
    PolygonSet()
    {
        _holes = gcnew List<Polygon^>;
        _exposedHoles = gcnew Collection<Polygon^>(_holes);
        _polygons = gcnew List<Polygon^>;
        _exposedPolygons = gcnew Collection<Polygon^>(_polygons);
    }
    
    property Collection<Polygon^>^ Holes
    {
        Collection<Polygon^>^ get()
        {
            return _exposedHoles;
        }
    }

    property Collection<Polygon^>^ Polygons
    {
        Collection<Polygon^>^ get()
        {
            return _exposedPolygons;
        }
    }
        
    virtual String^ ToString() override
    {
        StringBuilder^ output = gcnew StringBuilder;
        output->Append("(");
        for (Int32 i = 0; i < _polygons->Count; i++)
        {
            output->Append(_polygons[i]->ToString());
        }
        for (Int32 i = 0; i < _holes->Count; i++)
        {
            output->Append(_holes[i]->ToString());
        }
        output->Append(")");
        return output->ToString();
    }
    
private:
    Collection<Polygon^>^ _exposedHoles;
    List<Polygon^>^ _holes;
    Collection<Polygon^>^ _exposedPolygons;
    List<Polygon^>^ _polygons;
};





public ref class Region
{
public:
    Region()
    {
        _polygonSets = gcnew List<PolygonSet^>;
        _exposedPolygonSets = gcnew Collection<PolygonSet^>(_polygonSets);
    }

    property Collection<PolygonSet^>^ PolygonSets
    {
        Collection<PolygonSet^>^ get()
        {
            return _exposedPolygonSets;
        }
    }

    virtual String^ ToString() override
    {
        StringBuilder^ output = gcnew StringBuilder;
        output->Append("(");
        for (Int32 i = 0; i < _polygonSets->Count; i++)
        {
            output->Append(_polygonSets[i]->ToString());
        }
        output->Append(")");
        return output->ToString();
    }

private:
    Collection<PolygonSet^>^ _exposedPolygonSets;
    List<PolygonSet^>^ _polygonSets;
};

}