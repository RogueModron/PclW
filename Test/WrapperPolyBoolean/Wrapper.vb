Imports System.Collections.ObjectModel
Imports ComplexA5.PolyBoolean.c20
'Imports ComplexA5.PolyBoolean.c30
Imports PclWCommon


Public Class PolyBooleanWrapper
    Private Const Scale As Integer = 50


    Public Function GetDifference(ByVal subject As Region,
            ByVal clip As Region) As Region
        Return ClipRegion(subject, clip, Operation.SBA)
    End Function

    Public Function GetIntersection(ByVal subject As Region,
            ByVal clip As Region) As Region
        Return ClipRegion(subject, clip, Operation.AND)
    End Function

    Public Function GetUnion(ByVal subject As Region,
            ByVal clip As Region) As Region
        Return ClipRegion(subject, clip, Operation.OR)
    End Function

    Public Function GetXor(ByVal subject As Region,
            ByVal clip As Region) As Region
        Return ClipRegion(subject, clip, Operation.XOR)
    End Function


    Private Function ClipRegion(ByVal subject As Region,
            ByVal clip As Region, ByVal operation As Operation) As Region
        Dim regionIterator As New RegionIterator(Scale)
        Dim factory As New Factory(regionIterator, subject, clip)
        Dim regionBuilder As New RegionBuilder(Scale)
        Dim result As Region = factory.CalculateResult(operation,
                regionBuilder, False)
        Return result
    End Function


    Private Class RegionBuilder
        Implements IRegionBuilder

        Private _Polygon As Polygon = Nothing
        Private _PolygonSet As PolygonSet = Nothing
        Private _Region As Region = Nothing
        Private _Scale As Integer = 1


        Public Sub New(ByVal scale As Integer)
            _Scale = scale
        End Sub

        Public Function AddNewPolygon() As Object _
                Implements IRegionBuilder.AddNewContour
            _Polygon = New Polygon
            If _PolygonSet.Polygons.Count = 0 Then
                _PolygonSet.Polygons.Add(_Polygon)
            Else
                _PolygonSet.Holes.Add(_Polygon)
            End If
            Return _Polygon
        End Function

        Public Function AddNewPolygonSet() As Object _
                Implements IRegionBuilder.AddNewPolygon
            _PolygonSet = New PolygonSet
            _Region.PolygonSets.Add(_PolygonSet)
            Return _PolygonSet
        End Function

        Public Sub AddTriangle(ByVal polygon As Object, ByVal v1 As Object,
                ByVal v2 As Object, ByVal v3 As Object) _
                Implements IRegionBuilder.AddTriangle
            'Not implemented.
        End Sub

        Public Function AddVertex(ByVal v As Object, ByVal x As Integer,
                ByVal y As Integer) As Object _
                Implements IRegionBuilder.AddVertex
            Dim vertex As Vertex
            vertex.X = x / _Scale
            vertex.Y = y / _Scale
            _Polygon.Vertices.Add(vertex)
            Return vertex
        End Function

        Public Function CreateNewRegion() As Object _
                Implements IRegionBuilder.CreateNewRegion
            _Region = New Region
            Return _Region
        End Function
    End Class


    Private Class RegionIterator
        Implements IRegionIterator

        Private _Polygon As Polygon = Nothing
        Private _PolygonIndex As Integer = 0
        Private _PolygonSet As PolygonSet = Nothing
        Private _PolygonSetIndex As Integer = 0
        Private _Region As Region = Nothing
        Private _Scale As Integer = 1
        Private _VertexIndex As Integer = 0


        Public Sub New(ByVal scale As Integer)
            _Scale = scale
        End Sub

        Public Sub Initialize(ByVal region As Object) _
                Implements IRegionIterator.Initialize
            _Region = region

            _PolygonIndex = 0
            _PolygonSetIndex = 0
            _VertexIndex = 0
        End Sub

        Public Function NextPolygon() As Boolean _
                Implements IRegionIterator.NextContour
            _VertexIndex = 0

            If _PolygonIndex >= _PolygonSet.Polygons.Count +
                    _PolygonSet.Holes.Count Then
                Return False
            End If

            If _PolygonIndex = 0 Then
                _Polygon = _PolygonSet.Polygons(_PolygonIndex)
            Else
                _Polygon = _PolygonSet.Holes(_PolygonIndex - 1)
            End If
            _PolygonIndex = _PolygonIndex + 1
            Return True
        End Function

        Public Function NextPolygonSet() As Boolean _
                Implements IRegionIterator.NextPolygon
            _PolygonIndex = 0
            _VertexIndex = 0

            If _PolygonSetIndex >= _Region.PolygonSets.Count Then
                Return False
            End If

            _PolygonSet = _Region.PolygonSets(_PolygonSetIndex)
            _PolygonSetIndex = _PolygonSetIndex + 1
            Return True
        End Function

        Public Function NextVertex(ByRef vertex As Object, ByRef x As Integer,
                ByRef y As Integer) As Boolean _
                Implements IRegionIterator.NextVertex
            If _VertexIndex >= _Polygon.Vertices.Count Then
                vertex = Nothing
                x = 0
                y = 0
                Return False
            End If

            vertex = _Polygon.Vertices(_VertexIndex)
            x = vertex.X * _Scale
            y = vertex.Y * _Scale
            _VertexIndex = _VertexIndex + 1
            Return True
        End Function
    End Class
End Class